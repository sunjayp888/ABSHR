using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations;
    using Interfaces;
    using System.ComponentModel;

    [MetadataType(typeof(BuildingMetadata))]
    public partial class Building : IOrganisationFilterable
    {
      
        private class BuildingMetadata
        {

            [DisplayName("Site")]
            public int SiteId { get; set; }

            [DisplayName("Address")]
            public string Address1 { get; set; }

        }
    }
}
