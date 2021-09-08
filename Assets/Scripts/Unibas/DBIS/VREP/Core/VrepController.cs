using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Client;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Core.Config;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  public class VrepController : MonoBehaviour
  {
    public static VrepController Instance;

    public Vector3 lobbySpawn = new Vector3(0, -9, 0);
    public ExhibitionManager exhibitionManager;

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
        ResetPlayerToLobby();
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
        case Mode.Generation:
          ex = await GenerateExhibition(GenerationRequest.GenTypeEnum.VISUALSOM);
          break;
        case Mode.Static:
          ex = await LoadExhibitionById();
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

    public GenerationRequest CreateGenerationRequest(GenerationRequest.GenTypeEnum type, List<string> idList = null)
    {
      idList ??= new List<string>();

      // TODO If the ID list is too short w.r.t. height/width, adjust height/width accordingly.

      return new GenerationRequest(
        type,
        idList,
        settings.GenerationSettings.Height,
        settings.GenerationSettings.Width,
        settings.GenerationSettings.Seed
      );
    }

    public async Task<Exhibition> GenerateExhibition(GenerationRequest.GenTypeEnum type, List<string> idList = null)
    {
      return await new GenerationApi().PostApiGenerateExhibitionAsync(CreateGenerationRequest(type, idList));
    }

    public async void GenerateAndLoadExhibition(GenerationRequest.GenTypeEnum type, List<string> idList = null)
    {
      var id = await GenerateExhibition(type, idList);

      await exhibitionManager.LoadNewExhibition(id);

      // TODO Teleport player into center of the generated exhibition (room).
    }

    public async Task GenerateAndLoadRoomForExhibition(GameObject origin, GenerationRequest.GenTypeEnum type,
      List<string> idList = null)
    {
      Room room = await new GenerationApi().PostApiGenerateRoomAsync(CreateGenerationRequest(type, idList));

      SetPositionForGeneratedRoom(origin, room);

      var displayal = origin.GetComponent<Displayal>();

      // Teleport info.
      room.Metadata[MetadataType.Predecessor.GetKey()] = displayal.id;

      // Check out GameObject.Find() and label rooms accordingly (IDs) in order to be able to easily teleport around.

      await exhibitionManager.LoadRoom(room);

      // TODO Move this into LoadRoom (also needed to load an exhibition!).
      TpSetup(origin, room);

      // TODO Teleport player into new room once it's ready.
    }

    private void TpSetup(GameObject origin, Room room)
    {
      var model = new SteamVRTeleportButton.TeleportButtonModel(0.1f, 0.02f, 1.0f, null,
        TexturingUtility.LoadMaterialByName("NMetal"),
        TexturingUtility.LoadMaterialByName("NPlastic"));

      var oldRoom = origin.GetComponentInParent<CuboidExhibitionRoom>().gameObject;
      var newRoom = GameObject.Find(ObjectFactory.RoomNamePrefix + room.Id);

      var backTpBtn = SteamVRTeleportButton.Create(
        newRoom,
        Vector3.zero,
        oldRoom.transform.position,
        model,
        "my text"
      );

      // backTpBtn.OnTeleportStart = room.OnRoomLeave;
      // backTpBtn.OnTeleportEnd = prev.OnRoomEnter;
    }

    public void ResetPlayerToLobby()
    {
      var go = GameObject.FindWithTag("Player");
      var lby = GameObject.Find("Lobby");

      if (go != null && lby != null)
      {
        go.transform.position = new Vector3(0, -9.9f, 0);
        lby.SetActive(true);
      }
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