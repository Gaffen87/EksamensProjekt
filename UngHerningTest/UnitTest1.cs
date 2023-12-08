using UngHerningSSP;
using UngHerningSSP.DataAccess;
using UngHerningSSP.Models.Repositories;
using UngHerningSSP.Models;

namespace UngHerningTest;

[TestClass]
public class UnitTest1
{
	DbAccess dbAccess;
	UserRepo repo;

    [TestInitialize]
	public void Init() 
	{
		dbAccess = new DbAccess();
		repo = new();
	}

	[TestMethod]
	public void TestMethod1()
	{
		User user = new()
		{
			FirstName = "Aske",
			LastName = "Lysgaard",
			UserName = "Username",
			Password = "Password",
			IsAdmin = true
		};
		user.ID = repo.Insert(user);
		User user1 = repo.Retrieve(user.ID);

		Assert.AreEqual(user.FirstName, user1.FirstName);

	}
}