using System.IO;
using System.Windows.Controls;

namespace CanvasMap
{
    internal class Tile : Grid
    {
        private readonly TileID _tid;

        public Tile(TileID tid)
        {
            _tid = tid;
            Width = Constants.TileSize;
            Height = Constants.TileSize;
        }

        public int Y
        {
            get { return _tid.Pos.Y; }
        }

        public int X
        {
            get { return _tid.Pos.X; }
        }

        public byte Zoom
        {
            get { return _tid.Zoom; }
        }

        public void Upload()
        {
            try
            {
                var imagesource = ImageCache.GetImage(_tid);

                //this.Source = imagesource;
                Children.Add(new Image { Source = imagesource });
                Children.Add(new TextBlock { Text = string.Format("{0} {1}", _tid.Pos.X, _tid.Pos.Y) });
            }
            catch (FileNotFoundException)
            {
            }
        }

        public void SetImage()
        {
            Upload();
        }
    }
}