using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectanglesZoom
{
    class DownTilesContainer
    {
        public TilePyramid UpLeft
        {
            get;
            set;
        }
        public TilePyramid DownLeft
        {
            get;
            set;
        }
        public TilePyramid UpRight
        {
            get;
            set;
        }
        public TilePyramid DownRight
        {
            get;
            set;
        }

    }
}
