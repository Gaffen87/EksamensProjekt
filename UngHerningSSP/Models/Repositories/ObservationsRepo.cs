using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories;
public class ObservationsRepo : IRepository<Observation>
{
	DbAccess dbAccess = new DbAccess();
    public ObservationsRepo()
    {
        observations = new List<Observation>(RetrieveAll());
    }

	private List<Observation> observations;
	public List<Observation> GetList() => observations;

    public void Delete(Observation observation)
	{
		throw new NotImplementedException();
	}

	public int Insert(Observation observation)
	{
		return dbAccess.SaveDataAndReturnID("spInsertObservation", new { observation.DateAndTime, observation.Severity, observation.Behavior, observation.Approach, observation.Count, observation.Description, LocationID = observation.Location.ID, UserID = observation.User.ID });
	}

	public Observation Retrieve(int id)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Observation> RetrieveAll()
	{
		return dbAccess.LoadMultiple<Observation>("spRetrieveAllObservations");
	}

	public void Update(Observation observation)
	{
		throw new NotImplementedException();
	}

}
