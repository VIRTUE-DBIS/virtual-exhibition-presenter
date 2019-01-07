using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace World
{
    /// <summary>
    /// Static class for object generation.
    /// </summary>
    public static class ObjectFactory
    {
        public static ExhibitionBuildingSettings Settings = ExhibitionBuildingSettings.Instance;

        public static GameObject GetDisplayalPrefab()
        {
            // TODO Use other prefabs
            /*if (Settings.UseStandardDisplayalPrefab)
            {*/
            var prefab = Resources.Load("Prefabs/"+Settings.StandardDisplayalPrefabName, typeof(GameObject)) as GameObject;
            if (prefab == null)
            {
                throw new Exception(string.Format("Could not load '{0}' as Resource", Settings.StandardDisplayalPrefabName));
            }
            
            return prefab;
            //}
        }

        public static GameObject BuildRoom(DefaultNamespace.VREM.Model.Room roomData)
        {
            Material[] mats =
            {
                TexturingUtility.LoadMaterialByName(roomData.floor),
                TexturingUtility.LoadMaterialByName(roomData.ceiling),
                GetMaterialForWallOrientation(WallOrientation.NORTH, roomData),
                GetMaterialForWallOrientation(WallOrientation.EAST, roomData),
                GetMaterialForWallOrientation(WallOrientation.SOUTH, roomData),
                GetMaterialForWallOrientation(WallOrientation.WEST, roomData)
            };
            CuboidRoomModel modelData = new CuboidRoomModel(roomData.position, roomData.size.x, roomData.size.y,
                mats[0], mats[1], mats[2], mats[3], mats[4], mats[5]);
            GameObject room = ModelFactory.CreateCuboidRoom(modelData);
            var er = room.AddComponent<CuboidExhibitionRoom>();
            er.RoomModel = modelData;
            er.Model = room;
            er.RoomData = roomData;
            var na = CreateAnchor(WallOrientation.NORTH, room, modelData);
            var ea = CreateAnchor(WallOrientation.EAST, room, modelData);
            var sa = CreateAnchor(WallOrientation.SOUTH, room, modelData);
            var wa = CreateAnchor(WallOrientation.WEST, room, modelData);

            var nw = CreateExhibitionWall(WallOrientation.NORTH, roomData, na);
            var ew = CreateExhibitionWall(WallOrientation.EAST, roomData, ea);
            var sw = CreateExhibitionWall(WallOrientation.SOUTH, roomData, sa);
            var ww = CreateExhibitionWall(WallOrientation.WEST, roomData, wa);
            
            er.Walls = new List<ExhibitionWall>(new []{nw,ew,sw,ww});
            er.Populate();
            
            GameObject light = new GameObject("RoomLight");
            var l = light.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 8;
            l.color = Color.white;
            l.intensity = 1.5f;
            l.renderMode = LightRenderMode.ForcePixel;
            //l.lightmapBakeType = LightmapBakeType.Mixed; // Build fails with this line uncommented, even though unity automatically upgrades to this one.
            //l.lightmappingMode = LightmappingMode.Mixed; // Build fails with this line uncommented. it is obsolete
            l.transform.parent = room.transform;
            l.transform.localPosition = new Vector3(0, 2.5f, 0);
            room.name = "Room";

            GameObject teleportArea = new GameObject("TeleportArea");
            var col = teleportArea.AddComponent<BoxCollider>();
            col.size = new Vector3(modelData.Size, 0.01f, modelData.Size);
            teleportArea.AddComponent<MeshRenderer>();
            var tpa = teleportArea.AddComponent<TeleportArea>();
            tpa.transform.parent = room.transform;
            tpa.transform.localPosition = new Vector3(0, 0.01f, 0);

            return room;
        }

        private static ExhibitionWall CreateExhibitionWall(WallOrientation orientation, DefaultNamespace.VREM.Model.Room room, GameObject anchor)
        {
            var wall = anchor.AddComponent<ExhibitionWall>();
            wall.Anchor = anchor;
            wall.WallModel = null;
            wall.WallData = room.GetWall(orientation);
            return wall;
        }

        private static Material GetMaterialForWallOrientation(WallOrientation orientation,
            DefaultNamespace.VREM.Model.Room roomData)
        {
            foreach (DefaultNamespace.VREM.Model.Wall wallData in roomData.walls)
            {
                WallOrientation wor = (WallOrientation) Enum.Parse(typeof(WallOrientation), wallData.direction, true);
                if (wor.Equals(orientation))
                {
                    return TexturingUtility.LoadMaterialByName(wallData.texture);
                }
            }

            throw new ArgumentException("Couldn't find material for orientation " + orientation + " in room at " +
                                        roomData.position);
        }

        private static GameObject CreateAnchor(WallOrientation orientation, GameObject room, CuboidRoomModel model)
        {
            GameObject anchor = new GameObject(orientation + "Anchor");
            anchor.transform.parent = room.transform;
            Vector3 pos = Vector3.zero;
            var a = 0f;
            var sizeHalf = model.Size / 2f;
            switch (orientation)
            {
                case WallOrientation.NORTH:
                    pos = new Vector3(-sizeHalf, 0, sizeHalf);
                    a = 0;
                    break;
                case WallOrientation.EAST:
                    pos = new Vector3(sizeHalf, 0, sizeHalf);
                    a = 90;
                    break;
                case WallOrientation.SOUTH:
                    pos = new Vector3(sizeHalf, 0, -sizeHalf);
                    a = 180;
                    break;
                case WallOrientation.WEST:
                    pos = new Vector3(-sizeHalf, 0, -sizeHalf);
                    a = 270;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("orientation", orientation, null);
            }
            anchor.transform.Rotate(Vector3.up, a);
            anchor.transform.localPosition = pos;
            return anchor;
        }
        
        private static ComplexCuboidModel GenerateButtonModel(float size, float border, float height)
        {
            ComplexCuboidModel model = new ComplexCuboidModel();
            // TODO Add material somehow
            model.Add(Vector3.zero, new CuboidModel(size, size, height));
            model.Add(new Vector3(border, border, -height), new CuboidModel(size - 2 * border, size - 2 * border, height));
            return model;
        }

        public static GameObject CreateTeleportButtonModel(Vector3 position, Vector3 destination, float size, float border)
        {
            var modelData = GenerateButtonModel(size, border, border / 2f);
            GameObject buttonObj = ModelFactory.CreateModel(modelData);
            PlayerTeleporter tpBtn = buttonObj.AddComponent<PlayerTeleporter>();
            tpBtn.Destination = destination;
            BoxCollider col = buttonObj.AddComponent<BoxCollider>();
            col.size = new Vector3(size,size,border*2);
            col.center = new Vector3(size/2f, size/2f, -border);
            buttonObj.AddComponent<Button>();
            var hand = new CustomEvents.UnityEventHand();
            hand.AddListener(h =>
            {
                tpBtn.TeleportPlayer();
            });
            buttonObj.AddComponent<UIElement>().onHandClick = hand;
            buttonObj.transform.position = position;
            buttonObj.name = "TeleportButton (Instance)";
            return buttonObj;
        }
        
        
        
        /// <summary>
        /// TODO Fix rotations!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Vector3 CalculateRotation(WallOrientation orientation) {
            switch (orientation) {
                case WallOrientation.NORTH:
                    return new Vector3(90, 180, 0);
                case WallOrientation.EAST:
                    return new Vector3(90, 0, 0);
                case WallOrientation.SOUTH:
                    return new Vector3(90, -180, 0);
                case WallOrientation.WEST:
                    return new Vector3(90, 90, 0);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Vector3 CalculateRotation(string orientation)
        {
            return CalculateRotation((WallOrientation) Enum.Parse(typeof(WallOrientation), orientation, true));
        }
    }
}