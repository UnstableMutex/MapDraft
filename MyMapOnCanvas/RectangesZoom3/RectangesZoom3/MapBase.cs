using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RectangesZoom3
{
  abstract  class MapBase : Canvas
    {
        protected byte currentZoom=2;
       

        protected abstract void OnZoom(Point mouse,  byte currentZoom, byte newZoom);
       

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {

            int newzoom = 0;
            if (e.Delta < 0)
            {
                newzoom = currentZoom + 1;
            }
            else
            {
                newzoom = currentZoom - 1;
            }
            if (newzoom < 0 | newzoom > 15)
            {
                return;

            }
            else
            { 
                OnZoom(e.GetPosition(this), currentZoom,(byte)newzoom);
                currentZoom = (byte)newzoom;
            }

         

        }

        private bool _mouseCaptured;
        private Point _previousMouse;

      protected abstract void OnDragMap(Vector v);
      
        /// <summary>Tries to capture the mouse to enable dragging of the map.</summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.Focus(); // Make sure we get the keyboard
            if (this.CaptureMouse())
            {
                _mouseCaptured = true;
                _previousMouse = e.GetPosition(null);
            }
        }

        /// <summary>Releases the mouse capture and stops dragging of the map.</summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.ReleaseMouseCapture();
            _mouseCaptured = false;
        }
        /// <summary>Drags the map, if the mouse was succesfully captured.</summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_mouseCaptured)
            {
                //this.BeginUpdate();
                Point position = e.GetPosition(null);
                var vector = position - _previousMouse;
                OnDragMap(vector);
                _previousMouse = position;
                //_offsetX.Translate(position.X - _previousMouse.X);
                //_offsetY.Translate(position.Y - _previousMouse.Y);
                //_previousMouse = position;
                //this.EndUpdate();
            }
        }

    }
}