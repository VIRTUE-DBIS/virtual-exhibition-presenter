#region

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#endregion

namespace Unibas.DBIS.VREP.Multimedia
{
  /// <summary>
  ///   Audio loader component for exhibits.
  /// </summary>
  public class AudioLoader : MonoBehaviour
  {
    private AudioSource _audioSource;
    private string _lastUrl;

    private bool _loaded;

    private void Start()
    {
      _audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    ///   Loads an audio source from the provided URL.
    /// </summary>
    /// <param name="url">The full audio URL.</param>
    /// <returns>The result yielded from the request.</returns>
    private IEnumerator LoadAudio(string url)
    {
      if (_loaded && _lastUrl.Equals(url))
      {
        Play();
        yield break;
      }

      using var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS);
      yield return request.SendWebRequest();

      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        var audioClip = DownloadHandlerAudioClip.GetContent(request);

        _audioSource.clip = audioClip;
        _audioSource.volume = 0.2f;
        _audioSource.loop = true;
        _audioSource.Play();
        _loaded = true;
        _lastUrl = url;
      }
      else
      {
        Debug.LogError(request.error);
        Debug.LogError(request.url);
        Debug.LogError(request.GetResponseHeaders());
      }
    }

    /// <summary>
    ///   Plays the audio which was previously loaded via ReloadAudio().
    /// </summary>
    public void Play()
    {
      if (_loaded)
        _audioSource.Play();
      else
        ReloadAudio(_lastUrl);
    }

    /// <summary>
    ///   Stops the audio which was previously loaded (and playing).
    /// </summary>
    public void Stop()
    {
      _audioSource.Stop();
    }

    /// <summary>
    ///   Reloads an audio source from the provided URL.
    /// </summary>
    /// <param name="url">The full audio source URL.</param>
    public void ReloadAudio(string url)
    {
      if (!string.IsNullOrEmpty(url)) StartCoroutine(LoadAudio(url));
    }
  }
}