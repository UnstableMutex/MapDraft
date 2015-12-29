using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RectanglesZoom
{
    static class ClipRects
    {

        public static Rects GetClipRectIndex(IEnumerable<Tile> tiles, int oldZoom)
        {
            var cri = GetClipIndex(tiles);


            Rects res = new Rects(oldZoom);
            res.IndexRect = cri;





            var rect = new Rect();
            rect = GetCoordsRect(tiles);
            res.Position = rect;

            res.TileSize = tiles.First().Width;

            return res;

        }


        private static Rect GetCoordsRect(IEnumerable<Tile> tiles)
        {
            var lefts = tiles.Select(x => Canvas.GetLeft(x)).ToArray();
            var left = lefts.Min();

            //  var right = tiles.Max(t => Canvas.GetLeft(t) + t.ActualWidth);

            var rights = tiles.Select(t => Canvas.GetLeft(t) + t.Width).ToArray();
            var right = rights.Max();
            var tops = tiles.Select(x => Canvas.GetTop(x)).ToArray();
            var top = tops.Min();

            //  var right = tiles.Max(t => Canvas.GetLeft(t) + t.ActualWidth);

            var bottoms = tiles.Select(t => Canvas.GetTop(t) + t.Height).ToArray();
            var bottom = bottoms.Max();
            return new Rect(new Point(left, top), new Point(right, bottom));
        }

        /// <summary>
        /// получает углы прямоугольника (индексы тайлов) в координатах старого зума
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        private static ClipRectIndex GetClipIndex(IEnumerable<Tile> tiles)
        {
            var tops = tiles.Select(x => x.Y).ToArray();
            var top = tops.Min();
            var bottoms = tiles.Select(t => t.Y).ToArray();
            var bottom = bottoms.Max();
            var lefts = tiles.Select(x => x.X).ToArray();
            var left = lefts.Min();
            var rights = tiles.Select(t => t.X).ToArray();
            var right = rights.Max();
            var cri = new ClipRectIndex();
            cri.Left = left;
            cri.Top = top;
            cri.Bottom = bottom;
            cri.Right = right;
            return cri;
        }

   public static Rects GetClipRectIndexNewZoom(IEnumerable<Tile> tiles, int Zoom)
        {
            var cri = GetClipIndex(tiles);
       //получим индексы прямоугольников в индексах нового зума




            Rects res = new Rects(Zoom);
            res.IndexRect = cri;





            var rect = new Rect();
            rect = GetCoordsRect(tiles);
            res.Position = rect;

            res.TileSize = tiles.First().Width;

            return res;

        }



        private Rects GetClipRectNewZoom(IEnumerable<Tile> tiles, int newZoom)
        {
            var res=new Rects(newZoom);



            return res;
        }
    }
}
