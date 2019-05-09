using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VinaInvoice.Data.DataContext;
using VinaInvoice.Model.JsonObjectModel;

namespace VinaInvoice.Common
{
    public class Excel_Utils
    {
        #region Product Import Related Excel

        public static void ProductImportExcel()
        {
            System.Windows.Forms.OpenFileDialog importProductExcelDialog = new System.Windows.Forms.OpenFileDialog();
            importProductExcelDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            importProductExcelDialog.Title = "Import Excel Invoice";

            if (importProductExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string import_excel_path = importProductExcelDialog.FileName;
                Console.WriteLine(import_excel_path);

                DataTable import_table = ProductExcel2DataTable(import_excel_path);
                List<ProductAddList> import_product_list = DataTable2ProductList(import_table);
                ProductAddListResponse response = ProductList2Server(import_product_list);
                System.Windows.MessageBox.Show(response.message.ToString());
            }
        }

        private static DataTable ProductExcel2DataTable(string import_excel_path)
        {
            DataTable table = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(import_excel_path)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                //Check if worksheet is empty
                if (worksheet.Dimension == null)
                    return table;

                List<string> columnNames = new List<string>();
                int currentColumn = 1;

                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        table.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    table.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = table.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    table.Rows.Add(newRow);
                }
            }

            Console.WriteLine("Excel to DataTable done");
            return table;
        }

        private static List<ProductAddList> DataTable2ProductList(DataTable import_table)
        {
            List<ProductAddList> product_add_list = new List<ProductAddList>();

            foreach (DataRow row in import_table.Rows)
            {
                product_add_list.Add(new ProductAddList
                {
                    item_code = row["Mã sản phẩm"].ToString(),
                    item_name = row["Hàng hóa dịch vụ"].ToString(),                    
                    unit_name = row["Đơn vị tính"].ToString(),
                    unit_price = (row["Đơn giá"].ToString() == "") ? 0 : double.Parse(row["Đơn giá"].ToString()),
                    quantity = (row["Số lượng"].ToString() == "") ? 0 : double.Parse(row["Số lượng"].ToString()),
                    item_total_amount_without_vat = (row["Tiền trước thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền trước thuế"].ToString()),
                    vat_percentage = (row["% VAT"].ToString() == "") ? 0 : int.Parse(row["% VAT"].ToString()),
                    vat_amount = (row["Tiền thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Tiền thuế VAT"].ToString()),
                    discount_percentage = (row["% Giảm giá"].ToString() == "") ? 0 : int.Parse(row["% Giảm giá"].ToString()),
                    discount_amount = (row["Tiền giảm giá"].ToString() == "") ? 0 : double.Parse(row["Tiền giảm giá"].ToString()),
                    total_amount = (row["Tổng tiền"].ToString() == "") ? 0 : double.Parse(row["Tổng tiền"].ToString()),
                    current_code = row["Đơn vị tiền tệ"].ToString()
                });
            }
            return product_add_list;
        }

        private static ProductAddListResponse ProductList2Server(List<ProductAddList> import_product_list)
        {
            var api = new ProductRestApi();
            ProductAddListResponse response = api.AddProductList(import_product_list);
            return response;
        }

        #endregion

        #region Customer Import Related Excel

        public static void CustomerImportExcel()
        {
            System.Windows.Forms.OpenFileDialog importCustomerExcelDialog = new System.Windows.Forms.OpenFileDialog();
            importCustomerExcelDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            importCustomerExcelDialog.Title = "Import Excel Invoice";

            if (importCustomerExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string import_excel_path = importCustomerExcelDialog.FileName;
                Console.WriteLine(import_excel_path);

                DataTable import_table = CustomerExcel2DataTable(import_excel_path);
                List<CustomerAddList> import_product_list = DataTable2CustomerList(import_table);
                CustomerAddListResponse response = CustomerList2Server(import_product_list);
                System.Windows.MessageBox.Show(response.message.ToString());
            }
        }

        private static DataTable CustomerExcel2DataTable(string import_excel_path)
        {
            DataTable table = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(import_excel_path)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                //Check if worksheet is empty
                if (worksheet.Dimension == null)
                    return table;

                List<string> columnNames = new List<string>();
                int currentColumn = 1;

                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        table.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    table.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = table.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    table.Rows.Add(newRow);
                }
            }

            Console.WriteLine("Excel to DataTable done");
            return table;
        }

        private static List<CustomerAddList> DataTable2CustomerList(DataTable import_table)
        {
            List<CustomerAddList> customer_add_list = new List<CustomerAddList>();

            foreach (DataRow row in import_table.Rows)
            {
                customer_add_list.Add(new CustomerAddList
                {
                    dislay_name = row["Tên khách hàng"].ToString(),
                    company_name = row["Tên công ty"].ToString(),
                    company_tax_code = row["Mã số thuế"].ToString(),
                    address = row["Địa chỉ"].ToString(),
                    phone_number = row["Số điện thoại"].ToString(),
                    fax_number = row["Số Fax"].ToString(),
                    email = row["Email"].ToString(),                
                    website = row["Website"].ToString(),
                    bank_name = row["Tên ngân hàng"].ToString(),
                    bank_number = row["Số TK ngân hàng"].ToString(),
                });
            }
            return customer_add_list;
        }

        private static CustomerAddListResponse CustomerList2Server(List<CustomerAddList> import_customer_list)
        {
            var api = new CustomerRestApi();
            CustomerAddListResponse response = api.AddCustomerList(import_customer_list);
            return response;
        }

        #endregion

        #region Invoice Import Related Excel

        public static void InvoiceImportExcelDraft()
        {
            OpenFileDialog importExcelDialog = new OpenFileDialog();
            importExcelDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            importExcelDialog.Title = "Import Excel Invoice";

            if (importExcelDialog.ShowDialog() == DialogResult.OK)
            {
                string import_excel_path = importExcelDialog.FileName;
                Console.WriteLine(import_excel_path);

                DataTable import_table = InvoiceExcel2DataTable(import_excel_path);
                try
                {
                    List<InvoiceAddListDraft> invoice_list = DataTable2ServerDraft(import_table);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(VinaInvoice.Common.Message.MSS_IMPORT_WRONG_FORMAT);
                    System.Windows.MessageBox.Show(e.ToString());
                }
            }
        }

        private static DataTable InvoiceExcel2DataTable(string excel_path)
        {
            DataTable table = new DataTable();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(excel_path)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                //Check if worksheet is empty
                if (worksheet.Dimension == null)
                    return table;

                List<string> columnNames = new List<string>();
                int currentColumn = 1;

                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    string columnName = cell.Text.Trim();

                    //check if the previous header was empty and add it if it was
                    if (cell.Start.Column != currentColumn)
                    {
                        columnNames.Add("Header_" + currentColumn);
                        table.Columns.Add("Header_" + currentColumn);
                        currentColumn++;
                    }

                    //add the column name to the list to count the duplicates
                    columnNames.Add(columnName);

                    //count the duplicate column names and make them unique to avoid the exception
                    //A column named 'Name' already belongs to this DataTable
                    int occurrences = columnNames.Count(x => x.Equals(columnName));
                    if (occurrences > 1)
                    {
                        columnName = columnName + "_" + occurrences;
                    }

                    //add the column to the datatable
                    table.Columns.Add(columnName);

                    currentColumn++;
                }

                //start adding the contents of the excel file to the datatable
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                    DataRow newRow = table.NewRow();

                    //loop all cells in the row
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }

                    table.Rows.Add(newRow);
                }
            }

            Console.WriteLine("Excel to DataTable done");
            return table;
        }

        private static List<InvoiceAddListDraft> DataTable2ServerDraft(DataTable import_table)
        {
            List<InvoiceAddListDraft> invoice_detail_list = new List<InvoiceAddListDraft>();
            List<ProductAddList> product_list = new List<ProductAddList>();
            List<CustomerAddList> customer_list = new List<CustomerAddList>();

            #region Invoice List Processing

            int invoice_count = 0;
            int item_count = 1;
            InvoiceAddListDraft tmp_invoice = new InvoiceAddListDraft();
            List<InvoiceAddListDraftData> tmp_item_list = new List<InvoiceAddListDraftData>();

            foreach (DataRow row in import_table.Rows)
            {
                //New Invoice
                if (int.Parse(row["ID"].ToString()) > invoice_count)
                {
                    //Add invoice to Invoice List
                    if (invoice_count > 0)
                    {
                        tmp_invoice.invoice_item_list = Clone(tmp_item_list);
                        invoice_detail_list.Add((InvoiceAddListDraft)tmp_invoice.Clone());
                        tmp_item_list.Clear();
                    }

                    invoice_count = int.Parse(row["ID"].ToString());
                    item_count = 1;
                    item_count++;

                    //Invoice Data Mapping
                    tmp_invoice.b_name = row["Tên khách hàng"].ToString();
                    tmp_invoice.b_company = row["Tên công ty"].ToString();
                    tmp_invoice.b_tax_code = row["Mã số thuế"].ToString();
                    tmp_invoice.b_address = row["Địa chỉ"].ToString();
                    tmp_invoice.b_phone_number = row["Số điện thoại"].ToString();
                    tmp_invoice.b_fax_number = row["Số Fax"].ToString();
                    tmp_invoice.b_email = row["Email"].ToString();
                    tmp_invoice.b_website = row["Website"].ToString();
                    tmp_invoice.b_bank_number = row["Số TK ngân hàng"].ToString();
                    tmp_invoice.b_bank_name = row["Tên ngân hàng"].ToString();

                    //tmp_invoice.invoice_form_name = row["Mẫu HĐ"].ToString();
                    //tmp_invoice.b_bank_name = row["Ký hiệu HĐ"].ToString();
                    //tmp_invoice.invoice_number = int.Parse(row["Số HĐ"].ToString());
                    //tmp_invoice.invoice_sign_date = row["Ngày HĐ"].ToString();

                    switch (row["Loại HĐ"].ToString())
                    {
                        case "NORMAL":
                            tmp_invoice.state_of_bill = 0;
                            break;
                        case "CHANGED":
                            tmp_invoice.state_of_bill = 1;
                            break;
                        case "CONVERTED":
                            tmp_invoice.state_of_bill = 2;
                            break;
                        case "ADJUSTED":
                            tmp_invoice.state_of_bill = 3;
                            break;
                        case "ADJUSTED_INC":
                            tmp_invoice.state_of_bill = 4;
                            break;
                    }
                    tmp_invoice.currency_code = row["Đơn vị tiền tệ"].ToString();
                    tmp_invoice.exchange_rate = tmp_invoice.sub_total = (row["Tỉ giá"].ToString() == "") ? 0 : double.Parse(row["Tỉ giá"].ToString());
                    switch (row["Hình thức thanh toán"].ToString())
                    {
                        case "Tiền mặt":
                            tmp_invoice.method_of_payment = 0;
                            break;
                        case "Chuyển khoản":
                            tmp_invoice.method_of_payment = 1;
                            break;
                        case "Tiền mặt / Chuyển khoản":
                            tmp_invoice.method_of_payment = 2;
                            break;
                    }

                    tmp_invoice.invoice_ref = row["Số chứng từ"].ToString();
                    tmp_invoice.original_invoice = row["Hóa đơn gốc"].ToString();
                    tmp_invoice.sub_total = (row["Tiền trước thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền trước thuế"].ToString());
                    tmp_invoice.vat_rate = int.Parse(row["% VAT"].ToString());
                    tmp_invoice.vat_amount = (row["Tiền thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Tiền thuế VAT"].ToString());
                    tmp_invoice.total_discount = (row["Giảm giá"].ToString() == "") ? 0 : double.Parse(row["Giảm giá"].ToString());
                    tmp_invoice.service_charge_percent = (row["% Phí DV"].ToString() == "") ? 0 : int.Parse(row["% Phí DV"].ToString());
                    tmp_invoice.total_service_charge = (row["Phí dịch vụ"].ToString() == "") ? 0 : double.Parse(row["Phí dịch vụ"].ToString());
                    tmp_invoice.total_amount = (row["Tổng tiền"].ToString() == "") ? 0 : float.Parse(row["Tổng tiền"].ToString());

                    //Item Data Mapping
                    tmp_item_list.Add(new InvoiceAddListDraftData
                    {
                        item_code = (row["Mã sản phẩm"] == null) ? "" : row["Mã sản phẩm"].ToString(),                        
                        item_name = row["Hàng hóa dịch vụ"].ToString(),
                        unit_name = row["Đơn vị tính"].ToString(),
                        unit_price = (row["Đơn giá"].ToString() == "") ? 0 : double.Parse(row["Đơn giá"].ToString()),
                        quantity = (row["Số lượng"].ToString() == "") ? 0 : double.Parse(row["Số lượng"].ToString()),
                        item_total_amount_without_vat = (row["Tiền chưa thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền chưa thuế"].ToString()),
                        vat_percentage = (row["% Thuế VAT"].ToString() == "") ? 0 : int.Parse(row["% VAT"].ToString()),
                        vat_amount = (row["Thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Thuế VAT"].ToString()),
                        //discount_percentage = (row["% Giảm giá"].ToString() == "") ? 0 : int.Parse(row["% Giảm giá"].ToString()),
                        discount_amount = (row["Tiền giảm giá"].ToString() == "") ? 0 : double.Parse(row["Tiền giảm giá"].ToString()),
                        total_amount = (row["Tổng cộng"].ToString() == "") ? 0 : double.Parse(row["Tổng cộng"].ToString()),
                        //current_code = row["Đơn vị tiền tệ"].ToString()
                    });

                    continue;
                }

                //Item Data Mapping
                tmp_item_list.Add(new InvoiceAddListDraftData
                {
                    item_code = row["Mã sản phẩm"].ToString(),
                    item_name = row["Hàng hóa dịch vụ"].ToString(),
                    unit_name = row["Đơn vị tính"].ToString(),
                    unit_price = (row["Đơn giá"].ToString() == "") ? 0 : double.Parse(row["Đơn giá"].ToString()),
                    quantity = (row["Số lượng"].ToString() == "") ? 0 : double.Parse(row["Số lượng"].ToString()),
                    item_total_amount_without_vat = (row["Tiền chưa thuế"].ToString() == "") ? 0 : double.Parse(row["Tiền chưa thuế"].ToString()),
                    vat_percentage = (row["% Thuế VAT"].ToString() == "") ? 0 : int.Parse(row["% Thuế VAT"].ToString()),
                    vat_amount = (row["Thuế VAT"].ToString() == "") ? 0 : double.Parse(row["Thuế VAT"].ToString()),
                    //discount_percentage = (row["% Giảm giá"].ToString() == "") ? 0 : int.Parse(row["% Giảm giá"].ToString()),
                    discount_amount = (row["Tiền giảm giá"].ToString() == "") ? 0 : double.Parse(row["Tiền giảm giá"].ToString()),
                    total_amount = (row["Tổng cộng"].ToString() == "") ? 0 : double.Parse(row["Tổng cộng"].ToString()),
                    //current_code = row["Đơn vị tiền tệ"].ToString()
                });

                //Console.WriteLine("Item num: " + item_count.ToString());
                item_count++;

                //End Row
                if (row.Table.Rows.IndexOf(row) == (import_table.Rows.Count - 1))
                {
                    tmp_invoice.invoice_item_list = Clone(tmp_item_list);
                    invoice_detail_list.Add((InvoiceAddListDraft)tmp_invoice.Clone());
                    //Console.WriteLine("End row of file");
                }
            }

            if (import_table.Rows.Count == 1)
            {
                tmp_invoice.invoice_item_list = Clone(tmp_item_list);
                invoice_detail_list.Add((InvoiceAddListDraft)tmp_invoice.Clone());
            }
            #endregion

            #region Customer and Product List Processing

            foreach (InvoiceAddListDraft invoice in invoice_detail_list)
            {
                Console.WriteLine(invoice.b_name);
                customer_list.Add(new CustomerAddList
                {
                    email = invoice.b_email,
                    dislay_name = invoice.b_name,
                    tax_code = invoice.b_tax_code,
                    address = invoice.b_address,
                    phone_number = invoice.b_phone_number,
                    fax_number = invoice.b_fax_number,
                    website = invoice.b_website,
                    bank_name = invoice.b_bank_name,
                    bank_number = invoice.b_bank_number,
                    company_name = invoice.b_company,
                });
                foreach (InvoiceAddListDraftData item in invoice.invoice_item_list)
                {
                    product_list.Add(new ProductAddList
                    {
                        item_name = item.item_name,
                        unit_name = item.unit_name,
                        unit_price = item.unit_price,
                        quantity = item.quantity,
                        item_total_amount_without_vat = item.item_total_amount_without_vat,
                        vat_percentage = item.vat_percentage,
                        vat_amount = item.vat_amount,
                        current_code = item.current_code,
                        item_type = item.item_type,
                        discount_amount = item.discount_amount,
                        total_amount = item.total_amount,
                        item_code = item.item_code
                    });
                }
            }

            #endregion

            #region API Import

            InvoiceAddListDraftResponse response_1 = InvoiceList2ServerDraft(invoice_detail_list);

            ProductAddListResponse response_2 = ProductList2Server(product_list);

            CustomerAddListResponse response_3 = CustomerList2Server(customer_list);

            System.Windows.MessageBox.Show(response_3.message.ToString());

            #endregion

            return invoice_detail_list;
        }

        private static InvoiceAddListDraftResponse InvoiceList2ServerDraft(List<InvoiceAddListDraft> invoice_list)
        {
            var api = new InvoiceRestApi();
            InvoiceAddListDraftResponse response = api.AddListDraft(invoice_list);
            return response;
        }

        public static void InvoiceImportExcelKeep()
        {
            throw new NotImplementedException();
        }

        private static List<InvoiceAddListDraft> DataTable2ServerKeep(DataTable import_table)
        {
            throw new NotImplementedException();
        }

        private static InvoiceAddListDraftResponse InvoiceList2ServerKeep(List<InvoiceAddListDraft> invoice_list)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Invoice Export Related Excel

        public static DataTable InvoiceListToDataTableDetail(Invoice_Detail_List[] list)
        {
            DataTable table = new DataTable();

            //Table type mapping
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Tên khách hàng", typeof(string));
            table.Columns.Add("Tên công ty", typeof(string));
            table.Columns.Add("Mã số thuế", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Số điện thoại", typeof(string));
            table.Columns.Add("Số Fax", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Website", typeof(string));
            table.Columns.Add("Số TK ngân hàng", typeof(string));
            table.Columns.Add("Tên ngân hàng", typeof(string));

            table.Columns.Add("Mẫu HĐ", typeof(string));
            table.Columns.Add("Ký hiệu HĐ", typeof(string));
            table.Columns.Add("Số HĐ", typeof(int));
            table.Columns.Add("Ngày HĐ", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));
            table.Columns.Add("Loại HĐ", typeof(string));
            table.Columns.Add("Đơn vị tiền tệ", typeof(string));
            table.Columns.Add("Tỉ giá", typeof(double));
            table.Columns.Add("Hình thức thanh toán", typeof(string));
            table.Columns.Add("Số chứng từ", typeof(string));
            table.Columns.Add("Hóa đơn gốc", typeof(string));

            table.Columns.Add("Tiền trước thuế", typeof(double));
            table.Columns.Add("% VAT", typeof(int));            
            table.Columns.Add("Tiền thuế VAT", typeof(double));
            table.Columns.Add("Giảm giá", typeof(double));
            table.Columns.Add("% Phí DV", typeof(int));
            table.Columns.Add("Phí dịch vụ", typeof(double));
            table.Columns.Add("Tổng tiền", typeof(double));

            table.Columns.Add("STT", typeof(int));
            table.Columns.Add("Mã sản phẩm", typeof(string));
            table.Columns.Add("Hàng hóa dịch vụ", typeof(string));
            table.Columns.Add("Đơn vị tính", typeof(string));
            table.Columns.Add("Đơn giá", typeof(double));
            table.Columns.Add("Số lượng", typeof(double));
            table.Columns.Add("Tiền chưa thuế", typeof(double));
            table.Columns.Add("% Thuế VAT", typeof(int));
            table.Columns.Add("Thuế VAT", typeof(double));
            table.Columns.Add("% Giảm giá", typeof(int));
            table.Columns.Add("Tiền giảm giá", typeof(double));
            table.Columns.Add("Tổng cộng", typeof(double));

            //int cnt_row = 1;
            for (int i = 0; i < list.Length; i++)
            {
                DataRow row = table.NewRow();
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 7, 0, 0, 0, System.DateTimeKind.Local);
                //Table Mapping
                row["ID"] = i + 1;
                row["Tên khách hàng"] = list[i].b_name;
                row["Tên công ty"] = list[i].b_company;
                row["Mã số thuế"] = list[i].b_tax_code;
                row["Địa chỉ"] = list[i].b_address;
                row["Số điện thoại"] = list[i].b_phone_number;
                row["Số Fax"] = list[i].b_fax_number;
                row["Email"] = list[i].b_email;
                row["Website"] = list[i].b_website;
                row["Số TK ngân hàng"] = list[i].b_bank_number;
                row["Tên ngân hàng"] = list[i].b_bank_number;

                row["Mẫu HĐ"] = list[i].invoice_form_name;
                row["Ký hiệu HĐ"] = list[i].invoice_serial_name;
                row["Số HĐ"] = list[i].invoice_number;
                row["Ngày HĐ"] = dtDateTime.AddSeconds(list[i].invoice_sign_date).ToLocalTime().ToString().Substring(0, 10);
                if (list[i].status == 1)
                    row["Trạng thái"] = "SIGNED";
                switch (list[i].state_of_bill)
                {
                    case 0:
                        row["Loại HĐ"] = "NORMAL";
                        break;
                    case 1:
                        row["Loại HĐ"] = "CHANGED";
                        break;
                    case 2:
                        row["Loại HĐ"] = "CONVERTED";
                        break;
                    case 3:
                        row["Loại HĐ"] = "ADJUSTED";
                        break;
                    case 4:
                        row["Loại HĐ"] = "ADJUSTED_INC";
                        break;
                }
                row["Đơn vị tiền tệ"] = list[i].currency_code;
                row["Tỉ giá"] = list[i].exchange_rate;
                switch (list[i].method_of_payment)
                {
                    case 0:
                        row["Hình thức thanh toán"] = "Tiền mặt";
                        break;
                    case 1:
                        row["Hình thức thanh toán"] = "Chuyển khoản";
                        break;
                    case 2:
                        row["Hình thức thanh toán"] = "Tiền mặt / Chuyển khoản";
                        break;
                }
                row["Số chứng từ"] = list[i].invoice_ref;
                row["Hóa đơn gốc"] = list[i].original_invoice;

                row["Tiền trước thuế"] = list[i].sub_total;
                row["% VAT"] = list[i].vat_rate;                
                row["Tiền thuế VAT"] = list[i].vat_amount;
                row["Giảm giá"] = list[i].total_discount;
                row["% Phí DV"] = list[i].service_charge_percent;
                row["Phí dịch vụ"] = list[i].total_service_charge;
                row["Tổng tiền"] = list[i].total_amount;        

                row["STT"] = 1;
                if (list[i].invoice_item_list != null && list[i].invoice_item_list.Length > 0)
                {
                    row["Mã sản phẩm"] = (list[i].invoice_item_list[0].item_code == null) ? "" : list[i].invoice_item_list[0].item_code.ToString();
                    row["Hàng hóa dịch vụ"] = list[i].invoice_item_list[0].item_name;
                    row["Đơn vị tính"] = list[i].invoice_item_list[0].unit_name;
                    row["Đơn giá"] = list[i].invoice_item_list[0].unit_price;
                    row["Số lượng"] = list[i].invoice_item_list[0].quantity;
                    row["Tiền chưa thuế"] = list[i].invoice_item_list[0].item_total_amount_without_vat;
                    row["% Thuế VAT"] = list[i].invoice_item_list[0].vat_percentage;
                    row["Thuế VAT"] = list[i].invoice_item_list[0].vat_amount;
                    //row["% Giảm giá"] = list[i].invoice_item_list[0];
                    row["Tiền giảm giá"] = list[i].invoice_item_list[0].discount_amount;
                    row["Tổng cộng"] = list[i].invoice_item_list[0].total_amount;
                }

                table.Rows.Add(row);

                //Item Mapping
                for (int j = 1; j < list[i].invoice_item_list.Length; j++)
                {
                    DataRow rw = table.NewRow();
                    rw["ID"] = i + 1;
                    rw["STT"] = j + 1;
                    rw["Mã sản phẩm"] = (list[i].invoice_item_list[j].item_code == null) ? "" : list[i].invoice_item_list[j].item_code.ToString();
                    rw["Hàng hóa dịch vụ"] = list[i].invoice_item_list[j].item_name.ToString();
                    rw["Đơn vị tính"] = list[i].invoice_item_list[j].unit_name;
                    rw["Đơn giá"] = list[i].invoice_item_list[j].unit_price;
                    rw["Số lượng"] = list[i].invoice_item_list[j].quantity;
                    rw["Tiền chưa thuế"] = list[i].invoice_item_list[j].item_total_amount_without_vat;
                    rw["% Thuế VAT"] = list[i].invoice_item_list[j].vat_percentage;
                    rw["Thuế VAT"] = list[i].invoice_item_list[j].vat_amount;
                    //rw["% Giảm giá"] = list[i].invoice_item_list[0];
                    rw["Tiền giảm giá"] = list[i].invoice_item_list[j].discount_amount;
                    rw["Tổng cộng"] = list[i].invoice_item_list[j].total_amount;
                    table.Rows.Add(rw);
                }
            }

            return table;
        }

        public static DataTable InvoiceListToDataTableNormal(Invoice_Detail_List[] list)
        {
            DataTable table = new DataTable();

            //Table type mapping
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Tên khách hàng", typeof(string));
            table.Columns.Add("Tên công ty", typeof(string));
            table.Columns.Add("Mã số thuế", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Số điện thoại", typeof(string));
            table.Columns.Add("Số Fax", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Website", typeof(string));
            table.Columns.Add("Số TK ngân hàng", typeof(string));
            table.Columns.Add("Tên ngân hàng", typeof(string));

            table.Columns.Add("Mẫu HĐ", typeof(string));
            table.Columns.Add("Ký hiệu HĐ", typeof(string));
            table.Columns.Add("Số HĐ", typeof(int));
            table.Columns.Add("Ngày HĐ", typeof(string));
            table.Columns.Add("Trạng thái", typeof(string));
            table.Columns.Add("Loại HĐ", typeof(string));
            table.Columns.Add("Đơn vị tiền tệ", typeof(string));
            table.Columns.Add("Tỉ giá", typeof(double));
            table.Columns.Add("Hình thức thanh toán", typeof(string));
            table.Columns.Add("Số chứng từ", typeof(string));
            table.Columns.Add("Hóa đơn gốc", typeof(string));

            table.Columns.Add("Tiền trước thuế", typeof(double));
            table.Columns.Add("% VAT", typeof(int));
            table.Columns.Add("Tiền thuế VAT", typeof(double));
            table.Columns.Add("Giảm giá", typeof(double));
            table.Columns.Add("% Phí DV", typeof(int));
            table.Columns.Add("Phí dịch vụ", typeof(double));
            table.Columns.Add("Tổng tiền", typeof(double));

            //int cnt_row = 1;
            for (int i = 0; i < list.Length; i++)
            {
                DataRow row = table.NewRow();
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 7, 0, 0, 0, System.DateTimeKind.Local);
                //Table Mapping
                row["ID"] = i + 1;
                row["Tên khách hàng"] = list[i].b_name;
                row["Tên công ty"] = list[i].b_company;
                row["Mã số thuế"] = list[i].b_tax_code;
                row["Địa chỉ"] = list[i].b_address;
                row["Số điện thoại"] = list[i].b_phone_number;
                row["Số Fax"] = list[i].b_fax_number;
                row["Email"] = list[i].b_email;
                row["Website"] = list[i].b_website;
                row["Số TK ngân hàng"] = list[i].b_bank_number;
                row["Tên ngân hàng"] = list[i].b_bank_number;

                row["Mẫu HĐ"] = list[i].invoice_form_name;
                row["Ký hiệu HĐ"] = list[i].invoice_serial_name;
                row["Số HĐ"] = list[i].invoice_number;
                row["Ngày HĐ"] = dtDateTime.AddSeconds(list[i].invoice_sign_date).ToLocalTime().ToString().Substring(0, 10);
                if (list[i].status == 1)
                    row["Trạng thái"] = "SIGNED";
                switch (list[i].state_of_bill)
                {
                    case 0:
                        row["Loại HĐ"] = "NORMAL";
                        break;
                    case 1:
                        row["Loại HĐ"] = "CHANGED";
                        break;
                    case 2:
                        row["Loại HĐ"] = "CONVERTED";
                        break;
                    case 3:
                        row["Loại HĐ"] = "ADJUSTED";
                        break;
                    case 4:
                        row["Loại HĐ"] = "ADJUSTED_INC";
                        break;
                }
                row["Đơn vị tiền tệ"] = list[i].currency_code;
                row["Tỉ giá"] = list[i].exchange_rate;
                switch (list[i].method_of_payment)
                {
                    case 0:
                        row["Hình thức thanh toán"] = "Tiền mặt";
                        break;
                    case 1:
                        row["Hình thức thanh toán"] = "Chuyển khoản";
                        break;
                    case 2:
                        row["Hình thức thanh toán"] = "Tiền mặt / Chuyển khoản";
                        break;
                }
                row["Số chứng từ"] = list[i].invoice_ref;
                row["Hóa đơn gốc"] = list[i].original_invoice;

                row["Tiền trước thuế"] = list[i].sub_total;
                row["% VAT"] = list[i].vat_rate;
                row["Tiền thuế VAT"] = list[i].vat_amount;
                row["Giảm giá"] = list[i].total_discount;
                row["% Phí DV"] = list[i].service_charge_percent;
                row["Phí dịch vụ"] = list[i].total_service_charge;
                row["Tổng tiền"] = list[i].total_amount;

                table.Rows.Add(row);
            }

            return table;
        }



        #endregion

        #region Others

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        #endregion
    }
}
