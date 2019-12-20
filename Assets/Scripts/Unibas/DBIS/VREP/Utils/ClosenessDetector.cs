using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;


public class ClosenessDetector : MonoBehaviour {
	public string url;
	private AudioSource audioSource;
	private bool playing = false;
	private bool downloading = false;

	public float maxDistance = 2;

	private WWW www = null;

	
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
		using (WWW www = new WWW (url)){
			yield return www;

			if (www.isDone){
				AudioClip audioClip = www.GetAudioClip(false, true) as AudioClip;
				this.audioSource.clip = audioClip;
			}
		}
	}

	
	/// <summary>
	/// 
	/// </summary>
	public void Play()
	{
		if (!playing && !AudioListener.pause && audioSource != null && audioSource.clip != null)
		{
			AudioListener.pause = true;
			audioSource.ignoreListenerPause = true;
			audioSource.Play();
			playing = true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void Stop()
	{
		if (playing && AudioListener.pause && audioSource != null  && audioSource.clip != null)
		{
			audioSource.Stop();
			playing = false;
			audioSource.ignoreListenerPause = false;
			AudioListener.pause = false;
		}
	}
   
	
}

