using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sketch.CoreModule
{
    public abstract class SketchObject
    {
        public SketchEngine SketchEngine { get; internal set; }

        public string Name { get; set; }

        /// <summary>
        /// Will be set to true when the object is completely instantiated
        /// </summary>
        public bool IsInstantiated { get; internal set; }

        /// <summary>
        /// Will be set to true when the object is marked to be destoried
        /// </summary>
        public bool IsDestroyed { get; internal set; }

        public SketchObject()
        {
        }

        protected virtual void OnInstantiated()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void Start()
        {

        }
        
    }
}
