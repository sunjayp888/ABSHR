using System;

namespace HR.Entity.Dto
{
    public class ApprovalFilter : PersonnelFilter
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public bool IsRequested { get; set; }
        public bool IsDeclined { get; set; }
        public bool IsApproved { get; set; }
        public bool IsInApproval { get; set; }
    }
}
