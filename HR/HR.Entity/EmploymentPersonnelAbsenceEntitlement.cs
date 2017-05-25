namespace HR.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmploymentPersonnelAbsenceEntitlement")]
    public partial class EmploymentPersonnelAbsenceEntitlement
    {
        public int EmploymentPersonnelAbsenceEntitlementId { get; set; }

        public int EmploymentId { get; set; }

        public int PersonnelAbsenceEntitlementId { get; set; }

        public int OrganisationId { get; set; }

        public virtual Employment Employment { get; set; }

        public virtual PersonnelAbsenceEntitlement PersonnelAbsenceEntitlement { get; set; }
    }
}
