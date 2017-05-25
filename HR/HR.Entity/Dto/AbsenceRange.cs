using HR.Entity.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Entity.Dto
{
    public class AbsenceRange
    {
        public AbsenceRange()
        {
            ApprovalStateId = (int)ApprovalStates.Requested;
        }

        public int? AbsenceId { get; set; }

        [Required]
        public int OrganisationId { get; set; }

        public int PersonnelAbsenceEntitlementId { get; set; }

        [Display(Name = "Begin Date")]
        [Required]
        public DateTime BeginDateUtc { get; set; }

        [Display(Name = "End Date")]
        [DateGreaterThan(nameof(BeginDateUtc), true, ErrorMessage = "End Date must be greater than or equal to Begin Date")]
        public DateTime EndDateUtc { get; set; }

        [Required]
        public AbsencePart BeginAbsencePart { get; set; }

        [Required]
        public AbsencePart EndAbsencePart { get; set; }

        public decimal Days { get; set; }

        public string Description { get; set; }

        [Display(Name = "Type")]
        [Required]
        public int AbsenceTypeId { get; set; }

        public string AbsenceTypeName { get; set; }

        //[Display(Name = "Status")]
        //public int AbsenceStatusId { get; set; }

        //public AbsenceStatus AbsenceStatus { get; set; }

        [Display(Name = "Status")]
        public int ApprovalStateId { get; set; }

        public ApprovalState ApprovalState { get; set; }

        public string AbsenceStatusByUser { get; set; }

        public DateTime? AbsenceStatusDateTimeUtc { get; set; }
       

        public bool? ReturnToWorkCompleted { get; set; }

        public bool? DoctorsNoteSupplied { get; set; }

        public int PersonnelId { get; set; }

        [Display(Name = "Period")]
        [Required]
        public int AbsencePeriodId { get; set; }

        public int CountryId { get; set; }

        public int PublicHolidayPolicyId { get; set; }

        public Permissions Permissions { get; set; }

        public string Employment { get; set; }

        public bool? CanApproveAbsence { get; set; }
    }

}
