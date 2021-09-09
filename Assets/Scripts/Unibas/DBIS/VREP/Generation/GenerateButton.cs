using System.Collections.Generic;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.LegacyObjects;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Generation
{
  public class GenerateButton : MonoBehaviour
  {
    public GenerationRequest.GenTypeEnum type;
    public List<string> ids;
    public string targetRoomId;

    public async void ButtonPress()
    {
      Debug.Log("Pressed.");

      var parent = transform.parent.gameObject;

      var room = GameObject.Find(ObjectFactory.RoomNamePrefix + targetRoomId);

      // TODO If we allow to generate from different seeds, make sure the IDs and the seed associated with an exhibit also get updated. 

      if (room != null)
      {
        // Room already exists and is loaded, teleport player and return.
        VrepController.TpPlayerToRoom(room.GetComponent<CuboidExhibitionRoom>().RoomData);
        return;
      }

      var newRoom = await VrepController.Instance.GenerateAndLoadRoomForExhibition(parent, type, ids);

      targetRoomId = newRoom.Id;
      RoomReferences references;

      var model = parent.GetComponent<Displayal>().GetExhibit();

      if (model.Metadata.ContainsKey(MetadataType.References.GetKey()))
      {
        var refJson = model.Metadata[MetadataType.References.GetKey()];
        references = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomReferences>(refJson);
      }
      else
      {
        references = new RoomReferences();
      }

      references.References[type.ToString()] = targetRoomId;
      model.Metadata[MetadataType.References.GetKey()] = Newtonsoft.Json.JsonConvert.SerializeObject(references);

      VrepController.Instance.exhibitionManager.Exhibition.Rooms.Add(newRoom);
    }
  }
}