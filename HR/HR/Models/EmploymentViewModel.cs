using HR.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HR.Models
{
    public class EmploymentViewModel : BaseViewModel
    {
        public SelectList ReportsTo { get; set; }
        public SelectList EmploymentType { get; set; }
        public Employment Employment { get; set; }
        public SelectList AbsencePolicy { get; set; }
        public SelectList PublicHolidayPolicy { get; set; }

        public string SelectedCompanyId { get; set; }
        public string SelectedBuildingId { get; set; }

        public string SelectedJobTitleId { get; set; }
        public string SelectedJobGradeId { get; set; }
        public string AbsencePolicyName { get; set; }
        public IEnumerable<CompanyBuilding> EmploymentCompanyBuildinglist { get; set; }
        public IEnumerable<JobTitleJobGrade> JobTitleJobGrades { get; set; }
        public string CompanyName => (EmploymentCompanyBuildinglist != null) ? EmploymentCompanyBuildinglist.FirstOrDefault(r => r.Company.CompanyId == Convert.ToInt32(SelectedCompanyId))?.Company.Name : string.Empty;
        public string BuildingName => (EmploymentCompanyBuildinglist != null) ? EmploymentCompanyBuildinglist.FirstOrDefault(r => r.Building.BuildingId == Convert.ToInt32(SelectedBuildingId))?.Building.Name : string.Empty;

        public string JobTitleName => (JobTitleJobGrades != null) ? JobTitleJobGrades.FirstOrDefault(r => r.JobTitle.JobTitleId == Convert.ToInt32(SelectedJobTitleId))?.JobTitle.Name : string.Empty;
        public string JobGradeName => (JobTitleJobGrades != null) ? JobTitleJobGrades.FirstOrDefault(r => r.JobGrade.JobGradeId == Convert.ToInt32(SelectedJobGradeId))?.JobGrade.Name : string.Empty;

        public string Colour => (EmploymentCompanyBuildinglist != null) ? EmploymentCompanyBuildinglist.FirstOrDefault(r => r.Company.CompanyId == Convert.ToInt32(SelectedCompanyId))?.Company.Colour.Hex : string.Empty;

        public bool AbsencePolicyHasAbsence { get; set; } = false;
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
        public Employment PreviousEmployment { get; set; }
        public IEnumerable<WorkingPatternDay> WorkingPatternDays { get; set; }
        public bool EditPreviousEmployment => (PreviousEmployment != null && !PreviousEmployment.EndDate.HasValue);
        public DateTime? PreviousEmploymentEndDate { get; set; }
        public int PreviousAbsencePolicyId { get; set; }
    }
}