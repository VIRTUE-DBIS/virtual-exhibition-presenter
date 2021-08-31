using Ch.Unibas.Dmi.Dbis.Vrem.Client.Model;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Core;
using UnityEngine;

namespace Unibas.DBIS.VREP.LegacyObjects
{
  /// <summary>
  /// Displayal object, GameObject representation of an Exhibit.
  /// </summary>
  public class Displayal : MonoBehaviour
  {
    // The corresponding exhibit for this displayal.
    private Exhibit _exhibitModel;
    public string id;

    public Vector3 originalPosition;
    public Quaternion originalRotation;

    private readonly CuboidModel _anchor = new CuboidModel(1, 0.01f, .1f);

    /// <summary>
    /// Restores the original position and resets velocity for this displayal.
    /// </summary>
    public void RestorePosition()
    {
      var t = transform;
      t.localPosition = originalPosition;
      t.localRotation = originalRotation;
      var rigid = GetComponent<Rigidbody>();
      if (rigid != null)
      {
        rigid.velocity = Vector3.zero;
      }
    }

    /// <summary>
    /// Sets the exhibit model for this displayal object, also processing the text to display on the plaquette
    /// and the image to load.
    /// </summary>
    /// <param name="exhibit"></param>
    public void SetExhibitModel(Exhibit exhibit)
    {
      _exhibitModel = exhibit;
      id = _exhibitModel.Id;
      name = "Displayal (" + id + ")";

      var tp = transform.Find("TitlePlaquette");
      if (tp != null)
      {
        if (string.IsNullOrEmpty(exhibit.Name))
        {
          tp.gameObject.SetActive(false);
        }
        else
        {
          tp.GetComponent<Plaquette>().text.text = exhibit.Name;
        }
      }
      else
      {
        Debug.LogError("no tp");
      }

      var dp = transform.Find("DescriptionPlaquette");
      if (dp != null)
      {
        if (string.IsNullOrEmpty(exhibit.Description))
        {
          dp.gameObject.SetActive(false);
        }
        else
        {
          dp.GetComponent<Plaquette>().text.text = exhibit.Description;
        }
      }
      else
      {
        Debug.LogError("no dp");
      }

      if (VrepController.Instance.settings.PlaygroundEnabled)
      {
        // TODO Find a fix so this works regardless of image dimensions.
        const float magicOffset = 0.5f;

        var anchor = ModelFactory.CreateCuboid(_anchor);
        var col = anchor.AddComponent<BoxCollider>();

        col.center = new Vector3(_anchor.width / 2, _anchor.height / 2, _anchor.depth / 2);
        col.size = new Vector3(_anchor.width, _anchor.height, _anchor.depth);

        anchor.name = "Anchor (" + id + ")";
        anchor.transform.parent = transform.parent;
        anchor.transform.localPosition = new Vector3(_exhibitModel.Position.X - _anchor.width / 2,
          _exhibitModel.Position.Y - (_exhibitModel.Size.Y / 2 + magicOffset),
          -_anchor.depth);
        anchor.transform.localRotation = Quaternion.Euler(Vector3.zero);
      }
    }

    /// <summary>
    /// Obtains the exhibit associated to this displayal.
    /// </summary>
    /// <returns>The associated exhibit model.</returns>
    public Exhibit GetExhibit()
    {
      return _exhibitModel;
    }
  }
}