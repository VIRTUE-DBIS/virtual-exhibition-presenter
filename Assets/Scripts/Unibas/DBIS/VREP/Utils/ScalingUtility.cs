﻿using UnityEngine;

namespace Unibas.DBIS.VREP.Utils
{
  /// <summary>
  /// Utility class for scaling vectors from meters to plane scale and back.
  /// </summary>
  public static class ScalingUtility
  {
    /// <summary>
    /// Transforms sizes from meters to unity scales.
    /// </summary>
    /// <param name="meterWidth">The width in meters.</param>
    /// <param name="meterHeight">The height in meters.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector3 ConvertMeters2PlaneScaleSize(float meterWidth, float meterHeight)
    {
      return ConvertMeters2PlaceScaleSize(new Vector3(meterWidth, 1, meterHeight));
    }

    /// <summary>
    /// Transforms sizes from meters to unity scales.
    /// </summary>
    /// <param name="meterSize">The size in meters.</param>
    /// <returns>The transformed vector.</returns>
    private static Vector3 ConvertMeters2PlaceScaleSize(Vector3 meterSize)
    {
      return meterSize / 10f;
    }
  }
}