using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VinaInvoice.Model
{
	public class DataMigration
	{
		private  string _xmlstring;
		public string Xmlstring { get => _xmlstring; set => _xmlstring = value; }

		

	
		public DataMigration(string xmlstring)
		{
			_xmlstring = xmlstring;		    
		}

        public invoice GetItems()
        {

            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "items";
            xRoot.Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1";
            xRoot.IsNullable = true;

            XmlSerializer serializer = new XmlSerializer(typeof(invoice));
            StringReader stringReader = new StringReader(_xmlstring);
            invoice InvoiceInvoiceDataItems = (invoice)serializer.Deserialize(stringReader);

            return InvoiceInvoiceDataItems;

        }
	}
}
