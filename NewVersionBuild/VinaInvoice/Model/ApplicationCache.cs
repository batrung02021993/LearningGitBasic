using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Model
{
	public class ApplicationCache
	{
		static Dictionary<string, object> CentralCache = new Dictionary<string, object>();

		public static void Add(string key, object Value)
		{
			if (!CentralCache.ContainsKey(key))
			{
				CentralCache.Add(key, Value);
			}
		}

		public static object GetItem(string key)
		{
			if (CentralCache.ContainsKey(key))
			{
				return CentralCache[key];
			}
			return null;
		}

	}
}
