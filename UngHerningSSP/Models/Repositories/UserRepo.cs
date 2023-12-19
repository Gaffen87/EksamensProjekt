using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories;
public class UserRepo
{
	private readonly DbAccess dbAccess = new();

	public UserRepo()
    {
	}

    public int Insert(User user)
	{
		return dbAccess.SaveDataAndReturnID("spInsertUser", new { user.FirstName, user.LastName, user.UserName, user.Password, user.IsAdmin });
	}

	public User Retrieve(int id)
	{
		return dbAccess.LoadSingle<User, dynamic>("spRetrieveUserByID", new { UserID = id });
	}

	public User Retrieve(string username, string password)
	{
		return dbAccess.LoadSingle<User, dynamic>("spRetrieveUser", new { UserName = username, Password = password });
	}

	public User RetrieveObservationUser(int id)
	{
		return dbAccess.LoadSingle<User, dynamic>("spGetObservationUser", new { ID = id });
	}

	public IEnumerable<User> RetrieveAll()
	{
		return dbAccess.LoadMultiple<User>("spRetrieveAllUsers");
	}

	public IEnumerable<User> RetrieveAll(bool isAdmin)
	{
		return dbAccess.LoadMultiple<User, dynamic>("spRetrieveAllUsers", new { IsAdmin = isAdmin });
	}

	public void Update(User user) 
	{
		dbAccess.SaveData<User>("spUpdateUser", user);
	}

	public void Delete(User user) 
	{
		dbAccess.SaveData<User>("spDeleteUser", user);
	}
}
