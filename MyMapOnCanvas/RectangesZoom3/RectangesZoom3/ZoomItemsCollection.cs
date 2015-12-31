using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace RectangesZoom3
{
    class ZoomItemsCollection : List<OverlayBase>, IZoomItems
    {
        private byte _initialZoom;

        public ZoomItemsCollection(byte initialZoom)
        {
            _initialZoom = initialZoom;
            //  viewPort = new Rect(0, 0, Constants.TileSize * multi, Constants.TileSize * multi);
        }

        public void Click(Point mouse)
        {
            foreach (var item in this)
            {
                item.Click(mouse);
            }
        }

        public byte Zoom
        {
            get { return _initialZoom; }
            set { _initialZoom = value; }
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