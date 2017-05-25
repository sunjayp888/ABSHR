using System.Linq;

namespace HR.Entity
{
    using Interfaces;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Validation;

    [MetadataType(typeof(EmploymentMetadata))]
    public partial class Employment : IOrganisationFilterable
    {
        
        [NotMapped]
        public string Departments => string.Join(",", EmploymentDepartments.Select(s => s.Department.Name));

        [NotMapped]
        public string Teams => string.Join(",", EmploymentTeams.Select(s => s.Team.Name));

        //Add Property joins the department with comma seperated
        private class EmploymentMetadata
        {
            [DisplayName("Building")]
            public int BuildingId { get; set; }

            //[DisplayName("Job Title")]
            //[Required]
            //public string JobTitle { get; set; }

            //[DisplayName("Job Description")]
            //public string JobDescription { get; set; }
            [DisplayName("Employment Type")]
            public int EmploymentTypeId { get; set; }

            [Display(Name = "Start Date")]
            [Required]
            public DateTime StartDate { get; set; }

            [Display(Name = "End Date")]
            [DateGreaterThan(nameof(StartDate), true, ErrorMessage = "End Date must be greater than Start Date")]
            public DateTime? EndDate { get; set; }

            [Display(Name = "Termination Date")]
            [DateGreaterThan(nameof(StartDate), true, ErrorMessage = "Termination Date must be greater than Start Date")]
            public DateTime? TerminationDate { get; set; }

            [Display(Name = "Reports To")]
            public int? ReportsToPersonnelId { get; set; }

            [Display(Name = "Absence Policy")]
            public int AbsencePolicyId { get; set; }

            [Display(Name = "Public Holiday Policy")]
             public int PublicHolidayPolicyId { get; set; }
        }
    }
}
