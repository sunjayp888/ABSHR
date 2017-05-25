namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AbsenceType")]
    public partial class AbsenceType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AbsenceType()
        {
            Absences = new HashSet<Absence>();
            AbsencePolicyEntitlements = new HashSet<AbsencePolicyEntitlement>();
            PersonnelAbsenceEntitlements = new HashSet<PersonnelAbsenceEntitlement>();
        }

        public int AbsenceTypeId { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int ColourId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Absence> Absences { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbsencePolicyEntitlement> AbsencePolicyEntitlements { get; set; }

        public virtual Colour Colour { get; set; }

        public virtual Organisation Organisation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelAbsenceEntitlement> PersonnelAbsenceEntitlements { get; set; }
    }
}
