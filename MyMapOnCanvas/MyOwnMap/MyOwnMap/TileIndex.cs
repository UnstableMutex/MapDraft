using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnMap
{
    struct TileIndex
    {
        public ushort X;
        public ushort Y;
    }

    struct RectIndex
    {
        public ushort minX;
        public ushort minY;
        public ushort maxX;
        public ushort maxY;
        public override string ToString()
        {
            return minX + " " + minY + " " + maxX + " " + maxY;
        }
    }
}
