namespace HR.Business.Models
{
    public class ApprovalEntityTypeAssignment
    {
        public int ApprovalEntityId { get; set; }
        public int ApprovalModelId { get; set; }
        public int PersonnelApprovalModelId { get; set; }
        public string Name { get; set; }
    }
}
