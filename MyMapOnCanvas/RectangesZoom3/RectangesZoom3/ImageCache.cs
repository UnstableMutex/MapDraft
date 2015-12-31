using System.Collections.Generic;
using System.Windows.Media;

namespace RectangesZoom3
{
    static class ImageCache
    {
        static Dictionary<TileID, ImageSource> dic = new Dictionary<TileID, ImageSource>();

        public static ImageSource GetImage(TileID tid)
        {
            if (!dic.ContainsKey(tid))
            {
                var image = MyImageDownloaderAsync.GetImageS(tid);
                dic.Add(tid, image);
            }
            return dic[tid];
        }
    }
}