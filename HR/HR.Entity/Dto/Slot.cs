using Newtonsoft.Json;
using System;

namespace HR.Entity.Dto
{
    //Check if NEeded in the UI before removing from this class
    public class Slot
    {
        
        public virtual int AbsenceId { get; set; }

        [JsonIgnore]
        public DateTime? BeginDate { get; set; }

        [JsonIgnore]
        public virtual DateTime? EndDate { get; set; }

        [JsonIgnore]
        public DateTime? SlotBeginDate { get; set; }

        //[JsonProperty("status")]
        //public virtual AbsenceStatus AbsenceStatus { get; set; }

        [JsonProperty("status")]
        public virtual ApprovalState ApprovalState { get; set; }

        [JsonProperty("isAbsence")]
        public virtual bool IsAbsence { get; set; }

        [JsonProperty("duration")]
        public virtual double Duration { get; set; }

        [JsonProperty("days")]
        public virtual double Days { get; set; }
        
        [JsonProperty("begins")]
        public virtual int Begins { get; set; }

        [JsonProperty("beginsPM")]
        public virtual bool BeginsPM { get; set; }     

        [JsonProperty("colour")]
        public string Colour { get; set; }
        
        [JsonProperty("details")]
        public virtual string Details { get; set; }

        [JsonProperty("title")]
        public virtual string Title { get; set; }

        [JsonProperty("overflowsBefore")]
        public virtual bool OverflowsBefore { get; set;}

        [JsonProperty("overflowsAfter")]
        public virtual bool OverflowsAfter { get; set; }


    }
}
