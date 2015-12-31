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