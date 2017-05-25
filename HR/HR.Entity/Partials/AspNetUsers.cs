namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(AspNetUsersMetadata))]
    public partial class AspNetUsers : IOrganisationFilterable
    {
        private class AspNetUsersMetadata
        {
        }
    }

}

