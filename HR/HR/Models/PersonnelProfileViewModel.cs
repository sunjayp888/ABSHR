using System.Collections.Generic;
using HR.Entity;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using HR.Business.Models;

namespace HR.Models
{
    public class PersonnelProfileViewModel : BaseViewModel
    {
        public SelectList Countries { get; set; }
        public Personnel Personnel { get; set; }
        public EmploymentViewModel EmploymentViewModel { get; set; }
        public IEnumerable<ApprovalEntityTypeAssignment> ApprovalEntityTypeAssignments { get; set; }
        public IEnumerable<ApprovalModel> ApprovalModels { get; set; }

        public List<int> SelectedDepartmentIds
        {
            get
            {
                return JsonConvert.DeserializeObject<List<int>>(SelectedDepartmentIdsJson);
            }
            set
            {
                SelectedDepartmentIdsJson = JsonConvert.SerializeObject(value);
            }
        }

        public string SelectedDepartmentIdsJson { get; set; }

        public List<int> SelectedTeamIds
        {
            get
            {
                return JsonConvert.DeserializeObject<List<int>>(SelectedTeamIdsJson);
            }
            set
            {
                SelectedTeamIdsJson = JsonConvert.SerializeObject(value);
            }
        }

        public string SelectedTeamIdsJson { get; set; }
    }
}