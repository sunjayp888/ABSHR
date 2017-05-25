
using System.ComponentModel;
using System.Web.Mvc;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(SiteMetadata))]
    public partial class Site : IOrganisationFilterable
    {
        private class SiteMetadata
        {

            [DisplayName("Organisation")]
            public int? OrganisationId { get; set; }

            [DisplayName("Site Id")]
            public int SiteId { get; set; }

            [DisplayName("Country")]
            public int CountryId { get; set; }
        }
    }
}
