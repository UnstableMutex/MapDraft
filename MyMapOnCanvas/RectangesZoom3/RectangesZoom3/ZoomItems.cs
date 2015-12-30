using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RectangesZoom3
{
    class ZoomItems
    {
        private readonly byte _zoom;
        private readonly Map _map;
        private bool _activeLayer;
        public ZoomItems(byte zoom, Map map)
        {
            _zoom = zoom;
            _map = map;
        }


        public void OnViewPortChange(Rect oldvp, Rect newvp)
        {





        }
        public byte Zoom
        {
            get { return _zoom; }
        }

        ImageSource GetImage(int x, int y)
        {
            return MyImageDownloaderAsync.GetImageS(_zoom, x, y);
        }

        ImageSource GetImage(TilePosition tp)
        {
            return MyImageDownloaderAsync.GetImageS(_zoom, tp.X, tp.Y);
        }

        internal void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom)
        {
            //var oldmulti = oldvp.Width/Constants.TileSize;
            //var oldzoom = Math.Log(oldmulti,2) - 1;
            //var newZoom = Math.Log(oldvp.Height/Constants.TileSize, 2) - 1;

            if (Zoom == currentZoom)
            {
                //���� ��� �������� ���� ������ �����
                foreach (var tile in _map.Tiles)
                {
                    _map.Children.Remove(tile);
                }

            }
            if (Zoom == newZoom)
            {
                AddTiles(newvp);
            }

        }

        public void AddInitial()
        {
            var rect = new Rect(0, 0, _map.ActualWidth, _map.ActualHeight);
            AddTiles(rect);
        }
        private void AddTiles(Rect newvp)
        {
            //���� ��� ����� �� �������� �����
            double vpX = newvp.X;


            var firstTileXIndex = (int)Math.Ceiling(vpX / Constants.TileSize);
            var firstTileYIndex = (int)Math.Ceiling(newvp.Y / Constants.TileSize);
            var coordX = vpX - firstTileXIndex * Constants.TileSize;
            var coordY = newvp.Y - firstTileYIndex * Constants.TileSize;

            // var image = GetImage(firstTileXIndex, firstTileYIndex);
            int currentXIndex = 0;
            int currentYIndex = 0;
            var currentCoordX = coordX;
            var currentCoordY = coordY;

            do
            {
                do
                {
                    var tp = new TilePosition();
                    tp.X = currentXIndex - firstTileXIndex;
                    tp.Y = currentYIndex - firstTileYIndex;

                    var image = GetImage(tp);
                    Tile tile = new Tile(tp, Zoom);
                    tile.Source = image;
                    _map.Children.Add(tile);
                    Canvas.SetLeft(tile, currentCoordX);
                    Canvas.SetTop(tile, currentCoordY);
                    currentCoordX += Constants.TileSize;
                    currentXIndex++;
                    var contX = MyImageDownloaderAsync.IsIndexCorrect(currentXIndex, Zoom);
                    if (!contX)
                    {
                        break;
                    }

                } while (currentCoordX <= newvp.Width);
                currentCoordY += Constants.TileSize;
                currentYIndex++;
                currentCoordX = coordX;
                currentXIndex = 0;
                var contY = MyImageDownloaderAsync.IsIndexCorrect(currentYIndex, Zoom);
                if (!contY)
                { break;
                    
                }
            } while (currentCoordY <= newvp.Height);
        }
    }
}