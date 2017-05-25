namespace HR.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Employment")]
    public partial class Employment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employment()
        {
            EmploymentPersonnelAbsenceEntitlements = new HashSet<EmploymentPersonnelAbsenceEntitlement>();
            EmploymentDepartments = new HashSet<EmploymentDepartment>();
            EmploymentTeams = new HashSet<EmploymentTeam>();
        }

        public int EmploymentId { get; set; }

        public int OrganisationId { get; set; }

        public int PersonnelId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? TerminationDate { get; set; }

        public int BuildingId { get; set; }

        public int? ReportsToPersonnelId { get; set; }        

        public int? EndEmploymentReasonId { get; set; }

        public int? WorkingPatternId { get; set; }

        public int PublicHolidayPolicyId { get; set; }

        public int AbsencePolicyId { get; set; }

        public int CompanyId { get; set; }

        public int EmploymentTypeId { get; set; }

        public int? JobTitleId { get; set; }

        public int? JobGradeId { get; set; }

        public virtual AbsencePolicy AbsencePolicy { get; set; }

        public virtual Building Building { get; set; }

        public virtual Company Company { get; set; }

        public virtual EmploymentType EmploymentType { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Personnel Personnel { get; set; }

        public virtual Personnel ReportsToPersonnel { get; set; }

        public virtual PublicHolidayPolicy PublicHolidayPolicy { get; set; }

        public virtual WorkingPattern WorkingPattern { get; set; }

        public virtual JobTitle JobTitle { get; set; }

        public virtual JobGrade JobGrade { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentPersonnelAbsenceEntitlement> EmploymentPersonnelAbsenceEntitlements { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentDepartment> EmploymentDepartments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentTeam> EmploymentTeams { get; set; }


    }
}
