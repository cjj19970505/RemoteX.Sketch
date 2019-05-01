using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteX.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;

namespace RemoteX.Input.Win10
{
    public class InputManager : IInputManager
    {
        public UIElement PointerDetectUIElement { get; private set; }

        /// <summary>
        /// Key: PointerId
        /// 
        /// </summary>
        public Dictionary<uint, IPointer> PointerDict;

        public event EventHandler<IPointer> PointerEntered;
        public event EventHandler<IPointer> PointerPressed;
        public event EventHandler<IPointer> PointerMoved;
        public event EventHandler<IPointer> PointerReleased;
        public event EventHandler<IPointer> PointerExited;

        public InputManager(UIElement pointerDetectUIElement)
        {
            PointerDict = new Dictionary<uint, IPointer>();
            PointerDetectUIElement = pointerDetectUIElement;
            pointerDetectUIElement.PointerCanceled += PointerDetectUIElement_PointerCanceled;
            pointerDetectUIElement.PointerCaptureLost += PointerDetectUIElement_PointerCaptureLost;
            pointerDetectUIElement.PointerEntered += PointerDetectUIElement_PointerEntered;
            pointerDetectUIElement.PointerExited += PointerDetectUIElement_PointerExited;
            pointerDetectUIElement.PointerMoved += PointerDetectUIElement_PointerMoved;
            pointerDetectUIElement.PointerPressed += PointerDetectUIElement_PointerPressed;
            pointerDetectUIElement.PointerReleased += PointerDetectUIElement_PointerReleased;
            pointerDetectUIElement.PointerWheelChanged += PointerDetectUIElement_PointerWheelChanged;
        }

        private void PointerDetectUIElement_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void PointerDetectUIElement_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint win10PointerPoint = e.GetCurrentPoint(PointerDetectUIElement);
            if(PointerDict.ContainsKey(win10PointerPoint.PointerId))
            {
                RXPointer pointer = PointerDict[win10PointerPoint.PointerId] as RXPointer;
                pointer.UpdatePointer(win10PointerPoint, PointerState.Released);
                PointerReleased?.Invoke(this, pointer);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void PointerDetectUIElement_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint win10PointerPoint = e.GetCurrentPoint(PointerDetectUIElement);
            if (PointerDict.ContainsKey(win10PointerPoint.PointerId))
            {
                RXPointer pointer = PointerDict[win10PointerPoint.PointerId] as RXPointer;
                pointer.UpdatePointer(win10PointerPoint, PointerState.Pressed);
                PointerPressed?.Invoke(this, pointer);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void PointerDetectUIElement_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }

        private void PointerDetectUIElement_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint win10PointerPoint = e.GetCurrentPoint(PointerDetectUIElement);
            if(PointerDict.ContainsKey(win10PointerPoint.PointerId))
            {
                RXPointer pointer = PointerDict[win10PointerPoint.PointerId] as RXPointer;
                pointer.UpdatePointer(win10PointerPoint, PointerState.Exited);
                PointerDict.Remove(win10PointerPoint.PointerId);
                PointerExited?.Invoke(this, pointer);
            }
        }

        private void PointerDetectUIElement_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint win10PointerPoint = e.GetCurrentPoint(PointerDetectUIElement);

            if(PointerDict.ContainsKey(win10PointerPoint.PointerId))
            {
                throw new NotImplementedException();
            }
            else
            {
                RXPointer pointer = new RXPointer(this);
                pointer.UpdatePointer(win10PointerPoint, PointerState.Entered);
                PointerDict.Add(pointer.Win10PointerPoint.PointerId, pointer);
                PointerEntered?.Invoke(this, pointer);
            }
        }

        private void PointerDetectUIElement_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint win10PointerPoint = e.GetCurrentPoint(PointerDetectUIElement);
            RXPointer pointer = new RXPointer(this);
            pointer.Win10PointerPoint = win10PointerPoint;
            System.Diagnostics.Debug.WriteLine("CaptureLost:" + pointer);
        }

        private void PointerDetectUIElement_PointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerPoint win10PointerPoint = e.GetCurrentPoint(PointerDetectUIElement);
            RXPointer pointer = new RXPointer(this);
            pointer.Win10PointerPoint = win10PointerPoint;
            System.Diagnostics.Debug.WriteLine("Canceled:" + pointer);
        }

        


    }
}
