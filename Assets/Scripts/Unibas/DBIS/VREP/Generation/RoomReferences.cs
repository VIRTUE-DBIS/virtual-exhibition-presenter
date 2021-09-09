using System;
using System.Collections.Generic;

namespace Unibas.DBIS.VREP.Generation
{
  [Serializable]
  public class RoomReferences
  {
    // Map generation type to the ID of the generated room for every exhibit (where rooms have already been generated).
    public Dictionary<string, string> References =
      new Dictionary<string, string>();
  }
}