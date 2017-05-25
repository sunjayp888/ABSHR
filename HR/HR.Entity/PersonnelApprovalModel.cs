namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PersonnelApprovalModel")]
    public partial class PersonnelApprovalModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonnelApprovalModel()
        {
        }

        public int PersonnelApprovalModelId { get; set; }

        public int OrganisationId { get; set; }

        public int PersonnelId { get; set; }

        public int ApprovalModelId { get; set; }

        public int ApprovalEntityTypeId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Personnel Personnel { get; set; }

        public virtual ApprovalModel ApprovalModel { get; set; }

        public virtual ApprovalEntityType ApprovalEntity { get; set; }
    }
}
