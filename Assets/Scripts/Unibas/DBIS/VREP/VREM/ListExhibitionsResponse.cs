using System;
using Unibas.DBIS.VREP.VREM.Model;

namespace Unibas.DBIS.VREP.VREM
{
  [Serializable]
  public class ListExhibitionsResponse
  {
    public ExhibitionSummary[] Exhibitions;
  }
}