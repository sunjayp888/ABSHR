namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ApprovalLevelUserMetadata))]
    public partial class ApprovalLevelUser : IOrganisationFilterable
    {
        private class ApprovalLevelUserMetadata
        {
        }
    }

}

