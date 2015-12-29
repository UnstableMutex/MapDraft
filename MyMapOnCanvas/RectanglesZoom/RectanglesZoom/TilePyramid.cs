using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectanglesZoom
{
    class TilePyramid
    {
        private readonly byte _zoom;
        private readonly int _x;
        private readonly int _y;
        private readonly TilePyramid _upperTile;

        public TilePyramid(byte zoom,int x,int y, TilePyramid upperTile=null)
        {
            _zoom = zoom;
            _x = x;
            _y = y;
            _upperTile = upperTile;
        }

        public Tile GetImage()
        {
            return new Tile(_zoom,_x,_y);
        }

        DownTilesContainer GetDownTiles()
        {
            DownTilesContainer c=new DownTilesContainer();
            byte lowZoom = (byte)(_zoom + 1);
          
            c.UpLeft =new TilePyramid(lowZoom,_x*2,_y*2,this);
            c.UpRight = new TilePyramid(lowZoom,_x*2+1,_y*2,this);
            c.DownLeft = new TilePyramid(lowZoom,_x*2,_y*2+1,this);
            c.DownRight = new TilePyramid(lowZoom,_x*2+1,_y*2+1,this);
            return c;

        }
       
    }
}
