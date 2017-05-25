namespace HR.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JobTitleJobGrade")]
    public partial class JobTitleJobGrade
    {
        public int JobTitleJobGradeId { get; set; }

        public int JobTitleId { get; set; }

        public int JobGradeId { get; set; }

        public int OrganisationId { get; set; }

        public virtual JobGrade JobGrade { get; set; }

        public virtual JobTitle JobTitle { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
