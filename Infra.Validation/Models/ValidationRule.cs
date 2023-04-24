using System.Collections.Generic;

namespace Infra.Validation.Models
{
    public class ValidationRule
    {
        public ViewValidationType Type { get; set; }
        public string ErrorMessage { get; set; }
        public string Pattern { get; set; }
        public List<string> Collection { get; set; }
    }
}
