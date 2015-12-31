using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            var arr =
                Enumerable.Range(0, 15).Select(x => new ZoomOverlay((byte) x, this)).OrderBy(x => x.Zoom).ToArray();
            zoomLayers = new ZoomItemsCollection(currentZoom);
            regionOverlay = new RegionOverlay(this);
            foreach (var item in arr)
            {
                zoomLayers.Add(item);
            }
            zoomLayers.Add(regionOverlay);
        }

        public void StartRegion()
        {
            regionOverlay.StartRegion();
        }

        public void EndRegion()
        {
            regionOverlay.EndRegion();
        }


        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            Debug.Print("click");
            zoomLayers.Click(e.GetPosition(this));
        }


        private Rect _viewPort = Rect.Empty;
        private byte zoomFactor = 2;

        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            //вьюпорт решает как сместить область!
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);
            var newtopleft = _viewPort.TopLeft + (mouse - _viewPort.TopLeft)*(1 - scaleMultiplier);
            var newzoomRect = new Rect(newtopleft,
                new Size(_viewPort.Size.Width*scaleMultiplier, _viewPort.Size.Height*scaleMultiplier));
            var newrect = new Rect(newzoomRect.TopLeft, newzoomRect.Size);
            ViewPortChange(_viewPort, newrect, currentZoom, newZoom, mouse);
            _viewPort = newrect;
            zoomLayers.Zoom = newZoom;
        }

        private void ViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse)
        {
            zoomLayers.OnViewPortChange(oldvp, newvp, currentZoom, newZoom, mouse);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (_viewPort.IsEmpty)
            {
                _viewPort = new Rect(0, 0, ActualWidth, ActualHeight);
            }
            var oldvp = _viewPort;
            var newvp = new Rect(oldvp.TopLeft, sizeInfo.NewSize);
            var valid = Validate(newvp, zoomLayers.Zoom);
            if (!valid)
            {
                var vector = (Point) sizeInfo.NewSize - (Point) sizeInfo.PreviousSize;
                newvp = oldvp;
                newvp.Offset(vector);
                newvp.Size = sizeInfo.NewSize;
            }
            ViewPortChange(oldvp, newvp, zoomLayers.Zoom, zoomLayers.Zoom, Mouse.GetPosition(this));
            _viewPort = newvp;
        }


        protected override void OnDragMap(Vector v)
        {
            var old = _viewPort;
            var check = _viewPort;
            check.Offset(v);
            var isvalid = Validate(check, zoomLayers.Zoom);
            if (!isvalid)
                return;
            _viewPort.Offset(v);
            ViewPortChange(old, _viewPort, zoomLayers.Zoom, zoomLayers.Zoom, Mouse.GetPosition(this));
        }

        private bool Validate(Rect check, int zoom)
        {
            var isnotvalid = (check.X > 0) | check.Y > 0;
            isnotvalid |= check.Size.Width - check.X >= Constants.TileSize*Math.Pow(2, zoom);
            isnotvalid |= check.Size.Height - check.Y >= Constants.TileSize*Math.Pow(2, zoom); //хз почему
            return !(isnotvalid);
        }
    }
}