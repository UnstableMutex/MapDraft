using System;
using System.Collections.Generic;
using System.Windows;

namespace CanvasMap
{
    internal class RegionOverlay : OverlayBase
    {
        private MyPolygonCollection polygons;

        private byte zoomFactor = 2;

        public RegionOverlay(MapMiddle map)
            : base(map)
        {
            polygons = new MyPolygonCollection(_map);
        }

        public override void Click(Point mouse)
        {
            polygons.Click(mouse);
        }

        internal IEnumerable<LinkedList<Point>> GetPolygonPoints()
        {
            return polygons.GetPolygonPoints();
        }

        internal void StartRegion()
        {
            polygons.Start(1);
        }

        internal void EndRegion()
        {
            polygons.End();
        }

        public override void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse)
        {
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);

            if (scaleMultiplier != 1)
            {
                polygons.Zoom(scaleMultiplier, mouse);
            }
            else
            {
                var vector = newvp.TopLeft - oldvp.TopLeft;
                polygons.Move(vector);
            }
        }

        public void LoadPolygons(List<LinkedList<Point>> pointPolygons)
        {
            polygons.Upload(pointPolygons);
        }
    }
}