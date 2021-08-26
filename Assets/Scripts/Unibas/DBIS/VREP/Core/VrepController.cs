using System.Collections.Generic;
using System.Threading.Tasks;
using Unibas.DBIS.VREP.VREM;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;
using UnityEngine.Networking;
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

      var genConfig = new GenerationConfig(
        GenerationObject.EXHIBITION,
        GenerationType.VISUAL_SOM,
        new List<string>(),
        3,
        16,
        0
      );

      var ex = VremClient.Instance.Generate<Exhibition>(genConfig);

      Debug.Log("Waiting for request to complete.");

      await ex;

      Debug.Log("Parsed exhibition.");

      await exhibitionManager.LoadNewExhibition(ex.Result);

      Debug.Log("Loaded exhibition.");

      GenerateRoomForExhibition();
    }

    public async void GenerateRoomForExhibition()
    {
      var listJson = exhibitionManager.exhibition.rooms[0].metadata["som.ids"];
      var fullList = JsonConvert.DeserializeObject<NodeMap>(listJson);
      var firstList = fullList.map[0];
      List<string> idList = new List<string>();

      foreach (IdDoublePair t in firstList)
      {
        idList.Add(t.id);
      }

      var genConfig = new GenerationConfig(
        GenerationObject.ROOM,
        GenerationType.VISUAL_SOM,
        idList,
        3,
        16,
        0
      );

      var room = await VremClient.Instance.Generate<Room>(genConfig);
      room.position = new Vector3(0.0f, 1.0f, 0.0f);

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