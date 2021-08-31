using System.Collections.Generic;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Client;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.VREM;
using UnityEngine;
using Valve.Newtonsoft.Json;

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
      // Fix URL.
      SanitizeHost();

      // Create exhibition manager.
      exhibitionManager = gameObject.AddComponent<ExhibitionManager>();

      // Reset player to lobby.
      if (settings.StartInLobby)
      {
        ResetPlayerToLobby();
      }

      Debug.Log("Making generation request.");

      var genReq = new GenerationRequest(GenerationRequest.GenTypeEnum.VISUALSOM, new List<string>(), 1, 16, 0);
      var ex = await new GenerationApi(new Configuration().BasePath = "http://localhost:4545")
        .PostApiGenerateExhibitionAsync(genReq);

      Debug.Log("Parsed exhibition.");

      await exhibitionManager.LoadNewExhibition(ex);

      Debug.Log("Loaded exhibition.");

      GenerateRoomForExhibition();
    }

    public async void GenerateRoomForExhibition()
    {
      var listJson = exhibitionManager.exhibition.Rooms[0].Metadata["som.ids"];
      var fullList = JsonConvert.DeserializeObject<NodeMap>(listJson);
      var firstList = fullList.map[0];
      List<string> idList = new List<string>();

      foreach (IdDoublePair t in firstList)
      {
        idList.Add(t.id);
      }
      
      var genReq = new GenerationRequest(GenerationRequest.GenTypeEnum.VISUALSOM, new List<string>(), 1, 16, 0);
      var room = await new GenerationApi(new Configuration().BasePath = "http://localhost:4545")
        .PostApiGenerateRoomAsync(genReq);

      room.Position = new Vector3f(0.0f, 1.0f, 0.0f);

      await exhibitionManager.LoadRoom(room);
    }

    public void LoadExhibition()
    {
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

    private void SanitizeHost()
    {
      if (!settings.VremAddress.EndsWith("/"))
      {
        settings.VremAddress += "/";
      }

      if (!settings.VremAddress.StartsWith("http://"))
      {
        settings.VremAddress = "http://" + settings.VremAddress;
      }
    }
  }
}