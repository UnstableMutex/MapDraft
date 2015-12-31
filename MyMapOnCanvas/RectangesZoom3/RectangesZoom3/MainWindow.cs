using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace RectangesZoom3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test();
        }
        const double maxY = 34619289.3718563;
        private void Test()
        {
        
            //у для самары 6453590....
            var samlat = 50.231435;
            var c = MercatorOSM.latToY(samlat);

            var percentright = (c + maxY)/(2*maxY);


            var lat = MercatorOSM.yToLat(c);
            //var ie = Enumerable.Range(-30000, 60000);
            //foreach (var i in ie)
            //{
            //    var res = MercatorOSM.yToLat(i);
            //    Debug.Print("res: {0}: lat: {1}",res,i);
            //}

        }

        private void Startrgn(object sender, RoutedEventArgs e)
        {
            map.StartRegion();
        }

        private void Endrgn(object sender, RoutedEventArgs e)
        {
            map.EndRegion();
        }
    }
}