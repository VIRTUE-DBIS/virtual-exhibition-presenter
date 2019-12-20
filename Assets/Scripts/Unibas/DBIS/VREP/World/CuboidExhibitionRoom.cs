using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.World
{
    /// <summary>
    /// A cuboid exhibition room.
    /// The exhibition room has four walls of tyep <see cref="ExhibitionWall"/> (north, east, south. west).
    ///
    /// The walls manage the exhibits associated with them by themselves. Call PopulateWalls() for that purpose.
    /// The room may has 3d exhibits associated with it, which are placed by PopulateRoom().
    ///
    /// There are two handlers for entering and leaving the room: OnRoomEnter() and OnRoomLeave() respectively.
    /// It is expected that these handlers are called when appropriate.
    ///
    /// The room is set up as its corresponding model is defined (RoomData). The virtual 3d appearance is driven by its model.
    /// 
    /// </summary>
    [RequireComponent(typeof(AudioLoader))]
    public class CuboidExhibitionRoom : MonoBehaviour
    {
        /// <summary>
        /// The room's 3d appearance as a model.
        /// </summary>
        [SerializeField]
        public CuboidRoomModel RoomModel { get; set; }

        /// <summary>
        /// The model of the room which defines its appearance, its walls and everything.
        /// This shall directly be passed from the backend server.
        /// </summary>
        [SerializeField]
        public DefaultNamespace.VREM.Model.Room RoomData { get; set; }

        /// <summary>
        /// The actual model game object.
        /// </summary>
        public GameObject Model { get; set; }

        /// <summary>
        /// A list of ExhibitionWalls, which form the walls of this room.
        /// </summary>
        public List<ExhibitionWall> Walls { get; set; }

        /// <summary>
        /// The private audio loader component reference.
        /// </summary>
        private AudioLoader _audioLoader;

        private void Start()
        {
        }

        /// <summary>
        /// Populates this room.
        /// In other words: The walls will load their exhibits and this room will place its exhibits in its space.
        /// </summary>
        public void Populate()
        {
            PopulateRoom();
            PopulateWalls();
        }

        /// <summary>
        /// Handler for leaving this room.
        /// Shall be called whenever the player leaves this room
        /// </summary>
        public void OnRoomLeave()
        {
            if (_audioLoader != null)
            {
               _audioLoader.Stop(); 
            }
        }

        /// <summary>
        /// Handler for entering this room
        /// Shall be called whenever the player enters this room
        /// </summary>
        public void OnRoomEnter()
        {
            LoadAmbientAudio();
        }

        /// <summary>
        /// Places the exhibits in this room's space.
        /// Currently not implemented.
        /// </summary>
        public void PopulateRoom()
        {
            Debug.LogWarning("Cannot place 3d objects yet");
            /*
             * GameObject parent = new GameObject("Model Anchor");
		     * GameObject model = new GameObject("Model");
		     * model.transform.SetParent(parent.transform);
		     * parent.transform.position = pos;
		     * ObjLoader objLoader = model.AddComponent<ObjLoader>();
		     * model.transform.Rotate(-90,0,0);
		     * model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		     * objLoader.Load(url);
             */
        }

        /// <summary>
        /// Induces the walls to place their exhibits.
        /// </summary>
        public void PopulateWalls()
        {
            Walls.ForEach(ew => ew.AttachExhibits());
        }

        /// <summary>
        /// Returns the wall for the orientation.
        /// </summary>
        /// <param name="orientation">The orientation for which the wall is requested</param>
        /// <returns>The ExhibitionWall component for the specified orientation</returns>
        public ExhibitionWall GetWallForOrientation(WallOrientation orientation)
        {
            return Walls.Find(wall => wall.GetOrientation() == orientation);
        }

        /// <summary>
        /// Loads the ambient audio of this room.
        /// As soon as the audio is loaded, it will be played.
        /// </summary>
        public void LoadAmbientAudio()
        {
            if (!string.IsNullOrEmpty(RoomData.GetURLEncodedAudioPath()))
            {
                Debug.Log("add audio to room");


                if (_audioLoader == null)
                {
                    _audioLoader = gameObject.AddComponent<AudioLoader>();
                }

                _audioLoader.ReloadAudio(RoomData.GetURLEncodedAudioPath());
            }
        }

        public Vector3 GetEntryPoint()
        {
            return transform.position + RoomData.entrypoint;
        }

        public void RestoreWallExhibits() {
            Walls.ForEach(w => w.RestoreDisplayals());
        }
    }
}