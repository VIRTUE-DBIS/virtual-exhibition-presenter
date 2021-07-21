using System;
using System.IO;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  [Serializable]
  public class Settings
  {
    /// <summary>
    /// Whether the player starts in the lobby.
    /// Default: False
    /// </summary>
    public bool StartInLobby = true;

    /// <summary>
    /// The Address of the VREM server instance, inclusive port.
    /// </summary>
    public string VREMAddress;

    /// <summary>
    /// Whether each exhibit has its own spotlight.
    /// Default: False 
    /// </summary>
    public bool SpotsEnabled;

    /// <summary>
    /// Whether in the Lobby the Unibas logo is displayed on the floor.
    /// Default: True
    /// </summary>
    public bool LobbyFloorLogoEnabled = true;

    /// <summary>
    /// Whether in the Lobby the Unibas logo is displayed on the ceiling.
    /// Default: True
    /// </summary>
    public bool LobbyCeilingLogoEnabled = true;

    /// <summary>
    /// Whether in each room the Logo is placed on the ceiling.
    /// </summary>
    public bool CeilingLogoEnabled = true;

    /// <summary>
    /// Whether in each room a timer is placed on the wall, counting down provided seconds.
    /// </summary>
    public int WallTimerCount;

    /// <summary>
    /// Whether experimental, "playground" features are enabled.
    /// Default: False
    /// </summary>
    public bool PlaygroundEnabled;

    /// <summary>
    /// Whether the server will be queried for exhibitions.
    /// Default: False
    /// </summary>
    public bool RequestExhibitions;

    /// <summary>
    /// The ID of the exhibition to load.
    /// Default: Empty
    /// </summary>
    public string ExhibitionId;

    /// <summary>
    /// The file name of this settings file.
    /// </summary>
    public const string FileName = "settings.json";

    private bool _default;

    /// <summary>
    /// Returns whether this settings file is the default one.
    /// </summary>
    /// <returns>True if this is the default config, false otherwise.</returns>
    public bool IsDefault()
    {
      return _default;
    }

    /// <summary>
    /// Loads the settings file.
    /// In UnityEditor this is at Assets/settings.json, in standalone this should be a sibling of the executable.
    /// </summary>
    /// <returns></returns>
    public static Settings LoadSettings()
    {
      Debug.Log("Settings path: " + GetPath());
      if (File.Exists(GetPath()))
      {
        var json = File.ReadAllText(GetPath());
        return JsonUtility.FromJson<Settings>(json);
      }
      else
      {
        return CreateDefault();
      }
    }

    public static Settings LoadSettings(string path)
    {
      var filePath = Application.dataPath + "/" + path;

      if (!File.Exists(filePath)) return CreateDefault();

      var json = File.ReadAllText(filePath);
      return JsonUtility.FromJson<Settings>(json);
    }

    private Settings()
    {
      // no public constructor allowed
    }

    private static Settings CreateDefault()
    {
      var s = new Settings {_default = true};
      return s;
    }

    public static Settings Default()
    {
      return CreateDefault();
    }

    private static string GetPath()
    {
      if (Application.isEditor)
      {
        return Application.dataPath + "/" + FileName;
      }
      else
      {
        return Application.dataPath + "/../" + FileName;
      }
    }

    /// <summary>
    /// Stores this settings file at the preferred location, if there isn't already one.
    /// </summary>
    public void StoreSettings()
    {
      if (!File.Exists(GetPath()))
      {
        var json = JsonUtility.ToJson(this, true);
        File.WriteAllText(GetPath(), json);
      }
      else
      {
        Debug.Log("Will not override settings.");
      }
    }
  }
}