using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyOwnMap
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


      

        byte zoom = 2;
        private bool _mouseCaptured;
        private Point _previousMouse;

        private async void C_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            await RepaintMap();
        }

        private async Task RepaintMap()
        {
            c.Children.Clear();

            var tilecounthorizontal = Math.Ceiling(c.ActualWidth / Constants.tileSize);
            var tilecountvertical = Math.Ceiling(c.ActualHeight / Constants.tileSize);
            for (ushort vindex = 0; vindex < tilecountvertical; vindex++)
                for (ushort hindex = 0; hindex < tilecounthorizontal; hindex++)
                {
                    Tile i = new Tile(new TileIndex { X = hindex, Y = vindex }, zoom);
                    await i.Upload();
                    i.Width = Constants.tileSize;
                    i.Height = Constants.tileSize;
                    Canvas.SetTop(i, vindex * Constants.tileSize);
                    Canvas.SetLeft(i, hindex * Constants.tileSize);
                    c.Children.Add(i);


                }
        }

        private void LeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            // base.OnMouseLeftButtonUp(e);
            c.ReleaseMouseCapture();
            _mouseCaptured = false;
        }

        private void LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //  base.OnMouseLeftButtonDown(e);
            this.Focus(); // Make sure we get the keyboard

            if (c.CaptureMouse())
            {

                _mouseCaptured = true;
                _previousMouse = e.GetPosition(c);
            }
        }

        private async void MouseMove(object sender, MouseEventArgs e)
        {

            base.OnMouseMove(e);
            if (_mouseCaptured)
            {
                // this.BeginUpdate();
                Point position = e.GetPosition(c);
                Vector v = position - _previousMouse;

                //двигаем существующие тайлы
                List<Tile> todelete = new List<Tile>();
                foreach (var child in c.Children)
                {
                    var chi = child as Tile;

                    var newPos = chi.GetNewPosition(v, c);

                    if (!newPos.HasValue)
                    {
                        todelete.Add(chi);
                    }
                    else
                    {
                        Canvas.SetTop(chi, newPos.Value.Y);
                        Canvas.SetLeft(chi, newPos.Value.X);
                    }

                }

                foreach (var td in todelete)
                {
                    c.Children.Remove(td);
                }
                if (todelete.Count > 0)
                    Debug.WriteLine("deleted " + todelete.Count);
                //координаты прямоугольника с картинками после удаления невидимых
                var rect = GetRect(c);
                //  Debug.WriteLine("rect: "+rect);
                await AddNewImages(rect);








                //_offsetX.Translate(position.X - _previousMouse.X);
                //_offsetY.Translate(position.Y - _previousMouse.Y);
                _previousMouse = position;
                // this.EndUpdate();
            }
        }

        private async Task AddNewImages(Rect rect)
        {
            var indexesToAdd = new HashSet<TileIndex>();
            //считаем количество тайлов по горизонтали
            var hcount = (ushort)Math.Ceiling((rect.Right - rect.Left) / Constants.tileSize);
            var vcount = (ushort)Math.Ceiling((rect.Bottom - rect.Top) / Constants.tileSize);
            var IndexRect = GetIndexRect();
            Debug.WriteLine("indexrect " + IndexRect);
            Debug.WriteLine("rect: " + rect);
            //определим строки для догрузки

            if (rect.Top > 0)
            {
                if (IndexRect.minY > 0)
                {
                    //если первая строка самая верхняя на карте ничего загружать нинада
                    for (ushort i = IndexRect.minX; i <= IndexRect.maxX; i++)
                    {
                        var ti = new TileIndex { X = i, Y = (ushort)(IndexRect.minY - 1) };
                        Tile t = new Tile(ti, zoom);

                        await t.Upload();
                        //позиционируем тайл
                        c.Children.Add(t);
                        var top = rect.Top - Constants.tileSize;
                        Debug.WriteLine("top " + top);
                        var left = rect.X + (Constants.tileSize * (i - IndexRect.minX));


                        Canvas.SetTop(t, top);
                        Canvas.SetLeft(t, left);
                        // Debug.WriteLine(ti.X+);
                    }
                }

            }
            if (rect.Left > 0)
            {
                if (IndexRect.minX > 0)
                {
                    for (ushort i = IndexRect.minY; i <= IndexRect.maxY; i++)
                    {
                        var ti = new TileIndex { Y = i, X = (ushort)(IndexRect.minX - 1) };
                        Tile t = new Tile(ti, zoom);

                        await t.Upload();
                        //позиционируем тайл
                        c.Children.Add(t);
                        var left = rect.Left - Constants.tileSize;
                        
                        var top = rect.Y + (Constants.tileSize * (i - IndexRect.minY));


                        Canvas.SetTop(t, top);
                        Canvas.SetLeft(t, left);
                        // Debug.WriteLine(ti.X+);
                    }
                }

            }
            if (rect.Right < c.ActualWidth)
            {
  for (ushort i = IndexRect.minY; i <= IndexRect.maxY; i++)
                {
                    var ti = new TileIndex { Y = i, X = (ushort)(IndexRect.maxX + 1) };
                    Tile t = new Tile(ti, zoom);

                    await t.Upload();
                    //позиционируем тайл
                    c.Children.Add(t);

                    Canvas.SetLeft(t, rect.Right);
                    Canvas.SetTop(t, rect.Y + (Constants.tileSize * (i - IndexRect.minY)));
                    // Debug.WriteLine(ti.X+);
                }
            }
            if (rect.Bottom < c.ActualHeight)
            {

                for (ushort i = IndexRect.minX; i <= IndexRect.maxX; i++)
                {
                    var ti = new TileIndex { X = i, Y = (ushort)(IndexRect.maxY + 1) };
                    Tile t = new Tile(ti, zoom);

                    await t.Upload();
                    //позиционируем тайл
                    c.Children.Add(t);

                    Canvas.SetTop(t, rect.Bottom);
                    Canvas.SetLeft(t, rect.X + (Constants.tileSize * (i - IndexRect.minX)));
                    // Debug.WriteLine(ti.X+);
                }
            }
            //определим углы для догрузки

            //скипнуто

            //загрузка тайлов
            foreach (var tileIndex in indexesToAdd)
            {

                //позиционируем тайл

            }


        }
        /// <summary>
        /// получает индексный прямоугольник уже загруженных тайлов
        /// </summary>
        /// <returns></returns>
        private RectIndex GetIndexRect()
        {
            var res = new RectIndex();
            res.minX = res.minY = ushort.MaxValue;


            foreach (var child in c.Children)
            {
                var t = (Tile)child;
                if (t.X < res.minX)
                {
                    res.minX = t.X;
                }
                if (t.Y < res.minY)
                {
                    res.minY = t.Y;
                }
                if (t.X > res.maxX)
                {
                    res.maxX = t.X;
                }
                if (t.Y > res.maxY)
                {
                    res.maxY = t.Y;
                }
            }

            return res;
        }

        private Rect GetRect(Canvas canvas)
        {

            Point res = new Point(double.MaxValue, double.MaxValue);//точка с минимальными коордами
            Point maxPoint = new Point();//точка с максимальными коордами
            foreach (var child in c.Children)
            {
                var chi = child as Tile;
                var x = Canvas.GetLeft(chi);
                var y = Canvas.GetTop(chi);
                if (x < res.X)
                {
                    res.X = x;
                }
                if (y < res.Y)
                {
                    res.Y = y;
                }
                if (maxPoint.X < x)
                {
                    maxPoint.X = x;
                }
                if (maxPoint.Y < y)
                {
                    maxPoint.Y = y;
                }




            }
            maxPoint.Offset(Constants.tileSize, Constants.tileSize);
            var rect = new Rect(res, maxPoint);
            return rect;

        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var ztemp = e.Delta > 0 ? 1 : -1;
          var  newzoom = zoom + ztemp;
            if (newzoom >= 0 & newzoom <= 12)
            {
                zoom = (byte)newzoom;
                RepaintMap();

            }

        }
        private void zoominc(object sender, RoutedEventArgs e)
        {
            if (zoom < 12)
            {
                zoom++;
                RepaintMap();
            }
        }


        private void zoomdec(object sender, RoutedEventArgs e)
        {
            if (zoom > 0)
            {
                zoom--;
                RepaintMap();
            }
        }


    }
}
