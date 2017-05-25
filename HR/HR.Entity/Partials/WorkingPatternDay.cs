using System.ComponentModel;

namespace HR.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(WorkingPatternDayMetadata))]
    public partial class WorkingPatternDay
    {
        /// Sunday = 0

        public DayOfWeek AsDayOfWeek => ((DayOfWeek)DayOfWeek);
        
        public string DayOfWeekName => Enum.GetName(typeof(DayOfWeek), DayOfWeek);

        public bool Day => AM && PM;

        public double? Duration => (AM ? 0.5 : 0) + (PM ? 0.5 : 0);

        private class WorkingPatternDayMetadata
        {
           
        }
    }
}
