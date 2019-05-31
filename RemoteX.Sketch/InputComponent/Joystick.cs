using RemoteX.Sketch.CoreModule;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch.InputComponent
{
    public abstract class Joystick : SketchObject, IInputComponent
    {
        public int Level { get; set; }
        public virtual IArea StartRegion { get; }
        protected SketchPointer OnSketchPointer { get; private set; }

        public Vector2 Delta { get; private set; }

        public Vector2 _StartPos;

        public RectTransform RectTransform { get; private set; }

        public bool Pressed { get; private set; }

        public Joystick() : base()
        {
            OnSketchPointer = null;
            //RectTransform = new RectTransform();
        }

        protected override void OnInstantiated()
        {
            base.OnInstantiated();
            var sketchInfo = SketchEngine.FindObjectByType<SketchInfo>();
            RectTransform = new RectTransform(sketchInfo);
            
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            
            SketchInputManager sketchInputManager = SketchEngine.FindObjectByType<SketchInputManager>();
            if(OnSketchPointer == null)
            {
                foreach (var sketchPointer in sketchInputManager.SketchPointers)
                {
                    if (StartRegion.IsOverlapPoint(sketchPointer.Point) && sketchPointer.HitLayer == Level)
                    {
                        if(sketchPointer.State == Input.PointerState.Pressed)
                        {
                            OnSketchPointer = sketchPointer;
                            _StartPos = OnSketchPointer.Point;
                            Pressed = true;
                            OnJoystickPressed();
                            
                        }
                    }
                }
            }
            else
            {
                if (OnSketchPointer.State == Input.PointerState.Pressed)
                {
                    Delta = OnSketchPointer.Point - _StartPos;
                    
                }
                else
                {
                    Pressed = false;
                    OnJoystickUp();
                    OnSketchPointer = null;
                }
            }
            
        }

        protected abstract void OnJoystickPressed();

        protected abstract void OnJoystickUp();
    }
}
