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
	public class InvoiceRestApi
	{
		private BaseRestApi _baseRestApi = new BaseRestApi();

		public IEnumerable<Form> GetListForm(int _page)
		{
			try
			{
				List<Form> list = new List<Form>();
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var request = new InvoiceFormRequest
				{
					page = _page,
					enterprise_id = profile.company_id
				};

				var task = _baseRestApi.AddAsync<InvoiceFormResponse, InvoiceFormRequest>(profile.token, request, "invoice/form/list");
				var ListResponse = task.GetAwaiter().GetResult();
				if (ListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (ListResponse.code == Const.Code_Successful) return ListResponse.data.invoice_form_list;
				else return null;
			}
			catch(Exception e)
			{
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}
			
		}

		public FormDetailResponse GetForm(FormDetailRequest request)
		{
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<FormDetailResponse, FormDetailRequest>(profile.token, request, "invoice/form/detail");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}
			
		}

		public IEnumerable<InvoiceSerial> GetListSerial(int _page)
		{
			try
			{

				List<InvoiceSerial> list = new List<InvoiceSerial>();
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var request = new InvoiceFormRequest
				{
					page = _page,
					enterprise_id = profile.company_id,
					type_search = "using"
				};

				var task = _baseRestApi.AddAsync<InvoiceSerialListResponse, InvoiceFormRequest>(profile.token, request, "invoice/serial/list");
				var ListResponse = task.GetAwaiter().GetResult();
				if (ListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (ListResponse.data != null)
				{
					list = ListResponse.data.invoice_serial_list.ToList();
					return list;
				}

				else return null;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public IEnumerable<Invoice> GetLisInvoice(int _page, int[] _status, int[] state_of_bill)
		{
			try
			{
				List<Invoice> list = new List<Invoice>();
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var request = new InvoiceListRequest
				{
					page = _page,
					status = _status,
					state_of_bill = state_of_bill
				};

				var task = _baseRestApi.AddAsync<InvoiceListResponse, InvoiceListRequest>(profile.token, request, "invoice/invoice/list");
				var ListResponse = task.GetAwaiter().GetResult();
				if (ListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (ListResponse.data.invoice_invoice_list != null)
				{
					return ListResponse.data.invoice_invoice_list;
				}
				else
					return null;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

        public IEnumerable<Invoice> GetListInvoice(int _page, int[] _status, int[] _state_of_bill, string _start_date, string _stop_date)
        {
			try
			{
				List<Invoice> list = new List<Invoice>();
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var request = new InvoiceListDetailRequest
				{
					page = _page,
					status = _status,
					state_of_bill = _state_of_bill,
					start_date = _start_date,
					stop_date = _stop_date,
					get_detail = false
				};

				var task = _baseRestApi.AddAsync<InvoiceListResponse, InvoiceListDetailRequest>(profile.token, request, "invoice/invoice/list");
				var ListResponse = task.GetAwaiter().GetResult();
				if (ListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
					return null;
				}
				if (ListResponse.data.invoice_invoice_list != null)
				{
					return ListResponse.data.invoice_invoice_list;
				}
				else
					return null;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
        }

        public InvoiceDetailResponse GetInvoiceDetail(string id)
		{
			try
			{
				var request = new InvoiceIdRequest
				{
					id = id
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceDetailResponse, InvoiceIdRequest>(profile.token, request, "invoice/invoice/detail");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

		public Invoice_Detail_List[] GetInvoiceListDetail(string start, string stop)
		{
			try
			{
				var request = new InvoiceListDetailRequest
				{
					status = new int[] { 1 },
					state_of_bill = new int[] { 0, 1, 2, 3, 4 },
					get_detail = true
				};
				if (start != "")
					request.start_date = start;
				if (stop != "")
					request.stop_date = stop;

				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceListDetailResponse, InvoiceListDetailRequest>(profile.token, request, "invoice/invoice/list");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response.data.invoice_detail_list;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

		public InvoiceSigntoKeepResponse ChangeInvoiceSignToKeep(string id , string serectPassword)
		{
			try
			{
				var request = new InvoiceIdRequest
				{
					id = id,
					serect_password = serectPassword
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceSigntoKeepResponse, InvoiceIdRequest>(profile.token, request, "invoice/invoice/serect_fix");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

		public SerialDetail GetInvoiceSerialDetail(string _id)
		{
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				GetInvoiceSerialDetailRequest request = new GetInvoiceSerialDetailRequest
				{
					id = _id
				};
				var task = _baseRestApi.AddAsync<SerialDetail, GetInvoiceSerialDetailRequest>(profile.token, request, "invoice/serial/detail");
				var reponse = task.GetAwaiter().GetResult();
				var invoiceSerialDetail = reponse;
				if (reponse.code != Const.Code_Successful)
				{
					MessageBox.Show(reponse.message, "Vina invoice thông báo:");
					return null;
				}
				return invoiceSerialDetail;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

		public InvoiceCreateResponse Create(Invoice request)
		{
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceCreateResponse, Invoice>(profile.token, request, "invoice/invoice/add");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

        public InvoiceAddListDraftResponse AddListDraft(List<InvoiceAddListDraft> invoice_list)
        {
            
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceAddListDraftResponse, InvoiceAddListDraft[]>(profile.token, invoice_list.ToArray(), "invoice/invoice/add_list");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceCreateResponse Edit(Invoice request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceCreateResponse, Invoice>(profile.token, request, "invoice/invoice/edit");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;

			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceUpdateXMLResponse SaveXML(InvoiceUpdateXMLRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceUpdateXMLResponse, InvoiceUpdateXMLRequest>(profile.token, request, "invoice/invoice/update_xml");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceChangeResponse Change(InvoiceChangeRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceChangeResponse, InvoiceChangeRequest>(profile.token, request, "invoice/invoice/change");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceChangeResponse DeleteLogic(InvoiceChangeRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceChangeResponse, InvoiceChangeRequest>(profile.token, request, "invoice/invoice/delete");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceChangeResponse Adjust(InvoiceChangeRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceChangeResponse, InvoiceChangeRequest>(profile.token, request, "invoice/invoice/adjust");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceChangeResponse AdjustReport(InvoiceChangeRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceChangeResponse, InvoiceChangeRequest>(profile.token, request, "invoice/invoice/adjust_report");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceConvertResponse Convert(InvoiceConvertRequest request)
		{
			try
			{

				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceConvertResponse, InvoiceConvertRequest>(profile.token, request, "invoice/invoice/convert");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceSignListResponse SignListDraft(InvoiceSignListRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceSignListResponse, InvoiceSignListRequest>(profile.token, request, "invoice/invoice/sign_list");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public InvoiceDocumentResponse GetDucumentContent(InvoiceDocumentRequest request)
		{
			
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceDocumentResponse, InvoiceDocumentRequest>(profile.token, request, "invoice/invoice/get_document_content");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				return Response;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		public IEnumerable<Invoice> Search(int _page, int _type_search, int _type_sort, int _sort_method, string _key, int[] _status, int[] _state_of_bill, string _start_date, string _stop_date)
		{
			
			try
			{
				List<Invoice> list = new List<Invoice>();
				InvoiceInvoiceSearchRequest request = new InvoiceInvoiceSearchRequest
				{
					page = _page,
					type_search = _type_search,
					type_sort = _type_sort,
					sort_method = _sort_method,
					key = _key,
					status = _status,
					state_of_bill = _state_of_bill,
                    start_date = _start_date,
                    stop_date = _stop_date,
                    get_detail = false
				};

				var profile = (ProfileData)ApplicationCache.GetItem("profile");

				var task = _baseRestApi.AddAsync<InvoiceInvoiceSearchResponse, InvoiceInvoiceSearchRequest>(profile.token, request, "invoice/invoice/search");
				var ListResponse = task.GetAwaiter().GetResult();

				if (ListResponse.code != Const.Code_Successful)
				{
					MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
					return null;
				}

				if (ListResponse.data.invoice_invoice_list != null)
				{
					return ListResponse.data.invoice_invoice_list;
				}
				else
					return null;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}
		}

        public IEnumerable<Invoice> Search(int _page, int _type_search, int _type_sort, int _sort_method, string _key, int[] _status, int[] _state_of_bill, string _start_date, string _stop_date, bool get_detail)
        {
            try
            {
                List<Invoice> list = new List<Invoice>();
                InvoiceInvoiceSearchRequest request = new InvoiceInvoiceSearchRequest
                {
                    page = _page,
                    type_search = _type_search,
                    type_sort = _type_sort,
                    sort_method = _sort_method,
                    key = _key,
                    status = _status,
                    state_of_bill = _state_of_bill,
                    start_date = _start_date,
                    stop_date = _stop_date,
                    get_detail = get_detail
                };

                var profile = (ProfileData)ApplicationCache.GetItem("profile");

                var task = _baseRestApi.AddAsync<InvoiceInvoiceSearchResponse, InvoiceInvoiceSearchRequest>(profile.token, request, "invoice/invoice/search");
                var ListResponse = task.GetAwaiter().GetResult();

                if (ListResponse.code != Const.Code_Successful)
                {
                    MessageBox.Show(ListResponse.message, "Vina invoice thông báo:");
                    return null;
                }

                if (ListResponse.data.invoice_invoice_list != null)
                {
                    return ListResponse.data.invoice_invoice_list;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
            }
        }

        public string Delete(string[] delete_list)
		{
			try
			{
				var request = new InvoiceDeleteRequest
				{
					id = delete_list
				};
				var profile = (ProfileData)ApplicationCache.GetItem("profile");
				var task = _baseRestApi.AddAsync<InvoiceDeleteResponse, InvoiceDeleteRequest>(profile.token, request, "invoice/invoice/delete_draft");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				string result = Response.data.ToString();


				return result;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


			
		}

        public InvoiceKeepDate GetDayOfInvoiceKeep(string id)
        {
            
			try
			{
				var profile = (ProfileData)ApplicationCache.GetItem("profile");

				var request = new InvoiceKeepDateRequest
				{
					invoice_id = id,
					enterprise_id = profile.company_id
				};
				var task = _baseRestApi.AddAsync<InvoiceKeepDateResponse, InvoiceKeepDateRequest>(profile.token, request, "invoice/invoice/get_days_inv_keep");
				var Response = task.GetAwaiter().GetResult();
				if (Response.code != Const.Code_Successful)
				{
					MessageBox.Show(Response.message, "Vina invoice thông báo:");
					return null;
				}
				InvoiceKeepDate result = Response.data;

				return result;
			}
			catch (Exception e)
            {
				MessageBox.Show(Const.Internet_ERROR, "Vina invoice thông báo:");
                InvoiceErrorClient.invoiceErrorClient(e, false);
                return null;
			}


		}

		#region class  for Json request

		public class InvoiceSignListRequest
		{
			public List<string> id { get; set; }
			public string invoice_serial_name { get; set; }
			public string invoice_form_name { get; set; }
			public string invoice_serial_id { get; set; }
			public string invoice_form_id { get; set; }
			public string invoice_sign_date { get; set; }
		}


		public class InvoiceSignListResponse
		{
			public int code { get; set; }
			public string message { get; set; }
			public SignListData data { get; set; }
		}

		public class SignListData
		{
			public SuccessResult[] SuccessList { get; set; }
			public string[] FailList { get; set; }
		}

		public class SuccessResult
		{
			public string invoice_id { get; set; }
			public int invoice_number { get; set; }
			public string search_invoice_id { get; set; }
		}



		public class GetInvoiceSerialDetailRequest
		{
			public string id { get; set; }
		}

		public class InvoiceDetailResponse
		{
			public int code { get; set; }
			public string message { get; set; }
			public Invoice data { get; set; }
		}


		public class InvoiceSigntoKeepResponse
		{
			public int code { get; set; }
			public string message { get; set; }
			public object data { get; set; }
		}

		public class InvoiceIdRequest
		{
			public string id { get; set; }
            public string serect_password { get; set; }

        }

        #endregion

    }
}
