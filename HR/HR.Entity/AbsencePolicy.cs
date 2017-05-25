namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AbsencePolicy")]
    public partial class AbsencePolicy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AbsencePolicy()
        {
            AbsencePolicyEntitlements = new HashSet<AbsencePolicyEntitlement>();
            AbsencePolicyPeriods = new HashSet<AbsencePolicyPeriod>();
            Employments = new HashSet<Employment>();
        }

        public int AbsencePolicyId { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int? WorkingPatternId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual WorkingPattern WorkingPattern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbsencePolicyEntitlement> AbsencePolicyEntitlements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbsencePolicyPeriod> AbsencePolicyPeriods { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employment> Employments { get; set; }
    }
}
