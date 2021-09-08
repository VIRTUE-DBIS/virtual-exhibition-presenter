using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Multimedia;
using UnityEngine;

namespace Unibas.DBIS.VREP.World
{
  /// <summary>
  /// A cuboid exhibition room.
  /// The exhibition room has four walls of type <see cref="ExhibitionWall"/> (north, east, south. west).
  ///
  /// The walls manage the exhibits associated with them by themselves. Call PopulateWalls() for that purpose.
  /// The room may has 3D exhibits associated with it, which are placed by PopulateRoom().
  ///
  /// There are two handlers for entering and leaving the room: OnRoomEnter() and OnRoomLeave() respectively.
  ///
  /// The room is set up as its corresponding model is defined (RoomData). The virtual 3D appearance is driven by its model.
  /// </summary>
  [RequireComponent(typeof(AudioLoader))]
  public class CuboidExhibitionRoom : MonoBehaviour
  {
    /// <summary>
    /// The room's 3D appearance as a model.
    /// </summary>
    public CuboidRoomModel RoomModel { get; set; }

    /// <summary>
    /// The model of the room which defines its appearance, its walls and everything.
    /// This shall directly be passed from the backend server.
    /// </summary>
    public Room RoomData { get; set; }

    /// <summary>
    /// The actual model game object.
    /// </summary>
    public GameObject Model { get; set; }

    /// <summary>
    /// A list of ExhibitionWalls, representing the walls of this room.
    /// </summary>
    public List<ExhibitionWall> Walls { get; set; }

    /// <summary>
    /// The private audio loader component reference.
    /// </summary>
    private AudioLoader _audioLoader;

    /// <summary>
    /// Populates this room (walls load their exhibits).
    /// </summary>
    public async Task Populate()
    {
      PopulateRoom(); // Not yet implemented.
      await PopulateWalls();
    }

    /// <summary>
    /// Handler for leaving this room.
    /// Shall be called whenever the player leaves this room.
    /// </summary>
    public void OnRoomLeave()
    {
      if (_audioLoader != null)
      {
        _audioLoader.Stop();
      }

      try
      {
        gameObject.transform.Find("Timer").transform.GetComponent<MeshRenderer>().enabled = false;
      }
      catch (Exception e)
      {
        Debug.Log("No timer to disable was found.");
      }
    }

    /// <summary>
    /// Handler for entering this room.
    /// Shall be called whenever the player enters this room.
    /// </summary>
    public void OnRoomEnter()
    {
      LoadAmbientAudio();

      try
      {
        gameObject.transform.Find("Timer").transform.GetComponent<MeshRenderer>().enabled = true;
      }
      catch (Exception e)
      {
        Debug.Log("No timer to enable was found.");
      }
    }

    /// <summary>
    /// Places the exhibits in this room's space.
    /// Currently not implemented.
    /// </summary>
    public static void PopulateRoom()
    {
      Debug.LogWarning("Cannot place 3D objects yet.");
    }

    /// <summary>
    /// Induces the walls to place their exhibits.
    /// </summary>
    public async Task PopulateWalls()
    {
      foreach (var w in Walls)
      {
        await w.AttachExhibits();
      }
    }

    /// <summary>
    /// Returns the wall for the orientation.
    /// </summary>
    /// <param name="orientation">The orientation for which the wall is requested.</param>
    /// <returns>The ExhibitionWall component for the specified orientation.</returns>
    public ExhibitionWall GetWallForOrientation(Wall.DirectionEnum orientation)
    {
      return Walls.Find(wall => wall.GetOrientation() == orientation);
    }

    /// <summary>
    /// Loads the ambient audio of this room.
    /// As soon as the audio is loaded, it will be played.
    /// </summary>
    public void LoadAmbientAudio()
    {
      if (string.IsNullOrEmpty(RoomData.Ambient)) return;
      Debug.Log("Add audio to room.");

      if (_audioLoader == null)
      {
        _audioLoader = gameObject.AddComponent<AudioLoader>();
      }

      _audioLoader.ReloadAudio(RoomData.Ambient);
    }

    /// <summary>
    /// Gets the entry point of this room object.
    /// </summary>
    /// <returns>A 3D vector representing the entry point of this room.</returns>
    public Vector3 GetEntryPoint()
    {
      return transform.position + new Vector3(RoomData.EntryPoint.X, RoomData.EntryPoint.Y, RoomData.EntryPoint.Z);
    }

    /// <summary>
    /// Restores all exhibits on all walls and resets the countdown for all of them.
    /// </summary>
    public void RestoreWallExhibits()
    {
      Walls.ForEach(w => w.RestoreDisplayals());
      gameObject.transform.Find("Timer").GetComponent<Countdown>().Restart();
    }
  }
}