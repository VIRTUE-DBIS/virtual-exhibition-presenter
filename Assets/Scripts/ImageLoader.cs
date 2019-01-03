using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ImageLoader : MonoBehaviour {

	private MeshRenderer _renderer;

	// Use this for initialization
	void Start () {
		_renderer = GetComponent<MeshRenderer>();
		//StartCoroutine(LoadImage());
	}
		
	private IEnumerator LoadImage(string url)
	{
		Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, true);
		using (WWW www = new WWW(url))
		{
			yield return www;
			if (string.IsNullOrEmpty(www.error)) {
				www.LoadImageIntoTexture(tex);
        			GetComponent<Renderer>().material.mainTexture = tex;
			} else {
				Debug.LogError(www.error);
				Debug.LogError(www.url);
				Debug.LogError(www.responseHeaders);
			}
			
		}

		
		_renderer.material.mainTexture = tex;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="url"></param>
	public void ReloadImage(string url)
	{
		StartCoroutine(LoadImage(url));
	}
	
}


