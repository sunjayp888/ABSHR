using HR.Entity;
using HR.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HR.Data.Interfaces
{
    public interface IHRDataService
    {

        // Create
        Absence CreateAbsence(int organisationId, Absence absence);
        IEnumerable<AbsenceDay> CreateAbsenceDays(int organisationId, IEnumerable<AbsenceDay> absenceDays);
        AbsencePeriod CreateAbsencePeriod(int organisationId, AbsencePeriod absencePeriod);
        AbsenceType CreateAbsenceType(int organisationId, AbsenceType absenceType);
        Building CreateBuilding(int organisationId, Building building);
        Company CreateCompany(int organisationId, Company company);
        Country CreateCountry(int organisationId, Country country);
        Department CreateDepartment(int organisationId, Department department);
        CompanyBuilding CreateCompanyBuilding(int organisationId, int conpanyId, int buildingId);
        EmergencyContact CreateEmergencyContact(int organisationId, EmergencyContact emergencyContact);
        Employment CreateEmployment(int organisationId, Employment employment);
        Personnel CreatePersonnel(int organisationId, Personnel personnel);
        IEnumerable<PersonnelAbsenceEntitlement> CreatePersonnelAbsenceEntitlements(int organisationId, IEnumerable<PersonnelAbsenceEntitlement> personnelAbsenceEntitlement);
        Site CreateSite(int organisationId, Site site);
        WorkingPattern CreateWorkingPattern(int organisationId, WorkingPattern workingPattern);
        WorkingPattern CreateWorkingPatternWithDays(int organisationId, IEnumerable<WorkingPatternDay> workingPatternDays);
        PublicHolidayPolicy CreatePublicHolidayPolicy(int organisationId, PublicHolidayPolicy publicHolidayPolicy);
        AbsencePolicy CreateAbsencePolicy(int organisationId, AbsencePolicy absencePolicy);
        AbsencePolicyEntitlement CreateAbsencePolicyEntitlement(int organisationId, int absencePolicyId, AbsencePolicyEntitlement absencepolicyEntitlement);
        WorkingPattern CreateAbsencePolicyWorkingPattern(int organisationId, AbsencePolicy absencePolicy, IEnumerable<WorkingPatternDay> workingPatternDays);
        AbsencePolicyPeriod CreateAbsencePolicyAbsencePeriod(int organisationId, AbsencePolicyPeriod absencePolicyPeriod);
        JobTitleJobGrade CreateJobTitleJobGrade(int organisationId, JobTitleJobGrade jobTitleJobGrade);
        IEnumerable<PublicHoliday> CreatePublicHolidays(int organisationId, IEnumerable<PublicHoliday> publicHolidays);
        T Create<T>(int organisationId, T t) where T : class;
        void Create<T>(int organisationId, IEnumerable<T> t) where T : class;


        // Retrieve
        Absence RetrieveAbsence(int organisationId, int absenceId);
        IEnumerable<Absence> RetrieveAbsences(int organisationId, int personnelId, int absencePeriodId);
        IEnumerable<Absence> RetrieveAbsences(int organisationId, Expression<Func<Absence, bool>> predicate, List<OrderBy> orderBy = null);
        IEnumerable<Absence> RetrieveAbsences(int organisationId, DateTime beginDate, DateTime endDate, PersonnelFilter personnelFilter);
        PagedResult<Absence> RetrieveAbsenceTransactions(int organisationId, ApprovalFilter approvalFilter, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<AbsenceForApproval> RetrieveAbsenceForApprovals(int organisationId, string approverAspNetUserId, bool isAdmin, Expression<Func<AbsenceForApproval, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        AbsencePeriod RetrieveAbsencePeriod(int organisationId, int absencePeriodId);
        PagedResult<AbsencePeriod> RetrieveAbsencePeriods(int organisationId, Expression<Func<AbsencePeriod, bool>> predicate, List<OrderBy> orderBy, Paging paging);
        IEnumerable<AbsenceDay> RetrieveAbsenceRangeBookedAbsenceDays(AbsenceRange absenceRange);
        PersonnelApprovalModel PersonnelApprovalModels(int organisationId, int personnelId, ApprovalTypes approvalTypes);
        IEnumerable<Approver> RetrieveNextApprovers(int organisationId, ApprovalTypes approvalEntities, int entityId);
        AbsenceType RetrieveAbsenceType(int organisationId, int absenceTypeId, Expression<Func<AbsenceType, bool>> predicate);
        PagedResult<AbsenceType> RetrieveAbsenceTypes(int organisationId, Expression<Func<AbsenceType, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        ApprovalModel RetrieveApprovalModel(int organisationId, int ApprovalModelId);
        Building RetrieveBuilding(int organisationId, int buildingId, Expression<Func<Building, bool>> predicate);
        IEnumerable<Colour> RetrieveColours(Expression<Func<Colour, bool>> predicate);
        PagedResult<Building> RetrieveBuildings(int organisationId, Expression<Func<Building, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        Company RetrieveCompany(int organisationId, int companyId, Expression<Func<Company, bool>> predicate);
        IEnumerable<Company> RetrieveCompanies(int organisationId, IEnumerable<int> companyIds);
        PagedResult<Company> RetrieveCompanies(int organisationId, Expression<Func<Company, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        Country RetrieveCountry(int organisationId, int countryId, Expression<Func<Country, bool>> predicate);
        PagedResult<Country> RetrieveCountries(int organisationId, Expression<Func<Country, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        IEnumerable<Employment> RetrieveCurrentEmployments(int organisationId);
        IEnumerable<Employment> RetrievePersonnelEmployments(int organisationId, int personnelId);
        IEnumerable<Employment> RetrieveActiveEmploymentsByAbsencePolicy(int organisationId, int absencePolicyId);
        IEnumerable<Employment> RetrieveEmployments(int organisationId, Expression<Func<Employment, bool>> predicate);
        IEnumerable<EmploymentDepartment> RetrieveEmploymentDepartments(int organisationId, int employmentId);
        IEnumerable<EmploymentTeam> RetrieveEmploymentTeams(int organisationId, int employmentId);
        Department RetrieveDepartment(int organisationId, int departmentId, Expression<Func<Department, bool>> predicate);
        IEnumerable<Department> RetrieveDepartments(int organisationId, IEnumerable<int> departmentIds);
        PagedResult<Department> RetrieveDepartments(int organisationId, Expression<Func<Department, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        IEnumerable<Host> RetrieveHosts();
        IEnumerable<Absence> RetrieveManagerAbsencesRequiringApproval(int organisationId, List<int> personnelIds);
        IEnumerable<Organisation> RetrieveOrganisations();
        Overtime RetrieveOvertime(int organisationId, int overtimeId);
        PagedResult<Overtime> RetrieveOvertimes(int organisationId, Expression<Func<Overtime, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<Overtime> RetrieveOvertimeTransactions(int organisationId, OvertimeFilter overtimeFilter, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<OvertimeForApproval> RetrieveOvertimeForApprovals(int organisationId, string approverAspNetUserId, bool isAdmin, Expression<Func<OvertimeForApproval, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<OvertimeSummary> RetrieveOvertimeSummaries(int organisationId, Expression<Func<OvertimeSummary, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        Personnel RetrievePersonnel(int organisationId, int personnelId, Expression<Func<Personnel, bool>> predicate);
        IEnumerable<Personnel> RetrievePersonnel(int organisationId, IEnumerable<int> companyIds, IEnumerable<int> departmentIds, IEnumerable<int> teamIds);
        PagedResult<Personnel> RetrievePersonnel(int organisationId, Expression<Func<Personnel, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        EmergencyContact RetrieveEmergencyContact(int organisationId, int emergencyContactId, Expression<Func<EmergencyContact, bool>> predicate);
        bool RetrievePersonnelAbsenceEntitlementExists(int organisationId, int personnelId);
        PersonnelAbsenceEntitlement RetrievePersonnelAbsenceEntitlement(int organisationId, int personnelId, int personnelAbsenceEntitlementId);
        IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, int personnelId);
        IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, int personnelId, int employmentId);
        IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, Expression<Func<PersonnelAbsenceEntitlement, bool>> predicate);
        Employment RetrievePersonnelCurrentEmployment(int organisationId, int personnelId);
        IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelCurrentAbsenceEntitlements(int organisationId, int personnelId, int employmentId);
        Employment RetrievePersonnelEmployment(int organisationId, int personnelId, int employmentId);

        IEnumerable<PersonnelPublicHoliday> RetrievePersonnelPublicHolidayInDateRange(int organisationId, DateTime beginDate, DateTime endDate);
        Site RetrieveSite(int organisationId, int siteId, Expression<Func<Site, bool>> predicate);
        PagedResult<Site> RetrieveSites(int organisationId, Expression<Func<Site, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        Team RetrieveTeam(int organisationId, int teamId, Expression<Func<Team, bool>> predicate);
        IEnumerable<Team> RetrieveTeams(int organisationId, IEnumerable<int> teamIds);
        PagedResult<Team> RetrieveTeams(int organisationId, Expression<Func<Team, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        UserAuthorisationFilter RetrieveUserAuthorisation(string aspNetUserId);
        WorkingPattern RetrievePersonnelWorkingPattern(int organisationId, int personnelId);
        PagedResult<PersonnelSearchField> RetrievePersonnelBySearchKeyword(int organisationId, string searchKeyword, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<PublicHolidayPolicy> RetrievePublicHolidayPolicies(int organisationId, Expression<Func<PublicHolidayPolicy, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<AbsencePolicy> RetrieveAbsencePolicies(int organisationId, Expression<Func<AbsencePolicy, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<AbsencePolicyEntitlement> RetrieveAbsencePolicyEntitlements(int organisationId, int absencePolicyId, List<OrderBy> orderBy, Paging paging);
        T Retrieve<T>(int organisationId, int Id) where T : class;
        List<T> Retrieve<T>(int organisationId, Expression<Func<T, bool>> predicate) where T : class;
        PagedResult<T> RetrievePagedResult<T>(int organisationId, Expression<Func<T, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null) where T : class;
        IEnumerable<CompanyBuilding> RetrieveCompanyBuilding(int organisationId, Expression<Func<CompanyBuilding, bool>> predicate);
        IEnumerable<Building> RetrieveBuildingsSitesUnassignedCompany(int organisationId, int companyId);
        AbsencePolicyEntitlement RetrieveAbsencePolicyEntitlement(int organisationId, int absencePolicyEntitlementId);
        AbsencePolicy RetrieveAbsencePolicy(int organisationId, int absencePolicyId);
        bool RetrieveAbsencesOfAbsencePolicyPeriod(int organisationId, int absencePolicyPeriodId);
        bool RetrieveAbsencesOfAbsencePolicyAbsenceType(int organisationId, int absencePolicyId, int absenceTypeId);
        PagedResult<AbsencePolicyPeriod> RetrieveAbsencePolicyAbsencePeriods(int organisationId, int absencePolicyId, List<OrderBy> orderBy = null, Paging paging = null);
        PagedResult<PublicHoliday> RetrievePublicHolidays(int organisationId, int publicHolidayPolicyId, Expression<Func<PublicHoliday, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        bool IsAbsenceTypeMappedToAbsence(int organisationId, int absenceTypeId);
        bool PersonnelEmploymentHasAbsences(int organisationId, int personnelId, int employmentId);
        bool AbsencePolicyPersonnelEmploymentHasAbsences(int organisationId, int employmentId, int absencePolicyId);
        JobGrade RetrieveJobGrade(int organisationId, int jobGradeId, Expression<Func<JobGrade, bool>> predicate);
        IEnumerable<JobGrade> RetrieveJobGrades(int organisationId, IEnumerable<int> jobGradeIds);
        PagedResult<JobGrade> RetrieveJobGrades(int organisationId, Expression<Func<JobGrade, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        JobTitle RetrieveJobTitle(int organisationId, int jobTitleId, Expression<Func<JobTitle, bool>> predicate);
        IEnumerable<JobTitle> RetrieveJobTitles(int organisationId, IEnumerable<int> jobTitleIds);
        PagedResult<JobTitle> RetrieveJobTitles(int organisationId, Expression<Func<JobTitle, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null);
        Employment RetrieveEmployment(int organisationId, int personnelId, DateTime dateTimeNow);
        IEnumerable<EmploymentPersonnelAbsenceEntitlement> RetrieveEmploymentPersonnelAbsenceEntitlements(int organisationId, Expression<Func<EmploymentPersonnelAbsenceEntitlement, bool>> predicate);
        IEnumerable<JobTitleJobGrade> RetrieveJobTitleJobGrade(int organisationId, int jobTitleId);
        PagedResult<JobGrade> RetrieveJobTitleJobGrade(int organisationId, int jobTitleId, List<OrderBy> orderBy, Paging paging);
        IEnumerable<JobTitleJobGrade> RetrieveJobTitleJobGrade(int organisationId, Expression<Func<JobTitleJobGrade, bool>> predicate);
        IEnumerable<AbsenceType> RetrieveAbsenceTypes(int organisationId, int ansencePolicyId);
        IEnumerable<PublicHoliday> RetrievePublicHolidays(int organisationId, int publicHolidayPolicyId);
        WorkingPattern RetrieveWorkingPattern(int organisationId, int workingPatternId);
        PagedResult<AbsencePolicyPeriod> RetrieveAbsencePolicyAbsencePeriodsByPersonnel(int organisationId, int personnelId, List<OrderBy> orderBy = null, Paging paging = null);
        // Update

        void UpdateAbsencePolicyWorkingPattern(int organisationId, int workingPatternId, List<WorkingPatternDay> workingPatternDays);
        T UpdateEntityEntry<T>(T t) where T : class;
        T UpdateOrganisationEntityEntry<T>(int organisationId, T t) where T : class;


        // Delete
        void DeleteAbsence(int organisationId, Absence absence);
        void DeleteAbsenceDays(int organisationId, IEnumerable<AbsenceDay> absenceDays);
        void DeleteAbsencePeriod(int organisationId, int id);
        void DeleteAbsenceType(int organisationId, int id);
        void DeleteBuilding(int organisationId, int id);
        void DeleteCountry(int organisationId, int id);
        void DeleteCompany(int organisationId, int id);
        void DeleteDepartment(int organisationId, int id);
        void DeleteEmergencyContact(int organisationId, int id);
        void DeleteEmploymentTeam(int organisationId, int id);
        void DeletePersonnelAbsenceEntitlements(int organisationId, int personnelId, int employmentId);
        void DeletePersonnel(int organisationId, int id);
        void DeleteSite(int organisationId, int id);
        void DeleteEmployment(int organisationId, int id);
        void DeleteTeam(int organisationId, int id);
        void DeleteWorkingPattern(int organisationId, int workingPatternId);
        void DeleteAbsencePolicy(int organisationId, int id);
        void Delete<T>(int organisationId, int Id) where T : class;
        void Delete<T>(int organisationId, Expression<Func<T, bool>> predicate) where T : class;
        void DeleteCompanyBuilding(int organisationId, int id);
        void DeletePublicHoliday(int organisationId, int id);
        void DeletePublicHolidayPolicy(int organisationId, int id);
        void DeleteAbsencePolicyPeriod(int organisationId, int id);
        void DeletePersonnelAbsenceEntitlementsForAbsencePolicyPeriod(int organisationId, int absencePolicyPeriodId);
        void DeletePersonnelAbsenceEntitlementsForAbsencePolicyAbsenceType(int organisationId, int absencePolicyId, int absenceTypeId);
        void DeleteAbsencePolicyAbsenceType(int organisationId, int id);
        void DeleteRange<T>(int organisationId, Expression<Func<T, bool>> predicate) where T : class;
        void DeleteJobGrade(int organisationId, int id);
        void DeleteJobTitle(int organisationId, int id);
        void DeleteJobTitleJobGrade(int organisationId, int jobTitleId, int jobGradeId);
    }
}
