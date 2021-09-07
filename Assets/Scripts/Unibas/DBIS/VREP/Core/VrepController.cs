using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Client;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Core.Config;
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
    }

    public async Task GenerateAndLoadRoomForExhibition(GameObject parent, GenerationRequest.GenTypeEnum type,
      List<string> idList = null)
    {
      var room = await new GenerationApi().PostApiGenerateRoomAsync(CreateGenerationRequest(type, idList));

      // Get position of exhibit associated with the button and normalize the direction.
      var roomPos = parent.GetComponentInParent<CuboidExhibitionRoom>().gameObject.transform.position;
      var displayalRelativePos = parent.transform.position - roomPos; // TODO Try with localPosition.
      var displayalPosNorm = new Vector3(displayalRelativePos.x, 0.0f, displayalRelativePos.z).normalized;

      // Use room data coordinates (NOT the actual Unity ones, i.e., before the transformation).
      var oldRoomPos = parent.GetComponentInParent<CuboidExhibitionRoom>().RoomData.Position;
      var oldRoomSize = parent.GetComponentInParent<CuboidExhibitionRoom>().RoomData.Size;
      
      var angle = Mathf.Atan2(displayalRelativePos.z, displayalRelativePos.x);
      var radius = 30.0f;
      
      // New position: x/z in direction of the (normalized) button pressed to generate the room, y (height) simply +1.0.
      room.Position = new Vector3f(
        oldRoomPos.X + (float)Math.Cos(angle) * radius,
        oldRoomPos.Y + 15.0f,
        oldRoomPos.Z + (float)Math.Sin(angle) * radius
      );

      await exhibitionManager.LoadRoom(room);
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