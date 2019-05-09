using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;
using VinaInvoice.Model;
using System.Windows;
using VinaInvoice.Common;

namespace VinaInvoice.Data.DataContext
{
	public class EnterpriseRestApi
	{
		private BaseRestApi _baseRestApi = new BaseRestApi();

		public IEnumerable<Member> GetListMember()
		{
			try
			{

				List<Member> list = new List<Member>();
				var profile = (ProfileData)ApplicationCache.GetItem("profile");

				var request = new EnterpriseRequest
				{
					enterprise_id = profile.company_id

				};


				var task = _baseRestApi.AddAsync<EnterpriseResponse, EnterpriseRequest>(profile.token, request, "enterprise/members");
				var userListResponse = task.GetAwaiter().GetResult();
				if (userListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(userListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (userListResponse.data != null)
				{
					if (userListResponse.data.enterprise_members.Count() > 0)
					{
						foreach (Enterprise_Members element in userListResponse.data.enterprise_members)
						{
							var member = new Member
							{
								Id = element.id,
								Role = element.role,
								Active = element.active,
								Email = element.email,
								CreateAccount = element.create_account,
								DeleteAccount = element.delete_account,
								CreateInvoice = element.create_invoice,
								SignInvoice = element.sign_invoice,
								DraftInvoice = element.draft_invoice,
								CancelInvoice = element.cancel_invoice,
								ViewInvoice = element.view_invoice,
								ViewMyInvoice = element.view_my_invoice,
								ChangeInvoice = element.change_invoice,
								ConvertInvoice = element.convert_invoice,
							};
							list.Add(member);
						}
					}
				}

				return list;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}


		}

		public EnterpriseDetail GetEnterpriseDetail()
		{
			try
			{

				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				GetEnterpriseDetailRequest request = new GetEnterpriseDetailRequest
				{
					id = ""
				};
				var task = _baseRestApi.AddAsync<EnterpriseDetail, GetEnterpriseDetailRequest>(profile.token, request, "enterprise/detail");
				var reponse = task.GetAwaiter().GetResult();
				if (reponse.code != Const.Code_Successful)
				{
					MessageBox.Show(reponse.message, "Vina invoice thông báo:");
					return null;
				}
				var enterpriseDetail = reponse;
				return enterpriseDetail;
			}
			catch
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
				return null;
			}


		}

		public Enterprise_ConfigResponse GetEnterprise_Config_Detail()
		{
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				GetEnterpriseDetailRequest request = new GetEnterpriseDetailRequest
				{
					id = ""
				};
				var task = _baseRestApi.AddAsync<Enterprise_ConfigResponse, GetEnterpriseDetailRequest>(profile.token, request, "config_enterprise/detail");

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


		
	}

	public class GetEnterpriseDetailRequest
	{
		public string id { get; set; }
	}
	public class Enterprise_ConfigResponse
	{
		public int code { get; set; }
		public string message { get; set; }
		public Enterprise_ConfigData data { get; set; }
	}

	public class Enterprise_ConfigData
	{
		public string id { get; set; }
		public string mail { get; set; }
		public string mail_user_name { get; set; }
		public string mail_password { get; set; }
		public int mail_port { get; set; }
		public int mail_ssl { get; set; }
		public string mail_smtp_server { get; set; }
		public string create_by { get; set; }
		public string create_time { get; set; }
		public string update_by { get; set; }
		public string update_time { get; set; }
		public string token_serial { get; set; }
	}

}
