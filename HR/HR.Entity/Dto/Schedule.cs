using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HR.Entity.Dto
{
    public class Schedule
    {
        [JsonProperty("beginDate")]
        public DateTime BeginDate { get; set; }

        [JsonProperty("items")]
        public IEnumerable<ScheduleItem> Items { get; set; }

        [JsonProperty("types")]
        public IEnumerable<ScheduleItemType> Types { get; set; }
    }
}
