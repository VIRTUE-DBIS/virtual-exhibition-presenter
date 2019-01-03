using System;
using System.Collections.Generic;
using System.Linq;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;

namespace Unibas.DBIS.DynamicModelling
{
    public static class ModelFactory
    {
        /// <summary>
        /// Creates a wall between two positions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="height"></param>
        /// <param name="materialName"></param>
        /// <returns></returns>
        public static GameObject CreatePositionedWall(Vector3 start, Vector3 end, float height,
            string materialName = null)
        {
            float width = Vector3.Distance(start, end);
            float a = Vector3.Angle(end - start, Vector3.right);
            GameObject go = new GameObject("PositionedWall");
            GameObject wall = CreateWall(width, height, materialName);

            wall.transform.parent = go.transform;
            wall.transform.position = Vector3.zero;
            wall.transform.Rotate(Vector3.up, -a);
            go.transform.position = start;
            return go;
        }
        
        public static GameObject CreateWall(WallModel model)
        {
            float width = Vector3.Distance(model.Start, model.End);
            float a = Vector3.Angle(model.Start - model.End, Vector3.right);
            GameObject go = new GameObject("PositionedWall");
            GameObject wall = CreateWall(width, model.Height, model.Material);

            wall.transform.parent = go.transform;
            wall.transform.position = Vector3.zero;
            wall.transform.Rotate(Vector3.up, -a);
            go.transform.position = model.Start;
            go.AddComponent<ModelContainer>().Model = model;
            return go;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">center of room</param>
        /// <param name="size"></param>
        /// <param name="height"></param>
        /// <param name="materialNames">0 floor, 1 ceiling, 2 north (pos z), 3 east (pos x), 4 south (neg z), 5 west (neg x)</param>
        /// <returns></returns>
        public static GameObject CreateCuboidRoom(Vector3 position, float size, float height, string[] materialNames)
        {
            GameObject root = new GameObject("SquareRoom");

            float halfSize = size / 2f;

            // North wall
            GameObject north = CreateWall(size, height, materialNames[2]);
            north.name = "NorthWall";
            north.transform.parent = root.transform;
            north.transform.position = new Vector3(-halfSize, 0, halfSize);
            // East wall
            GameObject east = CreateWall(size, height, materialNames[3]);
            east.name = "EastWall";
            east.transform.parent = root.transform;
            east.transform.position = new Vector3(halfSize, 0, halfSize);
            east.transform.Rotate(Vector3.up, 90);
            // South wall
            GameObject south = CreateWall(size, height, materialNames[4]);
            south.name = "SouthWall";
            south.transform.parent = root.transform;
            south.transform.position = new Vector3(halfSize, 0, -halfSize);
            south.transform.Rotate(Vector3.up, 180);
            // West wall
            GameObject west = CreateWall(size, height, materialNames[5]);
            west.name = "WestWall";
            west.transform.parent = root.transform;
            west.transform.position = new Vector3(-halfSize, 0, -halfSize);
            west.transform.Rotate(Vector3.up, 270);

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;

            GameObject floor = CreateWall(size, size, materialNames[0]);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;
            // North Aligned
            floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            // East Aligned
            //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
            //floorAnchor.transform.Rotate(Vector3f.back,90);

            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;

            GameObject ceiling = CreateWall(size, size, materialNames[1]);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;

            root.transform.position = position;
            // North Aligned
            ceilingAnchor.transform.position = new Vector3(halfSize, height, halfSize);
            ceilingAnchor.transform.Rotate(Vector3.right, -90);
            // East Aligned
            //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
            //ceilingAnchor.transform.Rotate( Vector3.back, -90);

            return root;
        }

        public static GameObject CreateCuboidRoom(CuboidRoomModel model)
        {
            GameObject root = new GameObject("CuboidRoom");

            float halfSize = model.Size / 2f;

            // North wall
            GameObject north = CreateWall(model.Size, model.Height, model.NorthMaterial);
            north.name = "NorthWall";
            north.transform.parent = root.transform;
            north.transform.position = new Vector3(-halfSize, 0, halfSize);
            // East wall
            GameObject east = CreateWall(model.Size, model.Height, model.EastMaterial);
            east.name = "EastWall";
            east.transform.parent = root.transform;
            east.transform.position = new Vector3(halfSize, 0, halfSize);
            east.transform.Rotate(Vector3.up, 90);
            // South wall
            GameObject south = CreateWall(model.Size, model.Height, model.SouthMaterial);
            south.name = "SouthWall";
            south.transform.parent = root.transform;
            south.transform.position = new Vector3(halfSize, 0, -halfSize);
            south.transform.Rotate(Vector3.up, 180);
            // West wall
            GameObject west = CreateWall(model.Size, model.Height, model.WestMaterial);
            west.name = "WestWall";
            west.transform.parent = root.transform;
            west.transform.position = new Vector3(-halfSize, 0, -halfSize);
            west.transform.Rotate(Vector3.up, 270);

            // Floor
            GameObject floorAnchor = new GameObject("FloorAnchor");
            floorAnchor.transform.parent = root.transform;

            GameObject floor = CreateWall(model.Size, model.Size, model.FloorMaterial);
            floor.name = "Floor";
            floor.transform.parent = floorAnchor.transform;
            // North Aligned
            floorAnchor.transform.position = new Vector3(-halfSize, 0, -halfSize);
            floorAnchor.transform.Rotate(Vector3.right, 90);
            // East Aligned
            //floorAnchor.transform.position = new Vector3(-halfSize, 0, halfSize);
            //floorAnchor.transform.Rotate(Vector3f.back,90);

            // Ceiling
            GameObject ceilingAnchor = new GameObject("CeilingAnchor");
            ceilingAnchor.transform.parent = root.transform;

            GameObject ceiling = CreateWall(model.Size, model.Size, model.CeilingMaterial);
            ceiling.name = "Ceiling";
            ceiling.transform.parent = ceilingAnchor.transform;

            
            // North Aligned
            ceilingAnchor.transform.position = new Vector3(-halfSize, model.Height, halfSize);
            ceilingAnchor.transform.Rotate(Vector3.right, -90);
            // East Aligned
            //ceilingAnchor.transform.position = new Vector3(halfSize, height, -halfSize);
            //ceilingAnchor.transform.Rotate( Vector3.back, -90);

            root.transform.position = model.Position;
            
            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }


        /// <summary>
        /// Creates a wall game object to position later.
        /// The wall is flat and always "upright", e.g. the normal of the mesh is negative z.
        /// Use the resulting gameobject to rotate and re-position the wall.
        /// </summary>
        /// <param name="width">The width of the wall in Unity units</param>
        /// <param name="height">The height of the wall in Unity units</param>
        /// <param name="materialName">The wall's material name. Expects the material file to be at Resources/Materials/materalName. If no present, the word Material will be suffixed.</param>
        /// <returns></returns>
        public static GameObject CreateWall(float width, float height, string materialName = null)
        {
            GameObject go = new GameObject("Wall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(width, 0, 0);
            vertices[2] = new Vector3(0, height, 0);
            vertices[3] = new Vector3(width, height, 0);

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            float xUnit = 1;
            float yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(xUnit, 0);
            uv[2] = new Vector2(0, yUnit);
            uv[3] = new Vector2(xUnit, yUnit);

            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (!string.IsNullOrEmpty(materialName))
            {
                if (!materialName.EndsWith("Material"))
                {
                    materialName = materialName + "Material";
                }

                Material mat = Resources.Load<Material>("Materials/" + materialName);
                meshRenderer.material.CopyPropertiesFromMaterial(mat);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = mat.name;
            }


            return go;
        }

        public static GameObject CreateWall(float width, float height, Material mat = null)
        {
            GameObject go = new GameObject("Wall");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(width, 0, 0);
            vertices[2] = new Vector3(0, height, 0);
            vertices[3] = new Vector3(width, height, 0);

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            float xUnit = 1;
            float yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(xUnit, 0);
            uv[2] = new Vector2(0, yUnit);
            uv[3] = new Vector2(xUnit, yUnit);

            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (mat != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(mat);
                //meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1,1));
                meshRenderer.material.name = mat.name;
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.color = Color.white;
            }


            return go;
        }


        private static Vector3 CalculateUnit(Vector3 dimensions)
        {
            float m = Math.Max(Math.Max(dimensions.x, dimensions.y), dimensions.z);
            return new Vector3(m/dimensions.x, m/dimensions.y, m/dimensions.z);
        }
        
        private static Vector2 CalculateUnit(float width, float height)
        {
            return CalculateNormalizedToLeastSquareUnit(width, height);
            //return CalculateNormalizedToOneUnit(width, height);
        }

        private static Vector2 CalculateNormalizedToLeastSquareUnit(float width, float height)
        {
            float xUnit = 1,
                yUnit = 1;

            if (width > height)
            {
                xUnit = width / height;
            }
            else
            {
                yUnit = height / width;
            }

            return new Vector2(xUnit, yUnit);
        }

        private static Vector2 CalculateNormalizedToOneUnit(float width, float height)
        {
            return new Vector2(1f / width, 1f / height);
        }

        public static GameObject CreateCuboid(CuboidModel cuboid)
        {
            GameObject cub = CreateCuboid(cuboid.Width, cuboid.Height, cuboid.Depth);
            MeshRenderer meshRenderer = cub.GetComponent<MeshRenderer>();
            if (cuboid.Material != null)
            {
                meshRenderer.material.CopyPropertiesFromMaterial(cuboid.Material);
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.material.name = "Default";
                meshRenderer.material.color = Color.white;
            }
            cub.AddComponent<ModelContainer>().Model = cuboid;
            return cub;
        }

        public static GameObject CreateCuboid(float width, float height, float depth)
        {
            GameObject go = new GameObject("Cuboid");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;

            // The naming is always from the front and downwards looking! e.g. From the back, left and right is swapped
            Vector3 frontLeftDown = Vector3.zero;
            Vector3 frontRightDown = new Vector3(width, 0, 0);
            Vector3 frontLeftUp = new Vector3(0, height, 0);
            Vector3 frontRightUp = new Vector3(width, height, 0);

            Vector3 backLeftDown = new Vector3(0, 0, depth);
            Vector3 backRightDown = new Vector3(width, 0, depth);
            Vector3 backLeftUp = new Vector3(0, height, depth);
            Vector3 backRightUp = new Vector3(width, height, depth);

            Vector3[] vertices = new[]
            {
                // Front
                frontLeftDown, frontRightDown, frontLeftUp, frontRightUp,
                // Back
                backLeftDown, backRightDown, backLeftUp, backRightUp,
                // Left
                backLeftDown, frontLeftDown, backLeftUp, frontLeftUp,
                // Right
                frontRightDown, backRightDown, frontRightUp, backRightUp,
                // Up
                frontLeftUp, frontRightUp, backLeftUp, backRightUp,
                // Down
                frontLeftDown, frontRightDown, backLeftDown, backRightDown
            };
            mesh.vertices = vertices;

            int[] triangles = new[]
            {
                // Front
                0, 2, 1, 2, 3, 1,
                // Back
                5, 7, 4, 7, 6, 4,
                // Left
                8, 10, 9, 10, 11, 9,
                // Right
                12, 14, 13, 14, 15, 13,
                // Up
                16, 18, 17, 18, 19, 17,
                // Down
                21, 23, 20, 23, 22, 20
            };
            mesh.triangles = triangles;

            Vector3[] normals = new[]
            {
                // Front
                -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
                // Back
                -Vector3.back, -Vector3.back, -Vector3.back, -Vector3.back,
                // Left
                -Vector3.left, -Vector3.left, -Vector3.left, -Vector3.left,
                // Right
                -Vector3.right, -Vector3.right, -Vector3.right, -Vector3.right,
                // Up
                -Vector3.up, -Vector3.up, -Vector3.up, -Vector3.up,
                // Down
                -Vector3.down, -Vector3.down, -Vector3.down, -Vector3.down
            };
            mesh.normals = normals;

            // Cube based uv mapping

            Vector3 units = CalculateUnit(new Vector3(width, height, depth));
            
            Vector2 xyUnits = new Vector2(units.x,units.y);
            Vector2 xzUnits = new Vector2(units.x, units.z);
            Vector2 yzUnits = new Vector2(units.y, units.z);
            
            Debug.Log(units);
            
            /*
            Vector2 xyUnits = CalculateUnit(width, height);
            Vector2 xzUnits = CalculateUnit(width, depth);//CalculateUnit(width, depth);
            Vector2 yzUnits = CalculateUnit(depth, height);//CalculateUnit(depth, height);
            */

            float xyThird = xyUnits.y / 3f;
            float xyQuart = xyUnits.x / 4f;

            float xzThird = xzUnits.y / 3f;
            float xzQuart = xzUnits.x / 4f;

            float yzThird = yzUnits.y / 3f;
            float yzQuart = yzUnits.x / 4f;

            Vector2[] uv = new[]
            {
                // Front
                new Vector2(xyQuart, xyThird), new Vector2(2 * xyQuart, xyThird), new Vector2(xyQuart, 2 * xyThird),
                new Vector2(2 * xyQuart, 2 * xyThird),
                // Back
                new Vector2(3 * xyQuart, xyThird), new Vector2(4 * xyQuart, xyThird),
                new Vector2(3 * xyQuart, 2 * xyThird),
                new Vector2(4 * xyQuart, 2 * xyThird),
                // Left
                new Vector2(0, yzThird), new Vector2(yzQuart, yzThird), new Vector2(0, 2 * yzThird),
                new Vector2(yzQuart, 2 * yzThird),
                // Right
                new Vector2(2 * yzQuart, yzThird), new Vector2(3 * yzQuart, yzThird),
                new Vector2(2 * yzQuart, 2 * yzThird),
                new Vector2(3 * yzQuart, 2 * yzThird),
                // Up
                new Vector2(xzQuart, 2 * xzThird), new Vector2(2 * xzQuart, 2 * xzThird),
                new Vector2(xzQuart, 3 * xzThird),
                new Vector2(2 * xzQuart, 3 * xzThird),
                // Down
                new Vector2(xzQuart, 0), new Vector2(2 * xzQuart, 0), new Vector2(xzQuart, xzThird),
                new Vector2(2 * xzQuart, xzThird)
            };

            //List<string> list= new List<string>();
            //uv.ToList().ForEach(vector2 => list.Add(vector2.ToString()));
            //Debug.Log("[Cuboid] "+string.Join(", ",list.ToArray()));
            
            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.name = "Default";
            meshRenderer.material.color = Color.green;

            return go;
        }

        public static GameObject CreateModel(ComplexCuboidModel model)
        {
            GameObject root = new GameObject("ComplexCuboid");
            for (int i = 0; i < model.Size(); i++)
            {
                Vector3 pos = model.GetPositionAt(i);
                CuboidModel cuboid = model.GetCuboidAt(i);
                GameObject cub = CreateCuboid(cuboid);
                cub.transform.parent = root.transform;
                cub.transform.position = pos;
            }
            root.AddComponent<ModelContainer>().Model = model;
            return root;
        }
    }
}