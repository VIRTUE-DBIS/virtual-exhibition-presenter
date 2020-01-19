using System;
using DefaultNamespace;
using DefaultNamespace.VREM;
using DefaultNamespace.VREM.Model;
using Unibas.DBIS.VREP.Core;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP
{
    public class VREPController : MonoBehaviour
    {
        private VREMClient _vremClient;
        private BuildingManager _buildingManager;
        public String ExhibitionId = "5c17b10ea6abfddbb3fa66ae";

        public Vector3 LobbySpawn = new Vector3(0, -9, 0);

        public Settings Settings;

        public string settingsPath;

        public static VREPController Instance;
        
        private ExhibitionManager _exhibitionManager;

        private Player player;
        private PlayerTeleporter teleporter;
        
        private void Awake()
        {
            if (Application.isEditor)
            {
                if (string.IsNullOrEmpty(settingsPath))
                {
                    Settings = Settings.LoadSettings();
                }
                else
                {
                    Settings = Settings.LoadSettings(settingsPath);
                }
            }
            else
            {
                Settings = Settings.LoadSettings();
            }

            Instance = this;
        }

        

        private void OnApplicationQuit()
        {
            Settings.StoreSettings();
        }

        public void SpawnPlayerInLobby()
        {
            SpawnPlayerAt(new Vector3(0,-9.8f,0));
        }

        public void SpawnPlayerAt(Vector3 pos)
        {
            teleporter = gameObject.AddComponent<PlayerTeleporter>();
            teleporter.Destination = pos;
            teleporter.TeleportPlayer();
        }

        private void Start()
        {
            if (Settings == null)
            {
                Settings = Settings.LoadSettings();
                if (Settings == null)
                {
                    Settings = Settings.Default();
                }
            }
            
            var go = GameObject.FindWithTag("Player");
            if (go != null)
            {
                player = go.GetComponent<Player>();
            }
            if (Settings.StartInLobby)
            {
                SpawnPlayerInLobby();
            }

            var lby = GameObject.Find("Lobby");
            if (lby != null && !Settings.StartInLobby)
            {
                lby.SetActive(false);
            }

            Debug.Log("Starting ExMan");
            _vremClient = gameObject.AddComponent<VREMClient>();
            _buildingManager = GetComponent<BuildingManager>();

            LoadAndCreateExhibition();
        }

        public void LoadAndCreateExhibition()
        {
            _vremClient.ServerUrl = Settings.Server.Address;

            var exId = "";
            if (Settings.Server.Exhibitions != null && Settings.Server.Exhibitions.Length > 0 && Settings.Server.Exhibitions[0] != null)
            {
                exId = Settings.Server.Exhibitions[0];
            }
            else
            {
                exId = ExhibitionId;
            }


            _vremClient.RequestExhibition(exId, ParseExhibition);
            Debug.Log("Requested ex");
        }

        private void Update() {
            if (Input.GetKey(KeyCode.F12)) {
                _exhibitionManager.RestoreExhibits();
                var pos = _exhibitionManager.GetRoomByIndex(0).GetEntryPoint();
                SpawnPlayerAt(pos);
            }
        }

        private void ParseExhibition(string json)
        {
            if (json == null)
            {
                Debug.LogError("Couldn't load exhibition from backend");
                Debug.Log("Loading placeholder instead");
                var jtf = Resources.Load<TextAsset>("Configs/placeholder-exhibition");
                json = jtf.text;
            }

            Debug.Log(json);
            Exhibition ex = JsonUtility.FromJson<Exhibition>(json);
            Debug.Log(json);
            Debug.Log(ex);
            Debug.Log(_buildingManager);
            // TODO create lobby
            
            _exhibitionManager = new ExhibitionManager(ex);
            _exhibitionManager.GenerateExhibition();
            if (VREPController.Instance.Settings.StartInLobby)
            {
                GameObject.Find("Lobby").GetComponent<Lobby>().activateRoomTrigger(_exhibitionManager);
            }
            else
            {
                GameObject.Find("Room").gameObject.transform.Find("Timer").transform.GetComponent<MeshRenderer>().enabled = true;
            }
            
            //_buildingManager.Create(ex);
            
            
            //_buildingManager.BuildRoom(ex.rooms[0]);
/*
      if (Settings.PlaygroundEnabled)
      {*/
            /*
            ex.rooms[0].position = new Vector3(15,0,0);
            ObjectFactory.BuildRoom(ex.rooms[0]);
            */
            //    }
        }
    }
}