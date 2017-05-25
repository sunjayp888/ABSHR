namespace HR.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AbsenceDay")]
    public partial class AbsenceDay
    {
        public int AbsenceDayId { get; set; }

        public int OrganisationId { get; set; }

        public int AbsenceId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public bool AM { get; set; }

        public bool PM { get; set; }

        public double Duration { get; set; }

        public virtual Absence Absence { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
