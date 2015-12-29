using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace MapControl
{
    public static class YandexGeocoder
    {
        #region const, fields, constructor, properties
        const string REQUESRT_URL = "http://geocode-maps.yandex.ru/1.x/?geocode={0}&format=xml&results={1}&lang={2}";

        private static string _key;

        static YandexGeocoder()
        {
            _key = string.Empty;
        }

        /// <summary>
        /// Yandex Maps API-key (not necessarily)
        /// </summary>
        public static string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        #endregion


        #region Geocode
        /// <summary>
        /// Location determination by geo object name.
        /// </summary>
        /// <param name="location">Name of a geographic location.</param>
        /// <example>Geocode("Moscow");</example>
        /// <returns>Collection of found locations</returns>
        public static GeoObjectCollection Geocode(string location)
        {
            return Geocode(location, 10);
        }

        /// <summary>
        /// Location determination by name, indicating the quantity of objects to return.
        /// </summary>
        /// <param name="location">Name of a geographic location.</param>
        /// <param name="results">Maximum number of objects to return.</param>
        /// <example>Geocode("Moscow", 10);</example>
        /// <returns>Collection of found locations</returns>
        public static GeoObjectCollection Geocode(string location, short results)
        {
            return Geocode(location, results, LangType.en_US);
        }

        /// <summary>
        /// Location determination by name, indicating the quantity of objects to return and preference language.
        /// </summary>
        /// <param name="location">Name of a geographic location.</param>
        /// <param name="results">Maximum number of objects to return.</param>
        /// <param name="lang">Preference language for describing objects.</param>
        /// <example>Geocode("Moscow", 10, LangType.en_US);</example>
        /// <returns>Collection of found locations</returns>
        public static GeoObjectCollection Geocode(string location, short results, LangType lang)
        {
            string request_ulr =
                string.Format(REQUESRT_URL, StringMakeValid(location), results, LangTypeToStr(lang)) +
                (string.IsNullOrEmpty(_key) ? string.Empty : "&key=" + _key);

            return new GeoObjectCollection(DownloadString(request_ulr));   //new GeoObjectCollection(File.ReadAllText(@"C:\Users\***\Desktop\2.x.xml"));
        }

        /// <summary>
        /// Location determination by name, indicating the quantity of objects to return and preference language.
        /// Allows limit the search or affect the issuance result.
        /// </summary>
        /// <param name="location">Name of a geographic location.</param>
        /// <param name="results">Maximum number of objects to return.</param>
        /// <param name="lang">Preference language for describing objects.</param>
        /// <param name="search_area">Search geographical area, affects to issuance of results.</param>
        /// <param name="rspn">Allows limit the search (true) or affect the issuance result (false - default).</param>
        /// <returns>Collection of found locations</returns>
        public static GeoObjectCollection Geocode(string location, short results, LangType lang, SearchArea search_area, bool rspn = false)
        {
            string request_ulr =
                string.Format(REQUESRT_URL, StringMakeValid(location), results, LangTypeToStr(lang)) +
                string.Format("&ll={0}&spn={1}&rspn={2}", search_area.LongLat.ToString("{0},{1}"), search_area.Spread.ToString("{0},{1}"), rspn ? 1 : 0) +
                (string.IsNullOrEmpty(_key) ? string.Empty : "&key=" + _key);

            return new GeoObjectCollection(DownloadString(request_ulr));
        }
        #endregion


        // ToDo: back geocoding methods
        #region ReGeokode
        private static string ReGeocode(double _long, double _lat)
        {
            return "";
        }
        #endregion

        #region helpers
        // ToDo: encode
        private static string StringMakeValid(string location)
        {
            location = location.Replace(" ", "+").Replace("&", "").Replace("?", "");
            return location;
        }
        private static string LangTypeToStr(LangType lang)
        {
            switch (lang)
            {
                case LangType.ru_RU: return "ru-RU";
                case LangType.uk_UA: return "uk-UA";
                case LangType.be_BY: return "be-BY";
                case LangType.en_US: return "en-US";
                case LangType.en_BR: return "en-BR";
                case LangType.tr_TR: return "tr-TR";
                default: return "ru-RU";
            }
        }
        private static string DownloadString(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
                return reader.ReadToEnd();
        }
        #endregion
    }

    public class GeoObjectCollection : IEnumerable<GeoObject>
    {
        List<GeoObject> _geo_objects;
        //ToDo:
        //GeocoderResponseMetaData

        public GeoObjectCollection()
        {
            _geo_objects = new List<GeoObject>();
        }

        public GeoObjectCollection(string xml)
        {
            _geo_objects = new List<GeoObject>();
            ParseXml(xml);
        }

        // ToDo: not best specification realise
        //       - null varibles and geo varibles
        //       - null response
        //       + ll,spn bounds
        private void ParseXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("ns", "http://maps.yandex.ru/ymaps/1.x");
            ns.AddNamespace("opengis", "http://www.opengis.net/gml");
            ns.AddNamespace("geocoder", "http://maps.yandex.ru/geocoder/1.x");

            // select geo objects
            XmlNodeList nodes = doc.SelectNodes("//ns:ymaps/ns:GeoObjectCollection/opengis:featureMember/ns:GeoObject", ns);
            foreach (XmlNode node in nodes)
            {
                var point_node = node.SelectSingleNode("opengis:Point/opengis:pos", ns);
                var bounds_node = node.SelectSingleNode("opengis:boundedBy/opengis:Envelope", ns);
                var meta_node = node.SelectSingleNode("opengis:metaDataProperty/geocoder:GeocoderMetaData", ns);

                GeoObject obj = new GeoObject
                {
                    Point = point_node == null ? new GeoPoint() : GeoPoint.Parse(point_node.InnerText),
                    BoundedBy = bounds_node == null ? new GeoBound() : new GeoBound(
                        GeoPoint.Parse(bounds_node["lowerCorner"].InnerText), GeoPoint.Parse(bounds_node["upperCorner"].InnerText)
                        ),
                    GeocoderMetaData = new GeoMetaData(meta_node["text"].InnerText, meta_node["kind"].InnerText)
                };
                _geo_objects.Add(obj);
            }
        }

        public GeoObject this[int i]
        {
            get
            {
                return _geo_objects[i];
            }
        }

        public IEnumerator<GeoObject> GetEnumerator()
        {
            return _geo_objects.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _geo_objects.GetEnumerator();
        }
    }

    public class GeoObject
    {
        public GeoPoint Point;
        public GeoBound BoundedBy;
        public GeoMetaData GeocoderMetaData;
    }

    public class GeoMetaData
    {
        public KindType Kind = KindType.locality;
        public string Text = string.Empty;
        //ToDo:
        //AddressDetails
        public GeoMetaData() { }
        public GeoMetaData(string text)
        {
            this.Text = text;
        }
        public GeoMetaData(string text, string kind)
        {
            this.Text = text;
            this.Kind = ParseKind(kind);
        }
        public GeoMetaData(string text, KindType kind)
        {
            this.Text = text;
            this.Kind = kind;
        }

        public static KindType ParseKind(string kind)
        {
            switch (kind)
            {
                case "district": return KindType.district;
                case "house": return KindType.house;
                case "locality": return KindType.locality;
                case "metro": return KindType.metro;
                case "street": return KindType.street;
                default: return KindType.locality; //throw new System.Exception();
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public enum LangType : byte { ru_RU, uk_UA, be_BY, en_US, en_BR, tr_TR }

    public enum KindType : byte { house, street, metro, district, locality }

    public enum ScoType : byte { longlat, latlong }
    public struct GeoPoint
    {
        public delegate string ToStringFunc(double x, double y);
        public double Long, Lat;

        public static GeoPoint Parse(string point)
        {
            string[] splitted = point.Split(new char[] { ' ' }, count: 2);
            return new GeoPoint(double.Parse(splitted[0], CultureInfo.InvariantCulture), double.Parse(splitted[1], CultureInfo.InvariantCulture));
        }

        public GeoPoint(double Long, double Lat)
        {
            this.Long = Long;
            this.Lat = Lat;
        }


        public override string ToString()
        {
            return string.Format("{0} {1}", this.Long.ToString(CultureInfo.InvariantCulture), this.Lat.ToString(CultureInfo.InvariantCulture));
        }
        public string ToString(string format)
        {
            return string.Format(format, this.Long.ToString(CultureInfo.InvariantCulture), this.Lat.ToString(CultureInfo.InvariantCulture));
        }

        public string ToString(ToStringFunc formatFunc)
        {
            return formatFunc(Long, Lat);
        }
    }

    public struct GeoBound
    {
        public GeoPoint LowerCorner, UpperCorner;
        public GeoBound(GeoPoint lowerCorner, GeoPoint upperCorner)
        {
            this.LowerCorner = lowerCorner;
            this.UpperCorner = upperCorner;
        }

        public override string ToString()
        {
            return string.Format("[{0}] [{1}]", LowerCorner.ToString(), UpperCorner.ToString());
        }
    }

    public struct SearchArea
    {
        public GeoPoint LongLat, Spread;
        public SearchArea(GeoPoint CenterLongLat, GeoPoint Spread)
        {
            this.LongLat = CenterLongLat;
            this.Spread = Spread;
        }
    }

}
