using System;
using System.Collections.Generic;
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
  /// Static class for object generation.
  /// </summary>
  public static class ObjectFactory
  {
    public static readonly ExhibitionBuildingSettings Settings = ExhibitionBuildingSettings.Instance;

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

    public static Vector3 CalculateRoomPosition(Room room)
    {
      // This currently works only for 1 exhibition (with multiple rooms).
      // Rework this if multiple exhibitions should be loaded.
      float x = room.position.x, y = room.position.y, z = room.position.z;
      var off = Settings.RoomOffset;
      return new Vector3(x * room.size.x + x * off, y * room.size.y + y * off, z * room.size.z + z * off);
    }

    public static GameObject BuildRoom(Room roomData)
    {
      Material[] mats =
      {
        TexturingUtility.LoadMaterialByName(roomData.floor),
        TexturingUtility.LoadMaterialByName(roomData.ceiling),
        GetMaterialForWallOrientation(WallOrientation.North, roomData),
        GetMaterialForWallOrientation(WallOrientation.East, roomData),
        GetMaterialForWallOrientation(WallOrientation.South, roomData),
        GetMaterialForWallOrientation(WallOrientation.West, roomData)
      };

      var modelData = new CuboidRoomModel(CalculateRoomPosition(roomData), roomData.size.x, roomData.size.y,
        mats[0], mats[1], mats[2], mats[3], mats[4], mats[5]);
      var room = ModelFactory.CreateCuboidRoom(modelData);

      var er = room.AddComponent<CuboidExhibitionRoom>();
      er.RoomModel = modelData;
      er.Model = room;
      er.RoomData = roomData;

      var na = CreateAnchor(WallOrientation.North, room, modelData);
      var ea = CreateAnchor(WallOrientation.East, room, modelData);
      var sa = CreateAnchor(WallOrientation.South, room, modelData);
      var wa = CreateAnchor(WallOrientation.West, room, modelData);

      var nw = CreateExhibitionWall(WallOrientation.North, roomData, na);
      var ew = CreateExhibitionWall(WallOrientation.East, roomData, ea);
      var sw = CreateExhibitionWall(WallOrientation.South, roomData, sa);
      var ww = CreateExhibitionWall(WallOrientation.West, roomData, wa);

      er.Walls = new List<ExhibitionWall>(new[] {nw, ew, sw, ww});
      er.Populate();

      var light = new GameObject("RoomLight");
      var l = light.AddComponent<Light>();
      l.type = LightType.Point;
      l.range = 8;
      l.color = Color.white;
      l.intensity = 1.5f;
      l.renderMode = LightRenderMode.ForcePixel;

      var t = l.transform;
      t.parent = room.transform;
      t.localPosition = new Vector3(0, 2.5f, 0);
      room.name = "Room";

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

    private static ExhibitionWall CreateExhibitionWall(WallOrientation orientation, Room room, GameObject anchor)
    {
      var wall = anchor.AddComponent<ExhibitionWall>();

      wall.Anchor = anchor;
      wall.WallModel = null;
      wall.WallData = room.GetWall(orientation);

      return wall;
    }

    private static Material GetMaterialForWallOrientation(WallOrientation orientation, Room roomData)
    {
      foreach (var wallData in roomData.walls)
      {
        var wor = (WallOrientation) Enum.Parse(typeof(WallOrientation), wallData.direction, true);
        if (!wor.Equals(orientation)) continue;

        Debug.Log("Material " + wallData.texture + " for room " + roomData.position);

        return TexturingUtility.LoadMaterialByName(wallData.texture, true);
      }

      throw new ArgumentException("Couldn't find material for orientation " + orientation + " in room at " +
                                  roomData.position + ".");
    }

    private static GameObject CreateAnchor(WallOrientation orientation, GameObject room, CuboidRoomModel model)
    {
      var anchor = new GameObject(orientation + "Anchor");
      anchor.transform.parent = room.transform;

      Vector3 pos;
      float a;
      var sizeHalf = model.size / 2f;

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


    public static Vector3 CalculateRotation(WallOrientation orientation)
    {
      // TODO Fix rotations!

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
      return CalculateRotation((WallOrientation) Enum.Parse(typeof(WallOrientation), orientation, true));
    }
  }
}