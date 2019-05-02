using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.Skia
{
    public static class Utility
    {
        public static SKPoint ToSKPoint(this Vector2 self)
        {
            return new SKPoint(self.X, self.Y);
        }
    }
}
