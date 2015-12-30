using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RectangesZoom3
{
    class Tile :Grid
    {

        private readonly TilePosition _tilePosition;
        private readonly ushort _zoom;

        public Tile(TilePosition tilePosition, ushort zoom)
        {
            _tilePosition = tilePosition;
            _zoom = zoom;
            Width = Constants.TileSize;
            Height = Constants.TileSize;

        }

        public async Task Upload()
        {

            try
            {
                var imagesource = await MyImageDownloaderAsync.GetImage((byte)_zoom, _tilePosition.X, _tilePosition.Y);

                //this.Source = imagesource;
                this.Children.Add(new Image() {Source = imagesource});
                this.Children.Add(new TextBlock() {Text = string.Format("{0} {1}", _tilePosition.X, _tilePosition.Y)});
                Debug.Print("SETSOURCE");
            }
            catch (FileNotFoundException)
            {
                Debug.Print("ERR");

            }
        }

        public int Y
        {
            get { return _tilePosition.Y; }
        }

        public int X
        {
            get { return _tilePosition.X; }
        }

        public void SetImage()
        {
            Upload();
        }
    }
}
