using System.ComponentModel;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ApprovalModelMetadata))]
    public partial class ApprovalModel : IOrganisationFilterable
    {
        private class ApprovalModelMetadata
        {
        }
    }

}

