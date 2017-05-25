using System.ComponentModel.DataAnnotations;
using HR.Entity.Interfaces;

namespace HR.Entity
{
    [MetadataType(typeof(AbsencePolicyMetadata))]
    public partial class AbsencePolicy : IOrganisationFilterable
    {
        private class AbsencePolicyMetadata
        {

        }
    }
}

