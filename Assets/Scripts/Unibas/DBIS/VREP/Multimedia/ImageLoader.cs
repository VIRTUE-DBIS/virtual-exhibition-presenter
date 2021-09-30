using System;
using Ch.Unibas.Dmi.Dbis.Vrem.Client.Api;
using RestSharp.Extensions;
using UnityEngine;

namespace Unibas.DBIS.VREP.Multimedia
{
  /// <summary>
  /// Image loader component for exhibits.
  /// </summary>
  public class ImageLoader : MonoBehaviour
  {
    private MeshRenderer _renderer;

    public void AddImage(byte[] imageData)
    {
      if (_renderer == null)
      {
        _renderer = GetComponent<MeshRenderer>();
      }

      var tex = new Texture2D(512, 512, TextureFormat.ARGB32, true);

      tex.LoadImage(imageData);

      if (tex.height > 1024 || tex.width > 1024)
      {
        var resize = 1024.0 / Math.Max(tex.height, tex.width);
        TextureScale.Bilinear(tex, (int)(tex.width * resize), (int)(tex.height * resize));
      }

      _renderer.material.mainTexture = tex;

      GC.Collect();
    }

    /// <summary>
    /// Reloads an image from the provided URL.
    /// </summary>
    /// <param name="url">The full image URL.</param>
    public async void ReloadImage(string url)
    {
      var req = await new ContentApi().GetApiContentWithPathAsync(url);

      AddImage(req.ReadAsBytes());
    }
  }
}