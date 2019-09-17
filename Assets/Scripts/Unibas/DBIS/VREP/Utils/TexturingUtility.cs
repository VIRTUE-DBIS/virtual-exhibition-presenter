using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class TexturingUtility
    {
        public static string Translate(string name)
        {
            var material = "";

            switch (name)
            {
                case "BRICKS":
                    material = "NBricksMaterial";
                    break;
                case "CONCRETE":
                    material = "NConcreteMaterial";
                    break;
                case "CONCRETETILES":
                    material = "NConcreteMaterial";
                    break;
                case "FABRIC":
                    material = "Fabric02Material";
                    break;
                case "GOLD":
                    material = "GoldMaterial";
                    break;
                case "LOUISXIV":
                    material = "LouisXIVMaterial";
                    break;
                case "SCRATCHES":
                    material = "ScratchesMaterial";
                    break;
                case "SILVER":
                    material = "ShinySilverMaterial";
                    break;
                case "STARS":
                    material = "StarMaterial";
                    break;
                case "TILES":
                    material = "TilesMaterial";
                    break;
                case "WALLPAPER":
                    material = "WallpaperMaterial";
                    break;
                case "WOOD1":
                    material = "NWoodFloorMaterial";
                    break;
                case "WOOD2":
                    material = "NWoodMaterial";
                    break;
                case "WOOD3":
                    material = "WoodMaterial3";
                    break;
                case "NONE":
                    material = "";
                    break;
                default:
                    material = name;
                    break;
            }

            return material;
        }

        public static Material LoadMaterialByName(string name, bool translate = false)
        {
            string material = name;
            if (translate)
            {
                material = Translate(name);
            }
            //else
            //{
                if (!material.EndsWith("Material"))
                {
                    material = material + "Material";
                }

                //material = name;
            //}


            if (!string.IsNullOrEmpty(material))
            {
                var mat =  Resources.Load("Materials/" + material, typeof(Material)) as Material;
                if (mat == null)
                {
                    Debug.LogWarning("Couldn't load material for name: "+material);
                }

                return mat;
            }
            else
            {
                return null; //TODO: return default material
            }
        }
    }
}