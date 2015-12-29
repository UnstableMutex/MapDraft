using System;
using System.Collections.Generic;
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
            zoomLayers = new ZoomItemsCollection(2);
            foreach (var item in arr)
            {
                zoomLayers.Add(item.Zoom, item);
            }
        }


        private Rect viewPort;
        private byte zoomFactor = 2;
        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {

            var offsetForZoom = -1 * (mouse - viewPort.TopLeft) * zoomFactor;
            var old = viewPort;
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);

            viewPort.Scale(scaleMultiplier, scaleMultiplier);


            viewPort.Offset(offsetForZoom);
            ViewPortChange(old, viewPort);
        }

        void ViewPortChange(Rect oldvp, Rect newvp)
        {
            
            zoomLayers.ViewPortChange(oldvp,newvp);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var oldvp = viewPort;
            var newvp = new Rect(oldvp.TopLeft,sizeInfo.NewSize);
            ViewPortChange(oldvp,newvp);

        }


        protected override void OnDragMap(Vector v)
        {
            var old = viewPort;
            viewPort.Offset(v);
            ViewPortChange(old, viewPort);
        }
    }

    class ZoomItemsCollection : SortedList<byte, ZoomItems>
    {
        private readonly byte _initialZoom;
     
        public ZoomItemsCollection(byte initialZoom)
        {
            _initialZoom = initialZoom;
            var multi = Math.Pow(2, initialZoom);
          //  viewPort = new Rect(0, 0, Constants.TileSize * multi, Constants.TileSize * multi);

        }
        public void ViewPortChange(Rect oldvp, Rect newvp)
        {
            foreach (var item in this)
            {
                item.Value.OnViewPortChange(oldvp,newvp);
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

        
        public void OnViewPortChange(Rect oldvp,Rect newvp)
        {
            var oldmulti = oldvp.Width/Constants.TileSize;
            var oldzoom = Math.Log(oldmulti,2) - 1;
            var newZoom = Math.Log(oldvp.Height/Constants.TileSize, 2) - 1;

            if (Zoom == oldzoom)
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

            }





        }
        public byte Zoom
        {
            get { return _zoom; }
        }

        public ImageSource GetImage(int x, int y)
        {
            return MyImageDownloaderAsync.GetImageS(_zoom, x, y);
        }
    }
}
