using System.Collections.Generic;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Core
{
  /// <summary>
  /// Exhibition manager to create and load exhibitions from model exhibitions to actual VR exhibitions.
  /// </summary>
  public class ExhibitionManager : MonoBehaviour
  {
    public Exhibition Exhibition;

    public Dictionary<string, CuboidExhibitionRoom> RoomMap = new Dictionary<string, CuboidExhibitionRoom>();

    public List<CuboidExhibitionRoom> RoomList = new List<CuboidExhibitionRoom>();
    
    public void DestroyCurrentExhibition()
    {
      var rooms = RoomMap.Values;

      rooms.ForEach(r => Destroy(r.gameObject));
    }

    public void DeactivateAllRooms()
    {
      var rooms = RoomMap.Values;

      rooms.ForEach(r => r.gameObject.SetActive(false));
    }

    public void ActivateAllRooms()
    {
      var rooms = RoomMap.Values;

      rooms.ForEach(r => r.gameObject.SetActive(true));
    }

    public void ActivateRoomById(string id)
    {
      if (RoomMap.ContainsKey(id))
      {
        RoomMap[id].gameObject.SetActive(true);
      }
    }

    public async Task LoadNewExhibition(Exhibition ex)
    {
      Exhibition = ex;

      await LoadExhibition();
    }

    public async Task LoadExhibition()
    {
      DestroyCurrentExhibition();

      foreach (var room in Exhibition.Rooms)
      {
        await LoadRoom(room);
      }

      if (!Exhibition.Metadata.ContainsKey(MetadataType.Generated.GetKey()))
      {
        // Not generated, create the usual teleporters.
        VrepController.ImportedTpSetup();
      }
    }

    public async Task LoadRoom(Room room)
    {
      var roomGameObject = await ObjectFactory.BuildRoom(room);
      var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();

      // Disable all other rooms by default (don't do this if it turns out to be of no use).
      // RoomMap.Values.ForEach(r => r.gameObject.SetActive(false));

      // Add room to map.
      RoomMap[room.Id] = exhibitionRoom;
      RoomList.Add(exhibitionRoom);

      if (room.Metadata.ContainsKey(MetadataType.Generated.GetKey()))
      {
        VrepController.GeneratedTpSetup(room);
      }
    }

    public async void Update()
    {
      // Could be used to save exhibitions via hotkey.
      if (Input.GetKeyDown(KeyCode.F8))
      {
        await new ExhibitionApi().PostApiExhibitionsSaveAsync(Exhibition);
      }
    }
  }
}