namespace World
{
    
    /// <summary>
    /// Class to store various exhibition building related settings.
    /// 
    /// </summary>
    [System.Serializable]
    public class ExhibitionBuildingSettings
    {

        private float _wallOffset = 0.2f;

        /// <summary>
        /// Positive offset between the wall and the displayal.
        /// </summary>
        public float WallOffset
        {
            get { return _wallOffset; }
        }

        private float _roomOffset = 2f;

        public float RoomOffset
        {
            get { return _roomOffset; }
        }

        public bool UseStandardDisplayalPrefab = true;

        public string StandardDisplayalPrefabName = "Displayal";


        private static ExhibitionBuildingSettings _instance = null;

        public static ExhibitionBuildingSettings Instance
        {
            get { return _instance ?? (_instance = new ExhibitionBuildingSettings()); }
        }
    }
}