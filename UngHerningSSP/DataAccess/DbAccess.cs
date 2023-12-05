using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Data;

namespace UngHerningSSP.DataAccess;
public class DbAccess
{
    public DbAccess()
    {
		IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		connectionString = config.GetConnectionString("DbConnection");
	}
	private readonly string? connectionString;

	public int SaveDataAndReturnID<T>(string sql, T parameters)
	{
		using SqlConnection con = new(connectionString);
		return con.QuerySingle<int>(sql, parameters, commandType: CommandType.StoredProcedure);
	}

	public void SaveData<T>(string sql, T parameters)
	{
		using SqlConnection con = new(connectionString);
		con.QuerySingle<T>(sql, parameters, commandType: CommandType.StoredProcedure);
	}

	public T LoadSingle<T, U>(string sql, U parameters)
	{
		using SqlConnection con = new(connectionString);
		return con.QuerySingle<T>(sql, parameters, commandType: CommandType.StoredProcedure);
	}

	public IEnumerable<T> LoadMultiple<T, U>(string sql, U parameters)
	{
		using SqlConnection con = new(connectionString);
		return con.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure);
	}
}
