using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories
{
    public class HotspotRepo
    {
        private readonly DbAccess dbAccess = new();
		public HotspotRepo()
        {
		}

        public int InsertHotspot(Hotspot hotspot, User user, Location location)
        {
            return dbAccess.SaveDataAndReturnID("spInsertHotspot", new { hotspot.Name, hotspot.Priority, LocationID = location.ID, UserID = user.ID });
        }
    }
}
