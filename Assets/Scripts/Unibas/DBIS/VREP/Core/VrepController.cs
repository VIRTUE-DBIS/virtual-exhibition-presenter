﻿using Unibas.DBIS.VREP.VREM;
using Unibas.DBIS.VREP.VREM.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  public class VrepController : MonoBehaviour
  {
    private VremClient _vremClient;

    public Vector3 lobbySpawn = new Vector3(0, -9, 0);

    public Settings settings;
    public string settingsPath;

    public static VrepController Instance;
    private ExhibitionManager _exhibitionManager;

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

      SanitizeHost();
      Instance = this;
    }

    private void SanitizeHost()
    {
      if (!settings.VREMAddress.EndsWith("/"))
      {
        settings.VREMAddress += "/";
      }

      // TODO: TLS support.
      if (!settings.VREMAddress.StartsWith("http://"))
      {
        settings.VREMAddress = "http://" + settings.VREMAddress;
      }
    }

    private void OnApplicationQuit()
    {
      settings.StoreSettings();
    }

    private void Start()
    {
      // TODO: After what happens in Awake, this is not necessary.
      settings ??= Settings.LoadSettings() ?? Settings.Default();

      var go = GameObject.FindWithTag("Player");
      if (go != null && settings.StartInLobby)
      {
        go.transform.position = new Vector3(0, -9.9f, 0);
      }

      var lby = GameObject.Find("Lobby");
      if (lby != null && !settings.StartInLobby)
      {
        lby.SetActive(false);
      }

      Debug.Log("Starting ExMan");
      _vremClient = gameObject.AddComponent<VremClient>();

      LoadAndCreateExhibition();
    }

    public void LoadAndCreateExhibition()
    {
      _vremClient.serverUrl = settings.VREMAddress;

      _vremClient.RequestExhibition(settings.ExhibitionId, ParseExhibition);
      Debug.Log("Requested ex");
    }

    private void Update()
    {
      if (Input.GetKey(KeyCode.F12))
      {
        _exhibitionManager.RestoreExhibits();
      }
    }

    private void ParseExhibition(string json)
    {
      if (json == null)
      {
        Debug.LogError("Couldn't load exhibition from backend.");
        Debug.Log("Loading placeholder instead.");
        var jtf = Resources.Load<TextAsset>("Configs/placeholder-exhibition");
        json = jtf.text;
      }

      Debug.Log(json);
      var ex = JsonUtility.FromJson<Exhibition>(json);
      // TODO Create lobby.

      _exhibitionManager = new ExhibitionManager(ex);
      _exhibitionManager.GenerateExhibition();

      if (Instance.settings.StartInLobby)
      {
        GameObject.Find("Lobby").GetComponent<Lobby>().ActivateRoomTrigger(_exhibitionManager);
      }
      else
      {
        GameObject.Find("Room").gameObject.transform.Find("Timer").transform.GetComponent<MeshRenderer>().enabled =
          true;
      }
    }
  }
}