namespace HR.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonnelAbsenceEntitlement")]
    public partial class PersonnelAbsenceEntitlement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonnelAbsenceEntitlement()
        {
            EmploymentPersonnelAbsenceEntitlements = new HashSet<EmploymentPersonnelAbsenceEntitlement>();
            Absences = new HashSet<Absence>();
        }

        public int PersonnelAbsenceEntitlementId { get; set; }

        public int OrganisationId { get; set; }

        public int PersonnelId { get; set; }

        public int AbsencePolicyPeriodId { get; set; }

        public int? AbsenceTypeId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EndDate { get; set; }

        public double? Entitlement { get; set; }

        public double? CarriedOver { get; set; }

        public double? Used { get; set; }

        public double? Remaining { get; set; }

        public double? MaximumCarryForward { get; set; }

        public int FrequencyId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentPersonnelAbsenceEntitlement> EmploymentPersonnelAbsenceEntitlements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Absence> Absences { get; set; }

        public virtual AbsencePolicyPeriod AbsencePolicyPeriod { get; set; }

        public virtual AbsenceType AbsenceType { get; set; }

        public virtual Frequency Frequency { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Personnel Personnel { get; set; }
    }
}
