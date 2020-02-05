using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RemoteX.Sketch.CoreModule
{
    public class SketchEngine
    {
        public Time Time { get; }
        /// <summary>
        /// 但进入Update时这个会被锁住，各位若要在update之外访问Sketch内的资源时请使用这
        /// </summary>
        public object UpdateLock { get; private set; }

        private readonly List<SketchObject> _SketchObjectList;
        private readonly List<SketchObject> _ReadyToInstantiateSketchObjectList;
        private SemaphoreSlim _UpdateSemaphore;

        public readonly object _ReadyToUpdateObjectListLock;
        public readonly List<SketchObject> _ReadyToUpdateObjectList;

        public IReadOnlyList<SketchObject> SketchObjectList
        {
            get
            {
                return _SketchObjectList;
            }
        }

        public T FindObjectByType<T>() where T:SketchObject
        {
            foreach(var sketchObject in _SketchObjectList)
            {
                if(sketchObject is T)
                {
                    return sketchObject as T;
                }
            }
            return null;
        }

        public T[] FindObjectsByType<T>() where T:SketchObject
        {
            List<T> sketchObjectsOfType = new List<T>();
            foreach(var sketchObject in _SketchObjectList)
            {
                if(sketchObject is T)
                {
                    sketchObjectsOfType.Add(sketchObject as T);
                }
            }
            return sketchObjectsOfType.ToArray();
        }

        public SketchEngine()
        {
            _SketchObjectList = new List<SketchObject>();
            _ReadyToInstantiateSketchObjectList = new List<SketchObject>();
            Time = new Time();
            UpdateLock = new object();
            _UpdateSemaphore = new SemaphoreSlim(0, 1);
            _ReadyToUpdateObjectListLock = new object();
            _ReadyToUpdateObjectList = new List<SketchObject>();

        }

        /// <summary>
        /// 这个函数是线程安全的，只要一个物体注册了这个就可以被Update，并且激活整个引擎的Update
        /// </summary>
        /// <param name="sketchObject"></param>
        public void RegisterForNextUpdate(SketchObject sketchObject)
        {
            lock (_ReadyToUpdateObjectListLock)
            {
                if (!_ReadyToUpdateObjectList.Contains(sketchObject))
                {
                    _ReadyToUpdateObjectList.Add(sketchObject);
                }
            }
            try
            {
                _UpdateSemaphore.Release();
            }
            catch (SemaphoreFullException)
            {

            }
        }

        private void RegisterForNextUpdate()
        {
            try
            {
                _UpdateSemaphore.Release();
            }
            catch (SemaphoreFullException)
            {

            }
        }

        /// <summary>
        /// Object will be created, but was added to the scene in next frame
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>() where T : SketchObject, new()
        {
            T skiaObject = new T();
            skiaObject.SketchEngine = this;
            _ReadyToInstantiateSketchObjectList.Add(skiaObject);
            skiaObject.OnInstantiated();
            RegisterForNextUpdate();
            return skiaObject;
        }


        public void Start()
        {
            List<SketchObject> readyToUpdateList = new List<SketchObject>();
            while (true)
            {
                _UpdateSemaphore.Wait();
                
                lock (_ReadyToUpdateObjectListLock)
                {
                    readyToUpdateList.AddRange(_ReadyToUpdateObjectList);
                    _ReadyToUpdateObjectList.Clear();
                }
                //Time.DeltaTime = deltaTime;
                foreach (var sketchObject in _ReadyToInstantiateSketchObjectList)
                {
                    _SketchObjectList.Add(sketchObject);
                    sketchObject.IsInstantiated = true;
                    
                    //sketchObject.OnInstantiated();
                }
                foreach(var sketchObject in _ReadyToInstantiateSketchObjectList)
                {
                    sketchObject.Start();
                }
                _ReadyToInstantiateSketchObjectList.Clear();
                foreach (var updateObject in readyToUpdateList)
                {
                    updateObject.Update();
                }
                readyToUpdateList.Clear();
            }
        }

        /// <summary>
        /// SkiaObject will be completed destroy in next update
        /// </summary>
        /// <param name="sketchObject"></param>
        public void Destroy(SketchObject sketchObject)
        {
            throw new NotImplementedException();
        }

        public void Update(float deltaTime)
        {
            lock (UpdateLock)
            {
                Time.DeltaTime = deltaTime;
                foreach (var sketchObject in _ReadyToInstantiateSketchObjectList)
                {
                    _SketchObjectList.Add(sketchObject);
                    sketchObject.IsInstantiated = true;
                    //sketchObject.OnInstantiated();
                }
                _ReadyToInstantiateSketchObjectList.Clear();
                foreach (var sketchObject in _SketchObjectList)
                {
                    sketchObject.Update();
                }
            }
            
            
        }

        
    }
}
