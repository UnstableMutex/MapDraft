using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CanvasMap
{
    public class MapMiddle : MapBase
    {
        private Rect _viewPort = Rect.Empty;
        private RegionOverlay regionOverlay;
        private byte zoomFactor = 2;

        private ZoomItemsCollection zoomLayers;

        public MapMiddle()
        {
            var arr =
                Enumerable.Range(0, 19).Select(x => new ZoomOverlay((byte)x, this)).OrderBy(x => x.Zoom).ToArray();
            zoomLayers = new ZoomItemsCollection(currentZoom);
            regionOverlay = new RegionOverlay(this);
            foreach (var item in arr)
            {
                zoomLayers.Add(item);
            }
            zoomLayers.Add(regionOverlay);
        }

        internal Tile[] Tiles
        {
            get { return Children.OfType<Tile>().ToArray(); }
        }

        protected void StartRegion()
        {
            regionOverlay.StartRegion();
        }

        protected void EndRegion()
        {
            regionOverlay.EndRegion();
        }
        public Func<IList<GeoPolygon>> GetPolygonsDelegate
        { get { return GetPolygons; } }
        public Action<IList<GeoPolygon>> AddPolygonsDelegate
        { get { return AddPolygons; } }

        IList<GeoPolygon> GetPolygons()
        {
            var points = regionOverlay.GetPolygonPoints();
            List<GeoPolygon> lgp = new List<GeoPolygon>();
            foreach (var ppoly in points)
            {
                var gp = new GeoPolygon();
                foreach (var point in ppoly)
                {
                    var loc = MercatorWrapper.GetLocation(point, _viewPort, currentZoom);
                    gp.AddLast(loc);
                }
                lgp.Add(gp);
            }
            return lgp;
        }

        void AddPolygons(IList<GeoPolygon> geopolys)
        {
            List<LinkedList<Point>> pointPolygons = new List<LinkedList<Point>>();
            foreach (var geopoly in geopolys)
            {
                var pointPoly = new LinkedList<Point>();
                foreach (var loc in geopoly)
                {
                    var point = MercatorWrapper.GetPoint(loc, _viewPort, currentZoom);
                    pointPoly.AddLast(point);
                }
                pointPolygons.Add(pointPoly);
            }

            regionOverlay.LoadPolygons(pointPolygons);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var loc = MercatorWrapper.GetLocation(e.GetPosition(this), _viewPort, zoomLayers.Zoom);
            Debug.Print("lat:{0}", loc.Latitude);
            Debug.Print("lon:{0}", loc.Longitude);
            zoomLayers.Click(e.GetPosition(this));
        }

        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            //вьюпорт решает как сместить область!
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);
            var newtopleft = _viewPort.TopLeft + (mouse - _viewPort.TopLeft) * (1 - scaleMultiplier);
            var newzoomRect = new Rect(newtopleft,
                new Size(_viewPort.Size.Width * scaleMultiplier, _viewPort.Size.Height * scaleMultiplier));
            var newrect = new Rect(newzoomRect.TopLeft, newzoomRect.Size);
            // Validate(ref newrect, newZoom);
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
            var valid = true;// Validate(ref newvp, zoomLayers.Zoom);
            if (!valid)
            {
                var vector = (Point)sizeInfo.NewSize - (Point)sizeInfo.PreviousSize;
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

            _viewPort = check;
            ViewPortChange(old, _viewPort, zoomLayers.Zoom, zoomLayers.Zoom, Mouse.GetPosition(this));
        }

        protected Location MouseCoords()
        {
            var loc = MercatorWrapper.GetLocation(Mouse.GetPosition(this), _viewPort, zoomLayers.Zoom);
            return loc;
        }
    }
}