using Newtonsoft.Json;

namespace HR.Entity.Dto
{
    public class ScheduleItemType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("colour")]
        public string Colour { get; set; }
    }
}
