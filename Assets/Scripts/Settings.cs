using System;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Whether the player starts in the lobby
        /// Default: False
        /// </summary>
        public bool StartInLobby = false;

        /// <summary>
        /// The Address of the VREM server instance, inclusive port
        /// Usually this is 127.0.0.1:4567
        /// Default: 127.0.0.1:4567
        /// </summary>
        public string VREMAddress = "127.0.0.1:4567";

        /// <summary>
        /// Whether each exhibit has its own spotlight
        /// Default: False 
        /// </summary>
        public bool SpotsEnabled = false;

        /// <summary>
        /// Whether in the Lobby the UNIBAS logo is displayed on the floor
        /// Default: True
        /// </summary>
        public bool FloorLogoEnabled = true;

        /// <summary>
        /// Whether in the Lobby the UNIBAS logo is displayed on the ceiling
        /// Default: True
        /// </summary>
        public bool CeilingLogoEnabled = true;

        /// <summary>
        /// Whether experimental, "playground" features are enabled
        /// Default: False
        /// </summary>
        public bool PlaygroundEnabled = false;

        /// <summary>
        /// Whether the server will be queried for exhibitions
        /// Default: False
        /// </summary>
        public bool RequestExhibitions = false;

        /// <summary>
        /// A list of exhibition ids, which shall be loaded
        /// Default: Empty
        /// </summary>
        public string[] exhibitionIds = new string[0];

        /// <summary>
        /// The file name of this settings file.
        /// </summary>
        public const string FILE_NAME = "settings.json";

        private bool _default = false;

        /// <summary>
        /// Returns whether this settings file is the default one.
        /// </summary>
        /// <returns>Whether this settings file is the default one</returns>
        public bool IsDefault()
        {
            return _default;
        }

        /// <summary>
        /// Loads the settings file. In UnityEditor this is at Assets/settings.json, in standalone this should be a sibling of the executable
        /// </summary>
        /// <returns></returns>
        public static Settings LoadSettings()
        {
            Debug.Log("Settings path: "+getPath());
            if (File.Exists(getPath()))
            {
                string json = File.ReadAllText(getPath());
                return JsonUtility.FromJson<Settings>(json);
            }
            else
            {
                Settings s = new Settings();
                s._default = true;
                return s;
            }
        }

        private static string getPath()
        {
            return Application.dataPath + "/" + FILE_NAME;
        }

        /// <summary>
        /// Stores this settings file at the preferred location, if there isn't already one
        /// </summary>
        public void StoreSettings()
        {
            if (!File.Exists(getPath()))
            {
                string json = JsonUtility.ToJson(this,true);
                File.WriteAllText(getPath(),json);
            }
            else
            {
                Debug.LogWarning("Will not override settings");
            }
        }
    }
}