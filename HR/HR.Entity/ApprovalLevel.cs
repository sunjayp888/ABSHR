namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ApprovalLevel")]
    public partial class ApprovalLevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApprovalLevel()
        {
            Approvals = new HashSet<Approval>();
            ApprovalLevelUsers = new HashSet<ApprovalLevelUser>();
        }

        public int ApprovalLevelId { get; set; }

        public int OrganisationId { get; set; }

        public int ApprovalModelId { get; set; }

        [Required]
        public int LevelNumber { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual ApprovalModel ApprovalModel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Approval> Approvals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApprovalLevelUser> ApprovalLevelUsers { get; set; }
    }
}
