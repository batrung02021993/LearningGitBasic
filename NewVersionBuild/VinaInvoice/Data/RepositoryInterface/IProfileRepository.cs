using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.Interface
{
	public interface IProfileRepository: IRepository<Profile>
	{
		bool SaveToCache(ProfileData profileData);
		bool SaveToLocalDb();
	}
}
