using CanvasMap;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Location = CanvasMap.Location;

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
            this.DataContext = new VM();
        }
    }
}