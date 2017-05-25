using HR.Business.Models;
using HR.Entity;
using System.Collections.Generic;

namespace HR.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public int AbsencesRequiringApproval { get; set; }

        public int OvertimeRequiringApproval { get; set; }

        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<int> SelectedCompanyIds { get; set; }        
        public string CompanyIdsArray => SelectedCompanyIds != null ? string.Format("[{0}]", string.Join(",", SelectedCompanyIds)) : "null";

        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<int> SelectedDepartmentIds { get; set; }
        public string DepartmentIdsArray => SelectedDepartmentIds != null ? string.Format("[{0}]", string.Join(",", SelectedDepartmentIds)) : "null";
        
        public IEnumerable<int> SelectedTeamIds { get; set; }
        public string DivisionIdsArray => SelectedTeamIds != null ? string.Format("[{0}]", string.Join(",", SelectedTeamIds)) : "null";
    }
}