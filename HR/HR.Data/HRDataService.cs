using HR.Data.Extensions;
using HR.Data.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace HR.Data
{
    public partial class HRDataService : IHRDataService
    {
        private IHRDatabaseFactory _databaseFactory;
        private TransactionScope ReadUncommitedTransactionScope => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

        public HRDataService(IHRDatabaseFactory factory)
        {
            _databaseFactory = factory;
        }

        #region // Create
        public Absence CreateAbsence(int organisationId, Absence absence)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context.Absences.Add(absence);
                context.SaveChanges();

                return RetrieveAbsence(organisationId, result.AbsenceId);
            }
        }

        public IEnumerable<AbsenceDay> CreateAbsenceDays(int organisationId, IEnumerable<AbsenceDay> absenceDays)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context.AbsenceDays.AddRange(absenceDays);
                context.SaveChanges();

                return result;
            }
        }

        public AbsencePeriod CreateAbsencePeriod(int organisationId, AbsencePeriod absencePeriod)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                absencePeriod = context.AbsencePeriods.Add(absencePeriod);
                context.SaveChanges();
                return absencePeriod;
            }
        }

        public AbsenceType CreateAbsenceType(int organisationId, AbsenceType absenceType)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                absenceType = context.AbsenceTypes.Add(absenceType);
                context.SaveChanges();

                return absenceType;
            }
        }

        public Building CreateBuilding(int organisationId, Building building)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                building = context.Buildings.Add(building);
                context.SaveChanges();
                return building;
            }
        }

        public Company CreateCompany(int organisationId, Company company)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                company = context.Companies.Add(company);
                context.SaveChanges();
                return company;
            }
        }

        public Country CreateCountry(int organisationId, Country country)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                country = context.Countries.Add(country);
                context.SaveChanges();
                return country;
            }
        }

        //public CountryAbsenceType CreateCountryAbsenceType(int organisationId, CountryAbsenceType countryAbsenceType)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        countryAbsenceType = context.CountryAbsenceTypes.Add(countryAbsenceType);
        //        context.SaveChanges();
        //        return countryAbsenceType;
        //    }
        //}

        //public CountryPublicHoliday CreateCountryPublicHoliday(int organisationId, CountryPublicHoliday countryPublicHoliday)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        countryPublicHoliday = context.CountryPublicHolidays.Add(countryPublicHoliday);
        //        context.SaveChanges();
        //        return countryPublicHoliday;
        //    }
        //}

        public Department CreateDepartment(int organisationId, Department department)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                department = context.Departments.Add(department);
                context.SaveChanges();

                return department;
            }
        }

        //public Division CreateDivision(int organisationId, Division division)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        division = context.Divisions.Add(division);

        //        context.SaveChanges();
        //        return division;
        //    }
        //}

        //public DivisionCountryAbsencePeriod CreateDivisionCountryAbsencePeriod(int organisationId, DivisionCountryAbsencePeriod divisionCountryAbsencePeriod)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        divisionCountryAbsencePeriod = context.DivisionCountryAbsencePeriods.Add(divisionCountryAbsencePeriod);
        //        context.SaveChanges();
        //        return divisionCountryAbsencePeriod;
        //    }
        //}

        //public DivisionCountryAbsenceTypeEntitlement CreateDivisionCountryAbsenceTypeEntitlement(int organisationId, DivisionCountryAbsenceTypeEntitlement divisionCountryAbsenceTypeEntitlement)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        divisionCountryAbsenceTypeEntitlement = context.DivisionCountryAbsenceTypeEntitlements.Add(divisionCountryAbsenceTypeEntitlement);
        //        context.SaveChanges();
        //        return RetrieveDivisionCountryAbsenceTypeEntitlement(organisationId, divisionCountryAbsenceTypeEntitlement.DivisionId, divisionCountryAbsenceTypeEntitlement.CountryAbsenceTypeId);
        //    }
        //}

        //public DivisionCountryWorkingPattern CreateDivisionCountryWorkingPattern(int organisationId, DivisionCountryWorkingPattern divisionCountryWorkingPattern, IEnumerable<WorkingPatternDay> workingPatternDays)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        var workingPattern = context.WorkingPatterns.Add(new WorkingPattern
        //        {
        //            WorkingPatternDays = workingPatternDays.ToList()
        //        });

        //        divisionCountryWorkingPattern.WorkingPatternId = workingPattern.WorkingPatternId;

        //        divisionCountryWorkingPattern = context.DivisionCountryWorkingPatterns.Add(divisionCountryWorkingPattern);

        //        context.SaveChanges();
        //        return RetrieveDivisionCountryWorkingPattern(organisationId, divisionCountryWorkingPattern.DivisionId, divisionCountryWorkingPattern.CountryId);
        //    }
        //}

        //public DivisionSite CreateDivisionSite(int organisationId, DivisionSite divisionSite)
        //{
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        divisionSite = context.DivisionSites.Add(divisionSite);

        //        context.SaveChanges();
        //        return divisionSite;
        //    }
        //}

        public EmergencyContact CreateEmergencyContact(int organisationId, EmergencyContact emergencyContact)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                emergencyContact = context.EmergencyContacts.Add(emergencyContact);
                context.SaveChanges();

                return emergencyContact;
            }

        }

        public Employment CreateEmployment(int organisationId, Employment employment)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                employment = context.Employments.Add(employment);

                context.SaveChanges();
                return RetrievePersonnelEmployment(organisationId, employment.PersonnelId, employment.EmploymentId);
            }
        }

        public Personnel CreatePersonnel(int organisationId, Personnel personnel)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                personnel = context.Personnels.Add(personnel);
                context.SaveChanges();

                return personnel;
            }
        }

        public IEnumerable<PersonnelAbsenceEntitlement> CreatePersonnelAbsenceEntitlements(int organisationId, IEnumerable<PersonnelAbsenceEntitlement> personnelAbsenceEntitlement)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                personnelAbsenceEntitlement = context.PersonnelAbsenceEntitlements.AddRange(personnelAbsenceEntitlement);
                context.SaveChanges();
                return personnelAbsenceEntitlement;
            }
        }

        //public PublicHoliday CreatePublicHoliday(PublicHoliday publicHoliday)
        //{
        //    using (var context = _databaseFactory.Create())
        //    {
        //        publicHoliday = context.PublicHolidays.Add(publicHoliday);
        //        context.SaveChanges();
        //        return publicHoliday;
        //    }
        //}

        public Site CreateSite(int organisationId, Site site)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                site = context.Sites.Add(site);
                context.SaveChanges();
                return site;
            }
        }

        public WorkingPattern CreateWorkingPattern(int organisationId, WorkingPattern workingPattern)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                workingPattern = context.WorkingPatterns.Add(workingPattern);
                context.SaveChanges();
                return workingPattern;
            }
        }

        public WorkingPattern CreateWorkingPatternWithDays(int organisationId, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var workingPattern = context.WorkingPatterns.Add(new WorkingPattern
                {
                    WorkingPatternDays = workingPatternDays.ToList()
                });
                context.SaveChanges();
                return workingPattern;
            }
        }

        public IEnumerable<WorkingPatternDay> CreateWorkingPatternDays(int organisationId, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var days = context.WorkingPatternDays.AddRange(workingPatternDays);
                context.SaveChanges();
                return days;
            }
        }
        public CompanyBuilding CreateCompanyBuilding(int organisationId, int companyId, int buildingId)
        {
            var companyBuilding = new CompanyBuilding
            {
                OrganisationId = organisationId,
                CompanyId = companyId,
                BuildingId = buildingId

            };
            using (var context = _databaseFactory.Create(organisationId))
            {
                companyBuilding = context.CompanyBuildings.Add(companyBuilding);
                context.SaveChanges();
                return companyBuilding;
            }
        }

        public T Create<T>(int organisationId, T t) where T : class
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                t = context.Set<T>().Add(t);
                context.SaveChanges();
                return t;
            }
        }

        public void Create<T>(int organisationId, IEnumerable<T> t) where T : class
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                context.Set<T>().AddRange(t);
                context.SaveChanges();
            }
        }
        #endregion

        #region // Retrieve


        public bool IsAbsenceTypeMappedToAbsence(int organisationId, int absenceTypeId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context.Absences.Any(e => e.AbsenceTypeId == absenceTypeId);
                // || e.AbsenceType.CountryAbsenceTypes.Any(c => c.AbsenceTypeId == absenceTypeId));
            }
        }

        //public bool CanDeleteDivisionSite(int organisationId, int siteId, int divisionId)
        //{
        //    using (ReadUncommitedTransactionScope)
        //    using (var context = _databaseFactory.Create(organisationId))
        //    {
        //        return context
        //            .Employments
        //            .Include(a => a.Building)
        //            .Any(a => a.DivisionId == divisionId && a.Building.SiteId == siteId);

        //    }
        //}
        public Absence RetrieveAbsence(int organisationId, int absenceId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Absences
                    .Include(a => a.AbsenceDays)
                    //.Include(a => a.AbsenceStatus)
                    .Include(a => a.ApprovalState)
                    .Include(a => a.AbsenceType)
                    .Include(a => a.PersonnelAbsenceEntitlement)
                    .Include(a => a.PersonnelAbsenceEntitlement.Personnel)
                    .Include(a => a.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements.Select(e => e.Employment))
                    .AsNoTracking()
                    .FirstOrDefault(a => a.AbsenceId == absenceId);
            }
        }

        public IEnumerable<Absence> RetrieveAbsences(int organisationId, int personnelId, int absencePeriodId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                    .Absences
                    .Include(a => a.AbsenceDays)
                    .Include(a => a.AbsenceType)
                    //.Include(a => a.AbsenceStatus)
                    .Include(a => a.ApprovalState)
                    .Include(a => a.PersonnelAbsenceEntitlement)
                    .Include(a => a.PersonnelAbsenceEntitlement.AbsencePolicyPeriod)
                    //     .Include(a => a.PersonnelAbsenceEntitlement.Employment)
                    .Include(a => a.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements.Select(e => e.Employment))
                    .AsNoTracking()
                    .Where(a => a.PersonnelAbsenceEntitlement.PersonnelId == personnelId
                    && a.PersonnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePeriodId == absencePeriodId)
                    .OrderBy(a => a.AbsenceDays.Select(d => d.Date).FirstOrDefault())
                    .ToList();
                return result;

            }
        }

        public IEnumerable<Absence> RetrieveAbsences(int organisationId, Expression<Func<Absence, bool>> predicate, List<OrderBy> orderBy = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Absences
                    .Include(a => a.AbsenceDays)
                    .Include(a => a.AbsenceType)
                    //.Include(a => a.AbsenceStatus)
                    .Include(a => a.ApprovalState)
                    .Include(a => a.PersonnelAbsenceEntitlement)
                    //     .Include(a => a.PersonnelAbsenceEntitlement.Employment)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "AbsenceDays.Date", Direction = System.ComponentModel.ListSortDirection.Descending }
                    })
                    .ToList();
            }
        }

        public IEnumerable<Absence> RetrieveAbsences(int organisationId, DateTime beginDate, DateTime endDate, PersonnelFilter personnelFilter)
        {

            //var declinedAbsenceStatusId = AbsenceStatus.Status.Declined.GetHashCode();
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absences = context
                    .Absences
                    .Include(a => a.AbsenceDays)
                    .Include(a => a.AbsenceType)
                    .Include(a => a.AbsenceType.Colour)
                    //.Include(a => a.AbsenceStatus)
                    .Include(a => a.ApprovalState)
                    .Include(a => a.PersonnelAbsenceEntitlement)
                    .Include(a => a.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements)
                    .AsNoTracking()
                    .Where(a => //a.AbsenceStatusId != declinedAbsenceStatusId &&
                                a.ApprovalStateId != (int)ApprovalStates.Declined &&
                                a.AbsenceDays.Any(d => DbFunctions.TruncateTime(d.Date) >= DbFunctions.TruncateTime(beginDate) &&
                                                       DbFunctions.TruncateTime(d.Date) <= DbFunctions.TruncateTime(endDate)));

                if (personnelFilter.CompanyIds != null && personnelFilter.CompanyIds.Any())
                    absences = absences.Where(a => a.PersonnelAbsenceEntitlement.
                        EmploymentPersonnelAbsenceEntitlements.Any(e => personnelFilter.CompanyIds.Contains(e.Employment.CompanyId)));

                if (personnelFilter.DepartmentIds != null && personnelFilter.DepartmentIds.Any())
                    absences = absences.Where(a => a.PersonnelAbsenceEntitlement.
                        EmploymentPersonnelAbsenceEntitlements.All(
                            d => d.Employment.EmploymentDepartments.Any(e => personnelFilter.DepartmentIds.Contains(e.DepartmentId))));
                //absences = absences.Where(a=>a.)

                if (personnelFilter.TeamIds != null && personnelFilter.TeamIds.Any())
                    absences = absences.Where(a => a.PersonnelAbsenceEntitlement.
                         EmploymentPersonnelAbsenceEntitlements.All(
                             d => d.Employment.EmploymentTeams.Any(e => personnelFilter.TeamIds.Contains(e.TeamId))));

                return absences.ToList();
            }

        }

        public PagedResult<Absence> RetrieveAbsenceTransactions(int organisationId, ApprovalFilter approvalFilter, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                Expression<Func<Absence, bool>> predicate = (o => (approvalFilter.IsRequested || o.ApprovalStateId != (int)ApprovalStates.Requested)
                    && (approvalFilter.IsDeclined || o.ApprovalStateId != (int)ApprovalStates.Declined) && (approvalFilter.IsApproved || o.ApprovalStateId != (int)ApprovalStates.Approved)
                    && (approvalFilter.IsInApproval || o.ApprovalStateId != (int)ApprovalStates.InApproval) && o.AbsenceDays.Any(ad => ad.Date >= approvalFilter.Begin) && o.AbsenceDays.Any(ad => ad.Date >= approvalFilter.End)
                );

                var overtimeTransactions = context
                    .Absences
                    .Include(o => o.ApprovalState)
                    .Include(o => o.AbsenceDays)
                    .Include(o => o.PersonnelAbsenceEntitlement.Personnel)
                    .AsNoTracking()
                    .Where(predicate);

                if (approvalFilter.CompanyIds != null && approvalFilter.CompanyIds.Any())
                    overtimeTransactions = overtimeTransactions.Where(ot => ot.PersonnelAbsenceEntitlement.Personnel.Employments.Any(c => approvalFilter.CompanyIds.Contains(c.CompanyId)));

                if (approvalFilter.DepartmentIds != null && approvalFilter.DepartmentIds.Any())
                    overtimeTransactions = overtimeTransactions.Where(ot => ot.PersonnelAbsenceEntitlement.Personnel.Employments.Any(c => c.EmploymentDepartments.Any(d => approvalFilter.DepartmentIds.Contains(d.DepartmentId))));

                if (approvalFilter.TeamIds != null && approvalFilter.TeamIds.Any())
                    overtimeTransactions = overtimeTransactions.Where(ot => ot.PersonnelAbsenceEntitlement.Personnel.Employments.Any(c => c.EmploymentTeams.Any(d => approvalFilter.TeamIds.Contains(d.TeamId))));

                return overtimeTransactions
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "Date", Direction = System.ComponentModel.ListSortDirection.Descending }
                    })
                    .Paginate(paging);
            }
        }

        public PagedResult<AbsenceForApproval> RetrieveAbsenceForApprovals(int organisationId, string approverAspNetUserId, bool isAdmin, Expression<Func<AbsenceForApproval, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                //get all not declined approvals
                var forApprovals = (
                    from a in context.Approvals
                    join al in context.ApprovalLevels on a.ApprovalLevelId equals al.ApprovalLevelId
                    where a.ApprovalStateId != (int)ApprovalStates.Declined && a.ApprovalEntityTypeId == (int)ApprovalTypes.Absence
                    select new ForApproval
                    {
                        ApprovalId = a.ApprovalId,
                        ApprovalLevelId = a.ApprovalLevelId,
                        ApprovalStateId = a.ApprovalStateId,
                        EntityId = a.EntityId,
                        LevelNumber = al.LevelNumber
                    });
                //get approvals with max level approved
                var forApprovalWithApproved = forApprovals
                    .Where(fa => fa.ApprovalStateId == (int)ApprovalStates.Approved)
                    .OrderBy(fa => fa.LevelNumber)
                    .GroupBy(fa => fa.EntityId)
                    .Select(o => o.OrderByDescending(fa => fa.LevelNumber).FirstOrDefault());
                //get approvals with the least level unapproved
                forApprovals = forApprovals
                    .Where(fa => !forApprovalWithApproved.Select(fawa => fawa.EntityId).Contains(fa.EntityId) ||
                        forApprovalWithApproved.FirstOrDefault(fawa => fawa.EntityId == fa.EntityId).LevelNumber < fa.LevelNumber)
                    .OrderBy(fa => fa.LevelNumber)
                    .GroupBy(fa => fa.EntityId)
                    .Select(fa => fa.FirstOrDefault());

                var absenceConflict = (
                    from a in context.Absences
                        .Include(a => a.AbsenceDays)
                        .Include(a => a.PersonnelAbsenceEntitlement)
                        .Include(a => a.PersonnelAbsenceEntitlement.Personnel)
                        .Include(a => a.PersonnelAbsenceEntitlement.Personnel.Employments)
                    where a.ApprovalStateId != (int)ApprovalStates.Declined
                    select new
                    {
                        PersonnelId = a.PersonnelAbsenceEntitlement.PersonnelId,
                        AbsenceDateFrom = a.AbsenceDays.Min(ad => ad.Date),
                        AbsenceDateTo = a.AbsenceDays.Max(ad => ad.Date),
                        TeamIds = context.EmploymentTeams.Where(et => a.PersonnelAbsenceEntitlement.Personnel.Employments
                            .Where(e => !e.TerminationDate.HasValue)
                            .Select(e => e.EmploymentId).Contains(et.EmploymentId)
                        )
                    });

                var absences = (
                    from a in context.Absences
                        .Include(a => a.AbsenceDays)
                        .Include(a => a.AbsenceType)
                        .Include(a => a.ApprovalState)
                        .Include(a => a.PersonnelAbsenceEntitlement)
                    join fa in forApprovals on a.AbsenceId equals fa.EntityId
                    join alu in context.ApprovalLevelUsers on fa.ApprovalLevelId equals alu.ApprovalLevelId
                    join p in context.Personnels.Include(p => p.Employments) on a.PersonnelAbsenceEntitlement.PersonnelId equals p.PersonnelId
                    where a.ApprovalStateId == (int)ApprovalStates.Requested || a.ApprovalStateId == (int)ApprovalStates.InApproval
                    select new AbsenceForApproval
                    {
                        DateFrom = a.AbsenceDays.Min(ad => ad.Date),
                        DateTo = a.AbsenceDays.Max(ad => ad.Date),
                        AbsenceId = a.AbsenceId,
                        AbsenceTypeId = a.AbsenceTypeId,
                        NumberOfDays = a.AbsenceDays.Sum(ad => ad.Duration),
                        PersonnelAbsenceEntitlementId = a.PersonnelAbsenceEntitlement.PersonnelAbsenceEntitlementId,
                        Conflicts = absenceConflict.Where(ac =>
                            ac.TeamIds.Any(aca => context.EmploymentTeams.Where(et => a.PersonnelAbsenceEntitlement.Personnel.Employments.Where(e => !e.TerminationDate.HasValue)
                                .Select(e => e.EmploymentId).Contains(et.EmploymentId)).Any(aet => aet.TeamId == aca.TeamId))
                            && ac.PersonnelId != p.PersonnelId && (a.AbsenceDays.Any(aad => aad.Date == ac.AbsenceDateFrom || aad.Date == ac.AbsenceDateTo))
                        ).Count(),
                        AbsenceType = a.AbsenceType.Name,
                        Description = a.Description,
                        Forenames = p.Forenames,
                        Surname = p.Surname,

                        LevelNumber = fa.LevelNumber,
                        PersonnelId = a.PersonnelAbsenceEntitlement.PersonnelId,
                        ApprovalState = a.ApprovalState.Name,
                        ApprovalStateId = a.ApprovalStateId,
                        ApproverAspNetUserId = alu.AspNetUserId,
                    });

                absences = absences
                    .Where(a => isAdmin || a.ApproverAspNetUserId == approverAspNetUserId)
                    .GroupBy(a => a.AbsenceId)
                    .Select(a => a.FirstOrDefault());

                return absences
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy { Property = "DateFrom", Direction = System.ComponentModel.ListSortDirection.Descending },
                    })
                    .Paginate(paging);
            }
        }


        public AbsencePeriod RetrieveAbsencePeriod(int organisationId, int absencePeriodId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsencePeriods
                    .Include(a => a.AbsencePolicyPeriods)
                    .AsNoTracking()
                    .FirstOrDefault(a => a.AbsencePeriodId == absencePeriodId);
            }
        }

        public PagedResult<AbsencePeriod> RetrieveAbsencePeriods(int organisationId, Expression<Func<AbsencePeriod, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsencePeriods
                    .Include(d => d.AbsencePolicyPeriods)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "StartDate", Direction = System.ComponentModel.ListSortDirection.Descending }
                    })
                    .Paginate(paging);
            }
        }

        public IEnumerable<AbsenceDay> RetrieveAbsenceRangeBookedAbsenceDays(AbsenceRange absenceRange)
        {
            //var declinedAbsenceStatusId = AbsenceStatus.Status.Declined.GetHashCode();
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(absenceRange.OrganisationId))
            {
                return context
                    .AbsenceDays
                    .AsNoTracking()
                    .Where(d =>
                        //d.Absence.AbsenceStatusId != declinedAbsenceStatusId &&
                        d.Absence.ApprovalStateId != (int)ApprovalStates.Declined &&
                        d.Absence.PersonnelAbsenceEntitlement.PersonnelId == absenceRange.PersonnelId &&
                        d.Absence.PersonnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePeriodId == absenceRange.AbsencePeriodId &&
                        DbFunctions.TruncateTime(d.Date) >= DbFunctions.TruncateTime(absenceRange.BeginDateUtc) &&
                        DbFunctions.TruncateTime(d.Date) <= DbFunctions.TruncateTime(absenceRange.EndDateUtc))
                    .OrderBy(d => d.Date)
                    .ToList();
            }

        }

        public PersonnelApprovalModel PersonnelApprovalModels(int organisationId, int personnelId, ApprovalTypes approvalTypes)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                //var approvers = context.PersonnelApprovalModels
                //    .Join(context.ApprovalLevels, pam => pam.ApprovalModelId, al => al.ApprovalModelId, (pam, al) => new { pam, al })
                //    .Join(context.ApprovalLevelUsers, pamal => pamal.al.ApprovalLevelId, alu => alu.ApprovalLevelId, (pamal, alu) => new { pamal, alu })
                //    .Join(context.AspNetUsers, pamalalu => pamalalu.alu.AspNetUserId, anu => anu.Id, (pamalalu, anu) => new { pamalalu, anu })
                //    .Where(a => a.pamalalu.pamal.pam.PersonnelId == personnelId && a.pamalalu.pamal.pam.ApprovalEntityTypeId == (int)approvalTypes)
                //    .Select(a => new Approver
                //    {
                //        ApprovalLevelId = a.pamalalu.alu.ApprovalLevelId,
                //        PersonnelId = a.pamalalu.pamal.pam.PersonnelId,
                //        AspNetUserId = a.anu.Id,
                //        Email = a.anu.Email
                //    });
                //var approvers = context.PersonnelApprovalModels
                //    .Join(context.ApprovalLevels, pam => pam.ApprovalModelId, al => al.ApprovalModelId, (pam, al) => new { pam, al })
                //    .Join(context.ApprovalLevelUsers, pamal => pamal.al.ApprovalLevelId, alu => alu.ApprovalLevelId, (pamal, alu) => new { pamal, alu })
                //    .Join(context.AspNetUsers, pamalalu => pamalalu.alu.AspNetUserId, anu => anu.Id, (pamalalu, anu) => new { pamalalu, anu })
                //    .Where(a => a.pamalalu.pamal.pam.PersonnelId == personnelId && a.pamalalu.pamal.pam.ApprovalEntityTypeId == (int)approvalTypes)
                //    .Select(a => new Approver
                //    {
                //        ApprovalLevelId = a.pamalalu.alu.ApprovalLevelId,
                //        PersonnelId = a.pamalalu.pamal.pam.PersonnelId,
                //        AspNetUserId = a.anu.Id,
                //        Email = a.anu.Email
                //    });
                return context.PersonnelApprovalModels
                    .Include(pam => pam.ApprovalModel)
                    .Include(pam => pam.ApprovalModel.ApprovalLevels)
                    .Where(pam => pam.PersonnelId == personnelId && pam.ApprovalEntityTypeId == (int)approvalTypes)
                    .FirstOrDefault();
                //return approvers.ToList();
            }

        }

        public IEnumerable<Approver> RetrieveNextApprovers(int organisationId, ApprovalTypes approvalTypes, int entityId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                //get all approvers
                var approvers = (
                        from a in context.Approvals
                        join al in context.ApprovalLevels on a.ApprovalLevelId equals al.ApprovalLevelId
                        join alu in context.ApprovalLevelUsers on a.ApprovalLevelId equals alu.ApprovalLevelId
                        join anu in context.AspNetUsers on alu.AspNetUserId equals anu.Id
                        where a.EntityId == entityId && a.ApprovalEntityTypeId == (int)approvalTypes
                        select new Approver
                        {
                            ApprovalId = a.ApprovalId,
                            ApprovalLevelId = al.ApprovalLevelId,
                            ApprovalStateId = a.ApprovalStateId,
                            LevelNumber = al.LevelNumber,
                            AspNetUserId = anu.Id,
                            Email = anu.Email
                        });
                //get the max level of approved
                if (approvers.Any(a => a.ApprovalStateId == (int)ApprovalStates.Declined))
                {
                    return new List<Approver>();
                }
                var approver = approvers.OrderByDescending(a => a.LevelNumber).FirstOrDefault(b => b.ApprovalStateId == (int)ApprovalStates.Approved); //approved
                //check if there is max level approved
                int maxApprovedLevel = ((approver == null) ? 0 : approver.LevelNumber);
                var nextApprovers = approvers.Where(a => a.LevelNumber > maxApprovedLevel).ToList();
                return nextApprovers.Where(na => na.LevelNumber == nextApprovers.Min(a => a.LevelNumber)).ToList();
            }

        }

        public AbsenceType RetrieveAbsenceType(int organisationId, int absenceTypeId, Expression<Func<AbsenceType, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsenceTypes
                    .Include(a => a.AbsencePolicyEntitlements)
                    .Include(c => c.Colour)
                    .AsNoTracking()
                    .Where(predicate)
                    .SingleOrDefault(p => p.AbsenceTypeId == absenceTypeId);
            }
        }

        public PagedResult<AbsenceType> RetrieveAbsenceTypes(int organisationId, Expression<Func<AbsenceType, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsenceTypes
                    .AsNoTracking()
                    .Include(c => c.Colour)
                    //.Include(a => a.CountryAbsenceTypes)
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "Name", Direction = System.ComponentModel.ListSortDirection.Ascending }
                    })
                    .Paginate(paging);
            }
        }

        public ApprovalModel RetrieveApprovalModel(int organisationId, int ApprovalModelId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .ApprovalModels
                 .Include(am => am.ApprovalLevels)
                 .AsNoTracking()
                 .SingleOrDefault(p => p.ApprovalModelId == ApprovalModelId);
            }
        }

        public Building RetrieveBuilding(int organisationId, int buildingId, Expression<Func<Building, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Buildings
                    .Include(b => b.Employments)
                    .Include(b => b.Site)
                    .AsNoTracking()
                    .Where(predicate)
                    .FirstOrDefault(b => b.BuildingId == buildingId);
            }
        }

        public PagedResult<Building> RetrieveBuildings(int organisationId, Expression<Func<Building, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                    .Buildings
                    .Include(b => b.Site)
                    .Include(b => b.CompanyBuildings)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> { new OrderBy { Property = "Name", Direction = System.ComponentModel.ListSortDirection.Ascending}
                    })
                    .Paginate(paging);

            }
        }

        public IEnumerable<Colour> RetrieveColours(Expression<Func<Colour, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create())
            {
                return context.Colours
                    .AsNoTracking()
                    .Where(predicate)
                    .ToList();
            }
        }

        public Company RetrieveCompany(int organisationId, int companyId, Expression<Func<Company, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Companies
                    //.Include(c => c.Divisions)
                    .Include(c => c.Colour)
                    .AsNoTracking()
                    .Where(predicate)
                    .SingleOrDefault(p => p.CompanyId == companyId);
            }
        }

        public IEnumerable<Company> RetrieveCompanies(int organisationId, IEnumerable<int> companyIds)
        {
            using (ReadUncommitedTransactionScope)
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Companies
                    .Include(c => c.Colour)
                    .AsNoTracking()
                    .Where(c => !companyIds.Any() || companyIds.Contains(c.CompanyId))
                    .ToList();
            }
        }

        public PagedResult<Company> RetrieveCompanies(int organisationId, Expression<Func<Company, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Companies
                    .Include(c => c.Colour)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "Name", Direction = System.ComponentModel.ListSortDirection.Ascending }
                    })
                    .Paginate(paging);
            }
        }

        public Country RetrieveCountry(int organisationId, int countryId, Expression<Func<Country, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Countries
                    //.Include(c => c.Sites)
                    //.Include(c => c.CountryPublicHolidays)
                    //.Include(c => c.Personnels)
                    //.Include(c => c.EmergencyContacts)
                    //.Include(c => c.CountryAbsenceTypes)
                    .AsNoTracking()
                    .Where(predicate)
                    .SingleOrDefault(p => p.CountryId == countryId);
            }
        }

        public PagedResult<Country> RetrieveCountries(int organisationId, Expression<Func<Country, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Countries
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Name",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        //Used by Employment Tree code.
        public IEnumerable<Employment> RetrieveCurrentEmployments(int organisationId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Include(e => e.Personnel)
                    .AsNoTracking()
                    .ToList();

            }
        }

        public Department RetrieveDepartment(int organisationId, int departmentId, Expression<Func<Department, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Departments
                    .Include(d => d.EmploymentDepartments)
                    .Include(c => c.Colour)
                    .AsNoTracking()
                    .Where(predicate)
                    .SingleOrDefault(p => p.DepartmentId == departmentId);
            }
        }

        public IEnumerable<Department> RetrieveDepartments(int organisationId, IEnumerable<int> departmentIds)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Departments
                    .Include(d => d.Colour)
                    .Include(d => d.EmploymentDepartments)
                    .AsNoTracking()
                    .Where(d => !departmentIds.Any() || departmentIds.Contains(d.DepartmentId))
                    .ToList();
            }
        }

        public PagedResult<Department> RetrieveDepartments(int organisationId, Expression<Func<Department, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Departments
                    .Include(d => d.Colour)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "Name", Direction = System.ComponentModel.ListSortDirection.Ascending }
                    })
                    .Paginate(paging);
            }
        }

        public bool RetrieveDivisionCountryAbsencePeriodHasAbsences(int organisationId, int divisionCountryAbsencePeriodId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                    .PersonnelAbsenceEntitlements
                    .Include(a => a.Absences)
                    .AsNoTracking()
                    .Any(h => h.Absences.Any());

                return result;

            }
        }

        public bool PersonnelEmploymentHasAbsences(int organisationId, int personnelId, int employmentId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PersonnelAbsenceEntitlements
                    .Include(a => a.Absences)
                    .Include(a => a.EmploymentPersonnelAbsenceEntitlements)
                    .Any(
                        p =>
                            p.PersonnelId == personnelId
                           && p.EmploymentPersonnelAbsenceEntitlements.Any(e => e.EmploymentId == employmentId)
                            && p.Absences.Any());
            }
        }

        public IEnumerable<Employment> RetrieveDivisionCountryCurrentEmployments(int organisationId, int divisionId, int countryId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                    .Employments
                    .Where(a => /*a.DivisionId == divisionId &&*/
                        a.Building.Site.CountryId == countryId &&
                        !a.TerminationDate.HasValue)
                    .GroupBy(e => e.PersonnelId)
                    .Select(e => e.OrderByDescending(by => by.StartDate).FirstOrDefault())
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    //  .Include(e => e.Personnel)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public IEnumerable<EmploymentDepartment> RetrieveEmploymentDepartments(int organisationId, int employmentId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                    .EmploymentDepartments
                    .Where(a => a.EmploymentId == employmentId)
                    .Include(e => e.Department)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public IEnumerable<EmploymentTeam> RetrieveEmploymentTeams(int organisationId, int employmentId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                    .EmploymentTeams
                    .Where(a => a.EmploymentId == employmentId)
                    .Include(e => e.Team)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public IEnumerable<JobTitleJobGrade> RetrieveJobTitleJobGrade(int organisationId, int jobTitleId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                    .JobTitleJobGrades
                    .Where(a => a.JobTitleId == jobTitleId)
                    .Include(e => e.JobGrade)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public EmergencyContact RetrieveEmergencyContact(int organisationId, int emergencyContactId, Expression<Func<EmergencyContact, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .EmergencyContacts
                 .Include(e => e.Country)
                 .Where(predicate)
                 .SingleOrDefault(p => p.EmergencyContactId == emergencyContactId);
            }
        }

        public IEnumerable<Employment> RetrieveEmployments(int organisationId, Expression<Func<Employment, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    .Include(e => e.EmploymentDepartments)
                    .Include(e => e.EmploymentTeams)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderByDescending(e => e.StartDate).ToList();

            }
        }

        public IEnumerable<Host> RetrieveHosts()
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create())
            {
                return context
                    .Hosts
                    .Include(o => o.Organisation)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public IEnumerable<Absence> RetrieveManagerAbsencesRequiringApproval(int organisationId, List<int> personnelIds)
        {
            //var unapprovedAbsenceStatusId = AbsenceStatus.Status.Unapproved.GetHashCode();
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Absences
                    .Include(a => a.AbsenceDays)
                    .Include(a => a.AbsenceType)
                    //.Include(a => a.AbsenceStatus)
                    .Include(a => a.ApprovalState)
                    .Include(a => a.PersonnelAbsenceEntitlement)
                    .Include(a => a.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements)
                    .AsNoTracking()
                    .Where(a => personnelIds.Contains(a.PersonnelAbsenceEntitlement.PersonnelId) &&
                        //a.AbsenceStatusId == unapprovedAbsenceStatusId)
                        (a.ApprovalStateId == (int)ApprovalStates.InApproval || a.ApprovalStateId == (int)ApprovalStates.Requested))
                    .OrderBy(a => a.AbsenceDays.Select(d => d.Date).FirstOrDefault())
                    .ToList();
            }
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, Expression<Func<PersonnelAbsenceEntitlement, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PersonnelAbsenceEntitlements
                    .Include(e => e.AbsenceType)
                    .Include(e => e.AbsencePolicyPeriod)
                    .Include(e => e.AbsencePolicyPeriod.AbsencePeriod)
                    .Include(e => e.EmploymentPersonnelAbsenceEntitlements)
                    .Include(e => e.EmploymentPersonnelAbsenceEntitlements.Select(s => s.Employment))
                    .Include(e => e.EmploymentPersonnelAbsenceEntitlements.Select(s => s.Employment.JobTitle))
                    .Include(e => e.EmploymentPersonnelAbsenceEntitlements.Select(s => s.Employment.JobGrade))
                    .AsNoTracking()
                    .Where(predicate).ToList()
                .OrderByDescending(e => e.AbsencePolicyPeriod.AbsencePeriod.StartDate).ToList();

            }
        }

        public Employment RetrievePersonnelEmployment(int organisationId, int personnelId, int employmentId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    .Include(e => e.EmploymentDepartments)
                    .Include(e => e.EmploymentDepartments.Select(s => s.Department))
                    .Include(e => e.EmploymentTeams)
                    .Include(e => e.EmploymentTeams.Select(s => s.Team))
                    .Include(e => e.Personnel)
                    .Include(e => e.WorkingPattern)
                    .Include(e => e.WorkingPattern.WorkingPatternDays)
                    .AsNoTracking()
                    .SingleOrDefault(p => p.PersonnelId == personnelId && p.EmploymentId == employmentId);
            }
        }

        public IEnumerable<Employment> RetrievePersonnelEmployments(int organisationId, int personnelId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Include(e => e.Building)
                    .Include(e => e.EmploymentDepartments)
                    .Include(e => e.EmploymentDepartments.Select(s => s.Department))
                    .Include(e => e.EmploymentTeams)
                    .Include(e => e.EmploymentTeams.Select(s => s.Team))
                    .Include(e => e.ReportsToPersonnel)
                    .Include(e => e.WorkingPattern)
                    .Include(e => e.WorkingPattern.WorkingPatternDays)
                    .AsNoTracking()
                    .Where(e => e.PersonnelId == personnelId)
                    .OrderByDescending(e => e.StartDate).ToList();

            }
        }


        public IEnumerable<PersonnelPublicHoliday> RetrievePersonnelPublicHolidayInDateRange(int organisationId, DateTime beginDate, DateTime endDate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return (from p in context.Personnels
                        join e in context.Employments on p.PersonnelId equals e.PersonnelId
                        join php in context.PublicHolidayPolicies on e.PublicHolidayPolicyId equals php.PublicHolidayPolicyId
                        join ph in context.PublicHolidays.Where(ph => DbFunctions.TruncateTime(ph.Date) >= DbFunctions.TruncateTime(beginDate) && DbFunctions.TruncateTime(ph.Date) <= DbFunctions.TruncateTime(endDate)) on php.PublicHolidayPolicyId equals ph.PublicHolidayPolicyId
                        select new PersonnelPublicHoliday
                        {
                            PersonnelId = p.PersonnelId,
                            Name = ph.Name,
                            Date = ph.Date
                        }).ToList();
            }
        }

        public IEnumerable<Organisation> RetrieveOrganisations()
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create())
            {
                return context
                    .Organisations
                    .Include(o => o.Hosts)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public Overtime RetrieveOvertime(int organisationId,int overtimeId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Overtimes
                    .Include(o => o.ApprovalState)
                    .Include(o => o.OvertimePreference)
                    .Include(o => o.Personnel)
                    .AsNoTracking()
                    .FirstOrDefault(o => o.OvertimeId == overtimeId);
            }
        }

        public PagedResult<Overtime> RetrieveOvertimes(int organisationId, Expression<Func<Overtime, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Overtimes
                    .Include(o => o.ApprovalState)
                    .Include(o => o.OvertimePreference)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "Date", Direction = System.ComponentModel.ListSortDirection.Descending }
                    })
                    .Paginate(paging);
            }
        }

        public PagedResult<Overtime> RetrieveOvertimeTransactions(int organisationId, OvertimeFilter overtimeFilter, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                Expression<Func<Overtime, bool>> predicate = (o => (overtimeFilter.IsPaid || o.OvertimePreferenceId != (int)OvertimePreferences.Paid)
                     && (overtimeFilter.IsToil || o.OvertimePreferenceId != (int)OvertimePreferences.TOIL) && (overtimeFilter.IsRequested || o.ApprovalStateId != (int)ApprovalStates.Requested)
                     && (overtimeFilter.IsDeclined || o.ApprovalStateId != (int)ApprovalStates.Declined) && (overtimeFilter.IsApproved || o.ApprovalStateId != (int)ApprovalStates.Approved)
                     && (overtimeFilter.IsInApproval || o.ApprovalStateId != (int)ApprovalStates.InApproval) && DbFunctions.TruncateTime(o.Date) >= DbFunctions.TruncateTime(overtimeFilter.Begin.Date) && DbFunctions.TruncateTime(o.Date) <= DbFunctions.TruncateTime(overtimeFilter.End.Date)
                 );

                var overtimeTransactions = context
                    .Overtimes
                    .Include(o => o.ApprovalState)
                    .Include(o => o.OvertimePreference)
                    .Include(o => o.Personnel)
                    .AsNoTracking()
                    .Where(predicate);

                if (overtimeFilter.CompanyIds != null && overtimeFilter.CompanyIds.Any())
                    overtimeTransactions = overtimeTransactions.Where(ot => ot.Personnel.Employments.Any(c => overtimeFilter.CompanyIds.Contains(c.CompanyId)));

                if (overtimeFilter.DepartmentIds != null && overtimeFilter.DepartmentIds.Any())
                    overtimeTransactions = overtimeTransactions.Where(ot => ot.Personnel.Employments.Any(c => c.EmploymentDepartments.Any(d => overtimeFilter.DepartmentIds.Contains(d.DepartmentId))));

                if (overtimeFilter.TeamIds != null && overtimeFilter.TeamIds.Any())
                    overtimeTransactions = overtimeTransactions.Where(ot => ot.Personnel.Employments.Any(c => c.EmploymentTeams.Any(d => overtimeFilter.TeamIds.Contains(d.TeamId))));

                return overtimeTransactions
                    .OrderBy(orderBy ?? new List<OrderBy> {
                        new OrderBy { Property = "Date", Direction = System.ComponentModel.ListSortDirection.Descending }
                    })
                    .Paginate(paging);
            }
        }

        public PagedResult<OvertimeForApproval> RetrieveOvertimeForApprovals(int organisationId, string approverAspNetUserId, bool isAdmin, Expression<Func<OvertimeForApproval, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                //get all not declined approvals
                var forApprovals = (
                    from a in context.Approvals
                    join al in context.ApprovalLevels on a.ApprovalLevelId equals al.ApprovalLevelId
                    where a.ApprovalStateId != (int)ApprovalStates.Declined && a.ApprovalEntityTypeId == (int)ApprovalTypes.Overtime
                    select new ForApproval
                    {
                        ApprovalId = a.ApprovalId,
                        ApprovalLevelId = a.ApprovalLevelId,
                        ApprovalStateId = a.ApprovalStateId,
                        EntityId = a.EntityId,
                        LevelNumber = al.LevelNumber
                    });
                //get approvals with max level approved
                var forApprovalWithApproved = forApprovals
                    .Where(fa => fa.ApprovalStateId == (int)ApprovalStates.Approved)
                    .OrderBy(fa => fa.LevelNumber)
                    .GroupBy(fa => fa.EntityId)
                    .Select(o => o.OrderByDescending(fa => fa.LevelNumber).FirstOrDefault());
                //get approvals with the least level unapproved
                forApprovals = forApprovals
                    .Where(fa => !forApprovalWithApproved.Select(fawa => fawa.EntityId).Contains(fa.EntityId) ||
                        forApprovalWithApproved.FirstOrDefault(fawa => fawa.EntityId == fa.EntityId).LevelNumber < fa.LevelNumber)
                    .OrderBy(fa => fa.LevelNumber)
                    .GroupBy(fa => fa.EntityId)
                    .Select(fa => fa.FirstOrDefault());

                var overtimes = (
                    from o in context.Overtimes
                    join fa in forApprovals on o.OvertimeId equals fa.EntityId
                    join alu in context.ApprovalLevelUsers on fa.ApprovalLevelId equals alu.ApprovalLevelId
                    join ast in context.ApprovalStates on o.ApprovalStateId equals ast.ApprovalStateId
                    join op in context.OvertimePreferences on o.OvertimePreferenceId equals op.OvertimePreferenceId
                    join p in context.Personnels on o.PersonnelId equals p.PersonnelId
                    where o.ApprovalStateId == (int)ApprovalStates.Requested || o.ApprovalStateId == (int)ApprovalStates.InApproval
                    orderby o.CreatedDateUtc descending, fa.LevelNumber
                    select new OvertimeForApproval
                    {
                        Date = o.Date,
                        Hours = o.Hours,
                        OvertimeId = o.OvertimeId,
                        OvertimePreferenceId = o.OvertimePreferenceId,
                        LevelNumber = fa.LevelNumber,
                        PersonnelId = o.PersonnelId,
                        ApprovalState = ast.Name,
                        ApprovalStateId = o.ApprovalStateId,
                        ApproverAspNetUserId = alu.AspNetUserId,
                        Comment = o.Comment,
                        Forenames = p.Forenames,
                        OvertimePreference = op.Name,
                        Surname = p.Surname,
                        Reason = o.Reason,
                        CreatedBy = o.CreatedBy,
                        CreatedDateUtc = o.CreatedDateUtc,
                        UpdatedBy = o.UpdatedBy,
                        UpdateDateUtc = o.UpdatedDateUtc
                    });

                overtimes = overtimes
                    .Where(o => isAdmin || o.ApproverAspNetUserId == approverAspNetUserId)
                    .GroupBy(o => o.OvertimeId)
                    .Select(o => o.FirstOrDefault());

                return overtimes
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy { Property = "Date", Direction = System.ComponentModel.ListSortDirection.Descending },
                    })
                    .Paginate(paging);
            }
        }

        public PagedResult<OvertimeSummary> RetrieveOvertimeSummaries(int organisationId, Expression<Func<OvertimeSummary, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {

            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var departments = context
                    .Departments
                    .Include(d => d.EmploymentDepartments)
                    .AsNoTracking();

                var teams = context
                    .Teams
                    .Include(d => d.EmploymentTeams)
                    .AsNoTracking();

                var overtimeSummaries = context.Overtimes
                    .Where(os => os.ApprovalStateId == (int)ApprovalStates.Approved)
                    .Include(o => o.Personnel)
                    .GroupBy(o => o.Personnel)
                    .Select(o => new OvertimeSummary
                    {
                        PaidHours = o.Where(a => a.OvertimePreferenceId == (int)OvertimePreferences.Paid).Sum(a => (double?)a.Hours) ?? 0,
                        TOILHours = o.Where(a => a.OvertimePreferenceId == (int)OvertimePreferences.TOIL).Sum(a => (double?)a.Hours) ?? 0,
                        TotalHours = o.Sum(a => (double?)a.Hours) ?? 0,
                        CompanyId = o.Key.Employments.FirstOrDefault(e => !e.TerminationDate.HasValue).CompanyId,
                        Count = o.Count(),
                        PersonnelId = o.Key.PersonnelId,
                        Departments = departments.Where(d => d.EmploymentDepartments.Any(ed => o.Key.Employments.Where(e => !e.TerminationDate.HasValue).Select(e => e.EmploymentId).Contains(ed.EmploymentId))),
                        Teams = teams.Where(t => t.EmploymentTeams.Any(et => o.Key.Employments.Where(e => !e.TerminationDate.HasValue).Select(e => e.EmploymentId).Contains(et.EmploymentId))),
                        Forenames = o.Key.Forenames,
                        Surname = o.Key.Surname
                    });

                return overtimeSummaries
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy { Property = "Surname", Direction = System.ComponentModel.ListSortDirection.Ascending },
                    })
                    .Paginate(paging);
            }
        }

        public Personnel RetrievePersonnel(int organisationId, int personnelId, Expression<Func<Personnel, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Personnels
                    .Include(p => p.Country)
                    .Include(p => p.EmergencyContacts)
                    .Include(p => p.Employments)
                    .Include(p => p.PersonnelAbsenceEntitlements)
                    .AsNoTracking()
                    .Where(predicate)
                    .SingleOrDefault(p => p.PersonnelId == personnelId);

            }
        }

        public IEnumerable<Personnel> RetrievePersonnel(int organisationId, IEnumerable<int> companyIds, IEnumerable<int> departmentIds, IEnumerable<int> teamIds)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var personnel = context
                    .Personnels
                    .Include(p => p.PersonnelAbsenceEntitlements)
                    .Include(p => p.Employments)
                    .AsNoTracking();

                if (companyIds != null && companyIds.Any())
                    personnel = personnel.Where(p => p.Employments.Any(c => companyIds.Contains(c.CompanyId)));

                if (departmentIds != null && departmentIds.Any())
                    personnel = personnel.Where(p => p.Employments.Any(c => c.EmploymentDepartments.Any(d => departmentIds.Contains(d.DepartmentId))));

                if (teamIds != null && teamIds.Any())
                    personnel = personnel.Where(p => p.Employments.Any(c => c.EmploymentTeams.Any(d => teamIds.Contains(d.TeamId))));

                return personnel.ToList();
            }
        }

        public PagedResult<Personnel> RetrievePersonnel(int organisationId, Expression<Func<Personnel, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                    .Personnels
                    .Include(p => p.Organisation)
                    .Include(p => p.Employments)
                    .Include(p => p.ManageEmployments)
                    .Include(p => p.EmergencyContacts)
                    .Include(p => p.Country)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Forenames",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        public PersonnelAbsenceEntitlement RetrievePersonnelAbsenceEntitlement(int organisationId, int personnelId, int personnelAbsenceEntitlementId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PersonnelAbsenceEntitlements
                    .Include(e => e.AbsenceType)
                    .Include(e => e.EmploymentPersonnelAbsenceEntitlements)
                    .Include(e => e.AbsencePolicyPeriod)
                    .AsNoTracking()
                    .FirstOrDefault(h => h.PersonnelId == personnelId && h.PersonnelAbsenceEntitlementId == personnelAbsenceEntitlementId);
            }
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, int personnelId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PersonnelAbsenceEntitlements
                    .Include(p => p.EmploymentPersonnelAbsenceEntitlements)
                    .AsNoTracking()
                    .Where(e => e.PersonnelId == personnelId)
                    .ToList();
            }
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, int personnelId, int employmentId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                     .PersonnelAbsenceEntitlements
                     .Include(p => p.EmploymentPersonnelAbsenceEntitlements)
                     .Include(p => p.AbsencePolicyPeriod)
                     .Include(p => p.AbsenceType)
                     .AsNoTracking()
                     .Where(e => e.PersonnelId == personnelId && e.EmploymentPersonnelAbsenceEntitlements.Any(d => d.EmploymentId == employmentId))
                     .ToList();
                return result;
            }
        }

        public bool RetrievePersonnelAbsenceEntitlementExists(int organisationId, int personnelId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                  .PersonnelAbsenceEntitlements
                  .AsNoTracking()
                  .Any(h => h.PersonnelId == personnelId);

                return result;
            }

        }

        public Employment RetrievePersonnelCurrentEmployment(int organisationId, int personnelId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    .Include(e => e.ReportsToPersonnel)
                    .Include(e => e.WorkingPattern)
                    .Include(e => e.WorkingPattern.WorkingPatternDays)
                    .Include(e => e.EmploymentType)
                    .Include(e => e.EmploymentDepartments)
                    .Include(e => e.EmploymentTeams)
                    .AsNoTracking()
                    .OrderByDescending(e => e.StartDate)
                    .FirstOrDefault(e => e.PersonnelId == personnelId);
            }
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelCurrentAbsenceEntitlements(int organisationId, int personnelId, int employmentId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PersonnelAbsenceEntitlements
                    .Include(h => h.AbsenceType)
                    .Include(h => h.AbsencePolicyPeriod)
                    .Include(h => h.AbsencePolicyPeriod.AbsencePeriod)
                    .Include(h => h.EmploymentPersonnelAbsenceEntitlements)
                    //   .Include(h => h.Employment)
                    .AsNoTracking()
                    .Where(e => e.PersonnelId == personnelId && e.EmploymentPersonnelAbsenceEntitlements.Any(d => d.EmploymentId == employmentId)
                        && e.StartDate <= DateTime.Now)
                    .AsEnumerable()
                    .ToList();
            }
        }

        public Site RetrieveSite(int organisationId, int siteId, Expression<Func<Site, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Sites
                    .Include(s => s.Buildings)
                    //.Include(s => s.DivisionSites)
                    .AsNoTracking()
                    .Where(predicate)
                    .FirstOrDefault(s => s.SiteId == siteId);
            }
        }

        public PagedResult<Site> RetrieveSites(int organisationId, Expression<Func<Site, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Sites
                    .Include(c => c.Country)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Name",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        public Team RetrieveTeam(int organisationId, int teamId, Expression<Func<Team, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .Teams
                 .Include(c => c.Colour)
                 .AsNoTracking()
                 .Where(predicate)
                 .SingleOrDefault(p => p.TeamId == teamId);
            }
        }

        public IEnumerable<Team> RetrieveTeams(int organisationId, IEnumerable<int> teamIds)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Teams
                    .Include(t => t.Colour)
                    .Include(t => t.EmploymentTeams)
                    .AsNoTracking()
                    .Where(t => !teamIds.Any() || teamIds.Contains(t.TeamId))
                    .ToList();
            }
        }

        public PagedResult<Team> RetrieveTeams(int organisationId, Expression<Func<Team, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Teams
                    .Include(d => d.Colour)
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Name",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        public UserAuthorisationFilter RetrieveUserAuthorisation(string aspNetUserId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create())
            {
                return context
                    .UserAuthorisationFilters
                    .AsNoTracking()
                    .FirstOrDefault(u => u.AspNetUsersId == aspNetUserId);
            }
        }

        public WorkingPattern RetrievePersonnelWorkingPattern(int organisationId, int personnelId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                var employment = context
                    .Employments
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    .Include(e => e.WorkingPattern)
                    .Include(e => e.WorkingPattern.WorkingPatternDays)
                    .AsNoTracking()
                    .OrderByDescending(e => e.StartDate)
                    .FirstOrDefault(e => e.PersonnelId == personnelId);

                if (employment?.WorkingPattern != null)
                    return employment.WorkingPattern;

                var absencePolicyWorkingPatterns = context
                    .AbsencePolicies
                    .Include(d => d.WorkingPattern)
                    .FirstOrDefault(d => d.AbsencePolicyId == employment.AbsencePolicyId);

                return absencePolicyWorkingPatterns?.WorkingPattern;
            }
        }

        public PagedResult<PersonnelSearchField> RetrievePersonnelBySearchKeyword(int organisationId, string searchKeyword, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                //var organisationIdParameter = new SqlParameter("@OrganisationId", organisationId);
                var searchKeywordParamter = new SqlParameter("@SearchKeyword", searchKeyword);
                return context
                    .Database
                     //.SqlQuery<PersonnelSearchField>("SearchPersonnel @OrganisationId, @SearchKeyword", organisationIdParameter, searchKeywordParamter)
                     .SqlQuery<PersonnelSearchField>("SearchPersonnel @SearchKeyword", searchKeywordParamter)
                    .ToList()
                    .AsQueryable()
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Forenames",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        //generic type use only when no other tables are needed
        public T Retrieve<T>(int organisationId, int Id) where T : class
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var returnItems = context.Set<T>().Find(Id);
                return returnItems;
            }
        }

        //generic type use only when no other tables are needed
        public List<T> Retrieve<T>(int organisationId, Expression<Func<T, bool>> predicate) where T : class
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var returnItems = context.Set<T>().Where(predicate).ToList();
                return returnItems;
            }
        }

        //generic type use only when no other tables are needed
        public PagedResult<T> RetrievePagedResult<T>(int organisationId, Expression<Func<T, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null) where T : class
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Set<T>()
                    .AsNoTracking()
                    .Where(predicate)
                    //if there is no name column on the table this will generate an error
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Name",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        public IEnumerable<CompanyBuilding> RetrieveCompanyBuilding(int organisationId, Expression<Func<CompanyBuilding, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .CompanyBuildings
                 .Include(c => c.Building)
                 .Include(c => c.Building.Site)
                 .Include(c => c.Building.Site.Country)
                 .Include(c => c.Company)
                 .Include(c => c.Company.Colour)
                 .AsNoTracking()
                 .Where(predicate)
                 .AsEnumerable()
                 .ToList();


            }
        }

        public IEnumerable<JobTitleJobGrade> RetrieveJobTitleJobGrade(int organisationId, Expression<Func<JobTitleJobGrade, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .JobTitleJobGrades
                 .Include(c => c.JobGrade)
                 .Include(c => c.JobTitle)
                 .AsNoTracking()
                 .Where(predicate)
                 .AsEnumerable()
                 .ToList();
            }
        }

        public IEnumerable<Building> RetrieveBuildingsSitesUnassignedCompany(int organisationId, int companyId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                var result = context
                    .Buildings
                    .Include(b => b.Site)
                    .AsNoTracking();
                return result;

            }
        }

        //JobGrade
        public JobGrade RetrieveJobGrade(int organisationId, int jobGradeId, Expression<Func<JobGrade, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .JobGrades
                 .AsNoTracking()
                 .Where(predicate)
                 .SingleOrDefault(p => p.JobGradeId == jobGradeId);
            }
        }

        public IEnumerable<JobGrade> RetrieveJobGrades(int organisationId, IEnumerable<int> jobGradeIds)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .JobGrades
                    .AsNoTracking()
                    .Where(t => !jobGradeIds.Any() || jobGradeIds.Contains(t.JobGradeId))
                    .ToList();
            }
        }

        public PagedResult<JobGrade> RetrieveJobGrades(int organisationId, Expression<Func<JobGrade, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .JobGrades
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Name",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        //JobTitle
        public JobTitle RetrieveJobTitle(int organisationId, int jobTitleId, Expression<Func<JobTitle, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                 .JobTitles
                 .AsNoTracking()
                 .Where(predicate)
                 .SingleOrDefault(p => p.JobTitleId == jobTitleId);
            }
        }

        public IEnumerable<JobTitle> RetrieveJobTitles(int organisationId, IEnumerable<int> jobTitleIds)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .JobTitles
                    .AsNoTracking()
                    .Where(t => !jobTitleIds.Any() || jobTitleIds.Contains(t.JobTitleId))
                    .ToList();
            }
        }

        public PagedResult<JobTitle> RetrieveJobTitles(int organisationId, Expression<Func<JobTitle, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .JobTitles
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(orderBy ?? new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Property = "Name",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        },
                        new OrderBy
                        {
                            Property = "Description",
                            Direction = System.ComponentModel.ListSortDirection.Ascending
                        }
                    })
                    .Paginate(paging);
            }
        }

        public Employment RetrieveEmployment(int organisationId, int personnelId, DateTime dateTimeNow)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    .Include(e => e.ReportsToPersonnel)
                    .Include(e => e.WorkingPattern)
                    .Include(e => e.WorkingPattern.WorkingPatternDays)
                    .Include(e => e.EmploymentType)
                    .Include(e => e.EmploymentDepartments.Select(d => d.Department))
                    .Include(e => e.EmploymentTeams.Select(d => d.Team))
                    .Include(e => e.AbsencePolicy)
                    .Include(e => e.AbsencePolicy.AbsencePolicyPeriods)
                    .Include(e => e.AbsencePolicy.AbsencePolicyEntitlements)
                    .Include(e => e.AbsencePolicy.AbsencePolicyEntitlements.Select(d => d.AbsenceType))
                    .AsNoTracking()
                    .Where(e =>
                        e.PersonnelId == personnelId &&
                        e.StartDate <= dateTimeNow &&
                        (e.EndDate >= dateTimeNow || e.EndDate == null))
                    .OrderByDescending(e => e.EndDate)
                    .FirstOrDefault();
            }
        }

        public PagedResult<JobGrade> RetrieveJobTitleJobGrade(int organisationId, int jobTitleId, List<OrderBy> orderBy, Paging paging)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                .JobGrades
                .Include(c => c.JobTitleJobGrades)
                .AsNoTracking()
                .Where(c => c.JobTitleJobGrades.Any(x => x.JobTitleId == jobTitleId))
                .OrderBy(orderBy ?? new List<OrderBy>
                    {
                    new OrderBy
                    {
                        Property = "Name",
                        Direction = System.ComponentModel.ListSortDirection.Descending
                    }
                    })
                    .Paginate(paging);
            }
        }

        public WorkingPattern RetrieveWorkingPattern(int organisationId, int workingPatternId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {

                return context
                      .WorkingPatterns
                      .Include(e => e.WorkingPatternDays)
                      .Include(e => e.AbsencePolicies)
                      .Include(e => e.AbsencePolicies.Select(d => d.AbsencePolicyEntitlements))
                      .Include(e => e.AbsencePolicies.Select(d => d.AbsencePolicyPeriods))
                      .AsNoTracking()
                      .FirstOrDefault(e => e.WorkingPatternId == workingPatternId);
            }
        }


        #endregion

        #region // Update

        //public DivisionCountryWorkingPattern UpdateDivisionCountryWorkingPattern(int organisationId, DivisionCountryWorkingPattern divisionCountryWorkingPattern, List<WorkingPatternDay> workingPatternDays)
        //{
        //    DeleteWorkingPatternDays(organisationId, divisionCountryWorkingPattern.WorkingPatternId);
        //    CreateWorkingPatternDays(organisationId, workingPatternDays);

        //    return RetrieveDivisionCountryWorkingPattern(organisationId, divisionCountryWorkingPattern.DivisionId, divisionCountryWorkingPattern.CountryId);
        //}

        public T UpdateEntityEntry<T>(T t) where T : class
        {
            using (var context = _databaseFactory.Create())
            {
                context.Entry(t).State = EntityState.Modified;
                context.SaveChanges();

                return t;
            }
        }

        public T UpdateOrganisationEntityEntry<T>(int organisationId, T t) where T : class
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                context.Entry(t).State = EntityState.Modified;
                context.SaveChanges();

                return t;
            }
        }

        #endregion

        #region //Delete

        public void DeleteAbsence(int organisationId, Absence absence)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absenceDayIds = absence.AbsenceDays.Select(a => a.AbsenceDayId).ToList();
                var dbAbsenceDays = context.AbsenceDays.Where(d => absenceDayIds.Any(a => a == d.AbsenceDayId));
                if (dbAbsenceDays == null || !dbAbsenceDays.Any())
                    return;

                var dbAbsence = context.Absences.SingleOrDefault(d => absence.AbsenceId == d.AbsenceId);
                if (dbAbsence == null)
                    return;

                context.AbsenceDays.RemoveRange(dbAbsenceDays);
                context.Absences.Remove(dbAbsence);
                context.SaveChanges();
            }
        }

        public void DeleteAbsenceDays(int organisationId, IEnumerable<AbsenceDay> absenceDays)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absenceDayIds = absenceDays.Select(a => a.AbsenceDayId).ToList();
                var dbAbsenceDays = context.AbsenceDays.Where(d => absenceDayIds.Any(a => a == d.AbsenceDayId));
                if (dbAbsenceDays == null || !dbAbsenceDays.Any())
                    return;

                context.AbsenceDays.RemoveRange(dbAbsenceDays);
                context.SaveChanges();
            }
        }

        public void DeleteAbsencePeriod(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absencePeriod = context
                    .AbsencePeriods
                    .Find(id);
                if (absencePeriod == null)
                    return;

                context.AbsencePeriods.Remove(absencePeriod);
                context.SaveChanges();
            }
        }

        public void DeleteAbsenceType(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absenceType = context
                    .AbsenceTypes
                    .Find(id);
                if (absenceType == null)
                    return;

                context.AbsenceTypes.Remove(absenceType);
                context.SaveChanges();
            }
        }

        public void DeleteBuilding(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var building = context
                    .Buildings
                    .Find(id);
                if (building == null)
                    return;
                var companyBuilding = context.CompanyBuildings.Where(c => c.BuildingId == id);
                if (companyBuilding.Any())
                {
                    context.CompanyBuildings.RemoveRange(companyBuilding);
                }

                context.Buildings.Remove(building);
                context.SaveChanges();
            }
        }

        public void DeleteCountry(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var country = context
                    .Countries
                    .Find(id);
                if (country == null)
                    return;

                context.Countries.Remove(country);
                context.SaveChanges();
            }
        }

        public void DeleteCompany(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var company = context
                    .Companies
                    .Find(id);
                if (company == null)
                    return;

                var companyBuilding = context.CompanyBuildings.Where(c => c.CompanyId == id);
                if (companyBuilding.Any())
                {
                    context.CompanyBuildings.RemoveRange(companyBuilding);
                }
                context.Companies.Remove(company);
                context.SaveChanges();
            }
        }

        public void DeleteDepartment(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var department = context
                    .Departments
                    .Find(id);
                if (department == null)
                    return;

                context.Departments.Remove(department);
                context.SaveChanges();
            }
        }

        public void DeleteEmergencyContact(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var emergencyContact = context
                    .EmergencyContacts
                    .Find(id);
                if (emergencyContact == null)
                    return;

                context.EmergencyContacts.Remove(emergencyContact);
                context.SaveChanges();
            }
        }

        public void DeleteEmploymentTeam(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var employmentTeam = context
                    .EmploymentTeams
                    .Find(id);
                if (employmentTeam == null)
                    return;

                context.EmploymentTeams.Remove(employmentTeam);
                context.SaveChanges();
            }
        }

        public void DeletePersonnelAbsenceEntitlements(int organisationId, int personnelId, int employmentId)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var employmentPersonnelEntitlements = context.EmploymentPersonnelAbsenceEntitlements.Where(e => e.EmploymentId == employmentId);

                var personnelAbsenceEntitlementsTobeDeleted =
                     context.PersonnelAbsenceEntitlements.Where(e => e.PersonnelId == personnelId).Join(employmentPersonnelEntitlements,
                         p => p.PersonnelAbsenceEntitlementId, e => e.PersonnelAbsenceEntitlementId,
                         (p, e) => p).ToList();

                //Delete Mapping
                var employmentPersonnelAbsenceEntitlements = context.EmploymentPersonnelAbsenceEntitlements.Where(e => e.EmploymentId == employmentId);
                var removedEmploymentPersonnelAbsenceEntitlements = context.EmploymentPersonnelAbsenceEntitlements.RemoveRange(employmentPersonnelAbsenceEntitlements);
                //Get personnelEntitlements for same policy mapped to other employment 
                var allEmploymentPersonnelAbsenceEntitlements = RetrieveEmploymentPersonnelAbsenceEntitlements(organisationId,
                    e => e.PersonnelAbsenceEntitlement.PersonnelId == personnelId && e.EmploymentId != employmentId).ToList();

                var personnelEntitlements =
                    allEmploymentPersonnelAbsenceEntitlements.Join(employmentPersonnelEntitlements,
                        p => p.PersonnelAbsenceEntitlementId, e => e.PersonnelAbsenceEntitlementId, (p, e) => p).ToList().Any();

                if (!personnelEntitlements)
                    context.PersonnelAbsenceEntitlements.RemoveRange(personnelAbsenceEntitlementsTobeDeleted);

                context.SaveChanges();
            }
        }

        public void DeletePersonnel(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                // Remove PersonnelAbsenceEntitlements 
                var personnelAbsenceEntitlements = context
                    .PersonnelAbsenceEntitlements
                    .Where(e => e.PersonnelId == id);

                if (personnelAbsenceEntitlements != null)
                    context.PersonnelAbsenceEntitlements.RemoveRange(personnelAbsenceEntitlements);

                // Remove Employments
                var employments = context
                    .Employments
                    .Where(e => e.PersonnelId == id);

                if (employments != null)
                    context.Employments.RemoveRange(employments);

                // Remove 
                var personnel = context
                    .Personnels
                    .Find(id);

                if (personnel == null)
                    return;

                context.Personnels.Remove(personnel);
                context.SaveChanges();
            }
        }

        public void DeleteSite(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var site = context
                    .Sites
                    .Find(id);
                if (site == null)
                    return;

                context.Sites.Remove(site);
                context.SaveChanges();
            }
        }

        public void DeleteTeam(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var team = context
                    .Teams
                    .Find(id);
                if (team == null)
                    return;

                context.Teams.Remove(team);
                context.SaveChanges();
            }
        }

        public void DeleteWorkingPattern(int organisationId, int workingPatternId)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var workingPattern = context.WorkingPatterns.Find(workingPatternId);
                var workingPatternDays = context
                  .WorkingPatternDays
                  .Where(e => e.WorkingPatternId == workingPatternId);
                if (!workingPatternDays.Any())
                    return;

                context.WorkingPatternDays.RemoveRange(workingPatternDays);
                context.WorkingPatterns.Remove(workingPattern);
                context.SaveChanges();
            }
        }

        private void DeleteWorkingPatternDays(int organisationId, int workingPatternId)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var workingPatternDays = context
                  .WorkingPatternDays
                  .Where(e => e.WorkingPatternId == workingPatternId);
                if (!workingPatternDays.Any())
                    return;

                context.WorkingPatternDays.RemoveRange(workingPatternDays);
                context.SaveChanges();
            }
        }

        public void DeleteEmployment(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var employmentPersonnelEntitlements = context.EmploymentPersonnelAbsenceEntitlements.Where(e => e.EmploymentId == id);
                var personnelAbsenceEntitlements =
                    context.PersonnelAbsenceEntitlements.Join(employmentPersonnelEntitlements,
                        p => p.PersonnelAbsenceEntitlementId, e => e.PersonnelAbsenceEntitlementId,
                        (p, e) => p).ToList();

                var employmentDepartments = context.EmploymentDepartments.Where(e => e.EmploymentId == id);
                var employmentTeams = context.EmploymentTeams.Where(e => e.EmploymentId == id);
                var employment = context.Employments.Find(id);
                if (employment == null)
                    return;
                //var personnelAbsenceEntitlements = context.PersonnelAbsenceEntitlements.Where(e => e.EmploymentId == id);
                context.EmploymentTeams.RemoveRange(employmentTeams);
                context.EmploymentDepartments.RemoveRange(employmentDepartments);
                context.EmploymentPersonnelAbsenceEntitlements.RemoveRange(employmentPersonnelEntitlements);

                //If same policy then do not delete PersonnelAbsenceEntitlements
                var personnelEntitlementsMappedWithOtherEmployment =
                    personnelAbsenceEntitlements.Join(context.EmploymentPersonnelAbsenceEntitlements,
                        e => e.PersonnelAbsenceEntitlementId, p => p.PersonnelAbsenceEntitlementId, (e, p) => p).ToList();

                if (!personnelEntitlementsMappedWithOtherEmployment.Any())
                    context.PersonnelAbsenceEntitlements.RemoveRange(personnelAbsenceEntitlements);

                context.Employments.Remove(employment);
                context.SaveChanges();
            }
        }

        public void DeleteCompanyBuilding(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var companyBuilding = context
                    .CompanyBuildings
                    .Find(id);
                if (companyBuilding == null)
                    return;

                context.CompanyBuildings.Remove(companyBuilding);
                context.SaveChanges();
            }
        }

        public void DeleteJobGrade(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var jobGrade = context
                    .JobGrades
                    .Find(id);
                if (jobGrade == null)
                    return;

                context.JobGrades.Remove(jobGrade);
                context.SaveChanges();
            }
        }

        public void DeleteJobTitle(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var jobTitle = context
                    .JobTitles
                    .Find(id);
                if (jobTitle == null)
                    return;

                context.JobTitles.Remove(jobTitle);
                context.SaveChanges();
            }
        }

        public void Delete<T>(int organisationId, int Id) where T : class
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var item = context.Set<T>().Find(Id);
                context.Set<T>().Remove(item);
                context.SaveChanges();
            }
        }

        public void Delete<T>(int organisationId, Expression<Func<T, bool>> predicate) where T : class
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var items = context.Set<T>().Where(predicate).FirstOrDefault();
                if (items != null)
                {
                    context.Set<T>().Remove(items);
                    context.SaveChanges();
                }
            }
        }

        public void DeleteRange<T>(int organisationId, Expression<Func<T, bool>> predicate) where T : class
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var items = context.Set<T>().Where(predicate);
                context.Set<T>().RemoveRange(items);
                context.SaveChanges();
            }
        }

        #endregion
    }
}
