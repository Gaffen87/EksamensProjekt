using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;
using UngHerningSSP.Models;

namespace UngHerningSSP.Models.Repositories;
public class ScheduleRepo : IRepository<Schedule>
{
	DbAccess dbAccess = new();

	public void Delete(Schedule entity)
	{
		throw new NotImplementedException();
	}

	public int Insert(Schedule entity)
	{
		throw new NotImplementedException();
	}

	public int Insert(Schedule schedule, Hotspot hotspot)
	{
		return dbAccess.SaveDataAndReturnID("spInsertSchedule", new { schedule.DayOfWeek, schedule.StartTime,schedule.EndTime, HotspotID = hotspot.ID });
	}

	public Schedule Retrieve(int id)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Schedule> RetrieveAllByHotspotID(int hotspotID)
	{
		return dbAccess.LoadMultiple<Schedule, dynamic>("spRetrieveSchedulesByHotspotID", new { HotspotID = hotspotID });
	}

	public IEnumerable<Schedule> RetrieveAll()
	{
		return dbAccess.LoadMultiple<Schedule>("spRetrieveAllSchedules");
	}

	public void Update(Schedule entity)
	{
		throw new NotImplementedException();
	}
}
