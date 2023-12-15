using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories
{
    public class LocationRepo
    {
		private readonly DbAccess dbAccess = new();

		public LocationRepo()
        {
		}

		public int InsertLocation(Location location)
		{
			return dbAccess.SaveDataAndReturnID("spInsertLocation", new { location.Latitude, location.Longitude });
		}

		public Location GetHotspotLocation(int id)
		{
			return dbAccess.LoadSingle<Location, dynamic>("spGetHotspotLocation", new { HotspotID = id });
		}

		public List<Location> RetrieveAllLocations()
		{
			return dbAccess.LoadMultiple<Location>("spRetrieveAllLocations").ToList();
		}

		public void Delete(int id)
		{
			dbAccess.SaveData("spDeleteLocation", new { ID = id });
		}
    }
}
