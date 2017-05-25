using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(AbsencePolicyEntitlementMetadata))]
    public partial class AbsencePolicyEntitlement : IOrganisationFilterable
    {

        [NotMapped]
        public DateTime? StartDate { get; set; }

        [NotMapped]
        public DateTime? EndDate { get; set; }

        //[NotMapped]
        //[Display(Name = "Has Entitlement")]
        //public bool HasEntitlement => DivisionCountryAbsenceTypeEntitlementId > 0;

        private class AbsencePolicyEntitlementMetadata
        {
            [Display(Name = "Frequency")]
            public int FrequencyId { get; set; }

            [Display(Name = "Maximum Carry Forward")]
            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double MaximumCarryForward { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double Entitlement { get; set; }

            [Display(Name = "Is Unplanned")]
            public bool IsUnplanned { get; set; }

            [Display(Name = "Is Paid")]
            public bool IsPaid { get; set; }

            [Display(Name = "Has Entitlement")]
            public bool HasEntitlement { get; set; }

        }
    }
}

