using System;

namespace HR.Entity.Dto
{
    public class OvertimeForApproval : ForApproval
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        //public int ApprovalLevelId { get; set; }
        //public int ApprovalStateId { get; set; }
        public int OvertimeId { get; set; }
        public int OvertimePreferenceId { get; set; }
        //public int LevelNumber { get; set; }
        //public int PersonnelId { get; set; }
        //public string ApprovalState { get; set; }
        //public string ApproverAspNetUserId { get; set; }
        public string Comment { get; set; }
        public string Forenames { get; set; }
        public string OvertimePreference { get; set; }
        public string Surname { get; set; }
        public string Reason { get; set; }
    }
}
