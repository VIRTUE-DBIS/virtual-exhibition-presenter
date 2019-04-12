using System;
using UnityEngine;
using Graphics = UnityEngine.Graphics;

namespace DefaultNamespace {
  public class Utils {
    public static Vector2 cube_num(int width, int height)
    {
      float asp_ratio = (float) height / (float) width;
      float w_prime = 3;
      int h_prime = (int) (w_prime * asp_ratio);
      return new Vector2(w_prime, h_prime);
    }
  }
}