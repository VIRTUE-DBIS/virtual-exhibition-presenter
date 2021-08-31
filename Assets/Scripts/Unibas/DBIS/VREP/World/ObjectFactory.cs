using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Movement;
using Unibas.DBIS.VREP.Utils;
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
      float x = room.Position.X, y = room.Position.Y, z = room.Position.Z;
      var off = Settings.RoomOffset; // Space between rooms.
      return new Vector3(x * room.Size.X + x * off, y * room.Size.Y + y * off, z * room.Size.Z + z * off);
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
        roomData.Size.X,
        roomData.Size.Y,
        TexturingUtility.LoadMaterialByName(roomData.Floor),
        TexturingUtility.LoadMaterialByName(roomData.Ceiling),
        GetMaterialForWallOrientation(Wall.DirectionEnum.NORTH, roomData),
        GetMaterialForWallOrientation(Wall.DirectionEnum.EAST, roomData),
        GetMaterialForWallOrientation(Wall.DirectionEnum.SOUTH, roomData),
        GetMaterialForWallOrientation(Wall.DirectionEnum.WEST, roomData)
      );

      // Create the actual GameObject for the room.
      var room = ModelFactory.CreateCuboidRoom(modelData);

      // Add exhibition room component, containing the room model.
      var er = room.AddComponent<CuboidExhibitionRoom>();
      er.RoomModel = modelData;
      er.Model = room;
      er.RoomData = roomData;

      // Add walls.
      var na = CreateAnchor(Wall.DirectionEnum.NORTH, room, modelData);
      var ea = CreateAnchor(Wall.DirectionEnum.EAST, room, modelData);
      var sa = CreateAnchor(Wall.DirectionEnum.SOUTH, room, modelData);
      var wa = CreateAnchor(Wall.DirectionEnum.WEST, room, modelData);

      var nw = CreateExhibitionWall(Wall.DirectionEnum.NORTH, roomData, na);
      var ew = CreateExhibitionWall(Wall.DirectionEnum.EAST, roomData, ea);
      var sw = CreateExhibitionWall(Wall.DirectionEnum.SOUTH, roomData, sa);
      var ww = CreateExhibitionWall(Wall.DirectionEnum.WEST, roomData, wa);

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
    private static ExhibitionWall CreateExhibitionWall(Wall.DirectionEnum orientation, Room room, GameObject anchor)
    {
      var wall = anchor.AddComponent<ExhibitionWall>();

      wall.Anchor = anchor;
      wall.WallModel = null;
      wall.WallData = (from w in room.Walls
          let wor = orientation
          where wor.Equals(orientation)
          select w)
        .FirstOrDefault();

      return wall;
    }

    /// <summary>
    /// Returns the material (wall texture) for a specific wall orientation (= for 1 wall) as defined in the room model.
    /// </summary>
    /// <param name="orientation">The orientation of the wall to get the material for.</param>
    /// <param name="roomData">The room model object to take the data for the wall material from.</param>
    /// <returns>The resulting Material object.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static Material GetMaterialForWallOrientation(Wall.DirectionEnum orientation, Room roomData)
    {
      foreach (var wallData in roomData.Walls)
      {
        if (!orientation.Equals(wallData.Direction)) continue;

        Debug.Log("Material " + wallData.Texture + " for room " + roomData.Position);

        return TexturingUtility.LoadMaterialByName(wallData.Texture, true);
      }

      throw new ArgumentException("Couldn't find material for orientation " + orientation + " in room at " +
                                  roomData.Position + ".");
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
    private static GameObject CreateAnchor(Wall.DirectionEnum orientation, GameObject room, CuboidRoomModel model)
    {
      var anchor = new GameObject(orientation + "Anchor");
      anchor.transform.parent = room.transform;

      Vector3 pos;
      float a;
      var sizeHalf = model.size / 2f; // This probably only supports square rooms (with varying height).

      switch (orientation)
      {
        case Wall.DirectionEnum.NORTH:
          pos = new Vector3(-sizeHalf, 0, sizeHalf);
          a = 0;
          break;
        case Wall.DirectionEnum.EAST:
          pos = new Vector3(sizeHalf, 0, sizeHalf);
          a = 90;
          break;
        case Wall.DirectionEnum.SOUTH:
          pos = new Vector3(sizeHalf, 0, -sizeHalf);
          a = 180;
          break;
        case Wall.DirectionEnum.WEST:
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

    public static Vector3 CalculateRotation(Wall.DirectionEnum orientation)
    {
      return orientation switch
      {
        Wall.DirectionEnum.NORTH => new Vector3(90, 180, 0),
        Wall.DirectionEnum.EAST => new Vector3(90, 0, 0),
        Wall.DirectionEnum.SOUTH => new Vector3(90, -180, 0),
        Wall.DirectionEnum.WEST => new Vector3(90, 90, 0),
        _ => throw new ArgumentOutOfRangeException()
      };
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