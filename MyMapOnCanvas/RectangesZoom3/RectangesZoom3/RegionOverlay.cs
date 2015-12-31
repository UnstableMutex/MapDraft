using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RectangesZoom3
{
    class RegionOverlay : OverlayBase
    {
        public RegionOverlay(Map map) : base(map)
        {
        }

        public override void Click(Point mouse)
        {
            if (active == null)
                return;
            var thumb = active.AddThumb(mouse, _map);
            _map.Children.Add(thumb);
        }

        MyPolygon active;

        public void StartRegion()
        {
            active = new MyPolygon(1);
        }

        public void EndRegion()
        {
            active.MakePolygon(_map);
        }

        private byte zoomFactor = 2;

        public override void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse)
        {
            if (active != null)
            {
                var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);
                if (scaleMultiplier != 1)
                {
                    active.Zoom(scaleMultiplier, mouse);
                }
                else
                {
                    var vector = newvp.TopLeft - oldvp.TopLeft;
                    active.Move(vector);
                }
            }
        }
    }
}