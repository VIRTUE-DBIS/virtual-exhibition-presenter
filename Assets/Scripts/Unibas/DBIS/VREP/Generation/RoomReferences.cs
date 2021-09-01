using System;
using System.Collections.Generic;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;

namespace Unibas.DBIS.VREP.Generation
{
  [Serializable]
  public class RoomReferences
  {
    // Map generation type to the ID of the generated room.
    public Dictionary<GenerationRequest.GenTypeEnum, string> References =
      new Dictionary<GenerationRequest.GenTypeEnum, string>();
  }
}