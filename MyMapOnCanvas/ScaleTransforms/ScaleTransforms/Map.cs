using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScaleTransforms
{
    class Map : MapBase
    {
        private byte currentZoom = 2;
        float zoomFactor = 0.9f;
        public Map()
        {
            Background = Brushes.Transparent;
            // InitMap();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InitMap();
        }

        private void InitMap()
        {
            var currentRightX = 0;
            int currentXIndex = 0;
            var currentBottomY = 0;
            int currentYIndex = 0;

            while (currentBottomY < ActualHeight)
            {
                while (currentRightX < ActualWidth)
                {
                    var r = new Tile(currentZoom, currentXIndex, currentYIndex);
                    Canvas.SetTop(r, currentYIndex * Constants.TileSize);
                    Canvas.SetLeft(r, currentXIndex * Constants.TileSize);
                    this.Children.Add(r);
                    currentXIndex++;
                    currentRightX = currentXIndex * Constants.TileSize;
                }
                currentXIndex = 0;
                currentYIndex++;
                currentBottomY = currentYIndex * Constants.TileSize;
            }


        }


        TranslateTransform tr = new TranslateTransform(0, 0);

        protected override void OnDragMap(Vector v)
        {
            this.tr.X += v.X;
            this.tr.Y += v.Y;

            ApplyTransform();

        }



        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var cz = currentZoom + e.Delta / 120;
            currentZoom = (byte)cz;
            var mpos = e.GetPosition(this);
            var tiles = Children.OfType<Tile>();
            var r = tiles.First();
            var rect = new Rect(new Point(Canvas.GetLeft(r), Canvas.GetTop(r)),
                     new Size(r.ActualWidth, r.ActualHeight));
            ImageMoveResize imr = new ImageMoveResize(zoomFactor);
            var offset = imr.GetOffSet(mpos, rect, e.Delta);
            tr.X += offset.X;
            tr.Y += offset.Y;
            ApplyTransform();

        }

        private void ApplyTransform()
        {
            var scale = (double)zoomFactor;
            scale = Math.Pow(scale, currentZoom);
            var tg = new TransformGroup();

            var m = Mouse.GetPosition(this);
            var st = new ScaleTransform(scale, scale);
            tg.Children.Add(st);
            tg.Children.Add(tr);
            this.RenderTransform = tg;


        }
    }
}
