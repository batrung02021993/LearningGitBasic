using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Model;


namespace VinaInvoice.Data.DataContext
{
	public class SoftwareRestApi
	{
		private BaseRestApi _baseRestApi = new BaseRestApi();

		public VersionResponse GetVersion()
		{
			var profile = (ProfileData)ApplicationCache.GetItem("profile");
			VersionRequest request = new VersionRequest();
			var task = _baseRestApi.AddAsync<VersionResponse, VersionRequest>(profile.token, request, "client/software/check_version");
			return task.GetAwaiter().GetResult();			
		}

	}


	public class VersionRequest
	{
		public object Data { get; set; }
	}


	public class VersionResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public VersionData data { get; set; }
	}

	public class VersionData
	{
		public string version { get; set; }
		public string download_path { get; set; }
	}



}
