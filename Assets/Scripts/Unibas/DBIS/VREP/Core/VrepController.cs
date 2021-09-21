﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Client;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Generation;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  public class VrepController : MonoBehaviour
  {
    public static VrepController Instance;
    public static Vector3 LobbySpawn = new Vector3(0, -9, 0);

    public ExhibitionManager exhibitionManager;

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

      // Create exhibition manager.
      exhibitionManager = gameObject.AddComponent<ExhibitionManager>();

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

    public async Task LoadInitialExhibition()
    {
      Exhibition ex;

      switch (settings.ExhibitionMode)
      {
        case GenerationMode.Static:
          ex = await LoadExhibitionById();
          break;
        case GenerationMode.GenerationVisual:
          ex = await GenerateExhibition(GenMethod.VisualSom);
          break;
        case GenerationMode.GenerationSemantic:
          ex = await GenerateExhibition(GenMethod.SemanticSom);
          break;
        default:
          Debug.LogError("Invalid exhibition mode specified!");
          throw new ArgumentOutOfRangeException();
      }

      await exhibitionManager.LoadNewExhibition(ex);
    }

    public async Task<Exhibition> LoadExhibitionById()
    {
      return await new ExhibitionApi().GetApiExhibitionsLoadIdWithIdAsync(settings.ExhibitionId);
    }

    public RoomSpecification GetRoomSpec()
    {
      return new RoomSpecification(settings.GenerationSettings.Height, settings.GenerationSettings.Width);
    }

    public int GetSeed()
    {
      return settings.GenerationSettings.Seed;
    }

    public int GetNumEpochs()
    {
      return settings.GenerationSettings.NumEpochs;
    }

    public async Task<Exhibition> GenerateExhibition(GenMethod method)
    {
      var ex = await new GenerationApi().PostApiGenerateExhibitionAsync();

      var room = await GenerateRoomByMethod(method);

      ex.Rooms.Add(room);

      return ex;
    }

    public async Task<Room> GenerateRandomRoom(List<String> idList)
    {
      var config = new RandomGenerationRequest(GetRoomSpec(), idList, GetSeed());
      return await new GenerationApi().PostApiGenerateRoomRandomAsync(config);
    }

    public async Task<Room> GenerateSimilarityRoom(SimilarityGenerationRequest.GenTypeEnum genType, string objectId)
    {
      var config = new SimilarityGenerationRequest(GetRoomSpec(), genType, objectId);
      return await new GenerationApi().PostApiGenerateRoomSimilarAsync(config);
    }

    public async Task<Room> GenerateSomRoom(SomGenerationRequest.GenTypeEnum genType, List<String> idList)
    {
      var config = new SomGenerationRequest(GetRoomSpec(), genType, idList, GetSeed(), GetNumEpochs());
      return await new GenerationApi().PostApiGenerateRoomSomAsync(config);
    }

    public async Task<Room> GenerateRoomByMethod(GenMethod type, List<string> ids = null, string originId = "")
    {
      ids ??= new List<string>();

      return type switch
      {
        GenMethod.RandomAll => await GenerateRandomRoom(new List<string>()),

        GenMethod.RandomList => await GenerateRandomRoom(ids),

        GenMethod.VisualSimilarity => await GenerateSimilarityRoom(SimilarityGenerationRequest.GenTypeEnum.VISUAL,
          originId),

        GenMethod.SemanticSimilarity => await GenerateSimilarityRoom(SimilarityGenerationRequest.GenTypeEnum.SEMANTIC,
          originId),

        GenMethod.VisualSom => await GenerateSomRoom(SomGenerationRequest.GenTypeEnum.VISUAL, ids),

        GenMethod.SemanticSom => await GenerateSomRoom(SomGenerationRequest.GenTypeEnum.SEMANTIC, ids),

        _ => throw new ArgumentOutOfRangeException()
      };
    }

    public async Task<Room> GenerateAndLoadRoomForExhibition(GameObject origin, GenMethod type)
    {
      var config = origin.GetComponent<IdConfig>();
      var room = await GenerateRoomByMethod(type, config.associatedIds, config.originId);

      // Calculate new room's position relative to the room the generation was issued from and set it in the model.
      SetPositionForGeneratedRoom(origin, room);

      // Teleport info, necessary for restoring generated exhibitions.
      room.Metadata[MetadataType.Predecessor.GetKey()] = origin.GetComponent<Displayal>().id;

      // Load the room like a normal room.
      await exhibitionManager.LoadRoom(room);

      // Add the newly generated room to the exhibition model.
      exhibitionManager.Exhibition.Rooms.Add(room);

      // Update references.
      // TODO Move metadata manipulation into the Metadata namespace.
      RoomReferences references;
      var model = origin.GetComponent<Displayal>().GetExhibit();

      if (model.Metadata.ContainsKey(MetadataType.References.GetKey()))
      {
        var refJson = model.Metadata[MetadataType.References.GetKey()];
        references = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomReferences>(refJson);
      }
      else
      {
        references = new RoomReferences();
      }

      // TODO Consider adding some sort of mapping for GenTypeEnum.
      references.References[type.ToString()] = room.Id;

      // Store the updated references as metadata.
      model.Metadata[MetadataType.References.GetKey()] = Newtonsoft.Json.JsonConvert.SerializeObject(references);

      // Teleport the player.
      TpPlayerToLocation(new Vector3(room.Position.X, room.Position.Y, room.Position.Z));

      return room;
    }

    public static void TpPlayerToObjPos(GameObject go)
    {
      TpPlayerToLocation(go.transform.position);
    }

    public static void TpPlayerToLocation(Vector3 loc)
    {
      var go = GameObject.FindWithTag("Player");

      if (go != null)
      {
        go.transform.position = loc;
      }
    }

    public static void TpPlayerToLobby()
    {
      var lby = GameObject.Find("Lobby");

      if (lby != null)
      {
        TpPlayerToLocation(LobbySpawn);
      }
    }

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

    public void LobbyTpSetup(CuboidExhibitionRoom firstRoom)
    {
      var lby = GameObject.Find("Lobby");

      if (lby == null)
      {
        return;
      }

      var backTpBtn = CreateTeleport(firstRoom.gameObject, lby.transform.position, Vector3.zero, "Back to Lobby");
      backTpBtn.OnTeleportStart = firstRoom.OnRoomLeave;

      var fwdTpButton = CreateTeleport(lby, firstRoom.transform.position, Vector3.zero, "Go to Exhibition");
      fwdTpButton.OnTeleportEnd = firstRoom.OnRoomEnter;
    }

    public void ImportedTpSetup()
    {
      var roomList = exhibitionManager.RoomList;

      if (roomList.Count <= 1)
      {
        return;
      }

      for (int i = 0; i < roomList.Count; i++)
      {
        CuboidExhibitionRoom currRoom = roomList[i];
        CuboidExhibitionRoom nextRoom = (i + 1 == roomList.Count) ? roomList[0] : roomList[i + 1];

        var backTpBtn = CreateTeleport(nextRoom.gameObject, currRoom.transform.position, new Vector3(0.3f, 0.0f, 0.0f),
          "Back");
        backTpBtn.OnTeleportStart = nextRoom.OnRoomLeave;
        backTpBtn.OnTeleportEnd = currRoom.OnRoomEnter;

        var fwdTpButton = CreateTeleport(currRoom.gameObject, nextRoom.transform.position,
          new Vector3(-0.3f, 0.0f, 0.0f), "Forward");
        fwdTpButton.OnTeleportStart = currRoom.OnRoomLeave;
        fwdTpButton.OnTeleportEnd = nextRoom.OnRoomEnter;
      }
    }

    public static void GeneratedTpSetup(Room room)
    {
      if (!room.Metadata.ContainsKey(MetadataType.Predecessor.GetKey()))
      {
        return;
      }

      var exhibitId = room.Metadata[MetadataType.Predecessor.GetKey()];

      var origin = GameObject.Find(Displayal.NamePrefix + exhibitId);

      var oldRoom = origin.GetComponentInParent<CuboidExhibitionRoom>();

      var newRoomGo = GameObject.Find(ObjectFactory.RoomNamePrefix + room.Id);
      var newRoom = newRoomGo.GetComponent<CuboidExhibitionRoom>();

      var backTpBtn = CreateTeleport(newRoomGo, oldRoom.transform.position, Vector3.zero, "Back");

      backTpBtn.OnTeleportStart = newRoom.OnRoomLeave;
      backTpBtn.OnTeleportEnd = oldRoom.OnRoomEnter;
    }

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

      // TODO TLS support.
      if (!settings.VremAddress.StartsWith("http://"))
      {
        settings.VremAddress = "http://" + settings.VremAddress;
      }
    }
  }
}