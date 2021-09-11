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
        GenMethod.RandomAll => ("Random (all)"),
        GenMethod.RandomList => ("Random (cluster)"),
        GenMethod.VisualSimilarity => ("Visual Sim."),
        GenMethod.SemanticSimilarity => ("Semantic Sim."),
        GenMethod.VisualSom => ("Visual SOM"),
        GenMethod.SemanticSom => ("Semantic SOM"),
        _ => throw new ArgumentOutOfRangeException(nameof(gr), gr, null)
      };
    }
  }
}