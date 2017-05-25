namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WorkingPatternDay")]
    public partial class WorkingPatternDay
    {
        public int WorkingPatternDayId { get; set; }

        public int WorkingPatternId { get; set; }

        public short DayOfWeek { get; set; }

        public bool AM { get; set; }

        public bool PM { get; set; }

        public virtual WorkingPattern WorkingPattern { get; set; }
    }
}
