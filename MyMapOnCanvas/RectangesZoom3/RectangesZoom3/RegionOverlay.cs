using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;

namespace RectangesZoom3
{
    class RegionOverlay : OverlayBase
    {
        public RegionOverlay(Map map)
            : base(map)
        {
polygons = new MyPolygonCollection(_map);
        }

        private MyPolygonCollection polygons;

        public override void Click(Point mouse)
        {
            polygons.Click(mouse);
        }




        public void StartRegion()
        {
            polygons.Start(1);
        }


        public void EndRegion()
        {
            polygons.End();
        }

        private byte zoomFactor = 2;

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
    }
}