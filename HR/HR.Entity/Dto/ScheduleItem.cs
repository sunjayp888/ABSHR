using Newtonsoft.Json;
using System.Collections.Generic;

namespace HR.Entity.Dto
{
    public class ScheduleItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public int PersonnelId { get; set; }

        public Permissions Permissions { get; set; }

        public IEnumerable<Colour> Colours { get; set; }

        //change this one to slot
        [JsonProperty("slots")]
        public IEnumerable<Slot> Slots { get; set; }
    }
}
