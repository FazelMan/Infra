using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infra.Shared.Models
{
    public class Response
    {
        public string Message { get; set; }
        public object Result { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}
