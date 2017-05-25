using System;

namespace HR.Business.Models
{
    public class ApprovalEmailOvetime : ApprovalEmail
    {
        public string DateOfOvertime { get; set; }
        public double Hours { get; set; }
        public string OvertimePreference { get; set; }
        public string Reason { get; set; }
    }
}
