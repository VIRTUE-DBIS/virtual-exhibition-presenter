namespace Unibas.DBIS.VREP.World
{
  /// <summary>
  /// Class to store various exhibition building related settings.
  /// 
  /// </summary>
  [System.Serializable]
  public class ExhibitionBuildingSettings
  {
    /// <summary>
    /// Positive offset between the wall and the displayal.
    /// </summary>
    public float WallOffset => 0.125f;

    public float RoomOffset => 2f;

    public bool useStandardDisplayalPrefab = true;

    public string standardDisplayalPrefabName = "Displayal";
    public string throwableDisplayalPrefabName = "ThrowableDisplayal";

    private static ExhibitionBuildingSettings instance;

    public static ExhibitionBuildingSettings Instance => instance ??= new ExhibitionBuildingSettings();
  }
}