namespace Unibas.DBIS.VREP.World
{
  /// <summary>
  /// Store various settings for building exhibitions.
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

    private static ExhibitionBuildingSettings _instance;

    public static ExhibitionBuildingSettings Instance => _instance ??= new ExhibitionBuildingSettings();
  }
}