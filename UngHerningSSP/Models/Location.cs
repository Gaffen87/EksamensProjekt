using Esri.ArcGISRuntime.Geometry;
using System.Globalization;

namespace UngHerningSSP.Models;
public class Location
{
    public int ID { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Address { get; set; }

    public string AddressNum { get; set; }

    public int PostalCode { get; set; }

    public string City { get; set; }


	public static double GetLatitude(MapPoint mapPoint)
	{
        string latitude = CoordinateFormatter.ToLatitudeLongitude(mapPoint, LatitudeLongitudeFormat.DecimalDegrees, 3).Split('N')[0];

        return double.Parse(latitude, CultureInfo.InvariantCulture);
	}
    public static double GetLongitude(MapPoint mapPoint)
    {
        string longitude = CoordinateFormatter.ToLatitudeLongitude(mapPoint, LatitudeLongitudeFormat.DecimalDegrees, 3).Split('N')[1].Trim('E');

        return double.Parse(longitude, CultureInfo.InvariantCulture);
    }

}
