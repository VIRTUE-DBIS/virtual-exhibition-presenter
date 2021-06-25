using UnityEngine;

namespace Unibas.DBIS.VREP.Utils
{
  public static class TexturingUtility
  {
    public static string Translate(string name)
    {
      var material = name switch
      {
        "BRICKS" => "NBricksMaterial",
        "CONCRETE" => "NConcreteMaterial",
        "CONCRETETILES" => "NConcreteMaterial",
        "FABRIC" => "Fabric02Material",
        "GOLD" => "GoldMaterial",
        "LOUISXIV" => "LouisXIVMaterial",
        "SCRATCHES" => "ScratchesMaterial",
        "SILVER" => "ShinySilverMaterial",
        "STARS" => "StarMaterial",
        "TILES" => "TilesMaterial",
        "WALLPAPER" => "WallpaperMaterial",
        "WOOD1" => "NWoodFloorMaterial",
        "WOOD2" => "NWoodMaterial",
        "WOOD3" => "WoodMaterial3",
        "NONE" => "",
        _ => name
      };

      return material;
    }

    public static Material LoadMaterialByName(string name, bool translate = false)
    {
      var material = name;
      if (translate)
      {
        material = Translate(name);
      }

      if (!material.EndsWith("Material"))
      {
        material += "Material";
      }

      if (!string.IsNullOrEmpty(material))
      {
        var mat = Resources.Load("Materials/" + material, typeof(Material)) as Material;
        if (mat == null)
        {
          Debug.LogWarning("Couldn't load material for name: " + material);
        }

        return mat;
      }


      return null; //TODO: return default material
    }
  }
}