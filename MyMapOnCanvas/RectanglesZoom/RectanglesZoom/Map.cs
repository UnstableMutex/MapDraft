using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RectanglesZoom
{
    class Map : MapBase
    {
        public Map()
        {
            Background = Brushes.Transparent;
        }

        public override void EndInit()
        {
            base.EndInit();
        }

        protected override void OnDragMap(Vector v)
        {
            var images = this.Children.OfType<Tile>();

            List<Tile> toremove = new List<Tile>();

            foreach (var r in images)
            {
                r.SimpleMove(v);
                //r.MoveOnWheel(e.Delta, mouse);
                //r.ResizeOnWheel(e.Delta);
                /*передвинули картинки теперь можно 
                           * определить какие не видно и убрать*/
                var top = Canvas.GetTop(r);
                var left = Canvas.GetLeft(r);
                var needRemove = (left > this.ActualWidth) | (top > this.ActualHeight)
                                 | (left + r.ActualWidth < 0) | (top + r.ActualHeight < 0);
                if (needRemove)
                {
                    toremove.Add(r);
                }
            }

            foreach (var tile in toremove)
            {
                Children.Remove(tile);
            }






            RebuildWhiteZoneOnMove(CurrentZoomLevel);
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            StartFill();
        }

        private void StartFill()
        {
            byte level = CurrentZoomLevel;
            Debug.Print(CurrentZoomLevel.ToString());
            var gridSize = new GridSize(this);
            for (int hi = 0; hi < gridSize.HCount; hi++)
                for (int vi = 0; vi < gridSize.VCount; vi++)
                {
                    Tile t = new Tile(level, hi, vi);
                    t.SetSize(Constants.TileSize);
                    this.Children.Add(t);
                    t.SetPosition();
                }
        }


        private byte CurrentZoomLevel = 2;

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {

            var newzoom = CurrentZoomLevel;
            if (e.Delta < 0)
            {
                newzoom++;
            }
            if (e.Delta > 0)
            {
                newzoom--;
            }
            if (newzoom <= 0 | newzoom > 14)
            {
                return;

            }





            Debug.Print(CurrentZoomLevel.ToString());


            base.OnMouseWheel(e);

            MoveExistsTilesAndDeleteUnnessesary(e);

            //далее надо заполнить белое место
            //определяем прямоугольник с тайлами
            //  Rect rect=new Rect(new Point(double.MaxValue,double.MaxValue),new Point(double.MinValue,double.MinValue));


            RebuildWhiteZoneOnWheel(newzoom, CurrentZoomLevel);
            CurrentZoomLevel = newzoom;
        }
        #region OnMoveMap

        private void RebuildWhiteZoneOnMove(int currentZoom)
        {
            //BuildExtraleftLinesIfNessesaryOnMove();
            //BuildExtratopLinesIfNessesaryOnMove();
            //BuildExtraRightLinesIfNessaryOnMove();
            //BuildExtraBottomLinesIfNessaryOnMove();
        }


        private void BuildExtraleftLinesIfNessesaryOnMove()
        {
            var tiles = Children.OfType<Tile>();
            var cr = ClipRects.GetClipRectIndex(tiles, CurrentZoomLevel);

            while (cr.Position.Left > 0)
            {
                if (cr.Position.Left > 0) // & mintileX > 0)
                {
                    //надо добавить колонку слева
                    //определяем количество тайлов по вертикали
                    var vcount = (cr.Position.Height) / cr.TileSize;
                    int level = (int)(1 / Constants.Zoom);

                    for (int i = 0; i < vcount; i++)
                    {
                        Tile newTile = new Tile(CurrentZoomLevel, cr.IndexRect.Left - 1, cr.IndexRect.Top + i);
                        this.Children.Add(newTile);
                        Canvas.SetLeft(newTile, cr.Position.Left - cr.TileSize);
                        Canvas.SetTop(newTile, cr.Position.Top + i * cr.TileSize);
                        newTile.SetSize(cr.TileSize);
                    }
                }
                tiles = Children.OfType<Tile>();
                cr = ClipRects.GetClipRectIndex(tiles, CurrentZoomLevel);
            }
        }


        private void BuildExtraBottomLinesIfNessaryOnMove()
        {
            var tiles = Children.OfType<Tile>().ToArray();
            var bottom = tiles.Max(x => Canvas.GetTop(x) + x.Height);

            while (bottom < ActualHeight)
            {
                var lefts = tiles.Select(x => Canvas.GetLeft(x)).ToArray();
                var left = lefts.Min();

                //  var right = tiles.Max(t => Canvas.GetLeft(t) + t.ActualWidth);

                var rights = tiles.Select(t => Canvas.GetLeft(t) + t.Width).ToArray();
                var right = rights.Max();


                var mintileX = tiles.Min(t => t.X);
                var tilewidth = tiles.First().Width;
                var tileheight = tiles.First().Height;
                var maxtileY = tiles.Max(t => t.Y);


                if (bottom < ActualHeight) // & mintileX > 0)
                {
                    //надо добавить колонку сверху


                    //определяем количество тайлов по горизонтали

                    var vcount = (right - left) / tilewidth;
                    int level = (int)(1 / Constants.Zoom);

                    for (int i = 0; i < vcount; i++)
                    {
                        Tile newTile = new Tile(CurrentZoomLevel, mintileX + i, maxtileY + 1);
                        this.Children.Add(newTile);
                        Canvas.SetLeft(newTile, left + i * tilewidth);
                        Canvas.SetTop(newTile, bottom);
                        newTile.SetSize(tilewidth);
                    }
                }
                tiles = Children.OfType<Tile>().ToArray();
                bottom = tiles.Max(x => Canvas.GetTop(x) + x.Height);
            }
        }

        private void BuildExtraRightLinesIfNessaryOnMove()
        {
            var tiles = Children.OfType<Tile>();
            var right = tiles.Max(x => Canvas.GetLeft(x) + x.Width);

            while (right < this.ActualWidth)
            {
                var bottoms = tiles.Select(x => Canvas.GetTop(x) + x.Height).ToArray();
                var bottom = bottoms.Max();
                var tops = tiles.Select(x => Canvas.GetTop(x)).ToArray();
                var top = tops.Min();

                var maxTileX = tiles.Max(t => t.X);
                var tilewidth = tiles.First().Width;
                var tileheight = tiles.First().Height;
                var mintileY = tiles.Min(t => t.Y);


                if (right < ActualWidth) // & mintileX > 0)
                {
                    //надо добавить колонку справа


                    //определяем количество тайлов по вертикали

                    var vcount = (bottom - top) / tileheight;
                    int level = (int)(1 / Constants.Zoom);

                    for (int i = 0; i < vcount; i++)
                    {
                        Tile newTile = new Tile(CurrentZoomLevel, maxTileX + 1, mintileY + i);
                        this.Children.Add(newTile);
                        Canvas.SetLeft(newTile, right);
                        Canvas.SetTop(newTile, top + i * tileheight);
                        newTile.SetSize(tilewidth);
                    }
                }
                tiles = Children.OfType<Tile>();
                right = tiles.Max(x => Canvas.GetLeft(x) + x.Width);
            }
        }

        private void BuildExtratopLinesIfNessesaryOnMove()
        {
            var tiles = Children.OfType<Tile>();

            var top = tiles.Min(x => Canvas.GetTop(x));

            while (top > 0)
            {
                var lefts = tiles.Select(x => Canvas.GetLeft(x)).ToArray();
                var left = lefts.Min();
                var rights = tiles.Select(t => Canvas.GetLeft(t) + t.Width).ToArray();
                var right = rights.Max();
                var mintileX = tiles.Min(t => t.X);
                var tilewidth = tiles.First().Width;
                var tileheight = tiles.First().Height;
                var mintileY = tiles.Min(t => t.Y);


                if (top > 0) // & mintileX > 0)
                {
                    //надо добавить колонку сверху
                    //определяем количество тайлов по горизонтали

                    var vcount = (right - left) / tilewidth;
                    int level = (int)(1 / Constants.Zoom);

                    for (int i = 0; i < vcount; i++)
                    {
                        Tile newTile = new Tile(CurrentZoomLevel, mintileX + i, mintileY - 1);
                        this.Children.Add(newTile);

                        //закончил перепиывать с вертикали здесь
                        Canvas.SetLeft(newTile, left + i * tilewidth);
                        Canvas.SetTop(newTile, top - tileheight);
                        newTile.SetSize(tilewidth);
                    }
                }
                tiles = Children.OfType<Tile>();
                top = tiles.Min(x => Canvas.GetTop(x));
            }
        }



        #endregion



        private void RebuildWhiteZoneOnWheel(int newZoom, int oldZoom)
        {
            BuildExtraleftLinesIfNessesaryOnWheel(newZoom, oldZoom);
            BuildExtratopLinesIfNessesaryOnWheel();
            BuildExtraRightLinesIfNessaryOnWheel();
            BuildExtraBottomLinesIfNessaryOnWheel();
        }
        private void BuildExtraleftLinesIfNessesaryOnWheel(int newZoom, int oldZoom)
        {



            var tiles = Children.OfType<Tile>();

            var cr = ClipRects.GetClipRectIndex(tiles, oldZoom);
            if (cr.IndexRect.Left == 0)
            {
                return;
            }
            double newTileSizePositionX = 0;
            double newTileSizePositionY = 0;
            if (cr.IndexRect.Left % 2 == 0)
            {
                //если минимальный X старый индекс четный
                newTileSizePositionX = cr.Position.Left - cr.TileSize * 2;
            }
            else
            { //если минимальный X старый индекс не четный
                newTileSizePositionX = cr.Position.Left - cr.TileSize;
            }
            if (cr.IndexRect.Top % 2 == 0)
            {
                //если минимальный Y старый индекс четный
                newTileSizePositionY = cr.Position.Top;
            }
            else
            { //если минимальный Y старый индекс не четный
                newTileSizePositionY = cr.Position.Top - cr.TileSize;
            }


            int tileX = 0;
            if (cr.IndexRect.Left%2 == 0)
            {
              tileX=  (int) cr.IndexRect.Left/2 - 1;
            }
            else
            {
                tileX = (int) cr.IndexRect.Left/2;
            }
            double tileBottom = newTileSizePositionY;

            int tileYIndex = cr.IndexRect.Top/2;
            while (tileBottom < this.ActualHeight)
            {

                Tile tile = new Tile((byte)newZoom, tileX,tileYIndex++);
                tile.Width = Constants.TileSize;
                tile.Height = Constants.TileSize;
                this.Children.Add(tile);
                Canvas.SetLeft(tile, newTileSizePositionX);
                Canvas.SetTop(tile, tileBottom);
                tileBottom += Constants.TileSize;

            }






        }
        private void BuildExtraBottomLinesIfNessaryOnWheel()
        {

        }

        private void BuildExtraRightLinesIfNessaryOnWheel()
        {

        }

        private void BuildExtratopLinesIfNessesaryOnWheel()
        {

        }




        private void MoveExistsTilesAndDeleteUnnessesary(MouseWheelEventArgs e)
        {
            var images = this.Children.OfType<Tile>();
            var mouse = e.GetPosition(this);
            List<Tile> toremove = new List<Tile>();

            foreach (var r in images)
            {
                r.MoveOnWheel(e.Delta, mouse);
                r.ResizeOnWheel(e.Delta);
                /*передвинули картинки теперь можно 
                           * определить какие не видно и убрать*/
                var top = Canvas.GetTop(r);
                var left = Canvas.GetLeft(r);
                var needRemove = (left > this.ActualWidth) | (top > this.ActualHeight)
                                 | (left + r.ActualWidth < 0) | (top + r.ActualHeight < 0);
                if (needRemove)
                {
                    toremove.Add(r);
                }
            }

            foreach (var tile in toremove)
            {
                Children.Remove(tile);
            }
        }
    }
}