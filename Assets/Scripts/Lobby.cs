using System.Collections;
using System.Collections.Generic;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class Lobby : MonoBehaviour
{


	private static ComplexCuboidModel GenerateButtonModel(float size, float border, float height)
	{
		ComplexCuboidModel model = new ComplexCuboidModel();
		// TODO Add material somehow
		model.Add(Vector3.zero, new CuboidModel(size, size, height));
		model.Add(new Vector3(border,border,-height),new CuboidModel(size-2*border, size-2*border,height) );
		return model;
	}
	
	// Use this for initialization
	void Start ()
	{
		float size = .1f, border = 0.01f, height = .02f;
		GameObject buttonObj = ModelFactory.CreateModel(GenerateButtonModel(size, border, height));
		TeleportButton tpBtn = buttonObj.AddComponent<TeleportButton>();
		BoxCollider col = buttonObj.AddComponent<BoxCollider>();
		col.size = new Vector3(size,size,height);
		col.center = new Vector3(size/2f,size/2f,-height);
		buttonObj.AddComponent<Button>();
		buttonObj.AddComponent<UIElement>().onHandClick.AddListener(hand =>
		{
			Debug.Log("CLICKED!");
			tpBtn.TeleportPlayer();
		});
		buttonObj.transform.position = new Vector3(4.9f,10-1.5f,4.9f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
