using UnityEngine;
using UnityEngine.UI;

namespace Unibas.DBIS.VREP.LegacyObjects
{
  public class Plaquette : MonoBehaviour
  {
    public Text text;

    public Font font;

    private void Start()
    {
      if (font != null)
      {
        text.font = font;
      }
    }
  }
}