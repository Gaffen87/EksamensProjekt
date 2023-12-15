﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories
{
    public class HotspotRepo : IRepository<Hotspot>
    {
		private readonly LocationRepo locationRepo = new();
        private readonly ScheduleRepo scheduleRepo = new();
        private readonly DbAccess dbAccess = new();
		public HotspotRepo()
        {
		}

		public void Delete(Hotspot hotspot)
		{
			int id = hotspot.Location.ID;
			dbAccess.SaveData("spDeleteHotspot", new { hotspot.ID });
			locationRepo.Delete(id);
		}

		public int Insert(Hotspot hotspot)
		{
			throw new NotImplementedException();
		}

		public int Insert(Hotspot hotspot, User user, Location location)
        {
            return dbAccess.SaveDataAndReturnID("spInsertHotspot", new { hotspot.Title, hotspot.Priority, LocationID = location.ID, UserID = user.ID });
        }

		public Hotspot Retrieve(int id)
		{
			throw new NotImplementedException();
		}

		public List<Hotspot> RetrieveAll()
        {
            var hotspots = dbAccess.LoadMultiple<Hotspot>("spRetrieveAllHotspots").ToList();
			foreach (var hotspot in hotspots)
			{
				hotspot.Location = locationRepo.GetHotspotLocation(hotspot.ID);
                hotspot.Schedules = scheduleRepo.RetrieveAllByHotspotID(hotspot.ID).ToList();
			}

            return hotspots;
		}

		public void Update(Hotspot hotspot)
		{
			throw new NotImplementedException();
		}

		IEnumerable<Hotspot> IRepository<Hotspot>.RetrieveAll()
		{
			throw new NotImplementedException();
		}
	}
}
