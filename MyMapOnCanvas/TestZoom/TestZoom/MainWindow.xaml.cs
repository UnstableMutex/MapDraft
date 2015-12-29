using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TestZoom
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

        private float zoomSpeedC = 0.5f;
        private Vector GetOffSet(Point mouse, Rect rect, int wheel)
        {
            wheel = wheel * -1;
            float scale = 0;

            if (wheel > 0)
            {
                //увеличение
                scale = 1 / zoomSpeedC;
            }
            if (wheel < 0)
            {
                //уменьшение
                scale = zoomSpeedC;
            }
            float vectorMultiplier = (1 - scale);
            var deltamX = mouse.X - rect.X;
            var deltamY = mouse.Y - rect.Y;
            var v = new Vector(deltamX * vectorMultiplier, deltamY * vectorMultiplier);
            return v;
        }






        private void C_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mouse = e.GetPosition(c);

            var imr = new ImageMoveResize(zoomSpeedC);

            var rect = new Rect(new Point(Canvas.GetLeft(r), Canvas.GetTop(r)),
                        new Size(r.ActualWidth, r.ActualHeight));



            var v = imr.GetOffSet(mouse, rect, e.Delta);
            rect.Offset(v);
            rect.Size = imr.GetNewSize(rect.Size, e.Delta);
            r.Width = rect.Width;
            r.Height = rect.Height;

            Canvas.SetLeft(r, rect.Left);
            Canvas.SetTop(r, rect.Top);

        }

    }
}
