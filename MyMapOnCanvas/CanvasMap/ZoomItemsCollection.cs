using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CanvasMap
{
    internal class ZoomItemsCollection : List<OverlayBase>, IZoomItems
    {
        private byte _initialZoom;

        public ZoomItemsCollection(byte initialZoom)
        {
            _initialZoom = initialZoom;
            //  viewPort = new Rect(0, 0, Constants.TileSize * multi, Constants.TileSize * multi);
        }

        public byte Zoom
        {
            get { return _initialZoom; }
            set { _initialZoom = value; }
        }

        public void Click(Point mouse)
        {
            foreach (var item in this)
            {
                item.Click(mouse);
            }
        }

        public void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse)
        {
            foreach (var item in this)
            {
                item.OnViewPortChange(oldvp, newvp, currentZoom, newZoom, mouse);
            }
        }

        public void Show()
        {
            var curzoomlayer = this.OfType<ZoomOverlay>().Single(x => x.Zoom == _initialZoom);
            curzoomlayer.AddInitial();
        }
    }
}