namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AspNetUsers")]
    public partial class AspNetUsers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetUsers()
        {
            //ApprovalLevelUsers = new HashSet<ApprovalLevelUser>();
        }

        [StringLength(128)]
        public string Id { get; set; }

        public int OrganisationId { get; set; }

        public int? PersonnelId { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Personnel Personnel { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ApprovalLevelUser> ApprovalLevelUsers { get; set; }
    }
}
