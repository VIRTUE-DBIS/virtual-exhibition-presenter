using UnityEngine;
using UnityEngine.UI;

namespace Unibas.DBIS.VREP.LegacyObjects
{
  /// <summary>
  /// Plaquette component describing the title of an image.
  /// </summary>
  public class Plaquette : MonoBehaviour
  {
    public Text text;

    public Font font;

    /// <summary>
    /// Sets the text font if a font has been provided.
    /// </summary>
    private void Start()
    {
      if (font != null)
      {
        text.font = font;
      }
    }
  }
}