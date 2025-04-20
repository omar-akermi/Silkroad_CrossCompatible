using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace S1API.S1API.Internal.Utils
{
    internal class ImageUtils
    {
        public static Sprite LoadImage(string fileName)
        {
            string fullPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), fileName);
            if (!File.Exists(fullPath))
            {
                MelonLogger.Error($"❌ Icon file not found: {fullPath}");
                return null;
            }

            try
            {
                byte[] data = File.ReadAllBytes(fullPath);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(data))
                {
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error("❌ Failed to load sprite: " + ex);
            }

            return null;
        }
    }
}
