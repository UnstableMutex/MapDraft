using System.Windows;

namespace CanvasMap
{
    internal interface IZoomItems
    {
        void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse);

        void Click(Point mouse);
    }
}