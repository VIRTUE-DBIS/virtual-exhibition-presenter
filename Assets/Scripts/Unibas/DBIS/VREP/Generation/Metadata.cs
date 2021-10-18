namespace Unibas.DBIS.VREP.Generation
{
  /// <summary>
  /// Metadata fields set for exhibition, room, and exhibit models.
  /// Currently only used for generated exhibitions. 
  /// </summary>
  public enum GenerationMetadata
  {
    // Per room.
    Generated, // Set by VREM.
    PredecessorRoom, // Room IDs, set by VREP.
    PredecessorExhibit, // Exhibit IDs, set by VREP.
    Seed, // Int, set by VREM.

    // Per exhibit.
    MemberIds, // List of IdDoublePairs, set by VREM.
    ObjectId, // Cineast object ID, set by VREM.
    References // References to other rooms, set by VREP.
  }

  public static class GenerationMetadataUtil
  {
    public static string GetKey(this GenerationMetadata mt)
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