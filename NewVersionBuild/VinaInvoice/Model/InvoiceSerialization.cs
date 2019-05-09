using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace VinaInvoice.Model
{

	public class InvoiceSerialization
	{
		private invoice _invoice;
		public invoice Invoice { get => _invoice; set => _invoice = value; }
		public string Templatepath = AppDomain.CurrentDomain.BaseDirectory + "Template" + "\\einvoice_template.xml";
		public string Xsltpath = AppDomain.CurrentDomain.BaseDirectory + "Xslt" + "\\Current.xslt";


		public InvoiceSerialization()
		{
			_invoice = DeserializeObject(Templatepath);
		}

		public void ReloadXML(string InvoiceDraftPath)
		{
			_invoice = DeserializeObject(InvoiceDraftPath);
		}

		public void ToInvoiceXml(string path)
		{
			//invoice bs = DeserializeObject("01GTKT0-002_VA-18E_No_0000053_13-11-2018_signed.xml");

			XmlSerializer xs = new XmlSerializer(typeof(invoice));

			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
			ns.Add("inv", "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1");

			TextWriter txtWriter = new StreamWriter(path);

			xs.Serialize(txtWriter, _invoice, ns);

			txtWriter.Close();
		}

		public invoice DeserializeObject(string path)
		{
			// Create an instance of the XmlSerializer.
			XmlSerializer serializer =
			new XmlSerializer(typeof(invoice));

			// Declare an object variable of the type to be deserialized.
			invoice i;

			using (Stream reader = new FileStream(path, FileMode.Open))
			{
				// Call the Deserialize method to restore the object's state.
				i = (invoice)serializer.Deserialize(reader);
				return i;
			}
		}

		public void ToInvoiceHtml(string inputpath, string outputpath)
		{
            try
            {
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(Xsltpath);
                xslt.Transform(inputpath, outputpath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
	}


	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
	public class invoice
	{
		public invoice()
		{
			this.invoiceData = new invoiceInvoiceData();
			this.controlData = new object();
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("invoiceData", typeof(invoiceInvoiceData))]
		public invoiceInvoiceData invoiceData { get; set; }

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("controlData", typeof(object))]
		public object controlData { get; set; }
	}


	public class invoiceInvoiceData
	{
		[System.Xml.Serialization.XmlElementAttribute("ExchangeRate", typeof(string))]
		public string ExchangeRate { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("additionalReferenceDesc", typeof(string))]
		public string additionalReferenceDesc { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("adjustmentType", typeof(string))]
		public string adjustmentType { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerAddressLine", typeof(string))]
		public string buyerAddressLine { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerBankAccount", typeof(string))]
		public string buyerBankAccount { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerBankName", typeof(string))]
		public string buyerBankName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerDisplayName", typeof(string))]
		public string buyerDisplayName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerEmail", typeof(string))]
		public string buyerEmail { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerFaxNumber", typeof(string))]
		public string buyerFaxNumber { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerLegalName", typeof(string))]
		public string buyerLegalName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerPhoneNumber", typeof(string))]
		public string buyerPhoneNumber { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("buyerTaxCode", typeof(string))]
		public string buyerTaxCode { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("contractNumber", typeof(string))]
		public string contractNumber { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("currencyCode", typeof(string))]
		public string currencyCode { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("delivery", typeof(string))]
		public string delivery { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("discountAmount", typeof(string))]
		public string discountAmount { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("discountPercent", typeof(string))]
		public string discountPercent { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceAppRecordId", typeof(string))]
		public string invoiceAppRecordId { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceIssuedDate", typeof(string))]
		public string invoiceIssuedDate { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceName", typeof(string))]
		public string invoiceName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceNote", typeof(string))]
		public string invoiceNote { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceNumber", typeof(string))]
		public string invoiceNumber { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceSeries", typeof(string))]
		public string invoiceSeries { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceSigned", typeof(string))]
		public string invoiceSigned { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("invoiceTaxBreakdowns", typeof(invoiceInvoiceDataInvoiceTaxBreakdowns))]
		public invoiceInvoiceDataInvoiceTaxBreakdowns invoiceTaxBreakdowns { get; set; } = new invoiceInvoiceDataInvoiceTaxBreakdowns();

		[System.Xml.Serialization.XmlElementAttribute("invoiceType", typeof(string))]
		public string invoiceType { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("items", typeof(invoiceInvoiceDataItems))]
		public invoiceInvoiceDataItems items { get; set; } = new invoiceInvoiceDataItems();

		[System.Xml.Serialization.XmlElementAttribute("originalInvoice", typeof(string))]
		public string originalInvoice { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("paymentMethodName", typeof(string))]
		public string paymentMethodName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("payments", typeof(invoiceInvoiceDataPayments))]
		public invoiceInvoiceDataPayments payments { get; set; } = new invoiceInvoiceDataPayments();

		[System.Xml.Serialization.XmlElementAttribute("printFlag", typeof(string))]
		public string printFlag { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("printSample", typeof(string))]
		public string printSample { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("qrCodeData", typeof(string))]
		public string qrCodeData { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("referentNo", typeof(string))]
		public string referentNo { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerAddressLine", typeof(string))]
		public string sellerAddressLine { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerAppRecordId", typeof(string))]
		public string sellerAppRecordId { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerBankAccount", typeof(string))]
		public string sellerBankAccount { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerBankName", typeof(string))]
		public string sellerBankName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerContactPersonName", typeof(string))]
		public string sellerContactPersonName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerEmail", typeof(string))]
		public string sellerEmail { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerFaxNumber", typeof(string))]
		public string sellerFaxNumber { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerLegalName", typeof(string))]
		public string sellerLegalName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerPhoneNumber", typeof(string))]
		public string sellerPhoneNumber { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerSignedPersonName", typeof(string))]
		public string sellerSignedPersonName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerSubmittedPersonName", typeof(string))]
		public string sellerSubmittedPersonName { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerTaxCode", typeof(string))]
		public string sellerTaxCode { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("sellerWebsite", typeof(string))]
		public string sellerWebsite { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("serviceChargePercent", typeof(string))]
		public string serviceChargePercent { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("signedDate", typeof(string))]
		public string signedDate { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("submittedDate", typeof(string))]
		public string submittedDate { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("systemCode", typeof(string))]
		public string systemCode { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("templateCode", typeof(string))]
		public string templateCode { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("totalAmountWithVAT", typeof(string))]
		public string totalAmountWithVAT { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("totalAmountWithVATInWords", typeof(string))]
		public string totalAmountWithVATInWords { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("totalAmountWithoutVAT", typeof(string))]
		public string totalAmountWithoutVAT { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("totalServiceCharge", typeof(string))]
		public string totalServiceCharge { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("totalVATAmount", typeof(string))]
		public string totalVATAmount { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("userDefines", typeof(string))]
		public string userDefines { get; set; }

		[System.Xml.Serialization.XmlElementAttribute("vatPercentageBill", typeof(string))]
		public string vatPercentageBill { get; set; }



		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id
		{
			get;

			set;

		}
	}


	public class invoiceInvoiceDataInvoiceTaxBreakdowns
	{
		[XmlElementAttribute("invoiceTaxBreakdown")]
		public List<invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown> InvoiceTaxBreakdowns { get; set; } = new List<invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown>();
	}


	public class invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown
	{

		[System.Xml.Serialization.XmlElementAttribute("vatPercentage", typeof(string))]
		public string vatPercentage;

		[System.Xml.Serialization.XmlElementAttribute("vatTaxableAmount", typeof(string))]
		public string vatTaxableAmount;

		[System.Xml.Serialization.XmlElementAttribute("vatTaxAmount", typeof(string))]
		public string vatTaxAmount;
	}

	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
	public class invoiceInvoiceDataItems
	{

		private List<invoiceInvoiceDataItemsItem> itemField = new List<invoiceInvoiceDataItemsItem>();

		[XmlElementAttribute("item")]
		public List<invoiceInvoiceDataItemsItem> items
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}
	}


	public class invoiceInvoiceDataItemsItem
	{
		public string currency { get; set; }
		public string itemCode { get; set; }
		public string itemDiscount { get; set; }
		public string itemName { get; set; }
		public string itemTotalAmountWithoutVat { get; set; }
		public string lineNumber { get; set; }
		public string quantity { get; set; }
		public string totalAmount { get; set; }
		public string unitName { get; set; }
		public string unitPrice { get; set; }
		public string vatAmount { get; set; }
		public string vatPercentage { get; set; }
		// public string subTotal { get; set; }

	}


	public class invoiceInvoiceDataPayments
	{

		private List<invoiceInvoiceDataPaymentsPayment> paymentField = new List<invoiceInvoiceDataPaymentsPayment>();

		[XmlElementAttribute("payment")]
		public List<invoiceInvoiceDataPaymentsPayment> payments
		{
			get
			{
				return this.paymentField;
			}
			set
			{
				this.paymentField = value;
			}
		}
	}


	public class invoiceInvoiceDataPaymentsPayment
	{
		/// <remarks/>
		public string paymentMethodNameExt { get; set; }

	}
}
