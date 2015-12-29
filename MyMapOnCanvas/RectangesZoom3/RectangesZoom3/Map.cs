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
        private ZoomItemsCollection zoomLayers;
        public Map()
        {
            var arr = Enumerable.Range(0, 15).Select(x => new ZoomItems((byte)x,this)).OrderBy(x => x.Zoom).ToArray();
            zoomLayers = new ZoomItemsCollection();
            foreach (var item in arr)
            {
                zoomLayers.Add(item.Zoom, item);
            }
        }
        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            zoomLayers.OnZoom(mouse, currentZoom, newZoom);
        }

       

        protected override void OnDragMap(Vector v)
        {
            zoomLayers.OnDragMap(v);
        }
    }

    class ZoomItemsCollection : SortedList<byte, ZoomItems>
    {

        public ZoomItemsCollection()
        {

        }
        public void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            foreach (var item in this)
            {
                item.Value.OnZoom(mouse, currentZoom, newZoom);
            }

        }

        public void OnDragMap(Vector v)
        {
            foreach (var item in this)
            {
                item.Value.OnDragMap(v);
            }
        }

    }


    class ZoomItems
    {
        private readonly byte _zoom;
        private readonly Map _map;

        public ZoomItems(byte zoom,Map map)
        {
            _zoom = zoom;
            _map = map;
        }

        public void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            
        }

        public void OnDragMap(Vector v)
        {
            
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
