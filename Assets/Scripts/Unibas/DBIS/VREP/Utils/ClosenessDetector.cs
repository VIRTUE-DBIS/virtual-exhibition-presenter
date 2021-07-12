using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Utils
{
  public class ClosenessDetector : MonoBehaviour
  {
    public string url;
    private AudioSource _audioSource;
    private bool _playing;
    private bool _downloading;

    public float maxDistance = 2;

    // Use this for initialization.
    private void Start()
    {
      _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
      var cameraPosition = Camera.allCameras[0].transform.position;
      var objectPosition = gameObject.transform.position;

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

    private IEnumerator LoadAudio(string audioURL)
    {
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

    public void Play()
    {
      if (_playing || AudioListener.pause || _audioSource == null || _audioSource.clip == null) return;
      AudioListener.pause = true;
      _audioSource.ignoreListenerPause = true;
      _audioSource.Play();
      _playing = true;
    }

    public void Stop()
    {
      if (_playing && AudioListener.pause && _audioSource != null && _audioSource.clip != null)
      {
        _audioSource.Stop();
        _playing = false;
        _audioSource.ignoreListenerPause = false;
        AudioListener.pause = false;
      }
    }
  }
}