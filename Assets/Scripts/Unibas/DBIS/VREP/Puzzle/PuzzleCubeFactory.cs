using System;
using UnityEngine;

namespace Unibas.DBIS.VREP.Puzzle {
  public class PuzzleCubeFactory {


    public static GameObject[] createPuzzle(Texture2D texture, int size, Vector3 position) {
      var nbCubes = getNumberOfCubes(texture.width, texture.height);
      GameObject[] cubes = new GameObject[(int) (nbCubes.x * nbCubes.y)];

      var material = new Material(Shader.Find("Standard"));
      material.mainTexture = texture;
      for (int i=0; i<cubes.Length; i++) {
        cubes[i] = createPuzzleCube(i, size, material, nbCubes.x, nbCubes.y);
        // TODO cubes[i].transform.position = new Vector3();
      }
      
      return cubes;
    }
    
    public static Vector2Int getNumberOfCubes(int width, int height) {
      float asp_ratio = (float) width / (float) height;
      float w_prime = 3;
      int h_prime = (int) (w_prime * asp_ratio);
      return new Vector2Int(h_prime,(int) w_prime);
    }
    
    
    public static GameObject createPuzzleCube(int id, int size, Material mat, int nbXcubes, int nbYcubes) {
      GameObject cube = new GameObject("PuzzleCube");
      var s2 = size / 2f;

      // Front
      GameObject north = CreatePlaneForCube(size, CalculateUV(id, nbXcubes, nbYcubes), mat);
      //GameObject north = CreatePlane(size, size);
      north.name = "Front";
      north.transform.parent = cube.transform;
      north.transform.position = new Vector3(-s2, 0, -s2);
      // Right
      GameObject east = CreatePlaneForCube(size, CalculateUV(id, nbXcubes, nbYcubes), mat);
      // GameObject east = CreatePlane(size, size);
      east.name = "Right";
      east.transform.parent = cube.transform;
      east.transform.position = new Vector3(-s2, 0, s2);
      east.transform.Rotate(Vector3.up, 90);
      
      // Back
      GameObject south = CreatePlaneForCube(size, CalculateUV(id, nbXcubes, nbYcubes), mat);
      // GameObject south = CreatePlane(size, size);
      south.name = "Back";
      south.transform.parent = cube.transform;
      south.transform.position = new Vector3(s2, 0, s2);
      south.transform.Rotate(Vector3.up, 180);
      // Left
      GameObject west = CreatePlaneForCube(size, CalculateUV(id, nbXcubes, nbYcubes), mat);
      //GameObject west = CreatePlane(size, size);
      west.name = "Left";
      west.transform.parent = cube.transform;
      west.transform.position = new Vector3(s2, 0, -s2);
      west.transform.Rotate(Vector3.up, -90);

      // Bottom
      GameObject floorAnchor = new GameObject("BottomAnchor");
      floorAnchor.transform.parent = cube.transform;

      GameObject floor = CreatePlaneForCube(size, CalculateUV(id, nbXcubes, nbYcubes), mat);
      // GameObject floor = CreatePlane(size, size);
      floor.name = "Bottom";
      floor.transform.parent = floorAnchor.transform;
      floor.transform.Rotate(Vector3.right, -90);

      floorAnchor.transform.position = new Vector3(-s2, 0, s2);
      
      // Top
      GameObject ceilingAnchor = new GameObject("TopAnchor");
      ceilingAnchor.transform.parent = cube.transform;

      GameObject ceiling = CreatePlaneForCube(size, CalculateUV(id, nbXcubes, nbYcubes), mat);
      //GameObject ceiling = CreatePlane(size, size);
      ceiling.name = "Top";
      ceiling.transform.parent = ceilingAnchor.transform;

      // North Aligned
      ceilingAnchor.transform.position = new Vector3(-s2, size, -s2);
      ceilingAnchor.transform.Rotate(Vector3.right, 90);

      return cube;
    }

    public static Vector2[] CalculateUV(int id, int nbXCubes, int nbYcubes) {
      return new[] {
        new Vector2((id % nbXCubes) /(float) nbXCubes, ((id / nbXCubes))/(float)nbYcubes),//0
        new Vector2(((id % nbXCubes)+1) /(float) nbXCubes, ((id / nbXCubes))/(float)nbYcubes),//1
        new Vector2((id % nbXCubes) / (float)nbXCubes, (id / nbXCubes +1) / (float)nbYcubes),//2
        new Vector2(((id % nbXCubes)+1) /(float)nbXCubes, (id / nbXCubes +1)/(float)nbYcubes)//3
      };
    }

    public static Vector2[] ExtractTileUVMap(int id, Vector2[] uvmap, int nbXCubes, int nbYCubes) {
      return new[] {
        uvmap[id], uvmap[id + 1],
        uvmap[id + nbXCubes+1], uvmap[id+nbXCubes+2]
      };
    }

    public static Vector2[] CreateTiledMasterUVMap(int nbXcubes, int nbYcubes) {
      float u = 1f / nbXcubes;
      float v = 1f / nbYcubes;

      Vector2[] arr = new Vector2[(nbXcubes + 1) * (nbYcubes + 1)];

      for (int y = 0; y <= nbYcubes; y++) {
        for (int x = 0; x <= nbXcubes; x++) {
          arr[nbYcubes * y + x] = new Vector2(x * u, y * v);
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