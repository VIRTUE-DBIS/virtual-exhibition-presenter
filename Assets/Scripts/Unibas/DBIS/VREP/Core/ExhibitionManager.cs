using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    private List<CuboidExhibitionRoom> _rooms = new List<CuboidExhibitionRoom>();

    public void DestroyCurrentExhibition()
    {
      if (_rooms.Any())
      {
        _rooms.ForEach(Destroy);
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
    }

    public async Task LoadRoom(Room room)
    {
      var roomGameObject = await ObjectFactory.BuildRoom(room);
      var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();
      _rooms.Add(exhibitionRoom);
    }
  }
}