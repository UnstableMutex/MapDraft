using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RectanglesZoom2
{
    class Map:MapBase
    {
        public Map()
        {
            var tile = new TileGroup(0, new TilePosition() { X = 0, Y = 0 },this);
            Children.Add(tile);
        
            tile.MoveTopLeftToPoint(new Point(0, 0));
         

        }

        private int currentZoom = 0;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {

            var newzoom = 0;
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
                currentZoom = newzoom;
            }

            Zoom(e.GetPosition(this), currentZoom);

        }

        public void Zoom(Point mouse,int zoom)
        {
          var tiles = Children.OfType<TileGroup>();
            foreach (var tile in tiles)
            {
               // mouse = Mouse.GetPosition(tile);

                tile.Zoom(mouse,zoom);

            }  
        }


        protected override void OnDragMap(Vector v)
        {
            var tiles = Children.OfType<TileGroup>();
            foreach (var tile in tiles)
            {
                tile.Move(v);
            }  

        }
    }
}
