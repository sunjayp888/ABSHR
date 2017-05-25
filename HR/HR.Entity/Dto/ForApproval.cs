using System;

namespace HR.Entity.Dto
{
    public class ForApproval
    {
        public int ApprovalId { get; set; }
        public int ApprovalLevelId { get; set; }
        public int ApprovalStateId { get; set; }
        public int EntityId { get; set; }
        public int LevelNumber { get; set; }
        public string ApproverAspNetUserId { get; set; }
        public int PersonnelId { get; set; }
        public string ApprovalState { get; set; }
        //prevent the approval or decline of request on deleting the audit logs
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDateUtc { get; set; }
    }
}
