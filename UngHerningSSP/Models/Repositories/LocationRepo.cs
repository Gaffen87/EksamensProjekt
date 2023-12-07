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
    }
}
