
using RemoteX.Input;
using RemoteX.Sketch.CoreModule;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RemoteX.Sketch
{
    /// <summary>
    /// Update all input into update thread
    /// </summary>
    public class SketchInputManager : SketchObject
    {
        public Matrix3x2 InputSpaceToSketchSpaceMatrix { get; set; }
        private object _SketchPointersReadyToEnterLock;
        private List<SketchPointer> _SketchPointersReadyToEnter;
        private object _SketchPointersReadyToExitLock;
        private List<SketchPointer> _SketchPointersReadyToExit;
        
        public List<SketchPointer> SketchPointersList;
        
        private IInputManager _InputManager;

        public SketchInputManager() : base()
        {
            InputSpaceToSketchSpaceMatrix = Matrix3x2.Identity;

            _SketchPointersReadyToEnterLock = new object();
            _SketchPointersReadyToExitLock = new object();
            _SketchPointersReadyToEnter = new List<SketchPointer>();
            _SketchPointersReadyToExit = new List<SketchPointer>();
            SketchPointersList = new List<SketchPointer>();

            _InputManager = null;

        }
        public IInputManager InputManager
        {
            get
            {
                return _InputManager;
            }
            set
            {
                if (_InputManager != null)
                {
                    InputManager.PointerEntered -= InputManager_PointerEntered;
                    InputManager.PointerExited -= InputManager_PointerExited;
                    InputManager.PointerPressed -= InputManager_PointerPressed;
                    

                }
                _InputManager = value;
                InputManager.PointerEntered += InputManager_PointerEntered;
                InputManager.PointerExited += InputManager_PointerExited;
                InputManager.PointerPressed += InputManager_PointerPressed;
                InputManager.PointerReleased += InputManager_PointerReleased;
            }
        }

        

        protected override void OnInstantiated()
        {
            base.OnInstantiated();
        }

        public void Init(IInputManager inputManager)
        {
            InputManager = inputManager;
        }

        public SketchPointer[] SketchPointers
        {
            get
            {
                return SketchPointersList.ToArray();
            }
        }

        protected override void Update()
        {
            base.Update();
            lock(_SketchPointersReadyToEnterLock)
            {
                SketchPointersList.AddRange(_SketchPointersReadyToEnter);
                _SketchPointersReadyToEnter.Clear();
            }

            lock(_SketchPointersReadyToExitLock)
            {
                foreach(var readyToExtiPointer in _SketchPointersReadyToExit)
                {
                    SketchPointersList.Remove(readyToExtiPointer);
                }
                _SketchPointersReadyToExit.Clear();
                
            }
        }

        private void InputManager_PointerReleased(object sender, IPointer e)
        {
            lock (_SketchPointersReadyToExitLock)
            {
                SketchPointer sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });

                if (sketchPointer != null)
                {
                    sketchPointer.HitLayer = -1;
                }
            }
        }

        private void InputManager_PointerPressed(object sender, IPointer e)
        {
            lock (_SketchPointersReadyToExitLock)
            {
                SketchPointer sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });

                if(sketchPointer != null)
                {
                    //throw new NotImplementedException("Do Remember implement Hit test and get layer");
                    foreach(var sketchObject in SketchEngine.SketchObjectList)
                    {
                        if(sketchObject is IInputComponent)
                        {
                            if((sketchObject as IInputComponent).StartRegion.IsOverlapPoint(sketchPointer.Point))
                            {
                                sketchPointer.HitLayer = (sketchObject as IInputComponent).Level;
                                break;
                            }
                        }
                    }
                    if(sketchPointer.HitLayer < 0)
                    {
                        sketchPointer.HitLayer = 0;
                    }
                }
            }
        }

        private void InputManager_PointerExited(object sender, IPointer e)
        {
            lock(_SketchPointersReadyToExitLock)
            {
                SketchPointer sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });
                if (sketchPointer != null)
                {
                    _SketchPointersReadyToExit.Add(sketchPointer);
                }
            }
            
        }
        private void InputManager_PointerEntered(object sender, IPointer e)
        {
            lock(_SketchPointersReadyToEnterLock)
            {
                SketchPointer sketchPointer = new SketchPointer(this, e);
                _SketchPointersReadyToEnter.Add(sketchPointer);
            }
        }

        
    }

    public class SketchPointer
    {
        public IPointer Pointer { get; private set; }

        /// <summary>
        /// -1 Not pressed
        /// 0: Pressed but hit nothing
        /// >0: 
        /// </summary>
        public int HitLayer { get; internal set; }
        
        public Vector2 Point
        {
            get
            {
                //return SketchInputManager.InputSpaceToSketchSpaceMatrix.MultiplyPoint(Pointer.PointerPoint);
                //return Pointer.PointerPoint.
                return Vector2.Transform(Pointer.PointerPoint, SketchInputManager.InputSpaceToSketchSpaceMatrix);
            }
        }

        public SketchInputManager SketchInputManager { get; }
        public PointerState State
        {
            get
            {
                return Pointer.LatestState;
            }
        }

        public SketchPointer(SketchInputManager sketchInputManager, IPointer pointer)
        {
            SketchInputManager = sketchInputManager;
            Pointer = pointer;
        }

        public override string ToString()
        {
            return "[" + Pointer.PointerDeviceType + "|" + Point + "|"+HitLayer+"]";
        }


    }
}
