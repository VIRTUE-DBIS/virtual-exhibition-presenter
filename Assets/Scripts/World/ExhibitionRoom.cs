using System.Collections.Generic;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.World
{
    public class ExhibitionRoom : MonoBehaviour
    {
        //TODO Add audio support!
        
        public CuboidRoomModel RoomModel { get; set; }
        
        [SerializeField]
        public DefaultNamespace.VREM.Model.Room RoomData { get; set; }
        
        public GameObject Model { get; set; }
        
        public List<ExhibitionWall> Walls { get; set; }

        public void Populate()
        {
            PopulateFloor();
            PopulateWalls();
        }
        
        public void PopulateFloor()
        {
            Debug.LogWarning("Cannot place 3d objects yet");
            
        }

        public void PopulateWalls()
        {
            Walls.ForEach(ew => ew.AttachExhibits());
        }
    }
}