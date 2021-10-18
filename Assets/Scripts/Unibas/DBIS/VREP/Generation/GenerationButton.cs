using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using UnityEngine.UI;

namespace Unibas.DBIS.VREP.Generation
{
  public class GenerationButton : MonoBehaviour
  {
    public GenerationMethod type;
    public string targetRoomId = "";

    private MeshRenderer GetButtonRenderer()
    {
      return gameObject.GetComponentInChildren<Button>().gameObject.GetComponent<MeshRenderer>();
    }

    public void SetButtonLoading()
    {
      var buttonRenderer = GetButtonRenderer();
      buttonRenderer.material = TexturingUtility.LoadMaterialByName("DefaultYellow");
    }

    public void SetButtonReady()
    {
      var buttonRenderer = GetButtonRenderer();
      buttonRenderer.material = TexturingUtility.LoadMaterialByName("DefaultGreen");
    }

    public async void ButtonPress()
    {
      Debug.Log("Pressed.");

      var room = VrepController.Instance.exhibitionManager.RoomList.Find(it => it.RoomData.Id == targetRoomId);

      if (room == null && VrepController.Instance.isGenerating)
      {
        return;
      }

      if (room != null) // This is also null if the room was removed (in which case we want to generate a new one).
      {
        Debug.Log("Already exists.");
        // Room already exists and is loaded, teleport player and return.

        // Update button again, required for loaded rooms.
        SetButtonReady();
        
        // Teleport.
        VrepController.TpPlayerToLocation(room.transform.position);

        // Deactivate old room.
        GetComponentInParent<CuboidExhibitionRoom>().OnRoomLeave();

        // Activate new room.
        room.OnRoomEnter();

        return;
      }

      // Block.
      VrepController.Instance.isGenerating = true;

      var parent = gameObject.GetComponentInParent<ButtonWrapper>().displayal.gameObject;

      SetButtonLoading();

      var newRoom = await VrepController.Instance.GenerateAndLoadRoomForExhibition(parent, type);
      targetRoomId = newRoom.Id;

      // Deactivate room.
      // GetComponentInParent<CuboidExhibitionRoom>().OnRoomLeave();

      SetButtonReady();

      // Unblock.
      VrepController.Instance.isGenerating = false;
    }
  }
}