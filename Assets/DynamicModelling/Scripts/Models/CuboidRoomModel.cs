using UnityEngine;

namespace Unibas.DBIS.DynamicModelling.Models
{
    [System.Serializable]
    public class CuboidRoomModel : IModel
    {
        public Vector3 Position;
        public float Size;
        public float Height;

        public Material FloorMaterial;
        public Material CeilingMaterial;
        public Material NorthMaterial;
        public Material EastMaterial;
        public Material SouthMaterial;
        public Material WestMaterial;

        public CuboidRoomModel(Vector3 position, float size, float height)
        {
            Position = position;
            Size = size;
            Height = height;
        }

        public CuboidRoomModel(Vector3 position, float size, float height, Material floorMaterial = null, Material ceilingMaterial = null, Material northMaterial = null, Material eastMaterial = null, Material southMaterial = null, Material westMaterial = null)
        {
            Position = position;
            Size = size;
            Height = height;
            FloorMaterial = floorMaterial;
            CeilingMaterial = ceilingMaterial;
            NorthMaterial = northMaterial;
            EastMaterial = eastMaterial;
            SouthMaterial = southMaterial;
            WestMaterial = westMaterial;
        }
    }
}