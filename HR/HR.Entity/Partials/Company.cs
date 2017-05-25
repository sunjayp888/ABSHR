namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(CompanyMetadata))]
    public partial class Company : IOrganisationFilterable
    {
        [NotMapped]
        public string Hex => Colour?.Hex ?? string.Empty;

        private class CompanyMetadata
        {
            [Display(Name = "Colour")]
            [Required]
            public int? ColourId { get; set; }
        }
    }
}


