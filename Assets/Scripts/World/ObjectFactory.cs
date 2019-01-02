using System;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.World {
  
  /// <summary>
  /// Static class for object generation.
  /// </summary>
  public static class ObjectFactory {

    public static GameObject BuildRoom(DefaultNamespace.VREM.Model.Room roomData) {
      Material[] mats = {
        TexturingUtility.LoadMaterialByName(roomData.floor),
        TexturingUtility.LoadMaterialByName(roomData.ceiling),
        GetMaterialForWallOrientation(WallOrientation.NORTH, roomData),
        GetMaterialForWallOrientation(WallOrientation.EAST, roomData),
        GetMaterialForWallOrientation(WallOrientation.SOUTH, roomData),
        GetMaterialForWallOrientation(WallOrientation.WEST, roomData)
      };
      CuboidRoomModel modelData = new CuboidRoomModel(roomData.position, roomData.size.x, roomData.size.y,
        mats[0],mats[1],mats[2], mats[3], mats[4], mats[5]);
      GameObject room = ModelFactory.CreateCuboidRoom(modelData);
      var na = CreateAnchor(WallOrientation.NORTH, room, modelData);
      var ea = CreateAnchor(WallOrientation.EAST, room, modelData);
      var sa = CreateAnchor(WallOrientation.SOUTH, room, modelData);
      var wa = CreateAnchor(WallOrientation.WEST, room, modelData);
      
      // TODO populate room
      
      // TODO populate walls

      GameObject light = new GameObject("RoomLight");
      var l = light.AddComponent<Light>();
      l.type = LightType.Point;
      l.range = 8;
      l.color = Color.white;
      l.intensity = 1.5f;
      l.renderMode = LightRenderMode.ForcePixel;
      l.lightmapBakeType = LightmapBakeType.Mixed;
      l.transform.parent = room.transform;
      l.transform.localPosition = new Vector3(0,2.5f,0);
      room.name = "Room";
      return room;
    }

    private static Material GetMaterialForWallOrientation(WallOrientation orientation,
      DefaultNamespace.VREM.Model.Room roomData) {
      foreach (DefaultNamespace.VREM.Model.Wall wallData in roomData.walls) {
        WallOrientation wor = (WallOrientation)Enum.Parse(typeof(WallOrientation), wallData.direction, true);
        if (wor.Equals(orientation)) {
          return TexturingUtility.LoadMaterialByName(wallData.texture);
        }
      }
      throw new ArgumentException("Couldn't find material for orientation "+orientation+" in room at "+roomData.position);
    }

    private static GameObject CreateAnchor(WallOrientation orientation, GameObject room, CuboidRoomModel model) {
      GameObject anchor = new GameObject(orientation+"Anchor");
      anchor.transform.parent = room.transform;
      Vector3 pos = Vector3.zero;
      var sizeHalf = model.Size / 2f;
      switch (orientation) {
        case WallOrientation.NORTH:
          pos = new Vector3(-sizeHalf, 0, sizeHalf);
          break;
        case WallOrientation.EAST:
          pos = new Vector3(sizeHalf, 0, sizeHalf);
          break;
        case WallOrientation.SOUTH:
          pos = new Vector3(sizeHalf, 0, -sizeHalf);
          break;
        case WallOrientation.WEST:
          pos = new Vector3(-sizeHalf, 0, -sizeHalf);
          break;
        default:
          throw new ArgumentOutOfRangeException("orientation", orientation, null);
      }
      anchor.transform.position = pos;
      return anchor;
    }
    
  }
}