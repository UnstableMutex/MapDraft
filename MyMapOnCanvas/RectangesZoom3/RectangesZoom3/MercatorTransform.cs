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
}