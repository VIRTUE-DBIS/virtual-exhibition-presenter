using System;

namespace DefaultNamespace
{
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Whether the player starts in the lobby
        /// </summary>
        public bool StartInLobby = false;
        /// <summary>
        /// The Address of the VREM server instance, inclusive port
        /// Usually this is 127.0.0.1:4567
        /// </summary>
        public string VREMAddress = "127.0.0.1:4567";
        /// <summary>
        /// Whether each exhibit has its own spotlight 
        /// </summary>
        public bool SpotsEnabled = true;
        /// <summary>
        /// Whether in the Lobby the UNIBAS logo is displayed on the floor
        /// </summary>
        public bool FloorLogoEnabled = true;
        /// <summary>
        /// Whether in the Lobby the UNIBAS logo is displayed on the ceiling
        /// </summary>
        public bool CeilingLogoEnabled = true;
    }
}