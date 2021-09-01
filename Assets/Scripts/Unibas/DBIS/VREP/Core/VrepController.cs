﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Client;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Core.Config;
using Unibas.DBIS.VREP.Generation;
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
      // Create exhibition manager.
      exhibitionManager = gameObject.AddComponent<ExhibitionManager>();

      // Reset player to lobby.
      if (settings.StartInLobby)
      {
        ResetPlayerToLobby();
      }

      // VREM Client settings.
      Configuration.Default.BasePath = settings.VremAddress;

      await LoadInitialExhibition();
    }

    public async Task LoadInitialExhibition()
    {
      Exhibition ex;

      switch (settings.ExhibitionMode)
      {
        case Mode.Generation:
          ex = await GenerateExhibition();
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

    public async Task<Exhibition> GenerateExhibition()
    {
      var genReq = new GenerationRequest(
        GenerationRequest.GenTypeEnum.VISUALSOM,
        new List<string>(),
        settings.GenerationSettings.Height,
        settings.GenerationSettings.Width,
        settings.GenerationSettings.Seed
      );

      return await new GenerationApi().PostApiGenerateExhibitionAsync(genReq);
    }

    public async Task GenerateRoomForExhibition()
    {
      var listJson = exhibitionManager.Exhibition.Rooms[0].Metadata[MetadataType.SomIds.GetKey()];
      var fullList = JsonConvert.DeserializeObject<NodeMap>(listJson);
      var firstList = fullList.map[0];
      var idList = new List<string>();

      foreach (IdDoublePair t in firstList)
      {
        idList.Add(t.id);
      }

      var genReq = new GenerationRequest(GenerationRequest.GenTypeEnum.VISUALSOM, idList, 1, 16, 0);
      var room = await new GenerationApi().PostApiGenerateRoomAsync(genReq);

      room.Position = new Vector3f(0.0f, 1.0f, 0.0f);

      await exhibitionManager.LoadRoom(room);

      new ExhibitionApi().PostApiExhibitionsSave(exhibitionManager.Exhibition);
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
  }
}