	using System;
   using System.Collections;
   using System.Collections.Generic;
   using DefaultNamespace;
   using UnityEngine;
   
   namespace DefaultNamespace
   
   {
       public class Utils
       {
       public static void Letterboxing(Texture2D image)
       {
       internal float width = image.Width;
       internal float height = image.Height;
       float target_size = Math.Max(width, height);
       float pad_size = (target_size - Math.Min(width, height))/2
                        
       Texture2D new_tex = new Texture2D(target_size, target_size, TextureFormat.ARGB32, true);
       Graphics g = Graphics.FromImage(new_tex);
       
       float scale = Math.Max((float)image.Width / target_size, (float)image.Height / target_size);
       PointF p = new PointF((target_size - ((float)image.Width / scale))/2, (target_size - ((float)image.Height / scale))/2s);
       SizeF size = new SizeF((float)image.Width / scale, (float)image.Height / scale);
       
       g.DrawImage(image, new RectangleF(p, size));
       
           return Texture2D;
   }
   }
   }