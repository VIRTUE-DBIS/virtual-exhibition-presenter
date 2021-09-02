namespace Unibas.DBIS.VREP.Core
{
  public static class MetadataMethods
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
    Generated, // Per room (bool), set by VREM (could be set via VREP).
    Predecessor, // Per room (refers room ID), set by VREP.
    MemberIds, // Per exhibit (list of IdDoublePairs), set by VREM.
    Seed // Per room (int), set by VREM (could be set via VREP).
  }
}