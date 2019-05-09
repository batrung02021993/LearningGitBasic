using VinaInvoice.Data.Interface;
using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.Repository
{
	public class ProfileRepository : Repository<Profile>, IProfileRepository, IDisposable
	{
		public void Dispose()
		{
			this.Dispose();
		}

		public bool SaveToCache(ProfileData profileData)
		{
			try
			{				
				ApplicationCache.Add("profile", profileData);
				return true;
			}
			catch
			{
				return false;
			}									
		}

		public bool SaveToLocalDb()
		{
			throw new NotImplementedException();
		}
	}
}
