using HR.Entity;
using System.Collections.Generic;
using System.Linq;

namespace HR.Business.Models
{
    public class ApprovalUser
    {
        public int ApprovalLevelUserId { get; set; }
        public List<Department> Departments { get; set; }
        public string AspNetUserId { get; set; }
        public string Forenames { get; set; }
        public string Fullname { get; set; }
        public string DepartmentsArray => Departments != null ? string.Format("{0}", string.Join(", ", Departments.Select(d => d.Name))) : string.Empty;
        public string Surname { get; set; }
        public string Title { get; set; }
    }
}
