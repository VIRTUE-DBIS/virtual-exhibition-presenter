using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Converters;

namespace Unibas.DBIS.VREP.VREM
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [JsonConverter(typeof(StringEnumConverter))]
  public enum GenerationType
  {
    SEMANTIC_SOM,
    VISUAL_SOM,
    SEMANTIC_SIMILARITY,
    VISUAL_SIMILARITY,
    RANDOM
  }

  public class GenerationConfig
  {
    public GenerationType genType;
    public List<String> idList;
    public int height;
    public int width;
    public int seed;

    public GenerationConfig(GenerationType genType, List<String> idList, int height, int width, int seed)
    {
      this.genType = genType;
      this.idList = idList;
      this.height = height;
      this.width = width;
      this.seed = seed;
    }
  }
}