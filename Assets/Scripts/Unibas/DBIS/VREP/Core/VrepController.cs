using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Client;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Newtonsoft.Json;
using Unibas.DBIS.VREP.Generation;
using Unibas.DBIS.VREP.Generation.Model;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  /// <summary>
  /// Controller for the exhibition.
  /// This class is way too heavy and would require refactoring.
  /// TODO The refactoring should be done on any further development of VREP.
  /// </summary>
  public class VrepController : MonoBehaviour
  {
    public static Vector3 LobbySpawn = new Vector3(0, -9, 0);

    public static VrepController Instance;

    public Exhibition Exhibition;
    public List<CuboidExhibitionRoom> RoomList = new List<CuboidExhibitionRoom>();

    public bool isGenerating;

    public Settings settings;
    public string settingsPath;

    private VrepController()
    {
    }

    private void Awake()
    {
      if (Application.isEditor)
      {
        settings = string.IsNullOrEmpty(settingsPath) ? Settings.LoadSettings() : Settings.LoadSettings(settingsPath);
      }
      else
      {
        settings = Settings.LoadSettings();
      }

      Instance = this;
    }

    private async void Start()
    {
      // Clean address.
      SanitizeHost();

      // Reset player to lobby.
      if (settings.StartInLobby)
      {
        TpPlayerToLobby();
      }

      // VREM Client settings.
      Configuration.Default.BasePath = settings.VremAddress;
      Configuration.Default.Timeout = 3600_000;

      await LoadInitialExhibition();
    }

    public async void Update()
    {
      // Could be used to save exhibitions via hotkey.
      if (Input.GetKeyDown(KeyCode.F8))
      {
        await new ExhibitionApi().PostApiExhibitionsSaveAsync(Exhibition);
      }

      if (Input.GetKey(KeyCode.F10))
      {
        Restore();
      }

      if (Input.GetKey(KeyCode.F12))
      {
        RestoreExhibits();
      }
    }

    /// <summary>
    /// Destroys the currently loaded exhibition.
    /// </summary>
    public void DestroyCurrentExhibition()
    {
      RoomList.ForEach(r => Destroy(r.gameObject));
    }

    /// <summary>
    /// Loads a new exhibition.
    /// </summary>
    public async Task LoadNewExhibition(Exhibition ex)
    {
      Exhibition = ex;

      await LoadExhibition();
    }

    /// <summary>
    /// Disables all rooms except the primary room of a generated exhibition.
    /// </summary>
    public void EnableOnlyMainRoom()
    {
      RoomList.ForEach(it => it.gameObject.SetActive(it == RoomList.First()));
    }

    /// <summary>
    /// Loads the exhibition model specified in the Exhibition field.
    /// </summary>
    public async Task LoadExhibition()
    {
      DestroyCurrentExhibition();

      foreach (var room in Exhibition.Rooms)
      {
        var go = await LoadRoom(room);
        go.SetActive(false);
      }

      if (!Exhibition.Metadata.ContainsKey(GenerationMetadata.Generated.GetKey()))
      {
        // Not generated, connect all rooms.
        Instance.ImportedTpSetup();
      }

      EnableOnlyMainRoom();

      // Setup TP to first room.
      Instance.LobbyTpSetup(RoomList[0]);

      // TP player.
      TpPlayerToObjPos(RoomList[0].gameObject);
    }

    /// <summary>
    /// Loads a room model.
    /// </summary>
    /// <param name="room">The room model to load.</param>
    public async Task<GameObject> LoadRoom(Room room)
    {
      var roomGameObject = await ObjectFactory.BuildRoom(room);
      var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();

      // Add room to map.
      RoomList.Add(exhibitionRoom);

      if (room.Metadata.ContainsKey(GenerationMetadata.Generated.GetKey()))
      {
        GeneratedTpSetup(room);
      }

      return roomGameObject;
    }

    /// <summary>
    /// Restores positions of all exhibits.
    /// </summary>
    void RestoreExhibits()
    {
      RoomList.ForEach(r =>
      {
        // Only restores exhibits for the active rooms.
        if (r.gameObject.activeSelf)
        {
          r.RestoreWallExhibits();
        }
      });
    }

    /// <summary>
    /// Restores positions of all exhibits and teleports the player to the lobby.
    /// </summary>
    public void Restore()
    {
      RestoreExhibits();
      EnableOnlyMainRoom();
      TpPlayerToLobby();
    }

    /// <summary>
    /// Loads the initial exhibition as specified in the configuration.
    /// </summary>
    public async Task LoadInitialExhibition()
    {
      Exhibition ex;

      switch (settings.ExhibitionMode)
      {
        case GenerationMode.Static:
          ex = await LoadExhibitionById();
          break;
        case GenerationMode.GenerationVisual:
          ex = await GenerateExhibition(GenerationMethod.VisualSom);
          break;
        case GenerationMode.GenerationSemantic:
          ex = await GenerateExhibition(GenerationMethod.SemanticSom);
          break;
        default:
          Debug.LogError("Invalid exhibition mode specified!");
          throw new ArgumentOutOfRangeException();
      }

      await Instance.LoadNewExhibition(ex);
    }

    /// <summary>
    /// Loads an exhibition by the ID specified in the settings.
    /// </summary>
    /// <returns>The exhibition model.</returns>
    public async Task<Exhibition> LoadExhibitionById()
    {
      return await new ExhibitionApi().GetApiExhibitionsLoadIdWithIdAsync(settings.ExhibitionId);
    }

    /// <summary>
    /// Obtains the room specification from the configuration file.
    /// </summary>
    /// <returns>The specified room configuration.</returns>
    public RoomSpecification GetRoomSpec()
    {
      return new RoomSpecification(settings.GenerationSettings.Height, settings.GenerationSettings.Width);
    }

    /// <summary>
    /// Obtains the seed from the configuration file.
    /// </summary>
    /// <returns>The specified seed.</returns>
    public int GetSeed()
    {
      return settings.GenerationSettings.Seed;
    }

    /// <summary>
    /// Obtains the number of epochs to train SOMs for in VREM from the configuration file.
    /// </summary>
    /// <returns>The number of epochs.</returns>
    public int GetNumEpochs()
    {
      return settings.GenerationSettings.NumEpochs;
    }

    /// <summary>
    /// Generates an exhibition based on a method.
    /// </summary>
    /// <param name="method">The method to use for the initial room</param>
    /// <returns>The generated exhibition model.</returns>
    public async Task<Exhibition> GenerateExhibition(GenerationMethod method)
    {
      var ex = await new GenerationApi().PostApiGenerateExhibitionAsync();

      var room = await GenerateRoomByMethod(method);

      ex.Rooms.Add(room);

      return ex;
    }

    /// <summary>
    /// Generates a random room.
    /// </summary>
    /// <param name="idList">The ID list to use for the random room.</param>
    /// <returns>The room model.</returns>
    public async Task<Room> GenerateRandomRoom(List<String> idList)
    {
      var config = new RandomGenerationRequest(GetRoomSpec(), idList, GetSeed());
      return await new GenerationApi().PostApiGenerateRoomRandomAsync(config);
    }

    /// <summary>
    /// Generates a similarity room.
    /// </summary>
    /// <param name="genType">The type of features to use for generation.</param>
    /// <param name="objectId">The originating object ID</param>
    /// <returns>The room model.</returns>
    public async Task<Room> GenerateSimilarityRoom(string genType, string objectId)
    {
      var config = new SimilarityGenerationRequest(GetRoomSpec(), genType, objectId);
      return await new GenerationApi().PostApiGenerateRoomSimilarAsync(config);
    }

    /// <summary>
    /// Generates a SOM room.
    /// </summary>
    /// <param name="genType">The type of features to use for generation.</param>
    /// <param name="idList">The ID list to use for the SOM room.</param>
    /// <returns>The room model.</returns>
    public async Task<Room> GenerateSomRoom(string genType, List<String> idList)
    {
      var config = new SomGenerationRequest(GetRoomSpec(), genType, idList, GetSeed(), GetNumEpochs());
      return await new GenerationApi().PostApiGenerateRoomSomAsync(config);
    }

    /// <summary>
    /// Sends a generation request to VREM for a given method.
    /// </summary>
    /// <param name="type">The type of room to generate.</param>
    /// <param name="ids">The list of IDs to use for generation.</param>
    /// <param name="originId">The ID of the originating object (relevant for similarity rooms).</param>
    /// <returns>The generated room model.</returns>
    public async Task<Room> GenerateRoomByMethod(GenerationMethod type, List<string> ids = null, string originId = "")
    {
      ids ??= new List<string>();

      return type switch
      {
        GenerationMethod.RandomAll => await GenerateRandomRoom(new List<string>()),
        GenerationMethod.RandomList => await GenerateRandomRoom(ids),
        GenerationMethod.VisualSimilarity => await GenerateSimilarityRoom("visual", originId),
        GenerationMethod.SemanticSimilarity => await GenerateSimilarityRoom("semantic", originId),
        GenerationMethod.VisualSom => await GenerateSomRoom("visual", ids),
        GenerationMethod.SemanticSom => await GenerateSomRoom("semantic", ids),

        _ => throw new ArgumentOutOfRangeException()
      };
    }

    /// <summary>
    /// Generates and loads a room for an exhibition.
    /// </summary>
    /// <param name="origin">The originating exhibit for the generation.</param>
    /// <param name="type">The type of room to generate.</param>
    /// <returns>The newly created/obtained room received from VREM.</returns>
    public async Task<Room> GenerateAndLoadRoomForExhibition(GameObject origin, GenerationMethod type)
    {
      var config = origin.GetComponent<IdListPair>();
      var room = await GenerateRoomByMethod(type, config.associatedIds, config.originId);

      // Calculate new room's position relative to the room the generation was issued from and set it in the model.
      SetPositionForGeneratedRoom(origin, room);

      // Teleport info, necessary for restoring generated exhibitions.
      // room.Metadata[MetadataType.PredecessorExhibit.GetKey()] = origin.GetComponent<Displayal>().id;
      room.Metadata[GenerationMetadata.PredecessorRoom.GetKey()] =
        origin.GetComponentInParent<CuboidExhibitionRoom>().RoomData.Id;

      // Load the room like a normal room.
      await Instance.LoadRoom(room);

      // Add the newly generated room to the exhibition model.
      Instance.Exhibition.Rooms.Add(room);

      // Update references.
      // TODO Move metadata manipulation into the Metadata namespace.
      RoomReferences references;
      var model = origin.GetComponent<Displayal>().GetExhibit();

      if (model.Metadata.ContainsKey(GenerationMetadata.References.GetKey()))
      {
        var refJson = model.Metadata[GenerationMetadata.References.GetKey()];
        references = JsonConvert.DeserializeObject<RoomReferences>(refJson);
      }
      else
      {
        references = new RoomReferences();
      }

      references.References[type.ToString()] = room.Id;

      // Store the updated references as metadata.
      model.Metadata[GenerationMetadata.References.GetKey()] = JsonConvert.SerializeObject(references);

      // Teleport the player.
      TpPlayerToLocation(new Vector3(room.Position.X, room.Position.Y, room.Position.Z));

      return room;
    }

    /// <summary>
    /// Teleports the player to the location of a game object.
    /// </summary>
    /// <param name="go">The game object to teleport the player to.</param>
    public static void TpPlayerToObjPos(GameObject go)
    {
      TpPlayerToLocation(go.transform.position);
    }

    /// <summary>
    /// Teleports the player to a location.
    /// </summary>
    /// <param name="loc">Vector specifying the location to teleport the player to.</param>
    public static void TpPlayerToLocation(Vector3 loc)
    {
      var go = GameObject.FindWithTag("Player");

      if (go != null)
      {
        go.transform.position = loc;
      }
    }

    /// <summary>
    /// Teleports the player to the lobby.
    /// </summary>
    public static void TpPlayerToLobby()
    {
      var lby = GameObject.Find("Lobby");

      if (lby != null)
      {
        TpPlayerToLocation(LobbySpawn);
      }
    }

    /// <summary>
    /// Computes the entry point for a given room object and the associated model (holding the entry point).
    /// </summary>
    /// <param name="roomGo">The game object for the room.</param>
    /// <param name="roomModel">The model data for the room.</param>
    /// <returns>A vector with the absolute coordinates of the entry point.</returns>
    public static Vector3 CalcEntryPointForRoom(GameObject roomGo, Room roomModel)
    {
      var roomPos = roomGo.transform.position;
      var entryPos = roomModel.EntryPoint;

      return new Vector3(
        roomPos.x + entryPos.X,
        roomPos.y + entryPos.Y,
        roomPos.z + entryPos.Z
      );
    }

    /// <summary>
    /// Create a teleport button model.
    /// </summary>
    /// <param name="origin">The (room of) origin of the teleport.</param>
    /// <param name="dest">Destination coordinates for the teleport.</param>
    /// <param name="buttonPos">The position of the button in the room.</param>
    /// <param name="text">The text to display on the button.</param>
    /// <param name="size">The size of the button.</param>
    /// <returns>The created teleport button object.</returns>
    public static SteamVRTeleportButton CreateTeleport(GameObject origin, Vector3 dest, Vector3 buttonPos, String text,
      float size = 0.2f)
    {
      var model = new SteamVRTeleportButton.TeleportButtonModel(size, 0.02f, 1.0f, null,
        TexturingUtility.LoadMaterialByName("NMetal"),
        TexturingUtility.LoadMaterialByName("NPlastic"));

      var btn = SteamVRTeleportButton.Create(
        origin,
        new Vector3(buttonPos.x + 0.5f * size, buttonPos.y, buttonPos.z + 0.5f * size),
        dest,
        model,
        text
      );

      btn.gameObject.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
      return btn;
    }

    /// <summary>
    /// Sets up teleportation for the lobby.
    /// </summary>
    /// <param name="firstRoom">The first room of the exhibition to teleport to from the lobby.</param>
    public void LobbyTpSetup(CuboidExhibitionRoom firstRoom)
    {
      var lby = GameObject.Find("Lobby");

      if (lby == null)
      {
        return;
      }

      var backTpBtn = CreateTeleport(firstRoom.gameObject, lby.transform.position, Vector3.zero, "Back to Lobby");
      backTpBtn.OnTeleportStart = firstRoom.OnRoomLeave;

      var posFwd = CalcEntryPointForRoom(firstRoom.gameObject, firstRoom.RoomData);

      var fwdTpButton = CreateTeleport(lby, posFwd, Vector3.zero, "Go to Exhibition");
      fwdTpButton.OnTeleportEnd = firstRoom.OnRoomEnter;
    }

    /// <summary>
    /// Sets up teleportation for an imported exhibition.
    /// </summary>
    public void ImportedTpSetup()
    {
      var roomList = Instance.RoomList;

      if (roomList.Count <= 1)
      {
        return;
      }

      for (int i = 0; i < roomList.Count; i++)
      {
        CuboidExhibitionRoom currRoom = roomList[i];
        CuboidExhibitionRoom nextRoom = (i + 1 == roomList.Count) ? roomList[0] : roomList[i + 1];

        var posBack = CalcEntryPointForRoom(currRoom.gameObject, currRoom.RoomData);

        var backTpBtn = CreateTeleport(nextRoom.gameObject, posBack, new Vector3(0.3f, 0.0f, 0.0f), "Back");
        backTpBtn.OnTeleportStart = nextRoom.OnRoomLeave;
        backTpBtn.OnTeleportEnd = currRoom.OnRoomEnter;

        var posFwd = CalcEntryPointForRoom(nextRoom.gameObject, nextRoom.RoomData);

        var fwdTpButton = CreateTeleport(currRoom.gameObject, posFwd, new Vector3(-0.3f, 0.0f, 0.0f), "Forward");
        fwdTpButton.OnTeleportStart = currRoom.OnRoomLeave;
        fwdTpButton.OnTeleportEnd = nextRoom.OnRoomEnter;
      }
    }

    /// <summary>
    /// Sets up teleportation for a generated room.
    /// </summary>
    /// <param name="room">The generated room model.</param>
    public static void GeneratedTpSetup(Room room)
    {
      if (!room.Metadata.ContainsKey(GenerationMetadata.PredecessorRoom.GetKey()))
      {
        return;
      }

      // Get exhibit ID (could also be used to port player back in front of the exhibit).
      var roomId = room.Metadata[GenerationMetadata.PredecessorRoom.GetKey()];

      // Get ID of the room the exhibit is part of.
      var oldRoom = Instance.RoomList.Find(it => it.RoomData.Id == roomId);

      // Activate room.
      oldRoom.gameObject.SetActive(true);

      var newRoomGo = GameObject.Find(ObjectFactory.RoomNamePrefix + room.Id);
      var newRoom = newRoomGo.GetComponent<CuboidExhibitionRoom>();

      var lastPosX = 0.0f;

      var mainRoom = Instance.RoomList[0];
      var posMain = CalcEntryPointForRoom(mainRoom.gameObject, mainRoom.RoomData);

      if (Instance.settings.GenerationSettings.BackButton)
      {
        var mainTpBtn = CreateTeleport(newRoomGo, posMain, new Vector3(0.1f, 0.0f, 0.0f), "Back to start");
        mainTpBtn.OnTeleportStart = Instance.EnableOnlyMainRoom;
        mainTpBtn.OnTeleportEnd = Instance.RoomList.First().OnRoomEnter;
        lastPosX = -0.1f;
      }

      var posPrev = CalcEntryPointForRoom(oldRoom.gameObject, oldRoom.RoomData);

      var prevTpBtn = CreateTeleport(
        newRoomGo,
        posPrev,
        new Vector3(lastPosX, 0.0f, 0.0f),
        "Back to last"
      );
      prevTpBtn.OnTeleportStart = newRoom.OnRoomLeave;
      prevTpBtn.OnTeleportEnd = oldRoom.OnRoomEnter;

      // Deactivate room.
      oldRoom.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the position for a generated room based on its origin.
    /// </summary>
    /// <param name="origin">The game object the room was created from.</param>
    /// <param name="room">The generated room model.</param>
    private static void SetPositionForGeneratedRoom(GameObject origin, Room room)
    {
      const float heightIncrement = 15.0f;

      var cuboidRoom = origin.GetComponentInParent<CuboidExhibitionRoom>();

      // Get position relative position of exhibit.
      var roomPos = cuboidRoom.gameObject.transform.position;
      var displayalPos = origin.transform.position - roomPos;

      // Use room data coordinates (NOT the actual Unity ones, i.e., before the transformation).
      var originRoomPos = cuboidRoom.RoomData.Position;
      var originRoomSize = cuboidRoom.RoomData.Size;

      var angle = Mathf.Atan2(displayalPos.z, displayalPos.x);
      var radius = (float)(
        Math.Sqrt(Math.Pow(0.5 * originRoomSize.X, 2.0) + Math.Pow(0.5 * originRoomSize.Z, 2.0)) +
        Math.Sqrt(Math.Pow(0.5 * room.Size.X, 2.0) + Math.Pow(0.5 * room.Size.Z, 2.0))
      );

      // New position: x/z in direction of the button pressed to generate the room.
      room.Position = new Vector3f(
        originRoomPos.X + (float)Math.Cos(angle) * radius,
        originRoomPos.Y + heightIncrement,
        originRoomPos.Z + (float)Math.Sin(angle) * radius
      );
    }

    /// <summary>
    /// Fixes the server URL by adding the http:// prefix and/or trailing /.
    /// </summary>
    private void SanitizeHost()
    {
      // Add trailing /.
      if (!settings.VremAddress.EndsWith("/"))
      {
        settings.VremAddress += "/";
      }

      if (!settings.VremAddress.StartsWith("http://") && !settings.VremAddress.StartsWith("https://"))
      {
        settings.VremAddress = "http://" + settings.VremAddress;
      }
    }
  }
}