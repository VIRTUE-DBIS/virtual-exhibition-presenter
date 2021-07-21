﻿using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.Utils;
using UnityEngine;

namespace Unibas.DBIS.VREP.World
{
  /// <summary>
  /// Lobby setup.
  /// </summary>
  public class Lobby : MonoBehaviour
  {
    private SteamVRTeleportButton _text;
    private SteamVRTeleportButton _next;
    private SteamVRTeleportButton _wall;

    /// <summary>
    /// Create teleport button objects for the lobby.
    /// </summary>
    private void Start()
    {
      _text = SteamVRTeleportButton.Create(gameObject, new Vector3(0, 0, 4.5f), Vector3.zero,
        new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, null,
          TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
        "Text");

      _next = SteamVRTeleportButton.Create(gameObject, new Vector3(.5f, 0, 4.5f), Vector3.zero,
        new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("NWood"),
          TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
        Resources.Load<Sprite>("Sprites/UI/chevron-right"));

      _wall = SteamVRTeleportButton.Create(gameObject, new Vector3(0, 1.5f, 4.98f), Vector3.zero,
        new SteamVRTeleportButton.TeleportButtonModel(0.2f, .02f, 2f, null,
          TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"),
          false),
        "Wall");
    }

    /// <summary>
    /// Sets the triggers for the teleport buttons to execute upon entering an exhibition room.
    /// Currently only supports 1 room!
    /// </summary>
    /// <param name="manager">The ExhibitionManager object holding room details.</param>
    public void ActivateRoomTrigger(ExhibitionManager manager)
    {
      var room = manager.GetRoomByIndex(0);

      if (room == null) return;

      _text.OnTeleportEnd = room.OnRoomEnter;
      _next.OnTeleportEnd = room.OnRoomEnter;
      _wall.OnTeleportEnd = room.OnRoomEnter;
    }
  }
}