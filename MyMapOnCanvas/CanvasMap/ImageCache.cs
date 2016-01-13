using System.Collections.Generic;
using System.Windows.Media;

namespace CanvasMap
{
    internal static class ImageCache
    {
        private static Dictionary<TileID, ImageSource> dic = new Dictionary<TileID, ImageSource>();

        internal static ImageSource GetImage(TileID tid)
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