using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.CoreModule
{
    public class SketchEngine
    {
        private readonly List<SketchObject> _SketchObjectList;
        private readonly List<SketchObject> _ReadyToInstantiateSketchObjectList;


        public SketchEngine()
        {
            _SketchObjectList = new List<SketchObject>();

        }
        /// <summary>
        /// Object will be created, but was added to the scene in next frame
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Instantiate<T>() where T : SketchObject, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SkiaObject will be completed destroy in next update
        /// </summary>
        /// <param name="sketchObject"></param>
        public void Destroy(SketchObject sketchObject)
        {
            throw new NotImplementedException();
        }
        
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
