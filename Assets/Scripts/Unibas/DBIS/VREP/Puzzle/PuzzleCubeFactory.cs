using System;
using UnityEngine;

namespace Unibas.DBIS.VREP.Puzzle {
  public class PuzzleCubeFactory {
    public static GameObject createPuzzleCube(int id, int size, Vector2[] uvmap, Material mat, int k = 3) {
      GameObject cube = new GameObject("PuzzleCube");
      var s2 = size / 2f;
      GameObject frontPlane = CreatePlaneForCube(id, ExtractTileUVMap(id, uvmap, k), mat);
      frontPlane.transform.parent = cube.transform;
      frontPlane.transform.position = new Vector3(-s2, -s2);
      GameObject backPlane = CreatePlaneForCube(id, ExtractTileUVMap(id, uvmap, k), mat);
      backPlane.transform.parent = cube.transform;
      backPlane.transform.position = new Vector3(s2,s2);
      backPlane.transform.Rotate(Vector3.up, 180);
      GameObject leftPlane = CreatePlaneForCube(id, ExtractTileUVMap(id, uvmap, k), mat);
      leftPlane.transform.parent = cube.transform;
      leftPlane.transform.position = new Vector3(-s2,s2);
      leftPlane.transform.Rotate(Vector3.up, 270);
      GameObject rightPlane = CreatePlaneForCube(id, ExtractTileUVMap(id, uvmap, k), mat);
      rightPlane.transform.parent = cube.transform;
      rightPlane.transform.position = new Vector3(s2,-s2);
      rightPlane.transform.Rotate(Vector3.up, 90);
      GameObject topPlane = CreatePlaneForCube(id, ExtractTileUVMap(id, uvmap, k), mat);
      topPlane.transform.parent = cube.transform;
      topPlane.transform.position = new Vector3(-s2,-s2,s2);
      topPlane.transform.Rotate(Vector3.forward, 90);
      GameObject bottomPlane = CreatePlaneForCube(id, ExtractTileUVMap(id, uvmap, k), mat);
      bottomPlane.transform.parent = cube.transform;
      bottomPlane.transform.position = new Vector3(s2,s2,-s2);
      bottomPlane.transform.Rotate(Vector3.back, 90);
      return cube;
    }

    public static Vector2[] ExtractTileUVMap(int id, Vector2[] uvmap, int k = 3) {
      return new[] {
        uvmap[id + 1], uvmap[id + 2],
        uvmap[id + 2 * k - 1], uvmap[id + 2 * k]
      };
    }

    public static Vector2[] CreateTiledMasterUVMap(float width, float height, int k = 3) {
      float r = width / height;
      float u = width / r;
      float v = height / r;

      Vector2[] arr = new Vector2[(k + 1) * (k + 1)];

      for (int y = 0; y <= k; y++) {
        for (int x = 0; x <= k; x++) {
          arr[k * y + x] = new Vector2(x * u, y * v);
        }
      }

      return arr;
    }

    /// <summary>
    ///
    ///
    /// UVS:
    /// 2 -- 3
    /// |    |
    /// 0 -- 1
    /// </summary>
    /// <param name="size"></param>
    /// <param name="uvs"></param>
    /// <param name="mat"></param>
    public static GameObject CreatePlaneForCube(float size, Vector2[] uvs, Material mat) {
      GameObject go = new GameObject("Plane");
      MeshFilter meshFilter = go.AddComponent<MeshFilter>();
      MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
      Mesh mesh = meshFilter.mesh;
      Vector3[] vertices = new Vector3[4];
      vertices[0] = new Vector3(0, 0, 0);
      vertices[1] = new Vector3(size, 0, 0);
      vertices[2] = new Vector3(0, size, 0);
      vertices[3] = new Vector3(size, size, 0);

      mesh.vertices = vertices;

      int[] tri = new int[6];

      tri[0] = 0;
      tri[1] = 2;
      tri[2] = 1;

      tri[3] = 2;
      tri[4] = 3;
      tri[5] = 1;

      mesh.triangles = tri;

      Vector3[] normals = new Vector3[4];

      normals[0] = -Vector3.forward;
      normals[1] = -Vector3.forward;
      normals[2] = -Vector3.forward;
      normals[3] = -Vector3.forward;

      mesh.normals = normals;

      mesh.uv = uvs;

      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      mesh.RecalculateTangents();

      if (mat != null) {
        meshRenderer.material.CopyPropertiesFromMaterial(mat);
        //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
        meshRenderer.material.name = mat.name;
      } else {
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.white;
      }

      // TODO Highly experimental!

      var boxCollider = go.AddComponent<BoxCollider>();
      boxCollider.size = new Vector3(size, size, 0.0001f);

      return go;
    }

    public static GameObject CreatePlane(float width, float height, Material mat = null) {
      GameObject go = new GameObject("Plane");
      MeshFilter meshFilter = go.AddComponent<MeshFilter>();
      MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
      Mesh mesh = meshFilter.mesh;
      Vector3[] vertices = new Vector3[4];
      vertices[0] = new Vector3(0, 0, 0);
      vertices[1] = new Vector3(width, 0, 0);
      vertices[2] = new Vector3(0, height, 0);
      vertices[3] = new Vector3(width, height, 0);
      mesh.vertices = vertices;
      int[] tri = new int[6];
      tri[0] = 0;
      tri[1] = 2;
      tri[2] = 1;
      tri[3] = 2;
      tri[4] = 3;
      tri[5] = 1;
      mesh.triangles = tri;
      Vector3[] normals = new Vector3[4];
      normals[0] = -Vector3.forward;
      normals[1] = -Vector3.forward;
      normals[2] = -Vector3.forward;
      normals[3] = -Vector3.forward;
      mesh.normals = normals;
      Vector2[] uv = new Vector2[4];
      float xUnit = 1;

      float yUnit = 1;
      if (width > height) {
        xUnit = width / height;
      } else {
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
      if (mat != null) {
        meshRenderer.material.CopyPropertiesFromMaterial(mat);

        //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
        meshRenderer.material.name = mat.name;
      } else {
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.white;
      }

      // TODO Highly experimental!

      var boxCollider = go.AddComponent<BoxCollider>();
      boxCollider.size = new Vector3(width, height, 0.0001f);
      return go;
    }
  }
}