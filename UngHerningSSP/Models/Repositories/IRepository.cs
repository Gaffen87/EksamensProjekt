using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UngHerningSSP.Models.Repositories;
public interface IRepository<T> where T : class
{
	public int Insert(T entity);

	public T Retrieve(int id);

	public IEnumerable<T> RetrieveAll();

	public void Update(T entity);

	public void Delete(T entity);
}
