using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.VREM.Model;
using UnityEngine;

public class Displayal : MonoBehaviour {

	private Exhibit _exhibitModel;

	
	public void SetExhibitModel(Exhibit exhibit) {
		_exhibitModel = exhibit;
		var tp = transform.Find("TitlePlaquette");
		if (tp != null) {
			tp.GetComponent<Plaquette>().text.text = exhibit.name;
		} else {
			Debug.LogError("no tp");
		}

		var dp = transform.Find("DescriptionPlaquette");
		if (dp != null) {
			dp.GetComponent<Plaquette>().text.text = exhibit.description;
			///dp.gameObject.SetActive(false);
		} else {
			Debug.LogError("no dp");
		}
	}

	public Exhibit GetExhibit() {
		return _exhibitModel;
	}
	
	
	Renderer m_Renderer;
	// Use this for initialization
	void Start()
	{
		m_Renderer = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update()
	{
		
		
	}
}

