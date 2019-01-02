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
            
            switch(name) {
                case "BRICKS": 
                    material = "NBricksMaterial";
                    break;
                case "CONCRETE": 
                    material = "NConcreteMaterial";
                    break;
                case "CONCRETETILES":
                    material = "NConcreteMaterial2";
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
                case"WOOD2":
                    material = "NWoodMaterial";
                    break;
                case"WOOD3":
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
        
        public static Material LoadMaterialByName(string name)
        {
            if (!name.EndsWith("Material")) {
                name = name + "Material";
            }
            var material = Translate(name);

            if (!string.IsNullOrEmpty(material))
            {
                return Resources.Load("Materials/" + material, typeof(Material)) as Material; 
            }
            else
            {
                return null; //TODO: return default material
            }
            
        }
    }
}