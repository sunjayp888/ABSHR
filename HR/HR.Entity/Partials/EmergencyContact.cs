namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(EmergencyContactMetadata))]
    public partial class EmergencyContact : IOrganisationFilterable
    {
        private class EmergencyContactMetadata
        {
            [Display(Name = "Home Phone Number")]
            [Phone]
            public string Telephone { get; set; }

            [Display(Name = "Mobile Phone Number")]
            [Phone(ErrorMessage = "Not a valid mobile phone number")]
            public string Mobile { get; set; }

            [Display(Name = "Address")]
            public string Address1 { get; set; }

            public string Address2 { get; set; }

            public string Address3 { get; set; }

            public string Address4 { get; set; }

            [Display(Name = "Country")]
            public int? CountryId { get; set; }
        }
    }
}
