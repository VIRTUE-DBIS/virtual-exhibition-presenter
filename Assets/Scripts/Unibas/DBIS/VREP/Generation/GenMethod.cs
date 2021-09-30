using System;

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

  public static class GenMethodStringify
  {
    public static string GetName(this GenMethod gr)
    {
      return gr switch
      {
        GenMethod.RandomAll => ("Random (All)"),
        GenMethod.RandomList => ("Random (Cluster)"),
        GenMethod.VisualSimilarity => ("Visual Similarity"),
        GenMethod.SemanticSimilarity => ("Semantic Similarity"),
        GenMethod.VisualSom => ("Visual Cluster"),
        GenMethod.SemanticSom => ("Semantic Cluster"),
        _ => throw new ArgumentOutOfRangeException(nameof(gr), gr, null)
      };
    }
  }
}