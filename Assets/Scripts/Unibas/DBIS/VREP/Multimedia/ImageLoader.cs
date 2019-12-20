using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour {

	private MeshRenderer _renderer;

	// Use this for initialization
	void Start () {
	}
		
	private IEnumerator LoadImage(string url)
	{
		if (_renderer == null)
		{
			_renderer = GetComponent<MeshRenderer>();
		}
		Texture2D tex = new Texture2D(512, 512, TextureFormat.ARGB32, true);
		var hasError = false;
		using (var request = UnityWebRequestTexture.GetTexture(url))
		{
			yield return request.SendWebRequest();
			if (!(request.isNetworkError || request.isHttpError))
			{
				tex = DownloadHandlerTexture.GetContent(request);
			}
			else
			{
				Debug.LogError(request.error);
				Debug.LogError(request.url);
				Debug.LogError(request.GetResponseHeaders());
				hasError = true;
			}
		}

		if (hasError)
		{
			_renderer.material.mainTexture = Resources.Load<Texture>("Textures/not-available");
		}
		else
		{
			_renderer.material.mainTexture = tex;
		}
		GC.Collect();
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


