using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Data.Interface;
using VinaInvoice.Model;

namespace VinaInvoice.Data.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private int _page;
		public int Page { get => _page; set => _page = value; }

        protected readonly RemoteDataContext _context;
		
		public Repository()
		{
			_context = new RemoteDataContext();
		}

		public bool Any(Func<T, bool> predicate)
		{
			throw new NotImplementedException();
		}

		public bool Any()
		{
			throw new NotImplementedException();
		}

		public int Count(Func<T, bool> predicate)
		{
			throw new NotImplementedException();
		}

		public string Create(T entity)
		{
			return _context.Create<T>(entity);
		}

        public void Update(T entity)
        {
             _context.Update<T>(entity);
        }

        public string Delete(IEnumerable<T> delete_list)
		{
            return _context.Delete<T>(delete_list);
        }

		public IEnumerable<T> Find(Func<T, bool> predicate)
		{		
				var rawlist = _context.GetList<T>(_page);
			    List<T> list = new List<T>();
				if(rawlist != null)  list = rawlist.Where(predicate).ToList();
				return list;						
		}

		public T Get()
		{
			return (T)_context.Get<T>();
		}

		public T GetById(object id)
		{
			return (T)_context.Get<T>(id);
		}

		public IEnumerable<T> GetList()
		{
			var rawlist = _context.GetList<T>(_page);
			List<T> list = new List<T>();
			if (rawlist != null) list = rawlist.ToList();
			return list;
		}

        public IEnumerable<T> GetList(int[] status, int[] state_of_bill)
        {
            var rawlist = _context.GetList<T>(_page, status,state_of_bill);
            List<T> list = new List<T>();
            if (rawlist != null) list = rawlist.ToList();
            return list;
        }

        public IEnumerable<T> GetList(int[] status, int[] state_of_bill, string start_date, string stop_date)
        {
            var rawlist = _context.GetList<T>(_page, status, state_of_bill, start_date, stop_date);
            List<T> list = new List<T>();
            if (rawlist != null) list = rawlist.ToList();
            return list;
        }

        public IEnumerable<T> Search<T>(int type_search, int type_sort, int sort_method, string key)
        {
            var rawlist = _context.Search<T>(_page, type_search, type_sort, sort_method, key);
            List<T> list = new List<T>();
            if (rawlist != null) list = rawlist.ToList();
            return list;
        }

        public IEnumerable<T> Search(int type_search, int type_sort, int sort_method, string key, int[] status, int[] state_of_bill, string start_date, string stop_date)
        {
            var rawlist = _context.Search<T>(_page, type_search, type_sort, sort_method, key, status, state_of_bill, start_date, stop_date);
            List<T> list = new List<T>();
            if (rawlist != null) list = rawlist.ToList();
            return list;
        }

        public IEnumerable<T> Search(int type_search, int type_sort, int sort_method, string key, int[] status, int[] state_of_bill, string start_date, string stop_date, bool get_detail)
        {
            var rawlist = _context.Search<T>(_page, type_search, type_sort, sort_method, key, status, state_of_bill, start_date, stop_date, get_detail);
            List<T> list = new List<T>();
            if (rawlist != null) list = rawlist.ToList();
            return list;
        }
	}
}
