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

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [JsonConverter(typeof(StringEnumConverter))]
  public enum GenerationObject
  {
    EXHIBITION,
    ROOM
  }

  public class GenerationConfig
  {
    public GenerationObject genObj;
    public GenerationType genType;
    public int height;
    public List<String> idList;
    public int seed;
    public int width;

    public GenerationConfig(GenerationObject genObj, GenerationType genType, List<String> idList, int height, int width,
      int seed)
    {
      this.genObj = genObj;
      this.genType = genType;
      this.idList = idList;
      this.height = height;
      this.width = width;
      this.seed = seed;
    }
  }
}