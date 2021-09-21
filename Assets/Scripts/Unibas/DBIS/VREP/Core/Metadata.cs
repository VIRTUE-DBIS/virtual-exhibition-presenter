namespace Unibas.DBIS.VREP.Core
{
  public static class MetadataUtil
  {
    public static string GetKey(this MetadataType mt)
    {
      var s = mt.ToString();

      if (s.Length <= 1)
      {
        return s.ToLower();
      }

      return char.ToLower(s[0]) + s.Substring(1);
    }
  }

  public enum MetadataType
  {
    // Per room.
    Generated, // Set by VREM (could be set via VREP).
    Predecessor, // Room or exhibit IDs, set by VREP.
    Seed, // Int, set by VREM (could be set via VREP).

    // Per exhibit.
    MemberIds, // List of IdDoublePairs, set by VREM.
    ObjectId, // Cineast object ID, set by VREM.
    References // References to other rooms, set by VREP.
  }
}