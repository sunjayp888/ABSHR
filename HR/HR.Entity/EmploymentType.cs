namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmploymentType")]
    public partial class EmploymentType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmploymentType()
        {
            Employments = new HashSet<Employment>();
        }

        public int EmploymentTypeId { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual Organisation Organisation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employment> Employments { get; set; }
    }
}
