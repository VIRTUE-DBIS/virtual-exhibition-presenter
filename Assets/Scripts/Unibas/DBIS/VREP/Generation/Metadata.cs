namespace Unibas.DBIS.VREP.Generation
{
  public enum GenMetadata
  {
    // Per room.
    Generated, // Set by VREM (could be set via VREP).
    PredecessorRoom, // Room IDs, set by VREP.
    PredecessorExhibit, // Exhibit IDs, set by VREP.
    Seed, // Int, set by VREM (could be set via VREP).

    // Per exhibit.
    MemberIds, // List of IdDoublePairs, set by VREM.
    ObjectId, // Cineast object ID, set by VREM.
    References // References to other rooms, set by VREP.
  }

  public static class GenMetadataUtil
  {
    public static string GetKey(this GenMetadata mt)
    {
      var s = mt.ToString();

      if (s.Length <= 1)
      {
        return s.ToLower();
      }

      return char.ToLower(s[0]) + s.Substring(1);
    }
  }
}