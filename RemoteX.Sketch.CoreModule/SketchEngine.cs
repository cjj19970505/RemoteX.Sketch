using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.CoreModule
{
    public class SketchEngine
    {
        public Time Time { get; }

        private readonly List<SketchObject> _SketchObjectList;
        private readonly List<SketchObject> _ReadyToInstantiateSketchObjectList;

        public IReadOnlyList<SketchObject> SketchObjectList
        {
            get
            {
                return _SketchObjectList;
            }
        }

        public SketchEngine()
        {
            _SketchObjectList = new List<SketchObject>();
            _ReadyToInstantiateSketchObjectList = new List<SketchObject>();
            Time = new Time();

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
            
            return skiaObject;
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
            Time.DeltaTime = deltaTime;
            foreach (var sketchObject in _ReadyToInstantiateSketchObjectList)
            {
                _SketchObjectList.Add(sketchObject);
                sketchObject.IsInstantiated = true;
                sketchObject.OnInstantiated();
            }
            _ReadyToInstantiateSketchObjectList.Clear();
            foreach (var sketchObject in _SketchObjectList)
            {
                sketchObject.Update();
            }
            
        }

        
    }
}
