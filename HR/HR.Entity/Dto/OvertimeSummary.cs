using System;
using System.Collections.Generic;
using System.Linq;

namespace HR.Entity.Dto
{
    public class OvertimeSummary
    {
        public double PaidHours { get; set; }
        public double TOILHours { get; set; }
        public double TotalHours { get; set; }
        public int CompanyId { get; set; }
        public int Count { get; set; }
        public int PersonnelId { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Team> Teams { get; set; }
        public string DepartmentsArray => Departments != null ? string.Format("{0}", string.Join(", ", Departments.Select(d => d.Name))) : string.Empty;
        public string TeamsArray => Teams != null ? string.Format("{0}", string.Join(", ", Teams.Select(t => t.Name))) : string.Empty;
        public string Forenames { get; set; }
        public string Surname { get; set; }
    }
}
