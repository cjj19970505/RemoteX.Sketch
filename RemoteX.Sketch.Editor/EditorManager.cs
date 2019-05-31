using RemoteX.Sketch.CoreModule;
using RemoteX.Sketch.Editor.ComponentBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Sketch.Editor
{
    class EditorManager:SketchObject
    {
        public List<InputComponentBuilder> _CurrSelectedBuilderList;
        public InputComponentBuilder[] CurrSelectedBuilders
        {
            get
            {
                return _CurrSelectedBuilderList.ToArray();
            }
        }
        public EditorManager()
        {
            _CurrSelectedBuilderList = new List<InputComponentBuilder>();
        }

        
    }
}
