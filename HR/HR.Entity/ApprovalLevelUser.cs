namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ApprovalLevelUser")]
    public partial class ApprovalLevelUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApprovalLevelUser()
        {

        }

        public int ApprovalLevelUserId { get; set; }

        public int OrganisationId { get; set; }

        public int ApprovalLevelId { get; set; }

        [StringLength(128)]
        public string AspNetUserId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual ApprovalLevel ApprovalLevel { get; set; }

        //public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
