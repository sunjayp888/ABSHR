namespace HR.Business.Models
{
    public class ApprovalEmail
    {
        public virtual int PersonnelId { get; set; }
        public virtual int LevelNumber { get; set; }
        public virtual string ApprovalState { get; set; }
        public virtual string PersonnelName { get; set; }
        public virtual string Email { get; set; }
        public virtual string LinkUrl { get; set; }
        public virtual string LinkText { get; set; }
    }
}
