using System.Collections.Generic;

namespace HR.Business.Models
{
    public class PersonnelDetailFilter
    {
        public List<CompanyFilter> Company { get; set; }
        public List<DepartmentFilter> Department { get; set; }
        public List<TeamFilter> Team { get; set; }
    }
}
