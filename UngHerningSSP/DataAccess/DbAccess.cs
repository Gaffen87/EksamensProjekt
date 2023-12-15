using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace UngHerningSSP.DataAccess;
public class DbAccess
{
    public DbAccess()
    {
		connectionString = App.config.GetConnectionString("DbConnection");
	}
	private readonly string? connectionString;

	/// <summary>
	/// Bruges til at gemme/opdatere/slette et objekt i db med retur værdi
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql">Sql sætningen som skal køres</param>
	/// <param name="parameters">Parametrene (i et anonymt objekt) som sql sætningen kræver, skal passe præcist navnemæssigt</param>
	/// <returns>Returnerer det ID som db har givet objektet</returns>
	public int SaveDataAndReturnID<T>(string sql, T parameters)
	{
		using SqlConnection con = new(connectionString);
		return con.QuerySingle<int>(sql, parameters, commandType: CommandType.StoredProcedure);
	}

	/// <summary>
	/// Bruges til at gemme/opdatere/slette et objekt i db uden retur værdi
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql">Sql sætningen som skal køres</param>
	/// <param name="parameters">Parametrene (i et anonymt objekt) som sql sætningen kræver, skal passe præcist navnemæssigt</param>
	public void SaveData<T>(string sql, T parameters)
	{
		using SqlConnection con = new(connectionString);
		con.QuerySingle<int>(sql, parameters, commandType: CommandType.StoredProcedure);
	}

	/// <summary>
	/// Bruges til at hente flere objekter fra db
	/// </summary>
	/// <typeparam name="T">Typen af objekt der skal hentes</typeparam>
	/// <typeparam name="U">Typen af objekt hvis properties der sendes som parametre til db, her kan bruges anonym type hvis parametre i sql ikke stemmer 100% med objektet</typeparam>
	/// <param name="sql">Sql sætningen som skal køres</param>
	/// <param name="parameters">Parametrene (i et anonymt objekt) som sql sætningen kræver, skal passe præcist navnemæssigt</param>
	/// <returns>Returnerer et objekt af typen T</returns>
	public T LoadSingle<T, U>(string sql, U parameters)
	{
		using SqlConnection con = new(connectionString);
		return con.QuerySingle<T>(sql, parameters, commandType: CommandType.StoredProcedure);
	}

	/// <summary>
	/// Bruges til at hente alle objekter i db af en bestemt type
	/// </summary>
	/// <typeparam name="T">Typen af objekt der skal hentes</typeparam>
	/// <param name="sql">Sql sætningen som skal køres</param>
	/// <param name="parameters">Parametrene (i et anonymt objekt) som sql sætningen kræver, skal passe præcist navnemæssigt</param>
	/// <returns>Returnerer en objekterne i en IEnumerable af typen T</returns>
	public IEnumerable<T> LoadMultiple<T>(string sql)
	{
		using SqlConnection con = new(connectionString);
		return con.Query<T>(sql, commandType: CommandType.StoredProcedure);
	}
	/// <summary>
	/// Bruges til at hente alle objekter i db af en bestemt type efter nogle kriterier
	/// </summary>
	/// <typeparam name="T">Typen af objekt der skal hentes</typeparam>
	/// <typeparam name="U">Typen af objekt, hvis properties der sendes som parametre til db, her kan bruges anonym type hvis parametre i sql ikke stemmer 100% med objektet</typeparam>
	/// <param name="sql">Sql sætningen som skal køres</param>
	/// <param name="parameters">Parametrene (i et anonymt objekt) som sql sætningen kræver, skal passe præcist navnemæssigt</param>
	/// <returns>Returnerer en objekterne i en IEnumerable af typen T</returns>
	public IEnumerable<T> LoadMultiple<T, U>(string sql, U parameters)
	{
		using SqlConnection con = new(connectionString);
		return con.Query<T>(sql, parameters, commandType: CommandType.StoredProcedure);
	}
}
