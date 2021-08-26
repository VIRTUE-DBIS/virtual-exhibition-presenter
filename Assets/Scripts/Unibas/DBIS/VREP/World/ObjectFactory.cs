using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Movement;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.World
{
  /// <summary>
  /// Static class for Displayal/Room generation.
  /// </summary>
  public static class ObjectFactory
  {
    public static readonly ExhibitionBuildingSettings Settings = ExhibitionBuildingSettings.Instance;

    /// <summary>
    /// Derives a displayal GameObject from the prefab.
    /// </summary>
    /// <returns>The created displayal GameObject.</returns>
    /// <exception cref="Exception">If a prefab could not be loaded.</exception>
    public static GameObject GetDisplayalPrefab()
    {
      var prefabName = VrepController.Instance.settings.PlaygroundEnabled
        ? Settings.throwableDisplayalPrefabName
        : Settings.standardDisplayalPrefabName;
      var prefab = Resources.Load("Prefabs/" + prefabName, typeof(GameObject)) as GameObject;

      if (prefab == null)
      {
        throw new Exception($"Could not load '{Settings.standardDisplayalPrefabName}' as Resource.");
      }

      return prefab;
    }

    /// <summary>
    /// Calculates the absolute position of a room based on its relative position and size in the exhibition.
    /// </summary>
    /// <param name="room">The room to calculate the absolute position for.</param>
    /// <returns>The absolute position of the room.</returns>
    public static Vector3 CalculateRoomPosition(Room room)
    {
      // This currently works only for 1 exhibition (with multiple rooms).
      // Rework this if multiple exhibitions should be loaded.
      float x = room.position.x, y = room.position.y, z = room.position.z;
      var off = Settings.RoomOffset; // Space between rooms.
      return new Vector3(x * room.size.x + x * off, y * room.size.y + y * off, z * room.size.z + z * off);
    }

    /// <summary>
    /// Builds a room, i.e., creating and building its walls and exhibits as well as adding textures, lighting,
    /// teleports, etc.
    /// </summary>
    /// <param name="roomData">The room data that should be used to build this room (as received from VREM).</param>
    /// <returns>The GameObject for the created room.</returns>
    public async static Task<GameObject> BuildRoom(Room roomData)
    {
      // Create room model.
      var modelData = new CuboidRoomModel(
        CalculateRoomPosition(roomData),
        roomData.size.x,
        roomData.size.y,
        TexturingUtility.LoadMaterialByName(roomData.floor),
        TexturingUtility.LoadMaterialByName(roomData.ceiling),
        GetMaterialForWallOrientation(WallOrientation.North, roomData),
        GetMaterialForWallOrientation(WallOrientation.East, roomData),
        GetMaterialForWallOrientation(WallOrientation.South, roomData),
        GetMaterialForWallOrientation(WallOrientation.West, roomData)
      );

      // Create the actual GameObject for the room.
      var room = ModelFactory.CreateCuboidRoom(modelData);

      // Add exhibition room component, containing the room model.
      var er = room.AddComponent<CuboidExhibitionRoom>();
      er.RoomModel = modelData;
      er.Model = room;
      er.RoomData = roomData;

      // Add walls.
      var na = CreateAnchor(WallOrientation.North, room, modelData);
      var ea = CreateAnchor(WallOrientation.East, room, modelData);
      var sa = CreateAnchor(WallOrientation.South, room, modelData);
      var wa = CreateAnchor(WallOrientation.West, room, modelData);

      var nw = CreateExhibitionWall(WallOrientation.North, roomData, na);
      var ew = CreateExhibitionWall(WallOrientation.East, roomData, ea);
      var sw = CreateExhibitionWall(WallOrientation.South, roomData, sa);
      var ww = CreateExhibitionWall(WallOrientation.West, roomData, wa);

      er.Walls = new List<ExhibitionWall>(new[] { nw, ew, sw, ww });

      // Add exhibits to walls (this is expensive!).
      await er.Populate();

      // Light.
      var light = new GameObject("RoomLight");
      var l = light.AddComponent<Light>();
      l.type = LightType.Point;
      l.range = 8;
      l.color = Color.white;
      l.intensity = 1.5f;
      l.renderMode = LightRenderMode.ForcePixel;

      var t = l.transform;
      t.parent = room.transform; // Set room as parent for transform component.
      t.localPosition = new Vector3(0, 2.5f, 0);
      room.name = "Room";

      // Teleport.
      var teleportArea = new GameObject("TeleportArea");
      var col = teleportArea.AddComponent<BoxCollider>();
      col.size = new Vector3(modelData.size, 0.01f, modelData.size);
      teleportArea.AddComponent<MeshRenderer>();

      var tpa = teleportArea.AddComponent<TeleportArea>();
      var t1 = tpa.transform;
      t1.parent = room.transform;
      t1.localPosition = new Vector3(0, 0.01f, 0);

      return room;
    }

    /// <summary>
    /// Attaches an exhibition wall as defined in the room model to the corresponding GameObject of the room.
    /// </summary>
    /// <param name="orientation">The orientation of the wall.</param>
    /// <param name="room">The room model object to take the data for the wall from.</param>
    /// <param name="anchor">The GameObject to set as the anchor for the room.</param>
    /// <returns>The created wall component (with a set anchor).</returns>
    private static ExhibitionWall CreateExhibitionWall(WallOrientation orientation, Room room, GameObject anchor)
    {
      var wall = anchor.AddComponent<ExhibitionWall>();

      wall.Anchor = anchor;
      wall.WallModel = null;
      wall.WallData = room.GetWall(orientation);

      return wall;
    }

    /// <summary>
    /// Returns the material (wall texture) for a specific wall orientation (= for 1 wall) as defined in the room model.
    /// </summary>
    /// <param name="orientation">The orientation of the wall to get the material for.</param>
    /// <param name="roomData">The room model object to take the data for the wall material from.</param>
    /// <returns>The resulting Material object.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static Material GetMaterialForWallOrientation(WallOrientation orientation, Room roomData)
    {
      foreach (var wallData in roomData.walls)
      {
        var wor = (WallOrientation)Enum.Parse(typeof(WallOrientation), wallData.direction, true);
        if (!wor.Equals(orientation)) continue;

        Debug.Log("Material " + wallData.texture + " for room " + roomData.position);

        return TexturingUtility.LoadMaterialByName(wallData.texture, true);
      }

      throw new ArgumentException("Couldn't find material for orientation " + orientation + " in room at " +
                                  roomData.position + ".");
    }

    /// <summary>
    /// Creates a GameObject for a wall to later act as a parent for the exhibits.
    /// Placed at the bottom left corner of a wall, acting as 0/0/0 for relative exhibit coordinates
    /// in the exhibition model from VREM.
    /// </summary>
    /// <param name="orientation">Wall orientation.</param>
    /// <param name="room">GameObject for the room of the wall, acting as parent for the created anchor.</param>
    /// <param name="model">The CuboidRoomModel holding details about size and textures of the room.</param>
    /// <returns>The GameObject of the newly created anchor (with its parent set).</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static GameObject CreateAnchor(WallOrientation orientation, GameObject room, CuboidRoomModel model)
    {
      var anchor = new GameObject(orientation + "Anchor");
      anchor.transform.parent = room.transform;

      Vector3 pos;
      float a;
      var sizeHalf = model.size / 2f; // This probably only supports square rooms (with varying height).

      switch (orientation)
      {
        case WallOrientation.North:
          pos = new Vector3(-sizeHalf, 0, sizeHalf);
          a = 0;
          break;
        case WallOrientation.East:
          pos = new Vector3(sizeHalf, 0, sizeHalf);
          a = 90;
          break;
        case WallOrientation.South:
          pos = new Vector3(sizeHalf, 0, -sizeHalf);
          a = 180;
          break;
        case WallOrientation.West:
          pos = new Vector3(-sizeHalf, 0, -sizeHalf);
          a = 270;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
      }

      anchor.transform.Rotate(Vector3.up, a);
      anchor.transform.localPosition = pos;

      return anchor;
    }

    public static Vector3 CalculateRotation(WallOrientation orientation)
    {
      return orientation switch
      {
        WallOrientation.North => new Vector3(90, 180, 0),
        WallOrientation.East => new Vector3(90, 0, 0),
        WallOrientation.South => new Vector3(90, -180, 0),
        WallOrientation.West => new Vector3(90, 90, 0),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    public static Vector3 CalculateRotation(string orientation)
    {
      return CalculateRotation((WallOrientation)Enum.Parse(typeof(WallOrientation), orientation, true));
    }

    private static ComplexCuboidModel GenerateButtonModel(float size, float border, float height)
    {
      var model = new ComplexCuboidModel();

      // TODO Add material somehow.
      model.Add(Vector3.zero, new CuboidModel(size, size, height));
      model.Add(new Vector3(border, border, -height), new CuboidModel(size - 2 * border, size - 2 * border, height));

      return model;
    }

    public static GameObject CreateTeleportButtonModel(Vector3 position, Vector3 destination, float size, float border)
    {
      var modelData = GenerateButtonModel(size, border, border / 2f);
      var buttonObj = ModelFactory.CreateModel(modelData);

      var tpBtn = buttonObj.AddComponent<PlayerTeleporter>();
      tpBtn.destination = destination;

      var col = buttonObj.AddComponent<BoxCollider>();
      col.size = new Vector3(size, size, border * 2);
      col.center = new Vector3(size / 2f, size / 2f, -border);
      buttonObj.AddComponent<Button>();

      var hand = new CustomEvents.UnityEventHand();
      hand.AddListener(h => { tpBtn.TeleportPlayer(); });
      buttonObj.AddComponent<UIElement>().onHandClick = hand;
      buttonObj.transform.position = position;
      buttonObj.name = "TeleportButton (Instance)";

      return buttonObj;
    }
  }
}