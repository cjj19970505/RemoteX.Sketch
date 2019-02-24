using RemoteX.Sketch.CoreModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch
{
    public class SketchInputManager : SketchObject
    {
        public Input.IInputManager RXInputManager { get; private set; }
        

        public void Init(RemoteX.Input.IInputManager rxInputManager)
        {
            RXInputManager = rxInputManager;
        }

        protected override void Start()
        {
            RXInputManager.OnTouchAction += RXInputManager_OnTouchAction;
        }

        private void RXInputManager_OnTouchAction(Input.ITouch touch, Input.TouchMotionAction action)
        {
            throw new NotImplementedException();
        }

        class SketchTouch
        {
            public Input.ITouch Touch { get; private set; }
            public SketchInputManager SketchInputManager { get; }

            public SketchTouch(SketchInputManager sketchInputManager, Input.ITouch touch)
            {
                SketchInputManager = sketchInputManager;
                Touch = touch;
            }

        }
    }
}
