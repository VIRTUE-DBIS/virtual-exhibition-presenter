using UnityEngine;

namespace Unibas.DBIS.VREP.Utils
{
  /// <summary>
  /// Utility for loading textures/Material objects.
  /// </summary>
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

    /// <summary>
    /// Loads a Material object by name (e.g., from a wall model of VREM).
    /// </summary>
    /// <param name="name">The name of the texture.</param>
    /// <param name="translate">Whether to translate from a shorter/more intuitive name.</param>
    /// <returns>The loaded Material according to the specified texture.</returns>
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


      return null; // TODO Return default material.
    }
  }
}