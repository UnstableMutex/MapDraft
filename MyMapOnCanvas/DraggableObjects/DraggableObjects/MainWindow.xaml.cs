using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DraggableObjects.MapControl;

namespace DraggableObjects
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

        MyPolygon Find(object sender)
        {
            MyPolygon pol = null;
            var t = sender as FrameworkElement;
            foreach (var mp in list)
            {
                var b = mp.Figures.Contains(t);
                if (b)
                {
                    pol = mp;
                    return pol;
                }
            }
            throw new NotImplementedException();

        }
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
          
            var t = sender as Thumb;
            MyPolygon pol = Find(t);

            if (pol != null)
            {
                pol.DragThumb(t, new Point(e.HorizontalChange, e.VerticalChange));
            }


        }
        List<MyPolygon> list = new List<MyPolygon>();
        private MyPolygon active;
        private void StartPoly_Click(object sender, RoutedEventArgs e)
        {
            active = new MyPolygon(1);


        }

        private void EndPoly_Click(object sender, RoutedEventArgs e)
        {
            if (active == null)
                return;
            active.MakePolygon(canvas);
            list.Add(active);
            active = null;
        }





        private void Canvas_Click(object sender, MouseButtonEventArgs e)
        {
            if (active == null)
                return;
            var thumb = active.AddThumb(e.GetPosition(canvas), canvas);

        

            canvas.Children.Add(thumb);



        }


        private void Line_Click(object sender, MouseButtonEventArgs e)
        {
            var l = sender as Line;
            if (l == null)
            { return; }
            MyPolygon pol = Find(l);



            pol.AddThumbOnCenter(l);
        }




        private void DelThumb_Click(object sender, RoutedEventArgs e)
        {
          
         
            Thumb thumb = null;
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    thumb = cm.PlacementTarget as Thumb;
                }
            }
            if (thumb == null)
            {
                return;
            }
            var poly = Find(thumb);
            poly.DeleteThumb(thumb);
           

        }






        private void Line_MouseEnter(object sender, MouseEventArgs e)
        {

            var l = sender as Line;
            if (l != null)
            {
                l.Stroke = Brushes.Green;
            }

        }
        private void Line_MouseLeave(object sender, MouseEventArgs e)
        {
            var l = sender as Line;
            if (l != null)
            {
                l.Stroke = Brushes.Red;
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {

        }

        private void zoom(object sender, RoutedEventArgs e)
        {
            ScaleTransform t = new ScaleTransform
            {
                ScaleX = 0.7,
                ScaleY = 0.7,

            };
            Transform a = t;
            //var ang=new RotateTransform(40);
            //a = ang;

            mg.RenderTransform = a;
         

        }

        private void StartMinusPoly_Click(object sender, RoutedEventArgs e)
        {
            active = new MyPolygon(-1);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Point p=new Point(0,0);
            foreach (var child in ug.Children)
            {
                var i = (Image) child;
                i.Source = TileGenerator.GetTileImage(4, (int) p.X, (int) p.Y);

                NextImage(ref p);
            }
        }

       static void NextImage(ref Point p)
        {
           if (p.X < 4)
           {
               p.Offset(1, 0);
           }
           else
           {
               p.X = 0;
               p.Y +=   1;
           }
        }

        private void Mg_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomFactor = e.Delta/240D;

            foreach (var myPolygon in list)
            {
                myPolygon.Zoom(zoomFactor);
            }
        }
    }
}
