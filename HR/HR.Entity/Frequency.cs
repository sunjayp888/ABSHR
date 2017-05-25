namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Frequency")]
    public partial class Frequency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Frequency()
        {
            AbsencePolicyEntitlements = new HashSet<AbsencePolicyEntitlement>();
            PersonnelAbsenceEntitlements = new HashSet<PersonnelAbsenceEntitlement>();
        }

        public int FrequencyId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int Periods { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbsencePolicyEntitlement> AbsencePolicyEntitlements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelAbsenceEntitlement> PersonnelAbsenceEntitlements { get; set; }
    }
}
