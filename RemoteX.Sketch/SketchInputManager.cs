
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
        public event EventHandler<SketchPointer> PointerEntered;
        public event EventHandler<SketchPointer> PointerPressed;
        public event EventHandler<SketchPointer> PointerMoved;
        public event EventHandler<SketchPointer> PointerReleased;
        public event EventHandler<SketchPointer> PointerExited;
        public Matrix3x2 InputSpaceToSketchSpaceMatrix { get; set; }
        private object _SketchPointersReadyToEnterLock;
        private List<SketchPointer> _SketchPointersReadyToEnter;
        private object _SketchPointersReadyToExitLock;
        private List<SketchPointer> _SketchPointersReadyToExit;
        private object _SketchPointersListLock;
        public List<SketchPointer> SketchPointersList;

        private IInputManager _InputManager;
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
                InputManager.PointerMoved += InputManager_PointerMoved;
                InputManager.PointerPressed += InputManager_PointerPressed;
                InputManager.PointerReleased += InputManager_PointerReleased;
            }
        }



        private object _PointerInfoCachesLock;
        private Queue<PointerInfoCache> _PointerInfoCaches;

        public SketchInputManager() : base()
        {
            InputSpaceToSketchSpaceMatrix = Matrix3x2.Identity;

            _SketchPointersReadyToEnterLock = new object();
            _SketchPointersReadyToExitLock = new object();
            _SketchPointersReadyToEnter = new List<SketchPointer>();
            _SketchPointersReadyToExit = new List<SketchPointer>();
            _SketchPointersListLock = new object();
            SketchPointersList = new List<SketchPointer>();
            _PointerInfoCachesLock = new object();
            _PointerInfoCaches = new Queue<PointerInfoCache>();
            _InputManager = null;

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
            lock (_SketchPointersReadyToEnterLock)
            {
                SketchPointersList.AddRange(_SketchPointersReadyToEnter);
                _SketchPointersReadyToEnter.Clear();
            }
            Queue<PointerInfoCache> cacheQueue;
            lock (_PointerInfoCachesLock)
            {
                cacheQueue = new Queue<PointerInfoCache>(_PointerInfoCaches);
                _PointerInfoCaches.Clear();
            }
            while (cacheQueue.Count != 0)
            {
                PointerInfoCache cache = cacheQueue.Dequeue();
                if(cache.CacheEvent == PointerInfoCacheEvent.Moved)
                {
                    cache.HitLayer = cache.SketchPointer.LatestPointerInfoCache.HitLayer;
                }
                cache.SketchPointer.LatestPointerInfoCache = cache;
                if(cache.CacheEvent == PointerInfoCacheEvent.Pressed)
                {
                    foreach (var sketchObject in SketchEngine.SketchObjectList)
                    {
                        if (sketchObject is IInputComponent)
                        {
                            if ((sketchObject as IInputComponent).StartRegion.IsOverlapPoint(cache.SketchPointer.Point))
                            {
                                cache.HitLayer = (sketchObject as IInputComponent).Level;
                                break;
                            }
                        }
                    }
                    cache.SketchPointer.LatestPointerInfoCache = cache;
                }
                switch (cache.CacheEvent)
                {
                    case PointerInfoCacheEvent.Entered:
                        PointerEntered?.Invoke(this, cache.SketchPointer);
                        break;
                    case PointerInfoCacheEvent.Pressed:
                        PointerPressed?.Invoke(this, cache.SketchPointer);
                        break;
                    case PointerInfoCacheEvent.Moved:
                        PointerMoved?.Invoke(this, cache.SketchPointer);
                        break;
                    case PointerInfoCacheEvent.Released:
                        PointerReleased?.Invoke(this, cache.SketchPointer);
                        break;
                    case PointerInfoCacheEvent.Exited:
                        PointerExited?.Invoke(this, cache.SketchPointer);
                        break;
                }
            }

            lock (_SketchPointersReadyToExitLock)
            {
                foreach (var readyToExtiPointer in _SketchPointersReadyToExit)
                {
                    SketchPointersList.Remove(readyToExtiPointer);
                }
                _SketchPointersReadyToExit.Clear();

            }
        }

        private void InputManager_PointerReleased(object sender, IPointer e)
        {
            SketchPointer sketchPointer = null;
            lock (_SketchPointersListLock)
            {
                sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });
            }
            if (sketchPointer == null)
            {
                lock (_SketchPointersReadyToEnterLock)
                {
                    sketchPointer = _SketchPointersReadyToEnter.Find((obj) =>
                    {
                        if (obj.Pointer == e)
                        {
                            return true;
                        }
                        return false;
                    });
                }
            }
            if (sketchPointer != null)
            {
                var cache = new PointerInfoCache(sketchPointer, -1, PointerInfoCacheEvent.Released);
                lock (_PointerInfoCachesLock)
                {
                    _PointerInfoCaches.Enqueue(cache);
                }
            }
            SketchEngine.RegisterForNextUpdate(this);
        }

        private void InputManager_PointerPressed(object sender, IPointer e)
        {
            SketchPointer sketchPointer = null;
            lock (_SketchPointersListLock)
            {
                sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });
            }
            if (sketchPointer == null)
            {
                lock (_SketchPointersReadyToEnterLock)
                {
                    sketchPointer = _SketchPointersReadyToEnter.Find((obj) =>
                    {
                        if (obj.Pointer == e)
                        {
                            return true;
                        }
                        return false;
                    });
                }
            }

            if (sketchPointer != null)
            {
                var cache = new PointerInfoCache(sketchPointer, 0, PointerInfoCacheEvent.Pressed);
                //throw new NotImplementedException("Do Remember implement Hit test and get layer");
                

                lock (_PointerInfoCachesLock)
                {
                    _PointerInfoCaches.Enqueue(cache);
                }
            }
            SketchEngine.RegisterForNextUpdate(this);
        }

        private void InputManager_PointerMoved(object sender, IPointer e)
        {
            SketchPointer sketchPointer = null;
            lock (_SketchPointersListLock)
            {
                sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });
            }
            if (sketchPointer == null)
            {
                lock (_SketchPointersReadyToEnterLock)
                {
                    sketchPointer = _SketchPointersReadyToEnter.Find((obj) =>
                    {
                        if (obj.Pointer == e)
                        {
                            return true;
                        }
                        return false;
                    });
                }
            }

            if (sketchPointer != null)
            {
                var cache = new PointerInfoCache(sketchPointer, sketchPointer.LatestPointerInfoCache.HitLayer, PointerInfoCacheEvent.Moved);
                lock (_PointerInfoCachesLock)
                {
                    _PointerInfoCaches.Enqueue(cache);
                }
            }
            SketchEngine.RegisterForNextUpdate(this);
        }

        private void InputManager_PointerExited(object sender, IPointer e)
        {
            SketchPointer sketchPointer = null;
            lock (_SketchPointersListLock)
            {
                sketchPointer = SketchPointersList.Find((obj) =>
                {
                    if (obj.Pointer == e)
                    {
                        return true;
                    }
                    return false;
                });
            }
            if (sketchPointer == null)
            {
                lock (_SketchPointersReadyToEnterLock)
                {
                    sketchPointer = _SketchPointersReadyToEnter.Find((obj) =>
                    {
                        if (obj.Pointer == e)
                        {
                            return true;
                        }
                        return false;
                    });
                }
            }
            if (sketchPointer != null)
            {
                lock (_SketchPointersReadyToExitLock)
                {
                    _SketchPointersReadyToExit.Add(sketchPointer);
                }
                var cache = new PointerInfoCache(sketchPointer, sketchPointer.LatestPointerInfoCache.HitLayer, PointerInfoCacheEvent.Exited);
                lock (_PointerInfoCachesLock)
                {
                    _PointerInfoCaches.Enqueue(cache);
                }

            }
            SketchEngine.RegisterForNextUpdate(this);

        }
        private void InputManager_PointerEntered(object sender, IPointer e)
        {
            SketchPointer sketchPointer = new SketchPointer(this, e);
            lock (_SketchPointersReadyToEnterLock)
            {
                _SketchPointersReadyToEnter.Add(sketchPointer);
            }
            PointerInfoCache cache = new PointerInfoCache(sketchPointer, -1, PointerInfoCacheEvent.Entered);
            lock (_PointerInfoCachesLock)
            {
                _PointerInfoCaches.Enqueue(cache);
            }
            SketchEngine.RegisterForNextUpdate(this);
        }


    }

    internal enum PointerInfoCacheEvent { Entered, Pressed, Moved, Released, Exited }
    /// <summary>
    /// Use to cache a PointerInfo when in an event. later redispatch these event in update thread
    /// </summary>
    internal struct PointerInfoCache
    {
        public PointerInfoCacheEvent CacheEvent { get; set; }
        public SketchPointer SketchPointer { get; set; }
        public PointerState State { get; set; }
        public Vector2 PointerPoint { get; set; }
        public int HitLayer { get; set; }

        public PointerInfoCache(SketchPointer sketchPointer, int hitLayer, PointerInfoCacheEvent cacheEvent)
        {
            SketchPointer = sketchPointer;
            CacheEvent = cacheEvent;
            State = sketchPointer.Pointer.LatestState;
            PointerPoint = sketchPointer.Pointer.PointerPoint;
            HitLayer = hitLayer;
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
        public int HitLayer
        {
            get
            {
                return LatestPointerInfoCache.HitLayer;
            }
        }
        internal PointerInfoCache LatestPointerInfoCache { get; set; }
        public Vector2 Point
        {
            get
            {
                //return SketchInputManager.InputSpaceToSketchSpaceMatrix.MultiplyPoint(Pointer.PointerPoint);
                //return Pointer.PointerPoint.
                return Vector2.Transform(LatestPointerInfoCache.PointerPoint, SketchInputManager.InputSpaceToSketchSpaceMatrix);
            }
        }

        public SketchInputManager SketchInputManager { get; }
        public PointerState State
        {
            get
            {
                //return Pointer.LatestState;
                return LatestPointerInfoCache.State;
            }
        }

        public SketchPointer(SketchInputManager sketchInputManager, IPointer pointer)
        {
            SketchInputManager = sketchInputManager;
            Pointer = pointer;
        }



        public override string ToString()
        {
            return "[" + Pointer.PointerDeviceType + "|" + Point + "|" + State + "|" + HitLayer + "]";
        }


    }
}
