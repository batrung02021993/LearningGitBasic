using VinaInvoice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Data.DataContext
{
    public class RemoteDataContext
    {
        public object Get<T>()
        {
            Type itemType = typeof(T);
            return null;
        }

        public object Get<T>(object id)
        {
            Type itemType = typeof(T);
            if (itemType == typeof(Profile))
            {
                var api = new UserRestApi();

                return (object)api.GetProfile(id);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<T> GetList<T>(int page)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Product))
            {
                var api = new ProductRestApi();

                var listobject = api.GetListProduct(page);
                List<T> listT = new List<T>();
                int count = 0;
                if (listobject != null)
                {
                    count++;
                    foreach (Product item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;

            }

            if (itemType == typeof(Form))
            {
                var api = new InvoiceRestApi();

                var listobject = api.GetListForm(page);
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;
            }

            if (itemType == typeof(Member))
            {
                var api = new EnterpriseRestApi();

                var listobject = api.GetListMember();
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;
            }

            if (itemType == typeof(Customer))
            {
                var api = new CustomerRestApi();

                var listobject = api.GetList(page);
                List<T> listT = new List<T>();
                int count = 0;
                if (listobject != null)
                {
                    count++;
                    foreach (Customer item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;

            }

            if (itemType == typeof(InvoiceSerial))
            {
                var api = new InvoiceRestApi();

                var listobject = api.GetListSerial(page);
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;
            }

            return null;
        }

        public IEnumerable<T> GetList<T>(int page, int[] status, int[] state_of_bill)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Invoice))
            {
                var api = new InvoiceRestApi();

                var listobject = api.GetLisInvoice(page, status, state_of_bill);
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;
            }
            return null;
        }

        public IEnumerable<T> GetList<T>(int page, int[] status, int[] state_of_bill, string start_date, string stop_date)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Invoice))
            {
                var api = new InvoiceRestApi();
                var listobject = api.GetListInvoice(page, status, state_of_bill, start_date, stop_date);
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else return null;
            }
            return null;
        }

        public string Create<T>(T entity)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Invoice))
            {
                var api = new CustomerRestApi();
                var p = (object)entity;
                return api.Create((Invoice)p);
            }
            if (itemType == typeof(Customer))
            {
                var api = new CustomerRestApi();
                var p = (object)entity;
                return api.Create((Customer)p);
            }
			
			if (itemType == typeof(Product))
			{
				var api = new ProductRestApi();
				var p = (object)entity;
				return  api.Create((Product)p);				
			}

            return null;
        }

        public void Update<T>(T entity)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Product))
            {
                var api = new ProductRestApi();
                var p = (object)entity;
                api.Update((Product)p);
            }
            if (itemType == typeof(Customer))
            {
                var api = new CustomerRestApi();
                var p = (object)entity;
                api.Update((Customer)p);
            }

        }

        public string Delete<T>(IEnumerable<T> delete_list)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Product))
            {
                var api = new ProductRestApi();
                List<Product> _delete_list = new List<Product>();
                int count = 0;

                if (delete_list.ToList() != null)
                {
                    foreach (T item in delete_list.ToList())
                    {
                        var product = (Product)(object)item;
                        if (product.IsSelected == true)
                        {
                            _delete_list.Add(product);
                            count++;
                        }
                    }
                    string[] id_list = new string[count];
                    count = 0;
                    foreach (Product item in _delete_list)
                    {
                        id_list[count] = item.id;
                        count++;
                    }
                    return api.Delete(id_list);
                }
                else return null;
            }

            if (itemType == typeof(Customer))
            {
                var api = new CustomerRestApi();
                List<Customer> _delete_list = new List<Customer>();
                int count = 0;

                if (delete_list.ToList() != null)
                {
                    foreach (T item in delete_list.ToList())
                    {
                        var customer = (Customer)(object)item;
                        if (customer.IsSelected == true)
                        {
                            _delete_list.Add(customer);
                            count++;
                        }
                    }
                    string[] id_list = new string[count];
                    count = 0;
                    foreach (Customer item in _delete_list)
                    {
                        id_list[count] = item.Id;
                        count++;
                    }
                    return api.Delete(id_list);
                }
                else return null;
            }

            if (itemType == typeof(Invoice))
            {
                var api = new InvoiceRestApi();
                List<Invoice> _delete_list = new List<Invoice>();
                int count = 0;

                if (delete_list.ToList() != null)
                {
                    foreach (T item in delete_list.ToList())
                    {
                        var Invoice = (Invoice)(object)item;
                        if (Invoice.IsSelected == true)
                        {
                            _delete_list.Add(Invoice);
                            count++;
                        }
                    }
                    string[] id_list = new string[count];
                    count = 0;
                    foreach (Invoice item in _delete_list)
                    {
                        id_list[count] = item.id;
                        count++;
                    }
                    return api.Delete(id_list);
                }
                else return null;
            }

            return null;
        }

        public IEnumerable<T> Search<T>(int page, int type_search, int type_sort, int sort_method, string key)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Product))
            {
                var api = new ProductRestApi();

                var listobject = api.Search(page, type_search, key);
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else
                    return null;
            }

            if (itemType == typeof(Customer))
            {
                var api = new CustomerRestApi();

                var listobject = api.Search(page, type_search, key);
                List<T> listT = new List<T>();
                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else
                    return null;
            }

            return null;
        }

        public IEnumerable<T> Search<T>(int page, int type_search, int type_sort, int sort_method, string key, int[] status, int[] state_of_bill, string start_date, string stop_date)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Invoice))
            {
                var api = new InvoiceRestApi();

                var listobject = api.Search(page, type_search, type_sort, sort_method, key, status, state_of_bill, start_date, stop_date);

                List<T> listT = new List<T>();

                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else
                    return null;
            }

            return null;

        }

        public IEnumerable<T> Search<T>(int page, int type_search, int type_sort, int sort_method, string key, int[] status, int[] state_of_bill, string start_date, string stop_date, bool get_detail)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Invoice))
            {
                var api = new InvoiceRestApi();

                var listobject = api.Search(page, type_search, type_sort, sort_method, key, status, state_of_bill, start_date, stop_date, get_detail);

                List<T> listT = new List<T>();

                if (listobject != null)
                {
                    foreach (var item in listobject)
                    {
                        var ob = (object)item;
                        listT.Add((T)ob);
                    }
                    return listT;
                }
                else
                    return null;
            }

            return null;

        }

    }
}

