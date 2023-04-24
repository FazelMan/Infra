using System.Collections.Generic;

namespace Infra.Validation.Models
{
    public class FieldValidation
    {
        public object Value { get; set; }
        public string Name { get; set; }
        public List<ValidationRule> Rules { get; set; }
    }
}
