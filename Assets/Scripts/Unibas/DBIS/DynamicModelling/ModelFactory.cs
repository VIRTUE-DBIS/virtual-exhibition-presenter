using System;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling
{
  public static class ModelFactory
  {
    /// <summary>
    /// Quad of sorts:
    ///
    /// c---d
    /// |   |
    /// a---b
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    public static GameObject CreateFreeformQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d,
      Material material = null)
    {
      var go = new GameObject("FreeformQuad");
      var meshFilter = go.AddComponent<MeshFilter>();
      var meshRenderer = go.AddComponent<MeshRenderer>();
      var mesh = meshFilter.mesh;
      var vertices = new[]
      {
        a, b, c, d
      };
      mesh.vertices = vertices;

      var tri = new int[6];

      tri[0] = 0;
      tri[1] = 2;
      tri[2] = 1;

      tri[3] = 2;
      tri[4] = 3;
      tri[5] = 1;

      mesh.triangles = tri;

      /*
      Vector3[] normals = new Vector3[4];

      normals[0] = -Vector3.forward;
      normals[1] = -Vector3.forward;
      normals[2] = -Vector3.forward;
      normals[3] = -Vector3.forward;

      mesh.normals = normals;*/

      var uv = new Vector2[4];

      /*
      float xUnit = 1;
      float yUnit = 1;

      if (width > height)
      {
          xUnit = width / height;
      }
      else
      {
          yUnit = height / width;
      }
      */

      // TODO

      uv[0] = new Vector2(0, 0);
      uv[1] = new Vector2(1, 0);
      uv[2] = new Vector2(0, 1);
      uv[3] = new Vector2(1, 1);

      mesh.uv = uv;

      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      mesh.RecalculateTangents();

      if (material != null)
      {
        var mat = meshRenderer.material;
        mat.CopyPropertiesFromMaterial(material);
        //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
        mat.name = material.name + "(Instance)";
      }
      else
      {
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.white;
      }


      return go;
    }

    /// <summary>
    /// Creates a wall between two positions
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="height"></param>
    /// <param name="materialName"></param>
    /// <returns></returns>
    public static GameObject CreatePositionedWall(Vector3 start, Vector3 end, float height,
      string materialName = null)
    {
      var width = Vector3.Distance(start, end);
      var a = Vector3.Angle(end - start, Vector3.right);
      var go = new GameObject("PositionedWall");
      var wall = CreateWall(width, height, materialName);

      wall.transform.parent = go.transform;
      wall.transform.position = Vector3.zero;
      wall.transform.Rotate(Vector3.up, -a);
      go.transform.position = start;
      return go;
    }

    public static GameObject CreateWall(WallModel model)
    {
      var width = Vector3.Distance(model.start, model.end);
      var a = Vector3.Angle(model.start - model.end, Vector3.right);
      var go = new GameObject("PositionedWall");
      var wall = CreateWall(width, model.height, model.material);

      wall.transform.parent = go.transform;
      wall.transform.position = Vector3.zero;
      wall.transform.Rotate(Vector3.up, -a);
      go.transform.position = model.start;
      go.AddComponent<ModelContainer>().Model = model;
      return go;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">center of room</param>
    /// <param name="size"></param>
    /// <param name="height"></param>
    /// <param name="materialNames">0 floor, 1 ceiling, 2 north (pos z), 3 east (pos x), 4 south (neg z), 5 west (neg x)</param>
    /// <returns></returns>
    public static GameObject CreateCuboidRoom(Vector3 position, float size, float height, string[] materialNames)
    {
      var root = new GameObject("SquareRoom");

      var halfSize = size / 2f;

      // North wall
      var north = CreateWall(size, height, materialNames[2]);
      north.name = "NorthWall";
      north.transform.parent = root.transform;
      north.transform.position = new Vector3(-halfSize, 0, halfSize);
      // East wall
      var east = CreateWall(size, height, materialNames[3]);
      east.name = "EastWall";
      east.transform.parent = root.transform;
      east.transform.position = new Vector3(halfSize, 0, halfSize);
      east.transform.Rotate(Vector3.up, 90);
      // South wall
      var south = CreateWall(size, height, materialNames[4]);
      south.name = "SouthWall";
      south.transform.parent = root.transform;
      south.transform.position = new Vector3(halfSize, 0, -halfSize);
      south.transform.Rotate(Vector3.up, 180);
      // West wall
      var west = CreateWall(size, height, materialNames[5]);
      west.name = "WestWall";
      west.transform.parent = root.transform;
      west.transform.position = new Vector3(-halfSize, 0, -halfSize);
      west.transform.Rotate(Vector3.up, 270);

      // Floor
      var floorAnchor = new GameObject("FloorAnchor");
      floorAnchor.transform.parent = root.transform;

      var floor = CreateWall(size, size, materialNames[0]);
      floor.name = "Floor";
      floor.transform.parent = floorAnchor.transform;
      // North Aligned
      floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
      floorAnchor.transform.Rotate(Vector3.right, 90);
      // East Aligned
      //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
      //floorAnchor.transform.Rotate(Vector3f.back,90);

      // Ceiling
      var ceilingAnchor = new GameObject("CeilingAnchor");
      ceilingAnchor.transform.parent = root.transform;

      var ceiling = CreateWall(size, size, materialNames[1]);
      ceiling.name = "Ceiling";
      ceiling.transform.parent = ceilingAnchor.transform;

      root.transform.position = position;
      // North Aligned
      ceilingAnchor.transform.position = new Vector3(halfSize, height, halfSize);
      ceilingAnchor.transform.Rotate(Vector3.right, -90);
      // East Aligned
      //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
      //ceilingAnchor.transform.Rotate( Vector3.back, -90);

      return root;
    }

    public static GameObject CreateCuboidRoom(CuboidRoomModel model)
    {
      var root = new GameObject("CuboidRoom");

      var halfSize = model.size / 2f;

      // North wall
      var north = CreateWall(model.size, model.height, model.northMaterial);
      north.name = "NorthWall";
      north.transform.parent = root.transform;
      north.transform.position = new Vector3(-halfSize, 0, halfSize);
      // East wall
      var east = CreateWall(model.size, model.height, model.eastMaterial);
      east.name = "EastWall";
      east.transform.parent = root.transform;
      east.transform.position = new Vector3(halfSize, 0, halfSize);
      east.transform.Rotate(Vector3.up, 90);
      // South wall
      var south = CreateWall(model.size, model.height, model.southMaterial);
      south.name = "SouthWall";
      south.transform.parent = root.transform;
      south.transform.position = new Vector3(halfSize, 0, -halfSize);
      south.transform.Rotate(Vector3.up, 180);
      // West wall
      var west = CreateWall(model.size, model.height, model.westMaterial);
      west.name = "WestWall";
      west.transform.parent = root.transform;
      west.transform.position = new Vector3(-halfSize, 0, -halfSize);
      west.transform.Rotate(Vector3.up, 270);

      // Floor
      var floorAnchor = new GameObject("FloorAnchor");
      floorAnchor.transform.parent = root.transform;

      var floor = CreateWall(model.size, model.size, model.floorMaterial);
      floor.name = "Floor";
      floor.transform.parent = floorAnchor.transform;
      // North Aligned
      floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
      floorAnchor.transform.Rotate(Vector3.right, 90);
      // East Aligned
      //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
      //floorAnchor.transform.Rotate(Vector3f.back,90);

      // Ceiling
      var ceilingAnchor = new GameObject("CeilingAnchor");
      ceilingAnchor.transform.parent = root.transform;

      var ceiling = CreateWall(model.size, model.size, model.ceilingMaterial);
      ceiling.name = "Ceiling";
      ceiling.transform.parent = ceilingAnchor.transform;


      // North Aligned
      ceilingAnchor.transform.position = new Vector3(-halfSize, model.height, halfSize);
      ceilingAnchor.transform.Rotate(Vector3.right, -90);
      // East Aligned
      //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
      //ceilingAnchor.transform.Rotate( Vector3.back, -90);

      root.transform.position = model.position;

      root.AddComponent<ModelContainer>().Model = model;
      return root;
    }


    /// <summary>
    /// Creates a wall game object to position later.
    /// The wall is flat and always "upright", e.g. the normal of the mesh is negative z.
    /// Use the resulting gameobject to rotate and re-position the wall.
    /// </summary>
    /// <param name="width">The width of the wall in Unity units</param>
    /// <param name="height">The height of the wall in Unity units</param>
    /// <param name="materialName">The wall's material name. Expects the material file to be at Resources/Materials/materalName. If no present, the word Material will be suffixed.</param>
    /// <returns></returns>
    public static GameObject CreateWall(float width, float height, string materialName = null)
    {
      return CreateWall(width, height, LoadMaterialByName(materialName));
    }

    private static Material LoadMaterialByName(string materialName)
    {
      if (!string.IsNullOrEmpty(materialName))
      {
        if (!materialName.EndsWith("Material"))
        {
          materialName = materialName + "Material";
        }

        return Resources.Load<Material>("Materials/" + materialName);
      }

      return null;
    }

    public static GameObject CreateWall(float width, float height, Material mat = null)
    {
      var go = new GameObject("Wall");
      var meshFilter = go.AddComponent<MeshFilter>();
      var meshRenderer = go.AddComponent<MeshRenderer>();
      var mesh = meshFilter.mesh;
      var vertices = new Vector3[4];
      vertices[0] = new Vector3(0, 0, 0);
      vertices[1] = new Vector3(width, 0, 0);
      vertices[2] = new Vector3(0, height, 0);
      vertices[3] = new Vector3(width, height, 0);

      mesh.vertices = vertices;

      var tri = new int[6];

      tri[0] = 0;
      tri[1] = 2;
      tri[2] = 1;

      tri[3] = 2;
      tri[4] = 3;
      tri[5] = 1;

      mesh.triangles = tri;

      var normals = new Vector3[4];

      normals[0] = -Vector3.forward;
      normals[1] = -Vector3.forward;
      normals[2] = -Vector3.forward;
      normals[3] = -Vector3.forward;

      mesh.normals = normals;

      var uv = new Vector2[4];

      float xUnit = 1;
      float yUnit = 1;

      if (width > height)
      {
        xUnit = width / height;
      }
      else
      {
        yUnit = height / width;
      }

      uv[0] = new Vector2(0, 0);
      uv[1] = new Vector2(xUnit, 0);
      uv[2] = new Vector2(0, yUnit);
      uv[3] = new Vector2(xUnit, yUnit);

      mesh.uv = uv;

      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      mesh.RecalculateTangents();

      if (mat != null)
      {
        var material = meshRenderer.material;
        material.CopyPropertiesFromMaterial(mat);
        //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
        material.name = mat.name;
      }
      else
      {
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.white;
      }

      // TODO Highly experimental!

      var boxCollider = go.AddComponent<BoxCollider>();
      boxCollider.size = new Vector3(width, height, 0.0001f);

      return go;
    }


    private static Vector3 CalculateUnit(Vector3 dimensions)
    {
      var m = Math.Max(Math.Max(dimensions.x, dimensions.y), dimensions.z);
      return new Vector3(m / dimensions.x, m / dimensions.y, m / dimensions.z);
    }

    private static Vector2 CalculateUnit(float width, float height)
    {
      return CalculateNormalizedToLeastSquareUnit(width, height);
    }

    private static Vector2 CalculateNormalizedToLeastSquareUnit(float width, float height)
    {
      float xUnit = 1,
        yUnit = 1;

      if (width > height)
      {
        xUnit = width / height;
      }
      else
      {
        yUnit = height / width;
      }

      return new Vector2(xUnit, yUnit);
    }

    private static Vector2 CalculateNormalizedToOneUnit(float width, float height)
    {
      return new Vector2(1f / width, 1f / height);
    }

    public static GameObject CreateCuboid(CuboidModel cuboid)
    {
      var cub = CreateCuboid(cuboid.width, cuboid.height, cuboid.depth);
      var meshRenderer = cub.GetComponent<MeshRenderer>();
      if (cuboid.material != null)
      {
        meshRenderer.material.CopyPropertiesFromMaterial(cuboid.material);
      }
      else
      {
        var material = new Material(Shader.Find("Standard"));
        meshRenderer.material = material;
        material.name = "Default";
        material.color = Color.white;
      }

      cub.AddComponent<ModelContainer>().Model = cuboid;
      return cub;
    }

    public static GameObject CreateCuboid(float width, float height, float depth)
    {
      var go = new GameObject("Cuboid");
      var meshFilter = go.AddComponent<MeshFilter>();
      var meshRenderer = go.AddComponent<MeshRenderer>();
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

      var material = new Material(Shader.Find("Standard"));
      meshRenderer.material = material;
      material.name = "Default";
      material.color = Color.green;

      return go;
    }

    public static GameObject CreateModel(ComplexCuboidModel model)
    {
      var root = new GameObject("ComplexCuboid");
      for (var i = 0; i < model.Size(); i++)
      {
        var pos = model.GetPositionAt(i);
        var cuboid = model.GetCuboidAt(i);
        var cub = CreateCuboid(cuboid);
        cub.transform.parent = root.transform;
        cub.transform.position = pos;
      }

      root.AddComponent<ModelContainer>().Model = model;
      return root;
    }
  }
}