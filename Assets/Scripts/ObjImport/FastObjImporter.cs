using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ObjImport
{
  /// <summary>
  /// Kindly provided at http://wiki.unity3d.com/index.php/FastObjImporter
  /// Modified version to work with web
  /// </summary>
  public sealed class FastObjImporter
  {
    /* FastObjImporter.cs
     * by Marc Kusters (Nighteyes)
     * Used for loading .obj files exported by Blender
     * Example usage: Mesh myMesh = FastObjImporter.Instance.ImportFile("path_to_obj_file.obj");
     */

    #region singleton

    // Singleton code
    // Static can be called from anywhere without having to make an instance
    private static FastObjImporter _instance;

    // If called check if there is an instance, otherwise create it
    public static FastObjImporter Instance
    {
      get { return _instance ??= new FastObjImporter(); }
    }

    #endregion

    private List<int> _triangles;
    private List<Vector3> _vertices;
    private List<Vector2> _uv;
    private List<Vector3> _normals;
    private List<Vector3Int> _faceData;
    private List<int> _intArray;

    private const int MIN_POW_10 = -16;
    private const int MAX_POW_10 = 16;
    private const int NUM_POWS_10 = MAX_POW_10 - MIN_POW_10 + 1;
    private static readonly float[] Pow10 = GenerateLookupTable();

    // Use this for initialization
    public Mesh ImportFile(string text)
    {
      _triangles = new List<int>();
      _vertices = new List<Vector3>();
      _uv = new List<Vector2>();
      _normals = new List<Vector3>();
      _faceData = new List<Vector3Int>();
      _intArray = new List<int>();

      LoadMeshData(text);

      var newVerts = new Vector3[_faceData.Count];
      var newUVs = new Vector2[_faceData.Count];
      var newNormals = new Vector3[_faceData.Count];

      /* The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
       * for the appropriate Unity mesh array.
       */
      for (var i = 0; i < _faceData.Count; i++)
      {
        newVerts[i] = _vertices[_faceData[i].X - 1];
        if (_faceData[i].Y >= 1)
          newUVs[i] = _uv[_faceData[i].Y - 1];

        if (_faceData[i].Z >= 1)
          newNormals[i] = _normals[_faceData[i].Z - 1];
      }

      var mesh = new Mesh
      {
        vertices = newVerts, uv = newUVs, normals = newNormals, triangles = _triangles.ToArray()
      };


      mesh.RecalculateBounds();
      //mesh.Optimize();

      return mesh;
    }

    private void LoadMeshData(string text)
    {
      var sb = new StringBuilder();
      var start = 0;
      string objectName = null;
      var faceDataCount = 0;

      var sbFloat = new StringBuilder();

      for (var i = 0; i < text.Length; i++)
      {
        if (text[i] != '\n') continue;

        sb.Remove(0, sb.Length);

        // Start +1 for whitespace '\n'
        sb.Append(text, start + 1, i - start);
        start = i;

        switch (sb[0])
        {
          case 'o' when sb[1] == ' ':
          {
            sbFloat.Remove(0, sbFloat.Length);
            var j = 2;
            while (j < sb.Length)
            {
              objectName += sb[j];
              j++;
            }

            break;
          }
          // Vertices
          case 'v' when sb[1] == ' ':
          {
            var splitStart = 2;

            _vertices.Add(new Vector3(GetFloat(sb, ref splitStart, ref sbFloat),
              GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
            break;
          }
          // UV
          case 'v' when sb[1] == 't' && sb[2] == ' ':
          {
            var splitStart = 3;

            _uv.Add(new Vector2(GetFloat(sb, ref splitStart, ref sbFloat),
              GetFloat(sb, ref splitStart, ref sbFloat)));
            break;
          }
          // Normals
          case 'v' when sb[1] == 'n' && sb[2] == ' ':
          {
            var splitStart = 3;

            _normals.Add(new Vector3(GetFloat(sb, ref splitStart, ref sbFloat),
              GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
            break;
          }
          case 'f' when sb[1] == ' ':
          {
            var splitStart = 2;

            var j = 1;
            _intArray.Clear();
            var info = 0;
            // Add faceData, a face can contain multiple triangles, facedata is stored in following order vert, uv, normal. If uv or normal are / set it to a 0
            while (splitStart < sb.Length && char.IsDigit(sb[splitStart]))
            {
              _faceData.Add(new Vector3Int(GetInt(sb, ref splitStart, ref sbFloat),
                GetInt(sb, ref splitStart, ref sbFloat), GetInt(sb, ref splitStart, ref sbFloat)));
              j++;

              _intArray.Add(faceDataCount);
              faceDataCount++;
            }

            info += j;
            j = 1;
            while (j + 2 < info
            ) //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
            {
              _triangles.Add(_intArray[0]);
              _triangles.Add(_intArray[j]);
              _triangles.Add(_intArray[j + 1]);

              j++;
            }

            break;
          }
        }
      }
    }

    private float GetFloat(StringBuilder sb, ref int start, ref StringBuilder sbFloat)
    {
      sbFloat.Remove(0, sbFloat.Length);
      while (start < sb.Length &&
             (char.IsDigit(sb[start]) || sb[start] == '-' || sb[start] == '.'))
      {
        sbFloat.Append(sb[start]);
        start++;
      }

      start++;

      return ParseFloat(sbFloat);
    }

    private int GetInt(StringBuilder sb, ref int start, ref StringBuilder sbInt)
    {
      sbInt.Remove(0, sbInt.Length);
      while (start < sb.Length &&
             (char.IsDigit(sb[start])))
      {
        sbInt.Append(sb[start]);
        start++;
      }

      start++;

      return IntParseFast(sbInt);
    }


    private static float[] GenerateLookupTable()
    {
      var result = new float[(-MIN_POW_10 + MAX_POW_10) * 10];
      for (var i = 0; i < result.Length; i++)
        result[i] = (float) i / NUM_POWS_10 * Mathf.Pow(10, i % NUM_POWS_10 + MIN_POW_10);
      return result;
    }

    private static float ParseFloat(StringBuilder value)
    {
      float result = 0;
      var negate = false;
      var len = value.Length;
      var decimalIndex = value.Length;
      for (var i = len - 1; i >= 0; i--)
        if (value[i] == '.')
        {
          decimalIndex = i;
          break;
        }

      var offset = -MIN_POW_10 + decimalIndex;
      for (var i = 0; i < decimalIndex; i++)
        if (i != decimalIndex && value[i] != '-')
          result += Pow10[(value[i] - '0') * NUM_POWS_10 + offset - i - 1];
        else if (value[i] == '-')
          negate = true;
      for (var i = decimalIndex + 1; i < len; i++)
        if (i != decimalIndex)
          result += Pow10[(value[i] - '0') * NUM_POWS_10 + offset - i];
      if (negate)
        result = -result;
      return result;
    }

    private static int IntParseFast(StringBuilder value)
    {
      // An optimized int parse method.
      var result = 0;
      for (var i = 0; i < value.Length; i++)
      {
        result = 10 * result + (value[i] - 48);
      }

      return result;
    }
  }

  public sealed class Vector3Int
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Vector3Int()
    {
    }

    public Vector3Int(int x, int y, int z)
    {
      X = x;
      Y = y;
      Z = z;
    }
  }
}