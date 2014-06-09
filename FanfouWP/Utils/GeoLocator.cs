using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FanfouWP.Utils
{
    public class GeoLocator
    {
        public static GeoCoordinate current;
        public async static void reverseAddress(EventHandler<QueryCompletedEventArgs<IList<MapLocation>>> handler)
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(1));
                current = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Latitude);
                ReverseGeocodeQuery
                    reverseGeocodeQuery = new ReverseGeocodeQuery();
                reverseGeocodeQuery.GeoCoordinate = current;
                reverseGeocodeQuery.QueryCompleted += handler;
                reverseGeocodeQuery.QueryAsync();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
        }
        public async static Task<Pair<bool, string>> getGeolocator()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(1));
                current = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Latitude);
                if (geoposition.CivicAddress == null)
                {
                    double marLat = 0;
                    double marLon = 0;
                    EvilTransform.transform(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude, out marLat, out marLon);

                    return new Pair<bool, string>(true, marLat.ToString() + "," + marLon.ToString());
                }
                else
                {
                    return new Pair<bool, string>(true, geoposition.CivicAddress.ToString());
                }
            }
            catch (Exception ex)
            {
                var result = "";
                if ((uint)ex.HResult == 0x80004004)
                {
                    result = "无法定位";
                }
                else
                {
                    result = "未知地点";
                }
                return new Pair<bool, string>(false, result);
            }
        }
    }
}
