using System;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;

namespace Unibas.DBIS.VREP.Generation
{
  public static class GenTypeNaming
  {
    public static string GetName(this GenerationRequest.GenTypeEnum gr)
    {
      return gr switch
      {
        GenerationRequest.GenTypeEnum.SEMANTICSOM => ("Semantic SOM"),
        GenerationRequest.GenTypeEnum.VISUALSOM => ("Visual SOM"),
        GenerationRequest.GenTypeEnum.SEMANTICSIMILARITY => ("Semantic Sim."),
        GenerationRequest.GenTypeEnum.VISUALSIMILARITY => ("Visual Sim."),
        GenerationRequest.GenTypeEnum.RANDOM => ("Random"),
        _ => throw new ArgumentOutOfRangeException(nameof(gr), gr, null)
      };
    }
  }
}