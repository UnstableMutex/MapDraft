using System.Windows;

namespace RectangesZoom3
{
    internal interface IZoomItems
    {
        void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom);
        void Click(Point mouse);
    }
}