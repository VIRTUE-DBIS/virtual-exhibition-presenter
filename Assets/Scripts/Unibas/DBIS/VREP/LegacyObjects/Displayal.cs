using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;

namespace Unibas.DBIS.VREP.LegacyObjects
{
  public class Displayal : MonoBehaviour
  {
    private Exhibit _exhibitModel;
    public string id;

    public Vector3 originalPosition;
    public Quaternion originalRotation;

    private readonly CuboidModel _anchor = new CuboidModel(1, 0.01f, .1f);

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

    public void SetExhibitModel(Exhibit exhibit)
    {
      _exhibitModel = exhibit;
      id = _exhibitModel.id;
      name = "Displayal (" + id + ")";

      var tp = transform.Find("TitlePlaquette");
      if (tp != null)
      {
        if (string.IsNullOrEmpty(exhibit.name))
        {
          tp.gameObject.SetActive(false);
        }
        else
        {
          tp.GetComponent<Plaquette>().text.text = exhibit.name;
        }
      }
      else
      {
        Debug.LogError("no tp");
      }

      var dp = transform.Find("DescriptionPlaquette");
      if (dp != null)
      {
        if (string.IsNullOrEmpty(exhibit.description))
        {
          dp.gameObject.SetActive(false);
        }
        else
        {
          dp.GetComponent<Plaquette>().text.text = exhibit.description;
        }
      }
      else
      {
        Debug.LogError("no dp");
      }

      if (VrepController.Instance.settings.PlaygroundEnabled)
      {
        const float magicOffset = 0.17f;

        var anchor = ModelFactory.CreateCuboid(_anchor);
        var col = anchor.AddComponent<BoxCollider>();

        col.center = new Vector3(_anchor.width / 2, _anchor.height / 2, _anchor.depth / 2);
        col.size = new Vector3(_anchor.width, _anchor.height, _anchor.depth);

        anchor.name = "Anchor (" + id + ")";
        anchor.transform.parent = transform.parent;
        anchor.transform.localPosition = new Vector3(_exhibitModel.position.x - _anchor.width / 2,
          _exhibitModel.position.y - (_exhibitModel.size.y / 2 + magicOffset),
          -_anchor.depth); // 0.2 is the magic number for frame.
        anchor.transform.localRotation = Quaternion.Euler(Vector3.zero);
      }
    }

    public Exhibit GetExhibit()
    {
      return _exhibitModel;
    }
  }
}