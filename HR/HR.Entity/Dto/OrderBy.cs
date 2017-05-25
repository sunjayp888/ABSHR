using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace HR.Entity.Dto
{
    public class OrderBy
    {
        public string Property { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ListSortDirection Direction { get; set; }
    }
}
