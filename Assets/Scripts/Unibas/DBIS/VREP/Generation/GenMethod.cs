using System;
using System.Collections.Generic;
using Unibas.DBIS.VREP.Core;

namespace Unibas.DBIS.VREP.Generation
{
  public enum GenMethod
  {
    RandomAll,
    RandomList,
    VisualSimilarity,
    SemanticSimilarity,
    VisualSom,
    SemanticSom
  }

  public static class GenTypeUtil
  {
    public static string GetName(this GenMethod gr)
    {
      return gr switch
      {
        GenMethod.RandomAll => "Random (All)",
        GenMethod.RandomList => "Random (Cluster)",
        GenMethod.VisualSimilarity => "Visual Similarity",
        GenMethod.SemanticSimilarity => "Semantic Similarity",
        GenMethod.VisualSom => "Visual Cluster",
        GenMethod.SemanticSom => "Semantic Cluster",
        _ => throw new ArgumentOutOfRangeException(nameof(gr), gr, null)
      };
    }

    public static List<GenMethod> GetButtonTypes(int numIds)
    {
      var mode = VrepController.Instance.settings.GenerationSettings.ButtonMode;
      var types = new List<GenMethod>();

      // Some buttons only get added if there are IDs assigned to the image, as it would not make any sense otherwise.
      if (mode == ButtonMode.All)
        // types.Add(GenMethod.RandomAll);

        if (numIds > 1)
        {
          types.Add(GenMethod.RandomList);
        }

      if (mode == ButtonMode.Visual || mode == ButtonMode.All)
      {
        types.Add(GenMethod.VisualSimilarity);

        if (numIds > 1)
        {
          types.Add(GenMethod.VisualSom);
        }
      }

      if (mode == ButtonMode.Semantic || mode == ButtonMode.All)
      {
        types.Add(GenMethod.SemanticSimilarity);

        if (numIds > 1)
        {
          types.Add(GenMethod.SemanticSom);
        }
      }

      return types;
    }
  }
}