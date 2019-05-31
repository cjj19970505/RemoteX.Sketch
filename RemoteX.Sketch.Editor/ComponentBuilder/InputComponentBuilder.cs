using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RemoteX.Sketch.Editor.ComponentBuilder
{
    public class InputComponentBuilder: SketchObject, ISkiaRenderer
    {
        public Page InspectorPage { get; set; }
        public int Level { get; set; }

        public virtual IArea SelecteArea { get;}

        public InputComponentBuilder():base()
        {
            
        }
        protected override void Start()
        {
            base.Start();
        }

        void ISkiaRenderer.PaintSurface(SkiaManager skiaManager, SKCanvas canvas)
        {

        }
    }

    public class ExampleInputComponentBuilder : InputComponentBuilder
    {
        
    }
}
