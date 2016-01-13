using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CanvasMap
{
    public abstract class MapBase : Canvas
    {
        private bool _mouseCaptured;
        private Point _previousMouse;
        protected byte currentZoom = 2;

        protected abstract void OnZoom(Point mouse, byte currentZoom, byte newZoom);

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            int newzoom;
            if (e.Delta < 0)
            {
                newzoom = currentZoom + 1;
            }
            else
            {
                newzoom = currentZoom - 1;
            }
            if (newzoom < 0 | newzoom > 18)
            {
            }
            else
            {
                OnZoom(e.GetPosition(this), currentZoom, (byte)newzoom);
                currentZoom = (byte)newzoom;
            }
        }

        protected abstract void OnDragMap(Vector v);

        /// <summary>Tries to capture the mouse to enable dragging of the map.</summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Debug.Print("mdown");
            base.OnMouseLeftButtonDown(e);
            Focus(); // Make sure we get the keyboard
            if (CaptureMouse())
            {
                _mouseCaptured = true;
                _previousMouse = e.GetPosition(null);
            }
        }

        /// <summary>Releases the mouse capture and stops dragging of the map.</summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Debug.Print("mup");
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            _mouseCaptured = false;
        }

        /// <summary>Drags the map, if the mouse was succesfully captured.</summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_mouseCaptured)
            {
                Point position = e.GetPosition(null);
                var vector = position - _previousMouse;
                OnDragMap(vector);
                _previousMouse = position;
            }
        }
    }
}