using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Generation
{
  public class GenerateButton : MonoBehaviour
  {
    public GenMethod type;
    public string targetRoomId = "";

    public async void ButtonPress()
    {
      Debug.Log("Pressed.");

      var room = VrepController.Instance.exhibitionManager.RoomList.Find(it => it.RoomData.Id == targetRoomId);

      if (room != null) // This is also null if the room was removed (in which case we want to generate a new one).
      {
        // Room already exists and is loaded, teleport player and return.
        room.OnRoomEnter();
        GetComponentInParent<CuboidExhibitionRoom>().OnRoomLeave();

        VrepController.TpPlayerToLocation(room.transform.position);
        return;
      }

      var parent = gameObject.GetComponentInParent<Displayal>().gameObject;

      var newRoom = await VrepController.Instance.GenerateAndLoadRoomForExhibition(parent, type);
      targetRoomId = newRoom.Id;
    }
  }
}