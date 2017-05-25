namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(TeamMetadata))]
    public partial class Team : IOrganisationFilterable
    {
        [NotMapped]
        public string Hex => Colour?.Hex ?? string.Empty;

        private class TeamMetadata
        {
            [DisplayName("Team Id")]
            public int TeamId { get; set; }

            [Display(Name = "Colour")]
            [Required]
            public int? ColourId { get; set; }

        }
    }

}

