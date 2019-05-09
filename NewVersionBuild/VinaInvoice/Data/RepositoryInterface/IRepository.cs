using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Data.Interface
{	
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetList();

        IEnumerable<T> Find(Func<T, bool> predicate);

        IEnumerable<T> Search(int type_search, int type_sort, int sort_method, string key, int[] status, int[] state_of_bill, string start_date, string stop_date);

        T Get();

		T GetById(object id);

		string Create(T entity);

		void Update(T entity);

		string Delete(IEnumerable<T> delete_list);

		int Count(Func<T, bool> predicate);

		bool Any(Func<T, bool> predicate);

		bool Any();
	}
}
