using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CanvasMap
{
    internal class ZoomOverlay : OverlayBase
    {
        private readonly byte _zoom;

        public ZoomOverlay(byte zoom, MapMiddle map)
            : base(map)
        {
            _zoom = zoom;
        }

        public byte Zoom
        {
            get { return _zoom; }
        }

        public override void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse)
        {
            //var oldmulti = oldvp.Width/Constants.TileSize;
            //var oldzoom = Math.Log(oldmulti,2) - 1;
            //var newZoom = Math.Log(oldvp.Height/Constants.TileSize, 2) - 1;
            if (Zoom == currentZoom)
            {
                //если зум усттарел надо убрать тайлы
                var tilestoremove = _map.Tiles.Where(t => t.Zoom == Zoom).ToList();
                foreach (var tile in tilestoremove)
                {
                    _map.Children.Remove(tile);
                }
            }
            if (Zoom == newZoom)
            {
                AddTiles(newvp);
            }
        }

        public override void Click(Point mouse)
        {
        }

        public void AddInitial()
        {
            var rect = new Rect(0, 0, _map.ActualWidth, _map.ActualHeight);
            AddTiles(rect);
        }

        private void AddTiles(Rect newvp)
        {
            //если зум новый то добавить тайлы
            double vpX = newvp.X;
            var firstTileXIndex = (int)Math.Floor(-1 * vpX / Constants.TileSize);
            var firstTileYIndex = (int)Math.Floor(-1 * newvp.Y / Constants.TileSize);
            var coordX = vpX + firstTileXIndex * Constants.TileSize;
            var coordY = newvp.Y + firstTileYIndex * Constants.TileSize;

            while (firstTileYIndex < 0)
            {
                firstTileYIndex++;
                coordY += Constants.TileSize;
            }
            while (firstTileXIndex < 0)
            {
                firstTileXIndex++;
                coordX += Constants.TileSize;
            }

            int currentXIndex = 0;
            int currentYIndex = 0;
            var currentCoordX = coordX;
            var currentCoordY = coordY;
#if DEBUG
            int querypiccounter = 0;
#endif
            do
            {
                do
                {
                    var tp = new TilePosition
                    {
                        X = currentXIndex + firstTileXIndex,
                        Y = currentYIndex + firstTileYIndex
                    };
                    TileID tid = new TileID { Pos = tp, Zoom = Zoom };
                    Tile tile = new Tile(tid);
                    try
                    {
#if DEBUG

                        querypiccounter++;
#endif
                        tile.SetImage();
                    }
                    catch (TileIndexOutOfRangeException)
                    {
                    }
                    _map.Children.Add(tile);
                    Panel.SetZIndex(tile, 0);
                    Canvas.SetLeft(tile, currentCoordX);
                    Canvas.SetTop(tile, currentCoordY);
                    currentCoordX += Constants.TileSize;
                    currentXIndex++;
                    var contX = MyImageDownloaderAsync.IsIndexCorrect(currentXIndex, Zoom);
                    if (!contX)
                    {
                        break;
                    }
                } while (currentCoordX <= _map.ActualWidth);
                currentCoordY += Constants.TileSize;
                currentYIndex++;
                currentCoordX = coordX;
                currentXIndex = 0;
                var contY = MyImageDownloaderAsync.IsIndexCorrect(currentYIndex, Zoom);
                if (!contY)
                {
                    break;
                }
            } while (currentCoordY <= _map.ActualHeight);
#if DEBUG
            // Debug.Print("qpc {0}",querypiccounter);
#endif
        }
    }
}