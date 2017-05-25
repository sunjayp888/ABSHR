using System.ComponentModel;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ApprovalLevelMetadata))]
    public partial class ApprovalLevel : IOrganisationFilterable
    {
        private class ApprovalLevelMetadata
        {
        }
    }

}

