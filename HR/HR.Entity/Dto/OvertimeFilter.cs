using System;

namespace HR.Entity.Dto
{
    public class OvertimeFilter : ApprovalFilter
    {
        public bool IsPaid { get; set; }
        public bool IsToil { get; set; }
    }
}
