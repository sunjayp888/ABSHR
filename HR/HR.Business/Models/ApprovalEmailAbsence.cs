using System;

namespace HR.Business.Models
{
    public class ApprovalEmailAbsence : ApprovalEmail
    {
        //public bool AM { get; set; }
        //public bool PM { get; set; }
        public int Duration { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Description { get; set; }
        public string AbsenceType { get; set; }
    }
}
