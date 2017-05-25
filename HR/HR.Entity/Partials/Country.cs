using HR.Entity.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace HR.Entity
{
    [MetadataType(typeof(CountryMetadata))]
    public partial class Country : IOrganisationFilterable
    {
        private class CountryMetadata
        {

        }
    }
}
