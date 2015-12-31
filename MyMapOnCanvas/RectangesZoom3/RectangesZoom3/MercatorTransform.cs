using System;
using System.Windows;
namespace RectangesZoom3
{
    /// <summary>
    /// Transforms latitude and longitude values in degrees to cartesian coordinates
    /// according to the Mercator projection.
    /// </summary>
    public class MercatorTransform 
    {
        public static readonly double MaxLatitudeValue = Math.Atan(Math.Sinh(Math.PI)) / Math.PI * 180d;

        public static double RelativeScale(double latitude)
        {
            if (latitude <= -90d)
            {
                return double.NegativeInfinity;
            }

            if (latitude >= 90d)
            {
                return double.PositiveInfinity;
            }

            return 1d / Math.Cos(latitude * Math.PI / 180d);
        }

        public static double LatitudeToY(double latitude)
        {
            if (latitude <= -90d)
            {
                return double.NegativeInfinity;
            }

            if (latitude >= 90d)
            {
                return double.PositiveInfinity;
            }

            latitude *= Math.PI / 180d;
            return Math.Log(Math.Tan(latitude) + 1d / Math.Cos(latitude)) / Math.PI * 180d;
        }

        public static double YToLatitude(double y)
        {
            return Math.Atan(Math.Sinh(y * Math.PI / 180d)) / Math.PI * 180d;
        }

        public double MaxLatitude
        {
            get { return MaxLatitudeValue; }
        }

        public double RelativeScale(Location location)
        {
            return RelativeScale(location.Latitude);
        }

        public Point Transform(Location location)
        {
            return new Point(location.Longitude, LatitudeToY(location.Latitude));
        }

        public Location Transform(Point point)
        {
            return new Location(YToLatitude(point.Y), point.X);
        }
    }
    
    public static class MercatorOSM
    {
        private static readonly double R_MAJOR = 6378137.0;
        private static readonly double R_MINOR = 6356752.3142;
        private static readonly double RATIO = R_MINOR / R_MAJOR;
        private static readonly double ECCENT = Math.Sqrt(1.0 - (RATIO * RATIO));
        private static readonly double COM = 0.5 * ECCENT;

        private static readonly double DEG2RAD = Math.PI / 180.0;
        private static readonly double RAD2Deg = 180.0 / Math.PI;
        private static readonly double PI_2 = Math.PI / 2.0;

        public static double[] toPixel(double lon, double lat)
        {
            return new double[] { lonToX(lon), latToY(lat) };
        }

        public static double[] toGeoCoord(double x, double y)
        {
            return new double[] { xToLon(x), yToLat(y) };
        }

        public static double lonToX(double lon)
        {
            return R_MAJOR * DegToRad(lon);
        }

        public static double latToY(double lat)
        {
            lat = Math.Min(89.5, Math.Max(lat, -89.5));
            double phi = DegToRad(lat);
            double sinphi = Math.Sin(phi);
            double con = ECCENT * sinphi;
            con = Math.Pow(((1.0 - con) / (1.0 + con)), COM);
            double ts = Math.Tan(0.5 * ((Math.PI * 0.5) - phi)) / con;
            return 0 - R_MAJOR * Math.Log(ts);
        }

        public static double xToLon(double x)
        {
            return RadToDeg(x) / R_MAJOR;
        }

        public static double yToLat(double y)
        {
            double ts = Math.Exp(-y / R_MAJOR);
            double phi = PI_2 - 2 * Math.Atan(ts);
            double dphi = 1.0;
            int i = 0;
            while ((Math.Abs(dphi) > 0.000000001) && (i < 15))
            {
                double con = ECCENT * Math.Sin(phi);
                dphi = PI_2 - 2 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), COM)) - phi;
                phi += dphi;
                i++;
            }
            return RadToDeg(phi);
        }

        private static double RadToDeg(double rad)
        {
            return rad * RAD2Deg;
        }

        private static double DegToRad(double deg)
        {
            return deg * DEG2RAD;
        }
    }

    public static class MercatorWrapper
    {
        const double maxY = 34619289.3718563;

        public static Location GetLocation(Point p, Rect viewPort,byte zoom)
        {
            var vh = Constants.TileSize*Math.Pow(2, zoom);
            var percentincurrent = (p.Y - viewPort.Y)/vh;
            var y = maxY*2*(percentincurrent)-maxY;
        //у для самары 6453590....
            var samlat = 50.231435;
            var c = MercatorOSM.latToY(samlat);


            var lat=  MercatorOSM.yToLat(c);
            return new Location(lat,0);

        }

    }
}