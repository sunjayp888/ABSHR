namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(JobGradeMetada))]
    public partial class JobGrade : IOrganisationFilterable
    {
        private class JobGradeMetada
        {

        }
    }
}
