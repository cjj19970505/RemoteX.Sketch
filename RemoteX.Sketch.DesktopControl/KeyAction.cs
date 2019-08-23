using RemoteX.Sketch.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using WindowsInput;

namespace RemoteX.Sketch.DesktopControl
{
    public class KeyActionManager
    {
        public InputSimulator InputSimulator;
        Dictionary<VirtualKeyCode, KeyDownAction> KeyDownActionDict;
        public Dispatcher Dispatcher { get; }
        public KeyActionManager(Dispatcher dispatcher, InputSimulator inputSimulator)
        {
            Dispatcher = dispatcher;
            InputSimulator = inputSimulator;
            KeyDownActionDict = new Dictionary<VirtualKeyCode, KeyDownAction>();
        }
        public void KeyDown(VirtualKeyCode keyCode)
        {
            if(!KeyDownActionDict.ContainsKey(keyCode))
            {
                var action = new KeyDownAction(this, keyCode);
                action.Down();
                KeyDownActionDict.Add(keyCode, action);
            }
        }

        public void KeyUp(VirtualKeyCode keyCode)
        {
            if (!KeyDownActionDict.ContainsKey(keyCode))
            {
                InputSimulator.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)keyCode);
                
            }
            else
            {
                var keyDownAction = KeyDownActionDict[keyCode];
                KeyDownActionDict.Remove(keyCode);
                keyDownAction.Up();
                
            }
        }

        class KeyDownAction
        {
            DispatcherTimer Timer { get; }
            public TimeSpan InitTimeSpan
            {
                get
                {
                    return TimeSpan.FromSeconds(0.3);
                }
            }
            public TimeSpan RoutineTimeSpan
            {
                get
                {
                    return TimeSpan.FromSeconds(0.1);
                }
            }
            public KeyActionManager KeyActionManager { get; }

            public VirtualKeyCode KeyCode;
            public KeyDownAction(KeyActionManager keyActionManager, VirtualKeyCode keyCode)
            {
                KeyActionManager = keyActionManager;
                KeyCode = keyCode;
                Timer = new DispatcherTimer(DispatcherPriority.Normal, KeyActionManager.Dispatcher);
                Timer.Tick += Timer_Tick;
            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                Timer.Interval = RoutineTimeSpan;
                KeyActionManager.InputSimulator.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode)KeyCode);
            }

            public void Down()
            {
                KeyActionManager.InputSimulator.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode)KeyCode);
                Timer.Interval = InitTimeSpan;
                Timer.Start();
            }

            public void Up()
            {
                KeyActionManager.Dispatcher.Invoke(() =>
                {
                    Timer.Stop();
                    KeyActionManager.InputSimulator.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)KeyCode);
                });
            }

        }

        
    }
}
