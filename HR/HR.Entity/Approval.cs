namespace HR.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Approval")]
    public partial class Approval
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Approval()
        {
            CreatedDateUtc = DateTime.UtcNow;
        }

        public int ApprovalId { get; set; }

        public int OrganisationId { get; set; }

        public int ApprovalEntityTypeId { get; set; }

        public int EntityId { get; set; }
        
        public int ApprovalLevelId { get; set; }

        public int ApprovalStateId { get; set; }

        public DateTime? CreatedDateUtc { get; set; }

        public DateTime? UpdatedDateUtc { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual ApprovalEntityType ApprovalEntity { get; set; }

        public virtual ApprovalLevel ApprovalLevel { get; set; }

        public virtual ApprovalState ApprovalState { get; set; }

    }
}
