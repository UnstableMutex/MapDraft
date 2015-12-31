using System;
using System.Windows.Media;

namespace RectangesZoom3
{
    class Constants
    {
        public const int TileSize = 256;
        public static Brush NegativeBrush;
        const string nbColor = "FFFDC0C0";
       static Func<string,byte> colorParser = s=>byte.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
        static Constants()
        {
           // 

            var a = colorParser(nbColor.Substring(0, 2));
            var r= colorParser(nbColor.Substring(2, 2));
            var g = colorParser(nbColor.Substring(4, 2));
            var b= colorParser(nbColor.Substring(6, 2));
            NegativeBrush = new SolidColorBrush(Color.FromArgb(a,r,g,b));
        }
    }
}