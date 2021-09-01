using System;
using System.Collections.Generic;

namespace Unibas.DBIS.VREP.Generation
{
  [Serializable]
  public class NodeMap
  {
    public Dictionary<int, List<IdDoublePair>> map;
  }
}