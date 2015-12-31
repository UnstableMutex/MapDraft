namespace RectangesZoom3
{
    struct TilePosition
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return string.Format("x:{0} y:{1}", X, Y);
        }
    }
}