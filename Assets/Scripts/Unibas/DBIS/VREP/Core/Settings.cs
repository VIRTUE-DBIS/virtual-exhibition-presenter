using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.Core.Config
{
  public enum Mode
  {
    Static,
    Generation
  }

  [Serializable]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class GenerationSettings
  {
    public int Height = 1;

    public int Width = 16;

    public int Seed = 0;

    public int NumEpochs = 1;
  }

  /// <summary>
  /// Settings class; serialized from/to JSON.
  /// </summary>
  [Serializable]
  public class Settings
  {
    /// <summary>
    /// The Address of the VREM server instance, inclusive port.
    /// </summary>
    public string VremAddress;

    /// <summary>
    /// Whether in each room a timer is placed on the wall, counting down provided seconds.
    /// </summary>
    public int WallTimerCount;

    /// <summary>
    /// Whether the player starts in the lobby.
    /// Default: False
    /// </summary>
    public bool StartInLobby = true;

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
    /// Whether experimental features are enabled.
    /// Default: False
    /// </summary>
    public bool PlaygroundEnabled;

    public Mode ExhibitionMode = Mode.Static;

    /// <summary>
    /// The ID of the exhibition to load.
    /// Default: Empty
    /// </summary>
    public string ExhibitionId;

    public GenerationSettings GenerationSettings = new GenerationSettings();

    /// <summary>
    /// The file name of this settings file.
    /// </summary>
    [NonSerialized] public const string FileName = "settings.json";

    [NonSerialized] private bool _default;

    /// <summary>
    /// Returns whether this settings object is a default configuration (unless you programatically changed attributes).
    /// </summary>
    /// <returns>True if this is the default config, false otherwise.</returns>
    public bool IsDefault()
    {
      return _default;
    }

    /// <summary>
    /// Loads the settings file.
    /// In the UnityEditor this is at Assets/settings.json, in the standalone this should be a sibling of the executable.
    /// Default settings are used if the settings file cannot be read.
    /// </summary>
    /// <returns>The resulting Settings object.</returns>
    public static Settings LoadSettings()
    {
      Debug.Log("Settings path: " + GetPath());

      if (File.Exists(GetPath()))
      {
        var json = File.ReadAllText(GetPath());
        return JsonConvert.DeserializeObject<Settings>(json);
      }

      return CreateDefault();
    }

    /// <summary>
    /// Loads the settings file from a provided path.
    /// Default settings are used if the settings file cannot be read.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>The resulting Settings object.</returns>
    public static Settings LoadSettings(string path)
    {
      var filePath = Application.dataPath + "/" + path;

      if (!File.Exists(filePath)) return CreateDefault();

      var json = File.ReadAllText(filePath);
      return JsonConvert.DeserializeObject<Settings>(json);
    }

    private Settings()
    {
      // Settings can only be loaded via LoadSettings or Default!
    }

    /// <summary>
    /// Creates default settings.
    /// </summary>
    /// <returns>The created default Settings object.</returns>
    private static Settings CreateDefault()
    {
      var s = new Settings { _default = true };
      return s;
    }

    /// <summary>
    /// Creates default settings.
    /// </summary>
    /// <returns>The created default Settings object.</returns>
    public static Settings Default()
    {
      return CreateDefault();
    }

    /// <summary>
    /// Obtains the path of the configuration file, depending on whether we're running from the editor or not.
    /// For the actual deployment, the configuration file is assumed to be a sibling of the executable.
    /// </summary>
    /// <returns>The path to the configuration file.</returns>
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
    /// Stores this settings file at the preferred location unless the file already exists.
    /// </summary>
    public void StoreSettings()
    {
      if (!File.Exists(GetPath()))
      {
        var json = JsonConvert.SerializeObject(this);
        File.WriteAllText(GetPath(), json);
      }
      else
      {
        Debug.Log("Configuration already exists, not overriding settings.");
      }
    }
  }
}