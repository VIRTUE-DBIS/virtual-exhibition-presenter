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

      // Do NOT use UnityWebRequestTexture here or you will run out of memory quickly if you load many images.
      using (var request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (!(request.result == UnityWebRequest.Result.ConnectionError ||
              request.result == UnityWebRequest.Result.ProtocolError))
        {
          tex.LoadImage(request.downloadHandler.data);

          // Rescale so we don't run out of memory upon loading huge images.
          // TODO This should only be a temporary resolution; consider adjusting this based on the exhibits size vector.
          if (tex.height > 1024 || tex.width > 1024)
          {
            var resize = 1024.0 / Math.Max(tex.height, tex.width);
            TextureScale.Bilinear(tex, (int) (tex.width * resize), (int) (tex.height * resize));
          }
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

    public void ReloadImage(string url)
    {
      StartCoroutine(LoadImage(url));
    }
  }
}