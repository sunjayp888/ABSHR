namespace HR.Entity.Dto
{
    public class Approver
    {
        public int ApprovalId { get; set; }
        public int ApprovalLevelId { get; set; }
        public int ApprovalStateId { get; set; }
        public int LevelNumber { get; set; }
        public int PersonnelId { get; set; }
        public string AspNetUserId { get; set; }
        public string Email { get; set; }
    }
}
