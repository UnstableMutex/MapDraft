using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectangesZoom3
{
    static class MyImageDownloaderAsync
    {
        static MyImageDownloaderAsync()
        {
            CacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapCache");
        }


        private static readonly string CacheFolder;
        const string urlTemplate = @"http://tile.openstreetmap.org/{0}/{1}/{2}.png";

        public static async Task<ImageSource> GetImage(TileID tid)
        {
            return await GetImage(tid.Zoom, tid.Pos.X, tid.Pos.Y);
        }

        static async Task<ImageSource> GetImage(byte zoom, int x, int y)
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
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                buffer = new MemoryStream();
                using (var response = await request.GetResponseAsync())
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

        public static bool IsIndexCorrect(int index, byte zoom)
        {
            var max = Math.Pow(2, zoom);
            return !(index > max - 1);
            //if (x > max - 1 | y > max - 1)
            //{   
        }

        public static ImageSource GetImageS(TileID tid)
        {
            Debug.Print("query image {0}", tid);
            var max = Math.Pow(2, tid.Zoom);
            if (!(IsIndexCorrect(tid.Pos.X, tid.Zoom) & IsIndexCorrect(tid.Pos.Y, tid.Zoom)))
            {
                throw new TileIndexOutOfRangeException();
            }
            var zoom = tid.Zoom;
            var x = tid.Pos.X;
            var y = tid.Pos.Y;
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
                var request = (HttpWebRequest) WebRequest.Create(url);
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
    }
}