using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            zoomLayers = new ZoomItemsCollection(2);

            viewPort = new Rect(0, 0, ActualWidth, ActualHeight);

            foreach (var item in arr)
            {
                zoomLayers.Add(item.Zoom, item);
            }
        }


        private Rect viewPort;
        private byte zoomFactor = 2;
        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);
            var newX = mouse.X * (1 - scaleMultiplier);
            var newY = mouse.Y * (1 - scaleMultiplier);
            var newzoomRect = new Rect(newX, newY, viewPort.Width * scaleMultiplier, viewPort.Height * scaleMultiplier);
            var canvasrect = new Rect(0, 0, ActualWidth, ActualHeight);

            var maxWidth = Math.Min(canvasrect.Width, newzoomRect.Width + newzoomRect.X);
            var maxHeight = Math.Min(canvasrect.Height, newzoomRect.Height + newzoomRect.Y);
            var newrect = new Rect(0 - newzoomRect.X, 0 - newzoomRect.Y, maxWidth, maxHeight);

            ViewPortChange(viewPort, newrect, currentZoom, newZoom);

            viewPort = newrect;
            zoomLayers.Zoom = newZoom;
            //var offsetForZoom = -1 * (mouse - viewPort.TopLeft) * zoomFactor;
            //var old = viewPort;
            //viewPort.Scale(scaleMultiplier, scaleMultiplier);
            //viewPort.Offset(offsetForZoom);
            //ViewPortChange(old, viewPort);
        }

        private void ViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            zoomLayers.ViewPortChange(oldvp, newvp, currentZoom, newZoom);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var oldvp = viewPort;
            var newvp = new Rect(oldvp.TopLeft, sizeInfo.NewSize);
            oldvp.Transform(new Matrix());
            ViewPortChange(oldvp, newvp,zoomLayers.Zoom,zoomLayers.Zoom);

        }


        protected override void OnDragMap(Vector v)
        {
            var old = viewPort;
            viewPort.Offset(v);
            ViewPortChange(old, viewPort,zoomLayers.Zoom,zoomLayers.Zoom);
        }
    }

    class ZoomItemsCollection : SortedList<byte, ZoomItems>
    {
        private byte _initialZoom;

        public ZoomItemsCollection(byte initialZoom)
        {
            _initialZoom = initialZoom;
            var multi = Math.Pow(2, initialZoom);
            //  viewPort = new Rect(0, 0, Constants.TileSize * multi, Constants.TileSize * multi);

        }

        public byte Zoom
        {
            get { return _initialZoom; }
            set { _initialZoom = value; }
        }


        public void ViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            foreach (var item in this)
            {
                item.Value.OnViewPortChange(oldvp, newvp, currentZoom, newZoom);
            }
        }
    }


    class ZoomItems
    {
        private readonly byte _zoom;
        private readonly Map _map;
        private bool _activeLayer;
        public ZoomItems(byte zoom, Map map)
        {
            _zoom = zoom;
            _map = map;
        }


        public void OnViewPortChange(Rect oldvp, Rect newvp)
        {





        }
        public byte Zoom
        {
            get { return _zoom; }
        }

        ImageSource GetImage(int x, int y)
        {
            return MyImageDownloaderAsync.GetImageS(_zoom, x, y);
        }

        ImageSource GetImage(TilePosition tp)
        {
            return MyImageDownloaderAsync.GetImageS(_zoom, tp.X, tp.Y);
        }

        internal void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            //var oldmulti = oldvp.Width/Constants.TileSize;
            //var oldzoom = Math.Log(oldmulti,2) - 1;
            //var newZoom = Math.Log(oldvp.Height/Constants.TileSize, 2) - 1;

            if (Zoom == currentZoom)
            {
                //если зум усттарел надо убрать тайлы
                foreach (var tile in _map.Tiles)
                {
                    _map.Children.Remove(tile);
                }

            }
            if (Zoom == newZoom)
            {
                //если зум новый то добавить тайлы
                double vpX = newvp.X;


                var firstTileXIndex = (int)Math.Ceiling(vpX / Constants.TileSize);
                var firstTileYIndex = (int)Math.Ceiling(newvp.Y / Constants.TileSize);
                var coordX = vpX - firstTileXIndex * Constants.TileSize;
                var coordY = newvp.Y - firstTileYIndex * Constants.TileSize;

                // var image = GetImage(firstTileXIndex, firstTileYIndex);
                int currentXIndex = 0;
                int currentYIndex = 0;
                var currentCoordX = coordX;
                var currentCoordY = coordY;
                while (currentCoordY + Constants.TileSize <= newvp.Height)
                {
                    while (currentCoordX + Constants.TileSize <= newvp.Width)
                    {
                        var tp = new TilePosition();
                        tp.X = currentXIndex + firstTileXIndex;
                        tp.Y = currentYIndex + firstTileYIndex;
                        var image = GetImage(tp);
                        Tile tile = new Tile(tp, Zoom);
                        tile.Source = image;
                        _map.Children.Add(tile);
                        Canvas.SetLeft(tile,currentCoordX);
                        Canvas.SetTop(tile,currentCoordY);
                        currentCoordX += Constants.TileSize;
                        currentXIndex++;
                    }
                    currentCoordY += Constants.TileSize;
                    currentYIndex++;
                }




            }

        }
    }
}
