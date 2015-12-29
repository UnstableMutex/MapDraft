using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyOwnMap
{
    class Tile : Image
    {

        private readonly TileIndex _tilePosition;
        private readonly ushort _zoom;

        public Tile(TileIndex tilePosition, ushort zoom)
        {
            _tilePosition = tilePosition;
            _zoom = zoom;
            Width = Constants.tileSize;
            Height =  Constants.tileSize;

        }

        public async Task Upload()
        {

            try
            {
                var imagesource = await MyImageDownloaderAsync.GetImage((byte)_zoom, _tilePosition.X,_tilePosition.Y);
                this.Source = imagesource;
            }
            catch (FileNotFoundException)
            {


            }
        }

        public ushort Y
        {
            get { return _tilePosition.Y; }
        }

        public ushort X
        {
            get { return _tilePosition.X; }
        }
    }
}
