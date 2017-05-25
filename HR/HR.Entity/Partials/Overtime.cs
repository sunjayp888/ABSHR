using System.ComponentModel;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(OvertimeMetadata))]
    public partial class Overtime : IOrganisationFilterable
    {
        private class OvertimeMetadata
        {
            [DisplayName("Preference")]
            [Required]
            public int OvertimePreferenceId { get; set; }
        }

        [NotMapped]
        public bool? CanApproveOvertime { get; set; }
    }

}

