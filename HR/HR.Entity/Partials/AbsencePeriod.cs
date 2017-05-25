namespace HR.Entity
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Validation;

    [MetadataType(typeof(AbsencePeriodMetadata))]
    public partial class AbsencePeriod : IOrganisationFilterable
    {
        [Display(Name = "Name")]   
        [NotMapped]     
        public string Name => string.Format("{0} - {1}", StartDate.ToLongDateString(), EndDate.ToLongDateString());

        public bool OverlapsWith(AbsencePeriod absencePeriod)
        {
            return absencePeriod.StartDate.Date <= EndDate.Date && StartDate.Date <= absencePeriod.EndDate.Date;
        }

        public bool OverlapsWithAny(IEnumerable<AbsencePeriod> absencePeriods)
        {
            return absencePeriods.Any(ap => ap.OverlapsWith(this));
        }

        public bool IsCurrentPeriod()
        {
            return StartDate.Date <= DateTime.Today.Date && EndDate.Date >= DateTime.Today.Date;
        }

        private class AbsencePeriodMetadata
        {
            [Display(Name = "Start Date")]
            [Required]
            public DateTime StartDate { get; set; }

            [Display(Name = "End Date")]
            [DateGreaterThan(nameof(StartDate), ErrorMessage = "End Date must be greater than Start Date")]
            public DateTime EndDate { get; set; }

        }
    }
}