using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.ObjImport;
using DefaultNamespace.VREM.Model;
using Unibas.DBIS.VREP;
using UnityEngine;
using Valve.VR.InteractionSystem;
using World;

[Obsolete("Got replaced by CuboidExhibitionRoom")]
public class Room : MonoBehaviour {

	private string _northWallName = "NorthWall";
	private string _eastWallName = "EastWall";
	private string _southWallName = "SouthWall";
	private string _westWallName = "WestWall";

	public GameObject PlanePrefab;
	
	public AudioLoader audio;

	private List<GameObject> displayedImages = new List<GameObject>();

	private bool LightingActivated = false;

	public GameObject GlobePrefab;

	// Use this for initialization
	void Start () {
		if (audio == null)
		{
			audio = gameObject.AddComponent<AudioLoader>();
		}
	}

	
	/// <summary>
	/// 
	/// </summary>
	void OnLeave()
	{
		//TODO: call this when leaving room
		audio.Stop();
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	private Wall GetWall(string name) {
		var go = transform.Find(name);
		string str = "Components: ";
		
		foreach( var e in go.transform.GetComponents<MonoBehaviour>()) {
			str += e.ToString();
		}
		
		Debug.Log("[Room] "+str);
		
		return transform.Find(name).GetComponent<Wall>();
	}

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="orientation"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private Wall GetWallForOrientation(WallOrientation orientation) {
		switch (orientation) {
			case WallOrientation.NORTH:
				return GetWall(_northWallName);
			case WallOrientation.EAST:
				return GetWall(_eastWallName);
			case WallOrientation.SOUTH:
				return GetWall(_southWallName);
			case WallOrientation.WEST:
				return GetWall(_westWallName);
			default:
				throw new ArgumentOutOfRangeException("orientation", orientation, null);
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="url">url to access image</param>
	/// <param name="wall">wall orientation</param>
	/// <param name="x">x coordinate for corresponding wall based on left lower anchor</param>
	/// <param name="y">x coordinate for corresponding wall based on left lower anchor</param>
	public Displayal Display(string url,  WallOrientation wall, float x, float y, float w, float h, bool lightOn =  true, string audioUrl  = null) {
		Debug.Log(string.Format("{0}, {1}, {2}/{3}, {4}/{5}",url,wall,x,y,w,h));
		GameObject displayal = Instantiate(PlanePrefab);

		var go = GameObject.Find("VirtualExhibitionManager");
		if (go.GetComponent<VREPController>().Settings.PlaygroundEnabled)
		{
			//var r = displayal.AddComponent<Rigidbody>();
			//r.useGravity = false;
			/*var i = displayal.AddComponent<Interactable>();
			i.hideHandOnAttach = true;
			i.useHandObjectAttachmentPoint = true;
			i.handFollowTransformPosition = true;
			i.handFollowTransformRotation = true;
			i.highlightOnHover = true;
			var t = displayal.AddComponent<Throwable>();
			t.releaseVelocityStyle = ReleaseStyle.NoChange;
			t.restoreOriginalParent = false;*/
		}
		
		if(!LightingActivated || !lightOn){	
			displayal.transform.Find("Directional light").gameObject.SetActive(false);
		}
		
		
		Displayal disp = displayal.gameObject.GetComponent<Displayal>();
		
		ImageLoader image = displayal.transform.Find("Plane").gameObject.AddComponent<ImageLoader>(); // Displayal
		//ImageLoader image = displayal.AddComponent<ImageLoader>();// ImageDisplayPlane
		image.ReloadImage(url);
		Debug.Log(GetWallForOrientation(wall));
		Vector3 pos = GetWallForOrientation(wall).CalculatePosition(transform.position,new Vector2(x,y));
		Vector3 rot = GetWallForOrientation(wall).CalculateRotation();
		displayal.transform.position = pos;
		displayal.transform.rotation = Quaternion.Euler(rot);
		displayal.transform.localScale = ScalingUtility.convertMeters2PlaneScaleSize(w, h);

		if (audioUrl != null)
		{
			Debug.Log("added audio to display object");
			var closenessDetector = displayal.AddComponent<ClosenessDetector>();
			closenessDetector.url = audioUrl;
		}

		
		displayedImages.Add(displayal);
		return disp;
	}

	
	// UNUSED
	private volatile bool first = true;
	
	
	// Update is called once per frame
	void Update () {
		/*
		if (first) {
			// TESTING: 
			Debug.Log("[Room] debug");
			Display("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/402px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg", WallOrientation.NORTH, 5,5, .53f, .77f);
			Display("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/402px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg", WallOrientation.EAST, 5,5, .53f, .77f);
			Display("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/402px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg", WallOrientation.SOUTH, 5,5, .53f, .77f);
			Display("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg/402px-Mona_Lisa%2C_by_Leonardo_da_Vinci%2C_from_C2RMF_retouched.jpg", WallOrientation.WEST, 5,5, .53f, .77f);
			first = false;
		}*/
	}

	private DefaultNamespace.VREM.Model.Room _roomModel;

	private Room next;
	private Room prev;

	public void SetNextRoom(Room next) {
		this.next = next;
	}

	public void SetPrevRoom(Room prev) {
		this.prev = prev;
	}

	public Room GetNextRoom() {
		return next;
	}

	public Room GetPrevRoom() {
		return prev;
	}
	

	public DefaultNamespace.VREM.Model.Room GetRoomModel() {
		return _roomModel;
	}

	public void Populate(DefaultNamespace.VREM.Model.Room room) {
		Debug.Log(room);
		Debug.Log(room.walls);
		_roomModel = room;

		
		Debug.Log("adjusting ceiling and floor");
		// TODO Use new material loading code
		this.gameObject.transform.Find("Ceiling").gameObject.GetComponent<TexturedMonoBehaviour>().LoadMaterial(TexturingUtility.Translate(room.ceiling));
		this.gameObject.transform.Find("Floor").gameObject.GetComponent<TexturedMonoBehaviour>().LoadMaterial(TexturingUtility.Translate(room.floor));
		
		/*Debug.Log("add globe");
		if(GlobePrefab != null){ //TODO: add a new check
			var globe = Instantiate(GlobePrefab) as GameObject;
			globe.transform.rotation = Quaternion.Euler(-90, -90, 0);
			globe.transform.position = new Vector3(-2.5f, 0, -2.5f);
			globe.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		}*/
		
		if (!string.IsNullOrEmpty(room.GetURLEncodedAudioPath()))
		{		
			Debug.Log("add audio to room");

			if (audio == null)
			{
				audio = gameObject.AddComponent<AudioLoader>();
			}
			
			audio.ReloadAudio(room.GetURLEncodedAudioPath());
		}
		//
		
		PopulateWalls(room.walls);
		//PlaceExhibits(room.exhibits);

		
		// DEBUG TESTING TODO REMOVE this when done
		//Debug.Log("Test");
		//LoadAndPlaceModel(ServerSettings.SERVER_ID+"content/get/5bd3292c64aa33a460bcdade%2f1%2fexhibits%2fearth.obj", new Vector3(0,1,0));
		
	}

	private void PlaceExhibits(DefaultNamespace.VREM.Model.Exhibit[] exhibits) {
		foreach (Exhibit exhibit in exhibits) {
			LoadAndPlaceModel(exhibit.GetURLEncodedPath(), exhibit.position);
		}
	}

	private void LoadAndPlaceModel(string url, Vector3 pos) {
		GameObject parent = new GameObject("Model Anchor");
		GameObject model = new GameObject("Model");
		model.transform.SetParent(parent.transform);
		parent.transform.position = pos;
		ObjLoader objLoader = model.AddComponent<ObjLoader>();
		model.transform.Rotate(-90,0,0);
		model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		objLoader.Load(url);
	}

	private void PopulateWalls(DefaultNamespace.VREM.Model.Wall[] walls) {
		foreach (var wall in walls) {
			if (wall.direction == "NORTH") {
				LoadExhibits(wall, WallOrientation.NORTH);
				GetWallForOrientation(WallOrientation.NORTH).LoadMaterial(TexturingUtility.Translate(wall.texture));
			}

			if (wall.direction == "EAST") {
				LoadExhibits(wall, WallOrientation.EAST);
				GetWallForOrientation(WallOrientation.EAST).LoadMaterial(TexturingUtility.Translate(wall.texture));
			}

			if (wall.direction == "SOUTH") {
				LoadExhibits(wall, WallOrientation.SOUTH);
				GetWallForOrientation(WallOrientation.SOUTH).LoadMaterial(TexturingUtility.Translate(wall.texture));
			}

			if (wall.direction == "WEST") {
				LoadExhibits(wall, WallOrientation.WEST);
				GetWallForOrientation(WallOrientation.WEST).LoadMaterial(TexturingUtility.Translate(wall.texture));
			}
		}
	}

	private void LoadExhibits(DefaultNamespace.VREM.Model.Wall wall, WallOrientation orientation) {
		foreach (Exhibit e in wall.exhibits) {
			Debug.Log(string.Format("E: {0}/{1} at {2}/{3}", e.position.x, e.position.y, e.size.x, e.size.y));
			var disp = Display(e.GetURLEncodedPath(), orientation, e.position.x, e.position.y, e.size.x, e.size.y, e.light, e.GetURLEncodedAudioPath());
			disp.SetExhibitModel(e);
		}
	}
}
