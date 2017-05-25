using System.ComponentModel;

namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ApprovalMetadata))]
    public partial class Approval : IOrganisationFilterable
    {
        private class ApprovalMetadata
        {
        }
    }

}

