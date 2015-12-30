﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectangesZoom3
{
    class Map : MapBase
    {
        public Tile[] Tiles
        {
            get { return Children.OfType<Tile>().ToArray(); }
        }
        private ZoomItemsCollection zoomLayers;
        public Map()
        {
            var arr = Enumerable.Range(0, 15).Select(x => new ZoomItems((byte)x, this)).OrderBy(x => x.Zoom).ToArray();
            zoomLayers = new ZoomItemsCollection(currentZoom);



            foreach (var item in arr)
            {
                zoomLayers.Add(item.Zoom, item);
            }

        }

        //public override void EndInit()
        //{

        //}


        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);

        //    viewPort = new Rect(0, 0, ActualWidth, ActualHeight);
        //    zoomLayers.Show();
        //}



        private Rect viewPort = Rect.Empty;
        private byte zoomFactor = 2;
        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);

            //var newX = mouse.X * (1 - scaleMultiplier);
            //newX = viewPort.X + (mouse.X - viewPort.X) * (1 - scaleMultiplier);
            //var newY = mouse.Y * (1 - scaleMultiplier);
            //newY = viewPort.Y + (mouse.Y - viewPort.Y) * (1 - scaleMultiplier);

            var newtopleft = viewPort.TopLeft + (mouse - viewPort.TopLeft) * (1 - scaleMultiplier);

            var newzoomRect = new Rect(newtopleft, viewPort.Size);
            newzoomRect.Scale(scaleMultiplier, scaleMultiplier);


            var canvasrect = new Rect(0, 0, ActualWidth, ActualHeight);

            var maxWidth = Math.Min(canvasrect.Width, newzoomRect.Width + newzoomRect.X);
            var maxHeight = Math.Min(canvasrect.Height, newzoomRect.Height + newzoomRect.Y);
            var newrect = new Rect(newzoomRect.X, newzoomRect.Y, maxWidth, maxHeight);

            ViewPortChange(viewPort, newrect, currentZoom, newZoom);

            viewPort = newrect;
            zoomLayers.Zoom = newZoom;

        }

        private void ViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            zoomLayers.ViewPortChange(oldvp, newvp, currentZoom, newZoom);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (viewPort.IsEmpty)
            {
                viewPort = new Rect(0, 0, ActualWidth, ActualHeight);
            }
            var oldvp = viewPort;
            var newvp = new Rect(oldvp.TopLeft, sizeInfo.NewSize);
            oldvp.Transform(new Matrix());
            ViewPortChange(oldvp, newvp, zoomLayers.Zoom, zoomLayers.Zoom);

        }


        protected override void OnDragMap(Vector v)
        {
            var old = viewPort;
            var check = viewPort;
            check.Offset(v);
            var isvalid = Validate(check);
            if (!isvalid)
                return;

            viewPort.Offset(v);
            ViewPortChange(old, viewPort, zoomLayers.Zoom, zoomLayers.Zoom);
        }

        private bool Validate(Rect check)
        {
            var isnotvalid = (check.X > 0) | check.Y > 0;
            Debug.Print("checkrect: {0}", check);
            Debug.Print(check.BottomRight.ToString());
            isnotvalid |= check.BottomRight.X < 0;// | check.BottomRight.Y < 0;
            isnotvalid |= check.Size.Height - check.Y >= 1024;//хз почему
            return !(isnotvalid);
        }
    }
}
