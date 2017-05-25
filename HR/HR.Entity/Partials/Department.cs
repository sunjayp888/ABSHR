namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(DepartmentMetadata))]
    public partial class Department : IOrganisationFilterable
    {
        [NotMapped]
        public string Hex => Colour?.Hex ?? string.Empty;

        private class DepartmentMetadata
        {
            [Display(Name = "Colour")]
            [Required]
            public int? ColourId { get; set; }
        }
    }
}
