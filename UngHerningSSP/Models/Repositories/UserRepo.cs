using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UngHerningSSP.DataAccess;

namespace UngHerningSSP.Models.Repositories;
public class UserRepo(DbAccess dbAccess) : IRepository<User>
{
    public int Insert(User user)
	{
		return dbAccess.SaveDataAndReturnID("spInsertUser", new { user.FirstName, user.LastName, user.UserName, user.Password, user.IsAdmin });
	}

	public User Retrieve(int id) 
	{
		return dbAccess.LoadSingle<User, dynamic>("spRetrieveUser", new { ID = id });
	}

	public IEnumerable<User> RetrieveAll()
	{
		return dbAccess.LoadMultiple<User, dynamic>("spRetrieveAllUsers", new {  });
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
