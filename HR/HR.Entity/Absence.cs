namespace HR.Entity
{
    using Dto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Absence")]
    public partial class Absence
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Absence()
        {
            AbsenceDays = new HashSet<AbsenceDay>();
            ApprovalStateId = (int)ApprovalStates.Requested;
        }

        public int AbsenceId { get; set; }

        public int OrganisationId { get; set; }

        public int PersonnelAbsenceEntitlementId { get; set; }

        public int AbsenceTypeId { get; set; }

        [StringLength(128)]
        public string AbsenceStatusByUser { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? AbsenceStatusDateTimeUtc { get; set; }

        public string Description { get; set; }

        public bool? ReturnToWorkCompleted { get; set; }

        public bool? DoctorsNoteSupplied { get; set; }

        public int ApprovalStateId { get; set; }

        public virtual AbsenceType AbsenceType { get; set; }

        public virtual ApprovalState ApprovalState { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual PersonnelAbsenceEntitlement PersonnelAbsenceEntitlement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbsenceDay> AbsenceDays { get; set; }
    }
}
