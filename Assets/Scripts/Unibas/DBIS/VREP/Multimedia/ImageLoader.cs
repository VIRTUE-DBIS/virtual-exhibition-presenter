using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Multimedia
{
  public class ImageLoader : MonoBehaviour
  {
    private MeshRenderer _renderer;

    private IEnumerator LoadImage(string url)
    {
      if (_renderer == null)
      {
        _renderer = GetComponent<MeshRenderer>();
      }

      var tex = new Texture2D(512, 512, TextureFormat.ARGB32, true);
      var hasError = false;
      using (var request = UnityWebRequestTexture.GetTexture(url))
      {
        yield return request.SendWebRequest();
        if (!(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError))
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

      _renderer.material.mainTexture = hasError ? Resources.Load<Texture>("Textures/not-available") : tex;

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
}