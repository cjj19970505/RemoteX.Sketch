using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Numerics;
using Android.Util;

namespace RemoteX.Input.Droid
{
    public class InputManager : IInputManager
    {
        private const string TAG = "InputManager";
        public event EventHandler<IPointer> PointerEntered;
        public event EventHandler<IPointer> PointerPressed;
        public event EventHandler<IPointer> PointerMoved;
        public event EventHandler<IPointer> PointerReleased;
        public event EventHandler<IPointer> PointerExited;

        public Dictionary<int, Pointer> PointerDict;

        public InputManager()
        {
            PointerDict = new Dictionary<int, Pointer>();
        }

        public bool OnTouch(MotionEvent e)
        {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIndexMask)) >> ((int)(MotionEventActions.PointerIndexShift));
            int pointerId = e.GetPointerId(pointerIndex);
            int down = (int)(e.ActionMasked & MotionEventActions.PointerDown);
            int up = (int)(e.ActionMasked & MotionEventActions.PointerUp);
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        /*
                        Touch touch = new Touch(pointerId);
                        touch.Position = new Vector2(e.GetX(pointerIndex), e.GetY(pointerIndex));
                        _Touches.Add(pointerId, touch);
                        OnTouchAction?.Invoke(touch, TouchMotionAction.Down);
                        */
                        if(PointerDict.ContainsKey(pointerId))
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            Pointer pointer = new Pointer(this, pointerId);
                            Vector2 pos = new Vector2(e.GetX(pointerIndex), e.GetY(pointerIndex));
                            pointer.UpdatePointer(pos, PointerState.Entered);
                            PointerDict.Add(pointerId, pointer);
                            PointerEntered?.Invoke(this, pointer);
                            pointer.UpdatePointer(pos, PointerState.Pressed);
                            PointerPressed?.Invoke(this, pointer);
                            //Log.Info(TAG, "DOWN:" + pointer.ToString());
                        }
                        
                        break;

                    }
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    {
                        //Touch touch = _Touches[pointerId];
                        //_Touches.Remove(pointerId);
                        //OnTouchAction?.Invoke(touch, TouchMotionAction.Up);
                        if(PointerDict.ContainsKey(pointerId))
                        {
                            Pointer pointer = PointerDict[pointerId];
                            Vector2 pos = new Vector2(e.GetX(pointerIndex), e.GetY(pointerIndex));
                            pointer.UpdatePointer(PointerState.Released);
                            PointerReleased?.Invoke(this, pointer);
                            pointer.UpdatePointer(PointerState.Exited);
                            PointerDict.Remove(pointerId);
                            PointerExited?.Invoke(this, pointer);
                            //Log.Info(TAG, "UP:" + pointer.ToString());
                        }
                        else
                        {
                            //throw new NotImplementedException();
                        }
                        

                        break;
                    }
                case MotionEventActions.Move:
                    {
                        foreach (var pair in PointerDict)
                        {
                            int pIndex = e.FindPointerIndex(pair.Key);
                            if(pIndex < 0)
                            {
                                continue;
                            }
                            Vector2 currentPos = new Vector2(e.GetX(pIndex), e.GetY(pIndex));
                            if (pair.Value.PointerPoint != currentPos)
                            {
                                //pair.Value.PointerPoint = currentPos;
                                pair.Value.UpdatePointer(currentPos);
                                PointerMoved?.Invoke(this, pair.Value);
                                //Log.Info(TAG, "MOVE:" + pair.Value.ToString());
                            }
                        }
                        break;
                    }
            }
            return true;

        }
    }

    public class Pointer : IPointer
    {
        public IInputManager InputManager { get; private set; }

        public PointerDeviceType PointerDeviceType
        {
            get
            {
                return PointerDeviceType.Touch;
            }
        }

        public PointerState LatestState { get; private set; }
        public Vector2 PointerPoint { get; private set; }

        public int PointerId { get; private set; }

        public Pointer(InputManager inputManager, int pointerId)
        {
            InputManager = inputManager;
            PointerId = pointerId;
        }

        public void UpdatePointer(Vector2 position, PointerState latestState)
        {
            LatestState = latestState;
            PointerPoint = position;
        }

        public void UpdatePointer(Vector2 position)
        {
            PointerPoint = position;
        }

        public void UpdatePointer(PointerState latestState)
        {
            LatestState = latestState;
        }

        public override string ToString()
        {
            return PointerId + " " + PointerPoint;
        }

    }
}