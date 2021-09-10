using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Generation
{
  public class GenerateButton : MonoBehaviour
  {
    public GenerationRequest.GenTypeEnum type;
    public string targetRoomId;

    public async void ButtonPress()
    {
      Debug.Log("Pressed.");
      
      var room = GameObject.Find(ObjectFactory.RoomNamePrefix + targetRoomId);

      if (room != null) // This is also null if the room was removed (in which case we want to generate a new one).
      {
        // Room already exists and is loaded, teleport player and return.
        VrepController.TpPlayerToLocation(room.transform.position);
        return;
      }
      
      var parent = transform.parent.gameObject;

      var newRoom = await VrepController.Instance.GenerateAndLoadRoomForExhibition(parent, type);
      targetRoomId = newRoom.Id;
    }
  }
}