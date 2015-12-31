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
                Enumerable.Range(0, 17).Select(x => new ZoomOverlay((byte)x, this)).OrderBy(x => x.Zoom).ToArray();
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
            var loc = MercatorWrapper.GetLocation(e.GetPosition(this), _viewPort, zoomLayers.Zoom);
            Debug.Print("lat:{0}", loc.Latitude);
            zoomLayers.Click(e.GetPosition(this));
        }


        private Rect _viewPort = Rect.Empty;
        private byte zoomFactor = 2;

        protected override void OnZoom(Point mouse, byte currentZoom, byte newZoom)
        {
            //вьюпорт решает как сместить область!
            var scaleMultiplier = Math.Pow(zoomFactor, newZoom - currentZoom);
            var newtopleft = _viewPort.TopLeft + (mouse - _viewPort.TopLeft) * (1 - scaleMultiplier);
            var newzoomRect = new Rect(newtopleft,
                new Size(_viewPort.Size.Width * scaleMultiplier, _viewPort.Size.Height * scaleMultiplier));
            var newrect = new Rect(newzoomRect.TopLeft, newzoomRect.Size);
            Validate(ref newrect, newZoom);
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
            var valid = Validate(ref newvp, zoomLayers.Zoom);
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
            var isvalid = ValidateDrag(ref check, zoomLayers.Zoom);
            if (!isvalid)
                return;
            _viewPort = check;
            ViewPortChange(old, _viewPort, zoomLayers.Zoom, zoomLayers.Zoom, Mouse.GetPosition(this));
        }

        private bool ValidateDrag(ref Rect check, byte zoom)
        {
            var vpsize = Constants.TileSize * Math.Pow(2, zoom);
            var isvalid = check.X <= 0;
            isvalid &= check.Y <= 0;
            isvalid &= check.X+vpsize >= ActualWidth;
            isvalid &= check.Y+vpsize >= ActualHeight;
            return isvalid;
        }

        private bool Validate(ref Rect check, int zoom)
       {
  //          var canvasSize = new Size(ActualWidth, ActualHeight);
  //          var canvasRect = new Rect(new Point(0, 0), canvasSize);
  //          var max = Constants.TileSize * Math.Pow(2, zoom);




  //          var isnotvalid = (check.X > 0) | check.Y > 0;//слева сверху не должно быть белых пятен

  //          var widthOver = max < check.Width;//карта меньше чем экран по иксу
  //          if (widthOver)
  //          {
  //              check.X = 0;
  //          }
  //          else
  //          {
  // isnotvalid |= check.Size.Width - check.X >= max;
  //          }
  //          var heightOver = max < check.Height;//карта меньше экрана по игреку
  //          if (heightOver)
  //          {
  //              check.Y = 0;
  //          }
  //          else
  //          {
  //isnotvalid |= check.Size.Height - check.Y >= max; //хз почему
  //          }


  //          //if (check.Size.Width - check.X >= max)
  //          //{
  //          //    //надо сместить вьюпорт по иксу

  //          //}

         
          










  //          Debug.Print("rect: {0} zoom: {1}", check, zoom);
  //          Debug.Print("notvalid:{0}", isnotvalid);
  //          return !(isnotvalid);

            return true;


        }
    }
}