using System;
using UnityEngine;
using Graphics = UnityEngine.Graphics;

namespace DefaultNamespace {
  public class Utils {
    public static Texture2D Letterboxing(Texture2D image) {
      float width = image.width;
      float height = image.height;
      float target_size = Math.Max(width, height);
      float pad_size = (target_size - Math.Min(width, height)) / 2;

      Texture2D new_tex = new Texture2D(target_size, target_size, TextureFormat.ARGB32, true);
      Graphics g = Graphics.FromImage(new_tex);

      float scale = Math.Max((float) image.Width / target_size, (float) image.Height / target_size);

      PointF p = new PointF((target_size - ((float) image.Width / scale)) / 2,
        (target_size - ((float) image.Height / scale)) / 2);
      SizeF size = new SizeF((float) image.Width / scale, (float) image.Height / scale);

      g.DrawImage(image, new RectangleF(p, size));
      return new_tex;
    }
  }
}