using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class ClosenessDetector : MonoBehaviour {
	public string url;
	private AudioSource audioSource;
	private bool playing = false;
	private bool downloading = false;

	public float maxDistance = 2;
	
	// Use this for initialization
	private void Start()
	{
		audioSource =  gameObject.AddComponent<AudioSource>();
	}


	private void Update()
	{
		var cameraPosition = Camera.allCameras[0].transform.position;
		var objectPosition = this.gameObject.transform.position;

		if (!string.IsNullOrEmpty(url) && this.audioSource.clip == null && !downloading)
		{
			downloading = true;
			StartCoroutine(LoadAudio(url));	
		}

		var dist = Vector3.Distance(cameraPosition, objectPosition);
				
		if (Math.Abs(dist) < maxDistance){			
			Play();
		} else
		{
			Stop();
		}
	}

	private IEnumerator LoadAudio(string url)
	{     
		using (var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS)){
			yield return request;

			if (!(request.isNetworkError || request.isHttpError)) {
				var audioClip = DownloadHandlerAudioClip.GetContent(request);
				this.audioSource.clip = audioClip;
			}
			else
			{
				Debug.LogError(request.error);
				Debug.LogError(request.url);
				Debug.LogError(request.GetResponseHeaders());
			}
		}
	}

	
	/// <summary>
	/// 
	/// </summary>
	public void Play()
	{
		if (audioSource != null && audioSource.clip != null && playing == false)
		{
			audioSource.Play();
			playing = true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void Stop()
	{
		if (audioSource != null  && audioSource.clip != null && playing)
		{
			audioSource.Stop();
			playing = false;
		}
	}
   
	
}

