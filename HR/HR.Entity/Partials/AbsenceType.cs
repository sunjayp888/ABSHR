
namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(AbsenceTypeMetadata))]
    public partial class AbsenceType : IOrganisationFilterable
    {

        [NotMapped]
        public string Hex => Colour?.Hex ?? string.Empty;

        private class AbsenceTypeMetadata
        {
            [Display(Name = "Colour")]
            [Required]
            public int? ColourId { get; set; }
        }
    }
}