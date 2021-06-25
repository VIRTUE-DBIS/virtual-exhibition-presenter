using System.Collections.Generic;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.LegacyScripts
{
  public class BuildingManager : MonoBehaviour
  {
    public float roomSize = 10f;

    public float offset = 5f;

    public GameObject roomPrefab;

    private Exhibition _exhibition;

    private readonly List<Room> _theRooms = new List<Room>();

    private int GetNextPosition(int pos)
    {
      return (pos + 1) % _exhibition.rooms.Length;
    }

    private int GetPreviousPosition(int pos)
    {
      return (pos - 1 + _exhibition.rooms.Length) % _exhibition.rooms.Length;
    }


    private Vector3 CalculateRoomPosition(VREM.Model.Room room)
    {
      float x = room.position.x, y = room.position.y, z = room.position.z;
      return new Vector3(x * roomSize + x * offset, y * roomSize + y * offset, z * roomSize + z * offset);
    }

    public void BuildRoom(VREM.Model.Room room)
    {
      var goRoom = Instantiate(roomPrefab);
      goRoom.transform.position = CalculateRoomPosition(room);
      var roomLogic = goRoom.GetComponent<Room>();
      roomLogic.Populate(room);
    }

    private Room CreateRoom(VREM.Model.Room room)
    {
      var goRoom = Instantiate(roomPrefab);
      goRoom.transform.position = CalculateRoomPosition(room);
      var roomLogic = goRoom.GetComponent<Room>();
      return roomLogic;
    }


    public void Create(Exhibition exhibition)
    {
      _exhibition = exhibition;
      foreach (var room in exhibition.rooms)
      {
        Room r = CreateRoom(room);
        _theRooms.Add(r);
        r.Populate(room);
      }

      for (var i = 0; i < exhibition.rooms.Length; i++)
      {
        Room r = _theRooms[i];
        r.SetNextRoom(_theRooms[GetNextPosition(i)]);
        var tp = r.gameObject.transform.Find("TeleportPoint");
        if (tp != null && _theRooms.Count > 1)
        {
          var porter = tp.GetComponent<TeleportPoint>();
          var dest = CalculateRoomPosition(r.GetNextRoom().GetRoomModel());
          porter.transform.position = dest;
          r.SetPrevRoom(_theRooms[GetPreviousPosition(i)]);
        }
      }
    }
  }
}