using System;
using System.Collections.Generic;
using System.Linq;
using ObjImport;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.Multimedia;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.VREM.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.LegacyScripts
{
  [Obsolete("Got replaced by CuboidExhibitionRoom")]
  public class Room : MonoBehaviour
  {
    private const string NorthWallName = "NorthWall";
    private const string EastWallName = "EastWall";
    private const string SouthWallName = "SouthWall";
    private const string WestWallName = "WestWall";

    public GameObject planePrefab;

    public new AudioLoader audio;

    private readonly List<GameObject> _displayedImages = new List<GameObject>();

    private const bool LightingActivated = false;

    public GameObject globePrefab;

    // Use this for initialization
    private void Start()
    {
      if (audio == null)
      {
        audio = gameObject.AddComponent<AudioLoader>();
      }
    }

    private void OnLeave()
    {
      //TODO: call this when leaving room
      audio.Stop();
    }

    private Wall GetWall(string wallName)
    {
      var go = transform.Find(wallName);
      var str = go.transform.GetComponents<MonoBehaviour>().Aggregate("Components: ", (current, e) => current + e);

      Debug.Log("[Room] " + str);

      return transform.Find(wallName).GetComponent<Wall>();
    }

    private Wall GetWallForOrientation(WallOrientation orientation)
    {
      return orientation switch
      {
        WallOrientation.North => GetWall(NorthWallName),
        WallOrientation.East => GetWall(EastWallName),
        WallOrientation.South => GetWall(SouthWallName),
        WallOrientation.West => GetWall(WestWallName),
        _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
      };
    }

    /// <param name="url">url to access image</param>
    /// <param name="wall">wall orientation</param>
    /// <param name="x">x coordinate for corresponding wall based on left lower anchor</param>
    /// <param name="y">x coordinate for corresponding wall based on left lower anchor</param>
    public Displayal Display(string url, WallOrientation wall, float x, float y, float w, float h, bool lightOn = true,
      string audioUrl = null)
    {
      Debug.Log($"{url}, {wall}, {x}/{y}, {w}/{h}");
      var displayal = Instantiate(planePrefab);

      var go = GameObject.Find("VirtualExhibitionManager");

      if (!LightingActivated || !lightOn)
      {
        displayal.transform.Find("Directional light").gameObject.SetActive(false);
      }


      var disp = displayal.gameObject.GetComponent<Displayal>();

      var image = displayal.transform.Find("Plane").gameObject.AddComponent<ImageLoader>(); // Displayal
      image.ReloadImage(url);
      Debug.Log(GetWallForOrientation(wall));
      var pos = GetWallForOrientation(wall).CalculatePosition(transform.position, new Vector2(x, y));
      var rot = GetWallForOrientation(wall).CalculateRotation();
      displayal.transform.position = pos;
      displayal.transform.rotation = Quaternion.Euler(rot);
      displayal.transform.localScale = ScalingUtility.ConvertMeters2PlaneScaleSize(w, h);

      if (audioUrl != null)
      {
        Debug.Log("added audio to display object");
        var closenessDetector = displayal.AddComponent<ClosenessDetector>();
        closenessDetector.url = audioUrl;
      }


      _displayedImages.Add(displayal);
      return disp;
    }

    private VREM.Model.Room _roomModel;

    private Room _next;
    private Room _prev;

    public void SetNextRoom(Room next)
    {
      _next = next;
    }

    public void SetPrevRoom(Room prev)
    {
      _prev = prev;
    }

    public Room GetNextRoom()
    {
      return _next;
    }

    public Room GetPrevRoom()
    {
      return _prev;
    }


    public VREM.Model.Room GetRoomModel()
    {
      return _roomModel;
    }

    public void Populate(VREM.Model.Room room)
    {
      Debug.Log(room);
      Debug.Log(room.walls);
      _roomModel = room;


      Debug.Log("adjusting ceiling and floor");
      // TODO Use new material loading code
      gameObject.transform.Find("Ceiling").gameObject.GetComponent<TexturedMonoBehaviour>()
        .LoadMaterial(TexturingUtility.Translate(room.ceiling));
      gameObject.transform.Find("Floor").gameObject.GetComponent<TexturedMonoBehaviour>()
        .LoadMaterial(TexturingUtility.Translate(room.floor));

      if (!string.IsNullOrEmpty(room.GetURLEncodedAudioPath()))
      {
        Debug.Log("add audio to room");

        if (audio == null)
        {
          audio = gameObject.AddComponent<AudioLoader>();
        }

        audio.ReloadAudio(room.GetURLEncodedAudioPath());
      }

      PopulateWalls(room.walls);
    }

    private void PlaceExhibits(IEnumerable<Exhibit> exhibits)
    {
      foreach (var exhibit in exhibits)
      {
        LoadAndPlaceModel(exhibit.GetURLEncodedPath(), exhibit.position);
      }
    }

    private static void LoadAndPlaceModel(string url, Vector3 pos)
    {
      var parent = new GameObject("Model Anchor");
      var model = new GameObject("Model");
      model.transform.SetParent(parent.transform);
      parent.transform.position = pos;
      var objLoader = model.AddComponent<ObjLoader>();
      model.transform.Rotate(-90, 0, 0);
      model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
      objLoader.Load(url);
    }

    private void PopulateWalls(IEnumerable<VREM.Model.Wall> walls)
    {
      foreach (var wall in walls)
      {
        switch (wall.direction)
        {
          case "NORTH":
            LoadExhibits(wall, WallOrientation.North);
            GetWallForOrientation(WallOrientation.North).LoadMaterial(TexturingUtility.Translate(wall.texture));
            break;
          case "EAST":
            LoadExhibits(wall, WallOrientation.East);
            GetWallForOrientation(WallOrientation.East).LoadMaterial(TexturingUtility.Translate(wall.texture));
            break;
          case "SOUTH":
            LoadExhibits(wall, WallOrientation.South);
            GetWallForOrientation(WallOrientation.South).LoadMaterial(TexturingUtility.Translate(wall.texture));
            break;
          case "WEST":
            LoadExhibits(wall, WallOrientation.West);
            GetWallForOrientation(WallOrientation.West).LoadMaterial(TexturingUtility.Translate(wall.texture));
            break;
        }
      }
    }

    private void LoadExhibits(VREM.Model.Wall wall, WallOrientation orientation)
    {
      foreach (var e in wall.exhibits)
      {
        Debug.Log($"E: {e.position.x}/{e.position.y} at {e.size.x}/{e.size.y}");
        var disp = Display(e.GetURLEncodedPath(), orientation, e.position.x, e.position.y, e.size.x, e.size.y, e.light,
          e.GetURLEncodedAudioPath());
        disp.SetExhibitModel(e);
      }
    }
  }
}