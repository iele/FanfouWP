using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FanfouWP.Utils
{
    public class GeoLocatorUtils
    {       
        public static GeoCoordinate current;
        public async static Task<string> getGeolocator()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(1));
                current = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Latitude);
                if (geoposition.CivicAddress == null)
                {
                    return geoposition.Coordinate.Latitude.ToString() + "," + geoposition.Coordinate.Longitude.ToString();
                }
                else
                {
                    return geoposition.CivicAddress.ToString();
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    return "";
                }
                {
                    return "未知地点";
                }
            }
        }
    }
}
