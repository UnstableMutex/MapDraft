using System;
using System.Collections.Generic;
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

namespace WpfApplication1
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

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
           //     byte zoomFactor = 2;
           //var mouse = new Point(50,50);
           //Rect viewPort=new Rect(0,0,100,100);

           // var offset =-1* (mouse - viewPort.TopLeft)*zoomFactor;
            var l = Math.Log(8, 2);
            // var newTopleft= 
            // * zoomFactor;
        }
    }
}
