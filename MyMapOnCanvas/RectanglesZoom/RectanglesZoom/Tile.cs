using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectanglesZoom
{
    class Tile : Image
    {
        private readonly byte _zoom;
        private readonly int _y;
        private readonly int _x;
        static Random r = new Random(DateTime.Now.Millisecond);
        const string folder = @"C:\Users\ShevyakovDY\Desktop\pics\{0}.png";


        public Tile(byte zoom, int x, int y)
        {
            _zoom = zoom;
            _y = y;
            _x = x;

          //  var path = string.Format(folder, r.Next(1, 5));

            SetSingleImage();
            //  SetPosition();
        }

        public void MoveOnWheel(int mouseDelta, Point mouse)
        {
            ImageMoveResize imr = new ImageMoveResize();
            var rect = new Rect(new Point(Canvas.GetLeft(this), Canvas.GetTop(this)),
                new Size(ActualWidth, ActualHeight));

            var vector = imr.GetOffSet(mouse, rect, mouseDelta);
            rect.Offset(vector);
            Canvas.SetLeft(this, rect.Left);
            Canvas.SetTop(this, rect.Top);
        }
        public void SimpleMove(Vector v)
        {
            //ImageMoveResize imr = new ImageMoveResize();
            var rect = new Rect(new Point(Canvas.GetLeft(this), Canvas.GetTop(this)),
                new Size(ActualWidth, ActualHeight));

            //var vector = imr.GetOffSet(mouse, rect, mouseDelta);
            rect.Offset(v);
            Canvas.SetLeft(this, rect.Left);
            Canvas.SetTop(this, rect.Top);
        }


        public void ResizeOnWheel(int mouseDelta)
        {
            ImageMoveResize imr = new ImageMoveResize();
            var size = new Size(ActualWidth, ActualHeight);

            size = imr.GetNewSize(size, mouseDelta);

            Width = size.Width;
            Height = size.Height;
        }


        Canvas MapCanvas
        {
            get { return Parent as Canvas; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int X
        {
            get { return _x; }
        }


        async void SetSingleImage()
        {
            BitmapImage bi = null;
            //вычисляем имеет ли смысл на карте прямоугольник
            //bool isRectInValidonmap = (_x < 0 | _y < 0) | (_x > Math.Pow(2, _zoom) - 1) | (_y > Math.Pow(2, _zoom) - 1);


            //if (isRectInValidonmap)
            //{
                //testcode
                var path = string.Format(folder, r.Next(1, 5));
                bi = new BitmapImage(new Uri(path));
                var s = BitmapDrawNums.DrawNums(bi, _x, _y, _zoom);
                Source = s;
            //}
            //else
            //{
            //    ImageSource bi1 = await MyImageDownloaderAsync.GetImage(_zoom, (uint)_x, (uint)_y);
            //    bi = bi1 as BitmapImage;
            //    Source = bi;
            //}






        }


        public void SetPosition()
        {
            Canvas.SetTop(this, Constants.TileSize * _y);
            Canvas.SetLeft(this, Constants.TileSize * _x);
        }

        public void SetSize(double tileSize)
        {
            Height = Width = tileSize;
        }
    }
}