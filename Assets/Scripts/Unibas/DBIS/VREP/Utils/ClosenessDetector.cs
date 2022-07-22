using System;
using System.Collections;
using Unibas.DBIS.VREP.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Utils
{
  /// <summary>
  /// Closeness detector component to load and play audio sources.
  /// </summary>
  public class ClosenessDetector : MonoBehaviour
  {
    public string url;
    private AudioSource _audioSource;
    private bool _playing;
    private bool _downloading;

    public float maxDistance = 2;

    /// <summary>
    /// Add component to audio source.
    /// </summary>
    private void Start()
    {
      _audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Play or stop audio if the camera is close enough to the object.
    /// </summary>
    private void Update()
    {
      var cameraPosition = Camera.allCameras[0].transform.position;
      var objectPosition = gameObject.transform.position;

      // Load audio if it hasn't been previously loaded/assigned.
      if (!string.IsNullOrEmpty(url) && _audioSource.clip == null && !_downloading)
      {
        _downloading = true;

        StartCoroutine(LoadAudio(url));
      }

      var dist = Vector3.Distance(cameraPosition, objectPosition);

      if (Math.Abs(dist) < maxDistance)
      {
        Play();
      }
      else
      {
        Stop();
      }
    }

    /// <summary>
    /// Sends a request to load the specified audio file from VREM.
    /// </summary>
    /// <param name="audioURL">The URL of the audio file.</param>
    /// <returns>The result yielded from the request.</returns>
    private IEnumerator LoadAudio(string audioURL)
    {
      // Unfortunately, there is no easy way to convert a byte array to an audio file.
      // Therefore, we do not use the generated VREM client to access the API here.

      audioURL = VrepController.Instance.settings.VremAddress + "api/content/" + audioURL.Replace("/", "%2F");

      using var request = UnityWebRequestMultimedia.GetAudioClip(audioURL, AudioType.OGGVORBIS);

      yield return request.SendWebRequest();

      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        var audioClip = DownloadHandlerAudioClip.GetContent(request);
        _audioSource.clip = audioClip;
      }
      else
      {
        Debug.LogError(request.error);
        Debug.LogError(request.url);
        Debug.LogError(request.GetResponseHeaders());
      }
    }

    /// <summary>
    /// Plays the audio file in this component.
    /// </summary>
    public void Play()
    {
      if (_playing || AudioListener.pause || _audioSource == null || _audioSource.clip == null) return;
      AudioListener.pause = true;
      _audioSource.ignoreListenerPause = true;
      _audioSource.Play();
      _playing = true;
    }

    /// <summary>
    /// Stops the audio file of this component.
    /// </summary>
    public void Stop()
    {
      if (!_playing || !AudioListener.pause || _audioSource == null || _audioSource.clip == null) return;
      _audioSource.Stop();
      _playing = false;
      _audioSource.ignoreListenerPause = false;
      AudioListener.pause = false;
    }
  }
}