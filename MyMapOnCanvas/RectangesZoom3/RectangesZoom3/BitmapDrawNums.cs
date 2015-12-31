using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RectangesZoom3
{
    static class BitmapDrawNums
    {
        static Font f = new Font("Times New Roman", 20);
        private static Brush b = Brushes.Black;

        public static BitmapSource DrawNums(BitmapSource bs, int x, int y, int zoom)
        {
            ;
            using (var bm = BitmapFromSource(bs))
            using (var g = Graphics.FromImage(bm))
            {
                var s = string.Format("z{0} x{1} y{2}", zoom, x, y);
                g.DrawString(s, f, b, 1, 1);
                return ConvertBitmap(bm);
            }
        }


        static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
    }
}