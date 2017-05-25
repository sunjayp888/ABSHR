namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(PersonnelAbsenceEntitlementMetadata))]
    public partial class PersonnelAbsenceEntitlement : IOrganisationFilterable
    {

        public string Period => string.Format("{0} - {1}", StartDate.ToLongDateString(), EndDate.ToLongDateString());

        private class PersonnelAbsenceEntitlementMetadata
        {
            
            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double Entitlement { get; set; }

            [Display(Name = "Carried Over")]
            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double CarriedOver { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double Used { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double Remaining { get; set; }

            [Display(Name = "Maximum Carry Forward")]
            [Range(0, double.MaxValue, ErrorMessage = "Please enter valid number")]
            public double MaximumCarryForward { get; set; }
            

        }
    }

}

