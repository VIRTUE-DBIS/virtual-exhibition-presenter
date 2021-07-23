using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
  /// <summary>
  /// Represents a complex model, based on cuboids.
  /// Each cuboid has a position relative to the model's origin.
  ///
  /// This class is basically a list of (pos, cuboidmodel) tuples.
  /// </summary>
  [Serializable]
  public class ComplexCuboidModel : IModel
  {
    private List<Vector3> _positions = new List<Vector3>();
    private List<CuboidModel> _cuboids = new List<CuboidModel>();

    public ComplexCuboidModel(Vector3[] positions, CuboidModel[] models)
    {
      if (positions.Length != models.Length)
      {
        throw new ArgumentException("Must have equal amount of positions and cuboids");
      }

      _positions.AddRange(positions);
      _cuboids.AddRange(models);
    }

    public ComplexCuboidModel()
    {
    }

    public void Add(Vector3 position, CuboidModel cuboid)
    {
      _positions.Add(position);
      _cuboids.Add(cuboid);
    }

    public Vector3 GetPositionAt(int index)
    {
      return _positions[index];
    }

    public CuboidModel GetCuboidAt(int index)
    {
      return _cuboids[index];
    }

    public int Size()
    {
      return _positions.Count;
    }

    public bool IsEmpty()
    {
      return _cuboids.Count == 0;
    }
  }
}