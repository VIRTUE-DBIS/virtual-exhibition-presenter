using System;
using System.Collections.Generic;
using Unibas.DBIS.VREP.Core;

namespace Unibas.DBIS.VREP.Generation
{
  public enum GenerationMethod
  {
    RandomAll,
    RandomList,
    VisualSimilarity,
    SemanticSimilarity,
    VisualSom,
    SemanticSom
  }

  public static class GenerationMethodUtil
  {
    public static string GetName(this GenerationMethod gr)
    {
      return gr switch
      {
        GenerationMethod.RandomAll => "Random (All)",
        GenerationMethod.RandomList => "Random (Cluster)",
        GenerationMethod.VisualSimilarity => "Visual Similarity",
        GenerationMethod.SemanticSimilarity => "Semantic Similarity",
        GenerationMethod.VisualSom => "Visual Cluster",
        GenerationMethod.SemanticSom => "Semantic Cluster",
        _ => throw new ArgumentOutOfRangeException(nameof(gr), gr, null)
      };
    }

    public static List<GenerationMethod> GetButtonTypes(int numIds)
    {
      var mode = VrepController.Instance.settings.GenerationSettings.ButtonMode;
      var types = new List<GenerationMethod>();

      // Some buttons only get added if there are IDs assigned to the image, as it would not make any sense otherwise.
      if (mode == ButtonMode.All)
        // types.Add(GenMethod.RandomAll);

        if (numIds > 1)
        {
          types.Add(GenerationMethod.RandomList);
        }

      if (mode == ButtonMode.Visual || mode == ButtonMode.All)
      {
        types.Add(GenerationMethod.VisualSimilarity);

        if (numIds > 1)
        {
          types.Add(GenerationMethod.VisualSom);
        }
      }

      if (mode == ButtonMode.Semantic || mode == ButtonMode.All)
      {
        types.Add(GenerationMethod.SemanticSimilarity);

        if (numIds > 1)
        {
          types.Add(GenerationMethod.SemanticSom);
        }
      }

      return types;
    }
  }
}