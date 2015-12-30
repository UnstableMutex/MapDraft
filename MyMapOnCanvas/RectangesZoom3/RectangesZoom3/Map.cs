using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        RegionOverlay regionOverlay;
        public Map()
        {
            var arr = Enumerable.Range(0, 15).Select(x => new ZoomOverlay((byte)x, this)).OrderBy(x => x.Zoom).ToArray();
            zoomLayers = new ZoomItemsCollection(currentZoom);
            regionOverlay = new RegionOverlay(this);
            foreach (var item in arr)
            {
                zoomLayers.Add(item.Zoom, item);
            }
            zoomLayers.Add(regionOverlay);
        }

     

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            Debug.Print("click");
            zoomLayers.Click(e.GetPosition(this));
        }


        private Rect viewPort = Rect.Empty;
        private byte zoomFactor = 2;
        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            //вьюпорт решает как сместить область!
            Debug.Print("newzoom: {0}",newZoom);
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);
            var newtopleft = viewPort.TopLeft + (mouse - viewPort.TopLeft) * (1 - scaleMultiplier);

            var newzoomRect = new Rect(newtopleft,new Size(viewPort.Size.Width*scaleMultiplier,viewPort.Size.Height*scaleMultiplier)) ;
            var canvasrect = new Rect(0, 0, ActualWidth, ActualHeight);

            var x2 =canvasrect.Right;
            var y2 =canvasrect.Bottom;
            
            var newrect = new Rect(newzoomRect.TopLeft,newzoomRect.Size);
            var isvalid = Validate(newrect, newZoom);
            Debug.Print("valid: {0}", isvalid);
            ViewPortChange(viewPort, newrect, currentZoom, newZoom);

            viewPort = newrect;
            zoomLayers.Zoom = newZoom;

        }

        private void ViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            zoomLayers.OnViewPortChange(oldvp, newvp, currentZoom, newZoom);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {

            if (viewPort.IsEmpty)
            {
                viewPort = new Rect(0, 0, ActualWidth, ActualHeight);
            }
            var oldvp = viewPort;

            var newvp = new Rect(oldvp.TopLeft, sizeInfo.NewSize);

            var valid = Validate(newvp, zoomLayers.Zoom);
            if (!valid)
            {
                var vector = (Point)sizeInfo.NewSize - (Point)sizeInfo.PreviousSize;
                newvp = oldvp;
                newvp.Offset(vector);
                newvp.Size = sizeInfo.NewSize;

            }
            ViewPortChange(oldvp, newvp, zoomLayers.Zoom, zoomLayers.Zoom);
            viewPort = newvp;




        }


        protected override void OnDragMap(Vector v)
        {
            var old = viewPort;
            var check = viewPort;
            check.Offset(v);
            var isvalid = Validate(check, zoomLayers.Zoom);
            if (!isvalid)
                return;

            viewPort.Offset(v);
            ViewPortChange(old, viewPort, zoomLayers.Zoom, zoomLayers.Zoom);
        }

        private bool Validate(Rect check, int zoom)
        {
            var isnotvalid = (check.X > 0) | check.Y > 0;

            isnotvalid |= check.Size.Width - check.X >= Constants.TileSize * Math.Pow(2, zoom);// | check.BottomRight.Y < 0;
            isnotvalid |= check.Size.Height - check.Y >= Constants.TileSize * Math.Pow(2, zoom);//хз почему
            return !(isnotvalid);
        }
    }
}
