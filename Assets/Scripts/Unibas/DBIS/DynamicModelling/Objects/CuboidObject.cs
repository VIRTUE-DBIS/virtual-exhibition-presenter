using System;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Objects
{
  /// <summary>
  /// Custom cuboid model.
  /// This model's size is actually the one specified, in unity units.<br />
  /// In contrast, the vanilla unity cube object has to be resized, which this one doesn't, to get other shapes than a cube.<br />
  /// 
  /// <b>Note</b> The material is copied, so modifications on the material are not reflected at runtime.
  ///
  ///
  /// UV Mapping is based on the smallest dimension, e.g. depending on the texture, further adjustments are required.<br />
  /// This can be achieved by accessing the MeshRenderer via GetComponent.<br />
  ///
  /// If this object's dimensions are changed during runtime, the caller has to call GenerateModel() afterwards,
  /// to reflect the changes on the model.
  /// </summary>
  [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(BoxCollider))]
  public class CuboidObject : MonoBehaviour, IObject
  {
    /// <summary>
    /// Creates a cuboid object with the specified dimensions and an optional material parameter
    /// </summary>
    /// <param name="width">The width in unity units. Width is along the x axis.</param>
    /// <param name="height">The height in unity units. Height is along the y axis.</param>
    /// <param name="depth">The depth in unity units. Depth is along the z axis.</param>
    /// <param name="material">The material to use (defaults to white)</param>
    /// <returns>A gameobject whose mesh and collider represent a cuboid with the specified dimensions.</returns>
    public static CuboidObject Create(float width, float height, float depth, Material material = null)
    {
      var go = new GameObject("CuboidObject");
      var co = go.AddComponent<CuboidObject>();
      co.width = width;
      co.height = height;
      co.depth = depth;
      co.material = material;
      return co;
    }

    /// <summary>
    /// The width in unity units. Width is along the x axis.
    /// </summary>
    public float width;

    /// <summary>
    /// The height in unity units. Height is along the y axis.
    /// </summary>
    public float height;

    /// <summary>
    /// The depth in untiy units. Depth is along the z axis.
    /// </summary>
    public float depth;

    /// <summary>
    /// The material to use for texturing this cuboid.
    /// Be aware that while drag'n'drop a material to this gameobject works in the scene view,
    /// it is not the same and this material will always override the one from the inspector.
    /// Use null if you want the default object material.
    /// </summary>
    public Material material;

    private void OnValidate()
    {
      GenerateModel();
    }

    private void Start()
    {
      GenerateModel();
    }

    /// <summary>
    /// Generates the mesh based on the object's configuration
    /// </summary>
    public void GenerateModel()
    {
      var meshFilter = GetComponent<MeshFilter>(); // No null value due to RequireComponent statements
      var meshRenderer = GetComponent<MeshRenderer>();
      var mesh = meshFilter.mesh;

      // The naming is always from the front and downwards looking! e.g. From the back, left and right is swapped
      var frontLeftDown = Vector3.zero;
      var frontRightDown = new Vector3(width, 0, 0);
      var frontLeftUp = new Vector3(0, height, 0);
      var frontRightUp = new Vector3(width, height, 0);

      var backLeftDown = new Vector3(0, 0, depth);
      var backRightDown = new Vector3(width, 0, depth);
      var backLeftUp = new Vector3(0, height, depth);
      var backRightUp = new Vector3(width, height, depth);

      var vertices = new[]
      {
        // Front
        frontLeftDown, frontRightDown, frontLeftUp, frontRightUp,
        // Back
        backLeftDown, backRightDown, backLeftUp, backRightUp,
        // Left
        backLeftDown, frontLeftDown, backLeftUp, frontLeftUp,
        // Right
        frontRightDown, backRightDown, frontRightUp, backRightUp,
        // Up
        frontLeftUp, frontRightUp, backLeftUp, backRightUp,
        // Down
        frontLeftDown, frontRightDown, backLeftDown, backRightDown
      };
      mesh.vertices = vertices;

      var triangles = new[]
      {
        // Front
        0, 2, 1, 2, 3, 1,
        // Back
        5, 7, 4, 7, 6, 4,
        // Left
        8, 10, 9, 10, 11, 9,
        // Right
        12, 14, 13, 14, 15, 13,
        // Up
        16, 18, 17, 18, 19, 17,
        // Down
        21, 23, 20, 23, 22, 20
      };
      mesh.triangles = triangles;

      var normals = new[]
      {
        // Front
        -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
        // Back
        -Vector3.back, -Vector3.back, -Vector3.back, -Vector3.back,
        // Left
        -Vector3.left, -Vector3.left, -Vector3.left, -Vector3.left,
        // Right
        -Vector3.right, -Vector3.right, -Vector3.right, -Vector3.right,
        // Up
        -Vector3.up, -Vector3.up, -Vector3.up, -Vector3.up,
        // Down
        -Vector3.down, -Vector3.down, -Vector3.down, -Vector3.down
      };
      mesh.normals = normals;


      /*
       * Unwrapping of mesh for uf like following
       *  U
       * LFRB
       *  D
      */

      var u = Math.Min(Math.Min(width, height), depth);
      // var w = width / u;
      // var h = height / u;
      // var d = depth / u;

      var uvUnits = new Vector2(u, u);
      var fOff = uvUnits.x * depth;
      var rOff = uvUnits.x * width + fOff;
      var bOff = uvUnits.x * depth + rOff;
      var uOff = uvUnits.y * depth + uvUnits.y * height;
      var uv = new[]
      {
        // Front
        new Vector2(fOff, uvUnits.y * depth), new Vector2(fOff + uvUnits.x * width, uvUnits.y * depth),
        new Vector2(fOff, uvUnits.y * depth + uvUnits.y * height),
        new Vector2(fOff + uvUnits.x * width, uvUnits.y * depth + uvUnits.y * height),

        // Back
        new Vector2(bOff, uvUnits.y * depth), new Vector2(bOff + uvUnits.x * width, uvUnits.y * depth),
        new Vector2(bOff, uvUnits.y * depth + uvUnits.y * height),
        new Vector2(bOff + uvUnits.x * width, uvUnits.y * depth + uvUnits.y * height),

        // Left
        new Vector2(0, uvUnits.y * depth), new Vector2(uvUnits.x * depth, uvUnits.y * depth),
        new Vector2(0, uvUnits.y * depth + uvUnits.y * height),
        new Vector2(uvUnits.x * depth, uvUnits.y * depth + uvUnits.y * height),
        // Right
        new Vector2(rOff, uvUnits.y * depth), new Vector2(rOff + uvUnits.x * depth, uvUnits.y * depth),
        new Vector2(rOff, uvUnits.y * depth + uvUnits.y * height),
        new Vector2(rOff + uvUnits.x * depth, uvUnits.y * depth + uvUnits.y * height),
        // Up
        new Vector2(fOff, uOff), new Vector2(fOff + uvUnits.x * width, uOff),
        new Vector2(fOff, uOff + uvUnits.y * depth),
        new Vector2(fOff + uvUnits.x * width, uOff + uvUnits.y * depth),

        // Down
        new Vector2(fOff, 0), new Vector2(fOff + uvUnits.x * width, 0), new Vector2(fOff, uvUnits.y * depth),
        new Vector2(fOff + uvUnits.x * width, uvUnits.y * depth)
      };

      mesh.uv = uv;

      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      mesh.RecalculateTangents();

      if (material == null)
      {
        var mat = new Material(Shader.Find("Standard"));
        meshRenderer.material = mat;
        mat.name = "Default";
        mat.color = Color.green;
      }
      else
      {
        Material mat;
        (mat = meshRenderer.material).CopyPropertiesFromMaterial(material);
        mat.name = material.name + " (Copy)";
      }

      var col = GetComponent<BoxCollider>();
      col.center = new Vector3(width / 2, height / 2, depth / 2);
      col.size = new Vector3(width, height, depth);
    }
  }
}