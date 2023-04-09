namespace PashaSestroyob.Data.Models
{
    public class Distance
    {
        public static double GetDistance(double lat, double lon, double latitude, double longitude)
        {
            const int EarthRadiusKm = 6371;

            var dLat = (latitude - lat) * Math.PI / 180.0;
            var dLon = (longitude - lon) * Math.PI / 180.0;

            var lat1Rad = lat * Math.PI / 180.0;
            var lat2Rad = latitude * Math.PI / 180.0;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1Rad) * Math.Cos(lat2Rad);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }
    }
}
