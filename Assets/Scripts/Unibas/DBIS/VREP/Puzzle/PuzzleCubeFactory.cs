using System;
using UnityEngine;

namespace Unibas.DBIS.VREP.Puzzle {
  
  
  public class PuzzleCubeFactory {

    /// <summary>
    /// Crops an image from its center into a square image.
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Vector2[] CropCenteredCoordinates(Texture image) {
      float w2 = image.width / 2f;
      float h2 = image.height / 2f;
      float s2 = Math.Min(image.width, image.width) / 2f;
      return new[] {
        new Vector2(w2-s2, h2-s2), new Vector2(w2+s2, h2+s2)
      };
    }
    
    public static Vector2[] TileImage(Texture2D image) {
      float w2 = image.width / 2f;
      float h2 = image.height / 2f;
      float s2 = Math.Min(image.width, image.width) / 2f;

      Vector2 tile0LowerLeft = new Vector2(w2 - s2, h2 - s2);
      Vector2 tile0UpperRight = new Vector2(w2, h2);
      Vector2 tile1LowerLeft = new Vector2(w2, h2 - s2);
      Vector2 tile1UpperRight = new Vector2(w2 + s2, h2);
      Vector2 tile2LowerLeft = new Vector2(w2, h2);
      Vector2 tile2UpperRight = new Vector2(w2 + s2, h2 + s2);
      Vector2 tile3LowerLeft = new Vector2(w2 - s2, h2);
      Vector2 tile3UpperRight = new Vector2(w2, h2);

      return new[] {
        tile0LowerLeft, tile0UpperRight,
        tile1LowerLeft, tile1UpperRight,
        tile2LowerLeft, tile2UpperRight,
        tile3LowerLeft, tile3UpperRight
      };
    }
  }
}