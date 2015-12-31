namespace RectangesZoom3
{
    struct TileID
    {
        public byte Zoom;
        public TilePosition Pos;

        public override string ToString()
        {
            return Pos.ToString() + "z:" + Zoom;
        }
    }
}