namespace HR.Entity
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class WorkingPattern : IOrganisationFilterable
    {

        [NotMapped]
        public double? Duration => WorkingPatternDays?.Sum(day => day.Duration);
    }
}
