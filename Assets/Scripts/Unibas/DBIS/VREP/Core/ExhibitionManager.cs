using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  /// <summary>
  /// Exhibition manager to create and load exhibitions from model exhibitions to actual VR exhibitions.
  /// </summary>
  public class ExhibitionManager : MonoBehaviour
  {
    public Exhibition Exhibition;

    public List<CuboidExhibitionRoom> RoomList = new List<CuboidExhibitionRoom>();

    public void DestroyCurrentExhibition()
    {
      RoomList.ForEach(r => Destroy(r.gameObject));
    }

    public async Task LoadNewExhibition(Exhibition ex)
    {
      Exhibition = ex;

      await LoadExhibition();
    }

    public void DisableExtensionRooms()
    {
      RoomList.ForEach(it => it.gameObject.SetActive(it == RoomList.First()));
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
        // Not generated, connect all rooms.
        VrepController.Instance.ImportedTpSetup();
      }

      // Hide all rooms except the first one.
      DisableExtensionRooms();

      // Setup TP to first room.
      VrepController.Instance.LobbyTpSetup(RoomList[0]);

      // TP player.
      VrepController.TpPlayerToObjPos(RoomList[0].gameObject);
    }

    public async Task LoadRoom(Room room)
    {
      var roomGameObject = await ObjectFactory.BuildRoom(room);
      var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();

      // Add room to map.
      RoomList.Add(exhibitionRoom);

      if (room.Metadata.ContainsKey(MetadataType.Generated.GetKey()))
      {
        VrepController.GeneratedTpSetup(room);
      }
    }

    void RestoreExhibits()
    {
      RoomList.ForEach(r => r.RestoreWallExhibits());
    }

    public void Restore()
    {
      RestoreExhibits();
      DisableExtensionRooms();
      VrepController.TpPlayerToLobby();
    }

    public async void Update()
    {
      // Could be used to save exhibitions via hotkey.
      if (Input.GetKeyDown(KeyCode.F8))
      {
        await new ExhibitionApi().PostApiExhibitionsSaveAsync(Exhibition);
      }

      if (Input.GetKey(KeyCode.F10))
      {
        Restore();
      }

      if (Input.GetKey(KeyCode.F12))
      {
        RestoreExhibits();
      }
    }
  }
}