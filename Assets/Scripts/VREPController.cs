using System;
using DefaultNamespace;
using DefaultNamespace.VREM;
using DefaultNamespace.VREM.Model;
using UnityEngine;

namespace Unibas.DBIS.VREP {
  public class VREPController : MonoBehaviour {

    private VREMClient _vremClient;
    private BuildingManager _buildingManager;
    public String ExhibitionId = "5c17b10ea6abfddbb3fa66ae";

    public Vector3 LobbySpawn = new Vector3(0,-9,0);

    public Settings Settings;

    private void Awake()
    {
      Settings = Settings.LoadSettings();
    }

    private void OnApplicationQuit()
    {
      Settings.StoreSettings();
    }

    private void Start()
    {
      Debug.Log("Persistent Path: "+Application.persistentDataPath);
      var go = GameObject.FindWithTag("Player");
      if (go != null && Settings.StartInLobby)
      {
        go.transform.position = new Vector3(0,-9.9f,0);
      }
      Debug.Log("Starting ExMan");
      _vremClient = gameObject.AddComponent<VREMClient>();
      _buildingManager = GetComponent<BuildingManager>();
      LoadAndCreateExhibition();
    }

    public void LoadAndCreateExhibition() {
      _vremClient.ServerUrl = ServerSettings.SERVER_ID;
      _vremClient.RequestExhibition(this.ExhibitionId, ParseExhibition);
      Debug.Log("Requested ex");
    }

    private void ParseExhibition(string json) {
      Exhibition ex = JsonUtility.FromJson<Exhibition>(json);
      Debug.Log(json);
      Debug.Log(ex);
      Debug.Log(_buildingManager);
      // TODO create lobby
      _buildingManager.Create(ex);
      //_buildingManager.BuildRoom(ex.rooms[0]);
      
    }
    
  }
}