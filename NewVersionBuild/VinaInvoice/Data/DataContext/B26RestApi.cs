using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VinaInvoice.Common;
using VinaInvoice.Model;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.DataContext
{

	public class B26RestApi
	{
		private BaseRestApi _baseRestApi = new BaseRestApi();

		public B26GetlistResponse GetB26List(B26GetlistRequest request)
		{
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<B26GetlistResponse, B26GetlistRequest>(profile.token, request, "bc26/list");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}


			
		}

		public ReportBC26Response ReportB26(ReportBC26Request request)
		{

			var profile = (ProfileData)ApplicationCache.GetItem("profile");
			var task = _baseRestApi.AddAsync<ReportBC26Response, ReportBC26Request>(profile.token, request, "bc26/report");
			var Response = task.GetAwaiter().GetResult();
			return Response;
		}

	}


	public class B26GetlistRequest
	{
		public string start_time { get; set; }
		public string stop_time { get; set; }
	}


	public class B26GetlistResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public HSoThueDTuHSoKhaiThueCTieuTKhaiChinh data { get; set; }
	}


	public class ReportBC26Request
	{
		public string start_time { get; set; }
		public string stop_time { get; set; }
		public int type_bc { get; set; }
		public int type_value { get; set; }
	}


	public class ReportBC26Response
	{
		public int code { get; set; }
		public string message { get; set; }
		public object data { get; set; }
	}






}
