using System.ComponentModel.DataAnnotations;
using HR.Entity.Interfaces;

namespace HR.Entity
{
    [MetadataType(typeof(PublicHolidayPolicyMetaData))]
    public partial class PublicHolidayPolicy : IOrganisationFilterable
    {
        private class PublicHolidayPolicyMetaData
        {

        }
    }
}
