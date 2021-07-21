using System;
using Unibas.DBIS.VREP.VREM.Model;

namespace Unibas.DBIS.VREP.VREM
{
  /// <summary>
  /// ch.unibas.dmi.dbis.vrem.model.api.ListExhibitionsResponse
  /// </summary>
  [Serializable]
  public class ListExhibitionsResponse
  {
    public ExhibitionSummary[] exhibitions;
  }
}