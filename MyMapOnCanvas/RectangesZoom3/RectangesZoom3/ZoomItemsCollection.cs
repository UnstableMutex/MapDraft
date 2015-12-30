using System;
using System.Collections.Generic;
using System.Windows;

namespace RectangesZoom3
{
    class ZoomItemsCollection : SortedList<byte, ZoomItems>,IZoomItems
    {
        private byte _initialZoom;

        public ZoomItemsCollection(byte initialZoom)
        {
            _initialZoom = initialZoom;
            var multi = Math.Pow(2, initialZoom);
            //  viewPort = new Rect(0, 0, Constants.TileSize * multi, Constants.TileSize * multi);

        }

        public void Click(Point mouse)
        {
            foreach (var item in this)
            {
                item.Value.Click(mouse);
            }
        }

        public byte Zoom
        {
            get { return _initialZoom; }
            set { _initialZoom = value; }
        }


        public void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            foreach (var item in this)
            {
                item.Value.OnViewPortChange(oldvp, newvp, currentZoom, newZoom);
            }
        }

        public void Show()
        {
           this[_initialZoom].AddInitial();
        }
    }
}