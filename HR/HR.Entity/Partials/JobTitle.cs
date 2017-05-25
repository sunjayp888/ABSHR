namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(JobTitleMetada))]
    public partial class JobTitle : IOrganisationFilterable
    {
        private class JobTitleMetada
        {

        }
    }
}
