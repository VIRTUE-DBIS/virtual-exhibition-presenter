using System;
using DefaultNamespace.VREM;
using DefaultNamespace.VREM.Model;
using UnityEngine;

namespace DefaultNamespace {
  public class ExhibitionManager : MonoBehaviour {

    private RESTClient _restClient;
    private BuildingManager _buildingManager;
    public String ExhibitionId = "5c17b10ea6abfddbb3fa66ae";

    private void Start() {
      Debug.Log("Starting ExMan");
      _restClient = gameObject.AddComponent<RESTClient>();
      _buildingManager = GetComponent<BuildingManager>();
      LoadAndCreateExhibition();
    }

    public void LoadAndCreateExhibition() {
      _restClient.ServerUrl = ServerSettings.SERVER_ID;
      _restClient.RequestExhibition(this.ExhibitionId, ParseExhibition);
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