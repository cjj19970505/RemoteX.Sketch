using RemoteX.Sketch.CoreModule;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.Skia
{
    public interface ISkiaRenderer
    {
        void PaintSurface(SkiaManager skiaManager, SKCanvas canvas);
        
    }
}
