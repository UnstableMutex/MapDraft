using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RectanglesZoom2
{
    class TileGroup : TileGroupBase
    {
        private readonly int _originalZoom;
        private readonly TilePosition _pos;
        private readonly Canvas _canvas;
        private int currentZoom;
        private static Random r;

        static TileGroup()
        {
            r = new Random(DateTime.Now.Millisecond);
        }
        public TileGroup(int originalZoom, TilePosition pos, Canvas canvas)
        {
            currentZoom = _originalZoom = originalZoom;
            _pos = pos;
            _canvas = canvas;

            Background = new SolidColorBrush(Colors.Gray);
            Width = Height = Constants.TileSize;
            AddSingle();
        }

        public async void AddSingle()
        {


            AddSingle(currentZoom);

        }

        private async void AddSingle(int zoom)
        { 
            
            //var rect = new Rectangle();
            //rect.Stroke = GetRandomBrush();
            //rect.Width = rect.Height = Constants.TileSize;
            //rect.AddSingle = GetRandomBrush();
            Children.Clear();
            Rows = Columns = 1;
            var rect = new Tile(_pos, (ushort)zoom);

            this.Children.Add(rect);


            await UploadIfVisible(rect);
        }

        private async Task UploadIfVisible(Tile rect)
        {
            //var scrnPoint = rect.TranslatePoint(new Point(), _canvas);
            //// var scrnPoint= rect.PointToScreen(new Point(0, 0),_canvas);
            //Double dpiY = _canvas.ActualHeight;
            //var dpiX = _canvas.ActualWidth;


            //var isNotVisible = scrnPoint.X > dpiX | scrnPoint.Y > dpiY |
            //                   scrnPoint.X < 0 | scrnPoint.Y < 0;
            ////if (dpiX == 0 | dpiY == 0|!isNotVisible)
            ////{
            //if (rect.Source == null)
            //{
                await rect.Upload();
            //}


            // }
      
        
        }

        private static Brush GetRandomBrush()
        {
            return new SolidColorBrush(GetRandomColor());
        }
        private static Color GetRandomColor()
        {
            var c = new Color();
            byte[] arr = new byte[3];
            r.NextBytes(arr);
            c.R = arr[0];
            c.G = arr[1];
            c.B = arr[2];
            c.A = 255;
            return c;
        }

        Canvas ParentCanvas
        {
            get
            {
                return Parent as Canvas;

            }
        }

        Point GetCenter()
        {
            var LeftTopPoint = GetLeftTopPoint();
            var cl = Width / 2 + LeftTopPoint.X;
            var ct = Height / 2 + LeftTopPoint.Y;
            return new Point(cl, ct);


        }

        private Point GetLeftTopPoint()
        {
            var l = Canvas.GetLeft(this);
            var t = Canvas.GetTop(this);
            var LeftTopPoint = new Point(l, t);
            return LeftTopPoint;
        }

        public void Zoom(Point mousePos, int zoom)
        {
            Zoom(zoom, mousePos);
        }

        public void Zoom(int zoom, Point mousePos)
        {
            // зум может быть с разницей не только в единицу 
            //если поставить задержку на загрузку
            var deltaZoom = zoom - currentZoom;
            var sizeMultiplier = Math.Pow(2, deltaZoom);
            Width = Width * sizeMultiplier;
            Height = Height * sizeMultiplier;
            var topLeft = GetLeftTopPoint();
            //  mousePos = Mouse.GetPosition(this);
            var newTopLeft = GetNewTopLeftForZoom(topLeft, mousePos, sizeMultiplier);
            MoveTopLeftToPoint(newTopLeft);
            ChangeChildren(zoom, mousePos);

            currentZoom = zoom;
        }

        private void ChangeChildren(int zoom, Point mousePos)
        {
            Debug.WriteLine("zoom {0} originalzoom {1}", zoom, _originalZoom);
            var deltaZoom = zoom - _originalZoom;
            if (deltaZoom > 0)
            {
                if (Children.Count == 1)
                {
                    Children.Clear();
                    var tgarr = new TileGroup[]
                    {
                        new TileGroup(zoom, new TilePosition() {X = _pos.X*2, Y = _pos.Y*2},_canvas),
                        new TileGroup(zoom, new TilePosition() {X = _pos.X*2 + 1, Y = _pos.Y*2},_canvas),
                        new TileGroup(zoom, new TilePosition() {X = _pos.X*2, Y = _pos.Y*2 + 1},_canvas),
                        new TileGroup(zoom, new TilePosition() {X = _pos.X*2 + 1, Y = _pos.Y*2 + 1},_canvas)
                    };


                    this.Rows = this.Columns = 2;
                    foreach (var tg in tgarr)
                    {
                        Children.Add(tg);
                        tg.AddSingle();
                    }



                }
                else
                {
                    foreach (TileGroup child in Children)
                    {
                        child.Zoom(zoom, mousePos);
                    }
                }
            }
            else //deltazoom<0
            {
                var ch = this.Children;
                if (zoom == _originalZoom)
                {

                    AddSingle(_originalZoom);

                }
            }
        }


        public async void MoveTopLeftToPoint(Point newTopLeft)
        {
            Canvas.SetTop(this, newTopLeft.Y);
            Canvas.SetLeft(this, newTopLeft.X);
            foreach (Tile child in Children.OfType<Tile>())
            {
                Debug.Print("uifv");
               await UploadIfVisible(child);
            }
        }
        /// <summary>
        /// возвращает точку куда надо переместить левый верхний угол при зуммировании
        /// </summary>
        /// <param name="topLeft">текущий левый верхний угол</param>
        /// <param name="mousePos">позиция мыши</param>
        /// <param name="sizeMultiplier">множитель изменения размера</param>
        /// <returns></returns>
        Point GetNewTopLeftForZoom(Point topLeft, Point mousePos, double sizeMultiplier)
        {
            var x1 = -1 * (sizeMultiplier * (mousePos.X - topLeft.X) - mousePos.X);
            var y1 = -1 * (sizeMultiplier * (mousePos.Y - topLeft.Y) - mousePos.Y);
            return new Point(x1, y1);
        }

        public void Move(Vector v)
        {
            var lt = GetLeftTopPoint();
            Point newP = lt + v;
            MoveTopLeftToPoint(newP);


        }
    }
}
