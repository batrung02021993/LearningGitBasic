using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VinaInvoice.Model
{
    public abstract class PropertyValidateModel : IDataErrorInfo
    {
        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                var value = GetType().GetProperty(columnName).GetValue(this);                
                var validationContext = new ValidationContext(this)
                {
                    MemberName = columnName
                };
                var validationResults = new List<ValidationResult>();

                if (Validator.TryValidateProperty(value, validationContext, validationResults))
                    return null;

                return validationResults.First().ErrorMessage;
            }
        }
    }
}
