namespace HR.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AbsencePolicyPeriod")]
    public partial class AbsencePolicyPeriod
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AbsencePolicyPeriod()
        {
            PersonnelAbsenceEntitlements = new HashSet<PersonnelAbsenceEntitlement>();
        }

        public int AbsencePolicyPeriodId { get; set; }

        public int OrganisationId { get; set; }

        public int AbsencePolicyId { get; set; }

        public int AbsencePeriodId { get; set; }

        public virtual AbsencePeriod AbsencePeriod { get; set; }

        public virtual AbsencePolicy AbsencePolicy { get; set; }

        public virtual Organisation Organisation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonnelAbsenceEntitlement> PersonnelAbsenceEntitlements { get; set; }
    }
}
