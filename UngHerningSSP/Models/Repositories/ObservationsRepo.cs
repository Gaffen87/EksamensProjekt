﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories;
public class ObservationsRepo
{
	DbAccess dbAccess = new DbAccess();
	LocationRepo locationRepo = new LocationRepo();
	UserRepo userRepo = new UserRepo();
    public ObservationsRepo()
    {
        observations = new List<Observation>(RetrieveAll());
    }

	private List<Observation> observations;
	public List<Observation> GetList() => observations;

    public void Delete(Observation observation)
	{
		dbAccess.SaveData("spDeleteObservation", new { observation.ID });
		dbAccess.SaveData("spDeleteLocation", new { observation.Location.ID });
	}

	public int Insert(Observation observation)
	{
		return dbAccess.SaveDataAndReturnID("spInsertObservation", new { observation.DateAndTime, observation.Severity, observation.Behaviour, observation.Approach, observation.Count, observation.Description, LocationID = observation.Location.ID, UserID = observation.User.ID });
	}

	public Observation Retrieve(int id)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Observation> RetrieveAll()
	{
		var observations = dbAccess.LoadMultiple<Observation>("spRetrieveAllObservations");
		foreach (var observation in observations)
		{
			observation.Location = locationRepo.GetObservationLocation(observation.ID);
			observation.User = userRepo.RetrieveObservationUser(observation.ID);
		}
		return observations;
	}

	public void Update(Observation observation)
	{
		dbAccess.SaveData("spUpdateObservation", new 
		{ 
			observation.ID, 
			observation.DateAndTime, 
			observation.Severity, 
			observation.Behaviour, 
			observation.Approach,
			observation.Count,
			observation.Description,
			LocationID = observation.Location.ID,
			UserID = observation.User.ID
		});
	}

}
