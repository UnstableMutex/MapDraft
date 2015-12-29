using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScaleTransforms
{
    class Tile : Image
    {
        private readonly byte _zoom;
        private readonly int _y;
        private readonly int _x;
        static Random r = new Random(DateTime.Now.Millisecond);
        const string folder = @"C:\Users\ShevyakovDY\Desktop\pics\{0}.png";


        public Tile(byte zoom, int x, int y)
        {
            _zoom = zoom;
            _y = y;
            _x = x;
            Width = Constants.TileSize;
            Height = Constants.TileSize ;

            //  var path = string.Format(folder, r.Next(1, 5));

            SetSingleImage();
            //  SetPosition();
        }
        public ScaleTransform Scale { get; set; }
      
     
        
       

       

        public int Y
        {
            get { return _y; }
        }

        public int X
        {
            get { return _x; }
        }


        async void SetSingleImage()
        {
            BitmapImage bi = null;

            ImageSource bi1 = await MyImageDownloaderAsync.GetImage(_zoom, (uint)_x, (uint)_y);
            bi = bi1 as BitmapImage;
            Source = bi;
          






        }


     
    }

    class ImageMoveResize
    {
        private readonly float _zoomSpeed;

        public ImageMoveResize(float zoomSpeed)
        {
            _zoomSpeed = zoomSpeed;
        }

        float GetScale(int wheel)
        {
            wheel = wheel > 0 ? -1 : 1;
            if (wheel > 0)
            {
                //увеличение
                return 1 / _zoomSpeed;
            }
            if (wheel < 0)
            {
                //уменьшение
                return _zoomSpeed;
            }
            throw new NotImplementedException();
        }

        float GetVectorMultiplier(int wheel)
        {
            return 1 - GetScale(wheel);
        }
        /// <summary>
        /// получает новы размер прямоугольника
        /// </summary>
        /// <param name="size"></param>
        /// <param name="wheel"></param>
        /// <returns></returns>
        public Size GetNewSize(Size size, int wheel)
        {
            var s = this.GetScale(wheel);
            return new Size(size.Width * s, size.Height * s);
        }

        /// <summary>
        /// получает вектор на который будем двигать прямоугольник с учетом позиции мыши
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="rect"></param>
        /// <param name="wheel"></param>
        /// <returns></returns>
        public Vector GetOffSet(Point mouse, Rect rect, int wheel)
        {


            float vectorMultiplier = GetVectorMultiplier(wheel);
            var deltamX = mouse.X - rect.X;
            var deltamY = mouse.Y - rect.Y;
            var v = new Vector(deltamX * vectorMultiplier, deltamY * vectorMultiplier);
            return v;
        }


    }
    static class MyImageDownloaderAsync
    {
        static MyImageDownloaderAsync()
        {
            CacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapCache");
        }

        private static readonly string CacheFolder;
        const string urlTemplate = @"http://tile.openstreetmap.org/{0}/{1}/{2}.png";
        public static async Task<ImageSource> GetImage(byte zoom, uint x, uint y)
        {
            var max = Math.Pow(2, zoom);
            if (x > max - 1 | y > max - 1)
            {
                throw new FileNotFoundException();
            }



            var cachename = Path.Combine(CacheFolder, zoom.ToString(), x.ToString(), y.ToString() + ".png");
            var url = string.Format(urlTemplate, zoom.ToString(), x.ToString(), y.ToString());




            if (File.Exists(cachename))
            {
                FileStream file = null;
                try
                {
                    file = File.OpenRead(cachename);
                    return GetImageFromStream(file);
                }
                catch (NotSupportedException) // Problem creating the bitmap (file corrupt?)
                {
                }
                catch (IOException) // Or a prolbem opening the file. We'll try to re-download the file.
                {
                }
                finally
                {
                    if (file != null)
                    {
                        file.Dispose();
                    }
                }
            }

            MemoryStream buffer = null;
            try
            {
                // First download the image to our memory.
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Proxy = WebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                buffer = new MemoryStream();
                using (var response = request.GetResponse())
                {
                    var stream = response.GetResponseStream();
                    stream.CopyTo(buffer);
                    stream.Close();
                }

                // Then save a copy for future reference, making sure to rewind
                // the stream to the start.
                buffer.Position = 0;
                SaveCacheImage(buffer, cachename);

                // Finally turn the memory into a beautiful picture.
                buffer.Position = 0;
                return GetImageFromStream(buffer);
            }
            catch (WebException)
            {
                // RaiseDownloadError();
            }
            catch (NotSupportedException) // Problem creating the bitmap (messed up download?)
            {
                //RaiseDownloadError();
            }
            finally
            {
                // EndDownload();
                if (buffer != null)
                {
                    buffer.Dispose();
                }
            }
            return null;








        }
        private static void SaveCacheImage(Stream stream, string uri)
        {
            string path = uri;
            FileStream file = null;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                file = File.Create(path);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(stream));
                encoder.Save(file);
            }
            catch (IOException) // Couldn't save the file
            {
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }
        private static BitmapImage GetImageFromStream(Stream stream)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            bitmap.Freeze(); // Very important - lets us download in one thread and pass it back to the UI
            return bitmap;
        }
        private static bool FileExists(string cachename)
        {
            try
            {
                return File.Exists(cachename);
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
