using DefaultNamespace.VREM.Model;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP;
using UnityEngine;

public class Displayal : MonoBehaviour
{
    private Exhibit _exhibitModel;


    public string id;

    public Vector3 OriginalPosition;
    public Quaternion OriginalRotation;
    
    private CuboidModel _anchor = new CuboidModel(1,0.01f,.1f);
    

    public void RestorePosition() {
        transform.localPosition = OriginalPosition;
        transform.localRotation = OriginalRotation;
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

        if (VREPController.Instance.Settings.PlaygroundEnabled)
        {
            var magicOffset = 0.17f;
            
            var anch = ModelFactory.CreateCuboid(_anchor);
            var col = anch.AddComponent<BoxCollider>();
            col.center = new Vector3(_anchor.Width / 2, _anchor.Height / 2, _anchor.Depth/2);
            col.size = new Vector3(_anchor.Width, _anchor.Height, _anchor.Depth);
            anch.name = "Anchor (" + id + ")";
            anch.transform.parent = transform.parent;
            anch.transform.localPosition = new Vector3(_exhibitModel.position.x-_anchor.Width/2, _exhibitModel.position.y-(_exhibitModel.size.y/2+magicOffset), -_anchor.Depth); //0.2 is magic number for frame
            anch.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public Exhibit GetExhibit()
    {
        return _exhibitModel;
    }
}