using System.Collections.Generic;
using Unibas.DBIS.VREP.Utils;
using Unibas.DBIS.VREP.VREM.Model;
using Unibas.DBIS.VREP.World;
using UnityEngine;

namespace Unibas.DBIS.VREP.Core
{
  /// <summary>
  /// Exhibition manager to create and load exhibitions from model exhibitions to actual VR exhibitions.
  /// </summary>
  public class ExhibitionManager
  {
    private Exhibition _exhibition;
    private List<CuboidExhibitionRoom> _rooms = new List<CuboidExhibitionRoom>();

    public ExhibitionManager(Exhibition exhibition)
    {
      _exhibition = exhibition;
    }

    public CuboidExhibitionRoom GetRoomByIndex(int index)
    {
      return _rooms[index];
    }

    /// <summary>
    /// Restores all exhibits of the currently loaded exhibition.
    /// </summary>
    public void RestoreExhibits()
    {
      _rooms.ForEach(r => r.RestoreWallExhibits());
    }

    private int GetNextRoomIndex(int pos)
    {
      return (pos + 1) % _exhibition.rooms.Length;
    }

    private int GetPreviousRoomIndex(int pos)
    {
      return (pos - 1 + _exhibition.rooms.Length) % _exhibition.rooms.Length;
    }

    private int GetRoomIndex(Room room)
    {
      for (var i = 0; i < _exhibition.rooms.Length; i++)
      {
        if (room.Equals(_exhibition.rooms[i]))
        {
          return i;
        }
      }

      return -1;
    }

    private Room GetNext(Room room)
    {
      var pos = GetRoomIndex(room);

      return _exhibition.rooms[GetNextRoomIndex(pos)];
    }

    private Room GetPrevious(Room room)
    {
      var pos = GetRoomIndex(room);

      return _exhibition.rooms[GetPreviousRoomIndex(pos)];
    }

    /// <summary>
    /// Creates and loads the exhibition from the Exhibition model object currently stored.
    /// This includes building all rooms with their walls and generating displayals from exhibits.
    /// </summary>
    public void GenerateExhibition()
    {
      foreach (var room in _exhibition.rooms)
      {
        var roomGameObject = ObjectFactory.BuildRoom(room);
        var exhibitionRoom = roomGameObject.GetComponent<CuboidExhibitionRoom>();
        _rooms.Add(exhibitionRoom);

        if (VrepController.Instance.settings.CeilingLogoEnabled)
        {
          var pref = Resources.Load<GameObject>("Objects/unibas");
          var logo = Object.Instantiate(pref, exhibitionRoom.transform, false);

          logo.name = "UnibasLogo";
          logo.transform.localPosition = new Vector3(-1.493f, room.size.y - .01f, 3.35f); // manually found values
          logo.transform.localRotation = Quaternion.Euler(new Vector3(90, 180));
          logo.transform.localScale = Vector3.one * 10000;
        }

        if (VrepController.Instance.settings.WallTimerCount > 0)
        {
          var obj = new GameObject("Timer");
          obj.transform.SetParent(exhibitionRoom.transform, false);
          obj.transform.localPosition =
            new Vector3(-room.size.x / 2 + 0.2f, room.size.y - 0.2f, room.size.z / 2); // manually found values
          obj.transform.localScale = Vector3.one * 0.05f;

          var textMesh = obj.AddComponent<TextMesh>();
          textMesh.fontSize = 150;

          var timer = obj.AddComponent<Countdown>();
          timer.countdown = textMesh;
          timer.initTime = VrepController.Instance.settings.WallTimerCount;

          obj.transform.GetComponent<MeshRenderer>().enabled = false;
        }
      }

      // For teleporting, each room needs to be created.
      foreach (var room in _rooms)
      {
        CreateAndAttachTeleporters(room);
      }
    }

    /// <summary>
    /// Attaches teleporters to a previously generated CuboidExhibitionRoom for an exhibition.
    /// To properly navigate rooms, this includes one forward teleporter into the next room
    /// and one backward teleporter to the previous room.
    /// </summary>
    /// <param name="room">The CuboidExhibitionRoom to generate the teleporters for.</param>
    private void CreateAndAttachTeleporters(CuboidExhibitionRoom room)
    {
      var index = GetRoomIndex(room.RoomData);
      var next = _rooms[GetNextRoomIndex(index)];
      var prev = _rooms[GetPreviousRoomIndex(index)];

      var nd = next.GetEntryPoint();
      var pd = prev.GetEntryPoint();

      var backPos = new Vector3(-.25f, 0, .2f);
      var nextPos = new Vector3(.25f, 0, .2f);

      // TODO Configurable TPBtnModel.
      var model = new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, null,
        TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"));

      if (_exhibition.rooms.Length > 1)
      {
        // Back teleporter.
        var backTpBtn = SteamVRTeleportButton.Create(room.gameObject, backPos, pd, model,
          Resources.Load<Sprite>("Sprites/UI/chevron-left"));

        backTpBtn.OnTeleportStart = room.OnRoomLeave;
        backTpBtn.OnTeleportEnd = prev.OnRoomEnter;

        // Forward teleporter.
        var nextTpBtn = SteamVRTeleportButton.Create(room.gameObject, nextPos, nd, model,
          Resources.Load<Sprite>("Sprites/UI/chevron-right"));

        nextTpBtn.OnTeleportStart = room.OnRoomLeave;
        nextTpBtn.OnTeleportEnd = next.OnRoomEnter;
      }

      // If we start in the lobby, also allow the user to teleport back to the lobby.
      if (VrepController.Instance.settings.StartInLobby)
      {
        var lobbyTpBtn = SteamVRTeleportButton.Create(room.gameObject, new Vector3(0, 0, .2f),
          VrepController.Instance.lobbySpawn, model, "Lobby");
        lobbyTpBtn.OnTeleportStart = room.OnRoomLeave;
      }
    }
  }
}