using System;
using System.Collections.Generic;

namespace Unibas.DBIS.VREP.VREM
{
  [Serializable]
  public class NavigationMap
  {
    public string root;
    public Dictionary<string, string> predecessor = new Dictionary<string, string>();
    public Dictionary<string, string> successor = new Dictionary<string, string>();
  }
}