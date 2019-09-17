using UnityEngine;

namespace DefaultNamespace {
  public class ScalingUtility {

    /// <summary>
    /// Transforms sizes from meters to unity scales
    /// </summary>
    /// <param name="meterWidth">width in meters</param>
    /// <param name="meterHeight">height in meters</param>
    /// <returns></returns>
    public static Vector3 convertMeters2PlaneScaleSize(float meterWidth, float meterHeight) {
      return convertMeters2PlaceScaleSize(new Vector3(meterWidth, 1, meterHeight));
    }

    /// <summary>
    /// Transforms sizes from meters to unity scales
    /// </summary>
    /// <param name="meterSize">size in meters</param>
    /// <returns></returns>
    private static Vector3 convertMeters2PlaceScaleSize(Vector3 meterSize) {
      return meterSize / 10f;
    }
    
  }
}