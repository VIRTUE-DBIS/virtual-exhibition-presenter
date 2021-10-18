using System;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
  /// <summary>
  /// Abstract cuboid representation.
  /// A cuboid consists of three parameters:
  /// Width, Height and Depth.
  /// In Unity3d these parameters are along the x, y and z axis.
  /// 
  /// </summary>
  [Serializable]
  public class CuboidModel : IModel
  {
    /// <summary>
    /// The width of the cuboid (along the X axis) in standard units.
    /// </summary>
    public float width;

    /// <summary>
    /// The height of the cuboid (along the Y axis) in standard units.
    /// </summary>
    public float height;

    /// <summary>
    /// The depth of the cuboid (along the Z axis) in standard units.
    /// </summary>
    public float depth;

    /// <summary>
    /// The cuboid's material, iff any.
    /// </summary>
    public Material material;

    /// <summary>
    /// Creates a new cuboid based on its width, height and depth.
    /// Also optionally with a material
    /// </summary>
    /// <param name="width">The width of the cuboid in standard units. Width is along the x axis.</param>
    /// <param name="height">The height of the cuboid in standard units. Height is along the y axis.</param>
    /// <param name="depth">The depth of the cuboid in standard units. Depth is along the z axis.</param>
    /// <param name="material">The optional material of the cuboid. Otherwise it will be white</param>
    public CuboidModel(float width, float height, float depth, Material material = null)
    {
      this.width = width;
      this.height = height;
      this.depth = depth;
      this.material = material;
    }
  }
}