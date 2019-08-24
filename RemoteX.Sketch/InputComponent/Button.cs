using RemoteX.Sketch.CoreModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public abstract class Button : SketchObject, IInputComponent, IRectTransformable
    {
        public int Level { get; set; }

        public RectTransform RectTransform { get; private set; }
        public virtual IArea StartRegion { get; }
        public SketchInputManager SketchInputManager { get; private set; }

        readonly List<SketchPointer> OnSketchPointerList;

        public bool Pressed { get; private set; }
        public Button():base()
        {
            Pressed = false;
            OnSketchPointerList = new List<SketchPointer>();
        }

        protected override void OnInstantiated()
        {
            base.OnInstantiated();
            var sketchInfo = SketchEngine.FindObjectByType<SketchInfo>();
            RectTransform = new RectTransform(sketchInfo);
            SketchInputManager = SketchEngine.FindObjectByType<SketchInputManager>();
            SketchInputManager.PointerMoved += SketchInputManager_PointerMoved;
            SketchInputManager.PointerReleased += SketchInputManager_PointerReleased;
            SketchInputManager.PointerPressed += SketchInputManager_PointerPressed;
        }

        private void SketchInputManager_PointerPressed(object sender, SketchPointer e)
        {
            if (e.HitLayer == Level && StartRegion.IsOverlapPoint(e.Point))
            {
                OnSketchPointerList.Add(e);
                if (OnSketchPointerList.Count == 1)
                {
                    Pressed = true;
                    OnPressed();
                }
            }
        }

        private void SketchInputManager_PointerReleased(object sender, SketchPointer e)
        {
            if(OnSketchPointerList.Contains(e))
            {
                OnSketchPointerList.Remove(e);
                if (OnSketchPointerList.Count == 0)
                {
                    Pressed = false;
                    OnReleased();
                }
            }
        }
        private void SketchInputManager_PointerMoved(object sender, SketchPointer e)
        {
            if(OnSketchPointerList.Contains(e))
            {
                if(!StartRegion.IsOverlapPoint(e.Point))
                {
                    OnSketchPointerList.Remove(e);
                    if(OnSketchPointerList.Count == 0)
                    {
                        Pressed = false;
                        OnReleased();
                    }
                }
            }
            else
            {
                if (e.HitLayer == Level && StartRegion.IsOverlapPoint(e.Point))
                {
                    OnSketchPointerList.Add(e);
                    if(OnSketchPointerList.Count == 1)
                    {
                        Pressed = true;
                        OnPressed();
                    }
                }
            }
        }

        public virtual void OnReleased()
        {

        }

        public virtual void OnPressed()
        {

        }
    }
}
