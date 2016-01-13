namespace CanvasMap
{
    internal struct TileID
    {
        internal byte Zoom;
        internal TilePosition Pos;

        public override string ToString()
        {
            return Pos + "z:" + Zoom;
        }
    }
}