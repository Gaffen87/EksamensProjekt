using UngHerningSSP;
using UngHerningSSP.DataAccess;
using UngHerningSSP.Models.Repositories;
using UngHerningSSP.Models;
using UngHerningSSP.ViewModels;

namespace UngHerningTest;

[TestClass]
public class UnitTest1
{
	DbAccess dbAccess;
	LoginViewModel loginViewModel;
	Location location;
	User user;
	Schedule schedule;
	Hotspot hotspot;
	Observation observation;

	[TestInitialize]
	public void Init()
	{
		dbAccess = new();

		loginViewModel = new();

		location = new()
		{
			Latitude = 56.2,
			Longitude = 8.3
		};

		user = new()
		{
			FirstName = "Fornavn",
			LastName = "Efternavn",
			UserName = "Username",
			Password = "Password",
			IsAdmin = true
		};

		schedule = new()
		{
			DayOfWeek = "Monday",
			StartTime = DateTime.Now.AddHours(-5),
			EndTime = DateTime.Now
		};

		hotspot = new()
		{
			Title = "Test",
			Priority = "Rød",
			Location = location,
			User = user,
			Schedules = new List<Schedule> { schedule }
		};

		observation = new()
		{
			DateAndTime = DateTime.Now,
			Severity = "Rød",
			Behaviour = "Test",
			Approach = "Test",
			Count = 1,
			Description = "Test"
		};
	}

	[TestMethod]
	public void TestValidateIncorrectUser()
	{
		loginViewModel.Username = "Ikke valid bruger";
		loginViewModel.Password = "Forkert password";

		Assert.AreEqual(false, loginViewModel.ValidateUser());
	}

	[TestMethod]
	public void TestRetrieveUserFromDB()
	{
		user.ID = dbAccess.SaveDataAndReturnID("testInsertUser", new { user.FirstName, user.LastName, user.UserName, user.Password, user.IsAdmin });
		User testuser = dbAccess.LoadSingle<User, dynamic>("testRetrieveUserByID", new { UserID = user.ID });

		Assert.AreEqual(user.FirstName, testuser.FirstName);
		Assert.AreEqual(user.IsAdmin, testuser.IsAdmin);
		Assert.AreEqual(user.ID, testuser.ID);
	}

	[TestMethod]
	public void TestInsertAndRetrieveLocation()
	{
		location.ID = dbAccess.SaveDataAndReturnID("testInsertLocation", new { location.Latitude, location.Longitude });
		var locations = dbAccess.LoadMultiple<Location>("testRetrieveAllLocations");

		Assert.IsNotNull(locations);
		Assert.IsTrue(locations.Any(x => x.ID == location.ID));
	}

	[TestMethod]
	public void TestInsertAndRetrieveHotspot()
	{
		hotspot.ID = dbAccess.SaveDataAndReturnID("testInsertHotspot", new { hotspot.Title, hotspot.Priority, LocationID = 1, UserID = 1 });
		var hotspots = dbAccess.LoadMultiple<Hotspot>("testRetrieveAllHotspots");

		Assert.IsNotNull(hotspots);
		Assert.IsTrue(hotspots.Any(x => x.ID == hotspot.ID));
	}

	[TestMethod]
	public void TestRetrieveAllHotspots()
	{
		var hotspots = dbAccess.LoadMultiple<Hotspot>("testRetrieveAllHotspots");
		Assert.IsNotNull(hotspots);
	}

	[TestMethod]
	public void TestDeleteHotspot()
	{
		hotspot.ID = dbAccess.SaveDataAndReturnID("testInsertHotspot", new { hotspot.Title, hotspot.Priority, LocationID = 1, UserID = 1 });

		var hotspots = dbAccess.LoadMultiple<Hotspot>("testRetrieveAllHotspots");
		dbAccess.SaveData("testDeleteHotspot", new { hotspot.ID });
		var hotspots2 = dbAccess.LoadMultiple<Hotspot>("testRetrieveAllHotspots");

		Assert.IsTrue(hotspots2.Count() < hotspots.Count());
	}

	[TestMethod]
	public void TestInsertAndRetrieveSchedule()
	{
		hotspot.ID = dbAccess.SaveDataAndReturnID("testInsertHotspot", new { hotspot.Title, hotspot.Priority, LocationID = 1, UserID = 1 });

		schedule.ID = dbAccess.SaveDataAndReturnID("testInsertSchedule", new { schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, HotspotID = hotspot.ID });
		var schedules = dbAccess.LoadMultiple<Schedule>("testRetrieveAllSchedules");

		Assert.IsNotNull(schedules);
		Assert.IsTrue(schedules.Any(x => x.ID == schedule.ID));
	}

	[TestMethod]
	public void TestInsertAndRetrieveObservation()
	{
		observation.ID = dbAccess.SaveDataAndReturnID("testInsertObservation", new
		{
			observation.DateAndTime,
			observation.Severity,
			observation.Behaviour,
			observation.Approach,
			observation.Count,
			observation.Description,
			LocationID = 1,
			UserID = 1
		});
		var observations = dbAccess.LoadMultiple<Observation>("testRetrieveAllObservations");

		Assert.IsNotNull(observations);
		Assert.IsTrue(observations.Any(x => x.ID == observation.ID));
	}
}