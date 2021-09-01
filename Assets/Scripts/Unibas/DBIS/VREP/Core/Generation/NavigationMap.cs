using System;
using System.Collections.Generic;

namespace Unibas.DBIS.VREP.Core.Generation
{
  [Serializable]
  public class NavigationMap
  {
    public string root;
    public Dictionary<string, string> Predecessor = new Dictionary<string, string>();
    public Dictionary<string, string> Successor = new Dictionary<string, string>();
  }
}