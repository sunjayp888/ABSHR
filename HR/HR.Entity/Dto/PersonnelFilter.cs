using System;
using System.Collections.Generic;

namespace HR.Entity.Dto
{
    public class PersonnelFilter
    {
        public List<int> CompanyIds { get; set; }
        public List<int> DepartmentIds { get; set; }
        public List<int> TeamIds { get; set; }
    }
}
