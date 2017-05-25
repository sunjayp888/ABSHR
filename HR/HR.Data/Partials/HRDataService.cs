using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using HR.Data.Extensions;
using HR.Entity;
using HR.Entity.Dto;

namespace HR.Data
{
    public partial class HRDataService
    {
        public DateTime Today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        //Create 
        public PublicHolidayPolicy CreatePublicHolidayPolicy(int organisationId, PublicHolidayPolicy publicHolidayPolicy)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                publicHolidayPolicy = context.PublicHolidayPolicies.Add(publicHolidayPolicy);
                context.SaveChanges();
                return publicHolidayPolicy;
            }
        }

        public AbsencePolicy CreateAbsencePolicy(int organisationId, AbsencePolicy absencePolicy)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context.AbsencePolicies.Add(absencePolicy);
                context.SaveChanges();
                return result;
            }
        }

        public AbsencePolicyEntitlement CreateAbsencePolicyEntitlement(int organisationId, int absencePolicyId, AbsencePolicyEntitlement absencepolicyEntitlement)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context.AbsencePolicyEntitlements.Add(absencepolicyEntitlement);
                context.SaveChanges();
                return result;
            }
        }

        public JobTitleJobGrade CreateJobTitleJobGrade(int organisationId, JobTitleJobGrade jobTitleJobGrade)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context.JobTitleJobGrades.Add(jobTitleJobGrade);
                context.SaveChanges();
                return result;
            }
        }

        public IEnumerable<PublicHoliday> CreatePublicHolidays(int organisationId, IEnumerable<PublicHoliday> publicHolidays)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                publicHolidays = context.PublicHolidays.AddRange(publicHolidays);
                context.SaveChanges();
                return publicHolidays;
            }
        }

        public WorkingPattern CreateAbsencePolicyWorkingPattern(int organisationId, AbsencePolicy absencePolicy, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var workingPattern = context.WorkingPatterns.Add(new WorkingPattern
                {
                    WorkingPatternDays = workingPatternDays.ToList()
                });
                var absencePolicyData = context.AbsencePolicies.FirstOrDefault(e => e.AbsencePolicyId == absencePolicy.AbsencePolicyId);
                absencePolicyData.WorkingPatternId = workingPattern.WorkingPatternId;
                absencePolicyData.Name = absencePolicy.Name;
                context.SaveChanges();

                return RetrieveAbsencePolicy(organisationId, absencePolicy.AbsencePolicyId).WorkingPattern;
            }
        }

        public void CloneAbsencePolicy(int organisationId, AbsencePolicy absencePolicy)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                //Clone Working Pattern
                if (absencePolicy.WorkingPattern != null)
                    context.WorkingPatterns.Add(absencePolicy.WorkingPattern);
                //Clone Absence Policy Entitlement.
                if (absencePolicy.AbsencePolicyEntitlements.Any())
                    context.AbsencePolicyEntitlements.AddRange(absencePolicy.AbsencePolicyEntitlements);
                //Clone Absence Policy Period.
                if (absencePolicy.AbsencePolicyPeriods.Any())
                    context.AbsencePolicyPeriods.AddRange(absencePolicy.AbsencePolicyPeriods);
                context.SaveChanges();
            }
        }

        //Retrieve
        public PagedResult<PublicHolidayPolicy> RetrievePublicHolidayPolicies(int organisationId, Expression<Func<PublicHolidayPolicy, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PublicHolidayPolicies
                    .Include(p => p.Employments)
                    .Include(p => p.PublicHolidays)
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

        public PagedResult<AbsencePolicy> RetrieveAbsencePolicies(int organisationId, Expression<Func<AbsencePolicy, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsencePolicies
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

        public PagedResult<AbsencePolicyEntitlement> RetrieveAbsencePolicyEntitlements(int organisationId, int absencePolicyId, List<OrderBy> orderBy, Paging paging)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                .AbsencePolicyEntitlements
                .Include(c => c.AbsenceType)
                .Include(c => c.AbsenceType.Colour)
                .Include(c => c.Frequency)
                .AsNoTracking()
                .Where(c => c.AbsencePolicyId == absencePolicyId)
                .OrderBy(orderBy ?? new List<OrderBy>
                    {
                    new OrderBy
                    {
                        Property = "AbsenceType.Name",
                        Direction = System.ComponentModel.ListSortDirection.Descending
                    }
                    })
                    .Paginate(paging);
            }
        }

        public PagedResult<PublicHoliday> RetrievePublicHolidays(int organisationId, int publicHolidayPolicyId,
            Expression<Func<PublicHoliday, bool>> predicate, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                .PublicHolidays
                .AsNoTracking()
                .Where(p => p.PublicHolidayPolicyId == publicHolidayPolicyId)
                .Where(predicate)
                .OrderBy(orderBy ?? new List<OrderBy>
                        {
                        new OrderBy
                        {
                            Property = "Date",
                            Direction = System.ComponentModel.ListSortDirection.Descending
                        }
                        })
                        .Paginate(paging);
            }
        }

        public AbsencePolicyEntitlement RetrieveAbsencePolicyEntitlement(int organisationId, int absencePolicyEntitlementId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return
                    context.AbsencePolicyEntitlements.FirstOrDefault(
                        e => e.AbsencePolicyEntitlementId == absencePolicyEntitlementId);

            }
        }

        public AbsencePolicyPeriod CreateAbsencePolicyAbsencePeriod(int organisationId, AbsencePolicyPeriod absencePolicyPeriod)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context.AbsencePolicyPeriods.Add(absencePolicyPeriod);
                context.SaveChanges();
                return result;
            }
        }

        public PagedResult<AbsencePolicyPeriod> RetrieveAbsencePolicyAbsencePeriods(int organisationId, int absencePolicyId, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                  .AbsencePolicyPeriods
                  .Include(d => d.AbsencePeriod)
                  .Include(d => d.AbsencePolicy)
                  .AsNoTracking()
                  .Where(s => s.AbsencePolicyId == absencePolicyId)
                  .OrderBy(orderBy ?? new List<OrderBy> {
                            new OrderBy { Property = "AbsencePeriod.StartDate", Direction = System.ComponentModel.ListSortDirection.Ascending }
                   })
                   .Paginate(paging);
            }
        }

        public PagedResult<AbsencePolicyPeriod> RetrieveAbsencePolicyAbsencePeriodsByPersonnel(int organisationId, int personnelId, List<OrderBy> orderBy = null, Paging paging = null)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsencePolicyPeriods
                    .Include(d => d.AbsencePeriod)
                    .Include(d => d.AbsencePolicy)
                    .Include(d => d.AbsencePolicy.Employments)
                    .AsNoTracking()
                    .Where(d => d.AbsencePolicy.Employments.Any(e => e.PersonnelId == personnelId))
                     .OrderBy(orderBy ?? new List<OrderBy> {
                            new OrderBy { Property = "AbsencePeriod.StartDate", Direction = System.ComponentModel.ListSortDirection.Ascending }
                   })
                   .Paginate(paging);
            }
        }

        public bool RetrieveAbsencesOfAbsencePolicyPeriod(int organisationId, int absencePolicyPeriodId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                    .PersonnelAbsenceEntitlements
                    .Include(a => a.Absences)
                    .AsNoTracking()
                    .Any(h => h.AbsencePolicyPeriodId == absencePolicyPeriodId && h.Absences.Any());
                return result;
            }
        }

        public AbsencePolicy RetrieveAbsencePolicy(int organisationId, int absencePolicyId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                    .AbsencePolicies
                    .Include(a => a.WorkingPattern)
                    .Include(a => a.WorkingPattern.WorkingPatternDays)
                    .Include(a => a.AbsencePolicyEntitlements)
                    .Include(a => a.AbsencePolicyPeriods)
                    .AsNoTracking()
                    .FirstOrDefault(h => h.AbsencePolicyId == absencePolicyId);
                return result;
            }
        }

        public bool AbsencePolicyPersonnelEmploymentHasAbsences(int organisationId, int employmentId, int absencePolicyId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                    .Absences
                    .Include(a => a.PersonnelAbsenceEntitlement)
                    .Include(a => a.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements)
                    .AsNoTracking()
                    .Any(a => a.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements.
                        Any(
                            e =>
                                e.Employment.AbsencePolicyId == absencePolicyId &&
                                e.Employment.EmploymentId == employmentId));

                return result;
            }
        }

        public IEnumerable<EmploymentPersonnelAbsenceEntitlement> RetrieveEmploymentPersonnelAbsenceEntitlements(
            int organisationId, Expression<Func<EmploymentPersonnelAbsenceEntitlement, bool>> predicate)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .EmploymentPersonnelAbsenceEntitlements
                    .Include(d => d.Employment)
                    .Include(d => d.PersonnelAbsenceEntitlement)
                    .AsNoTracking()
                    .Where(predicate).ToList();
            }
        }

        public IEnumerable<AbsenceType> RetrieveAbsenceTypes(int organisationId, int absencePolicyId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .AbsencePolicyEntitlements
                    .Include(c => c.AbsenceType)
                    .Where(c => c.AbsencePolicyId == absencePolicyId).Select(e => e.AbsenceType).ToList();
            }
        }

        public IEnumerable<PublicHoliday> RetrievePublicHolidays(int organisationId, int publicHolidayPolicyId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .PublicHolidays
                    .AsNoTracking()
                    .Where(p => p.PublicHolidayPolicyId == publicHolidayPolicyId).ToList();
            }
        }


        //Update
        public void UpdateAbsencePolicyWorkingPattern(int organisationId, int workingPatternId, List<WorkingPatternDay> workingPatternDays)
        {
            DeleteWorkingPatternDays(organisationId, workingPatternId);
            CreateWorkingPatternDays(organisationId, workingPatternDays);
        }

        //Delete
        public void DeleteAbsencePolicyPeriod(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absencePolicyPeriod = context.AbsencePolicyPeriods.Find(id);
                if (absencePolicyPeriod == null)
                    return;
                var personnelAbsenceEntitlement = context.PersonnelAbsenceEntitlements.Where(e => e.AbsencePolicyPeriodId == id);
                var employmentPersonnelAbsenceEntitlements =
                    personnelAbsenceEntitlement.Select(s => s.EmploymentPersonnelAbsenceEntitlements);
                if (employmentPersonnelAbsenceEntitlements.Any())
                {
                    foreach (var employmentPersonnelAbsenceEntitlement in employmentPersonnelAbsenceEntitlements)
                    {
                        context.EmploymentPersonnelAbsenceEntitlements.RemoveRange(employmentPersonnelAbsenceEntitlement);
                    }

                }
                if (personnelAbsenceEntitlement.Any())
                {
                    context.PersonnelAbsenceEntitlements.RemoveRange(personnelAbsenceEntitlement);
                }
                context.AbsencePolicyPeriods.Remove(absencePolicyPeriod);
                context.SaveChanges();
            }
        }

        public void DeleteAbsencePolicy(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absencePolicy = context
                     .AbsencePolicies
                     .Find(id);
                if (absencePolicy == null)
                    return;

                var absencePolicyEntitlement = context.AbsencePolicyEntitlements.Where(e => e.AbsencePolicyId == id);
                if (absencePolicyEntitlement.Any())
                {
                    context.AbsencePolicyEntitlements.RemoveRange(absencePolicyEntitlement);
                }
                var absencePolicyPeriod = context.AbsencePolicyPeriods.Where(e => e.AbsencePolicyId == id);
                if (absencePolicyPeriod.Any())
                {
                    context.AbsencePolicyPeriods.RemoveRange(absencePolicyPeriod);
                }

                context.AbsencePolicies.Remove(absencePolicy);
                context.SaveChanges();
            }
        }

        public bool RetrieveAbsencesOfAbsencePolicyAbsenceType(int organisationId, int absencePolicyId, int absenceTypeId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                var result = context
                    .PersonnelAbsenceEntitlements
                    .Include(a => a.Absences)
                    .Include(a => a.AbsencePolicyPeriod)
                    .AsNoTracking()
                    .Any(h => h.AbsenceTypeId == absenceTypeId && h.AbsencePolicyPeriod.AbsencePolicyId == absencePolicyId
                        && h.Absences.Any());
                return result;
            }
        }

        public void DeleteAbsencePolicyAbsenceType(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var absencePolicyEntitlement = context.AbsencePolicyEntitlements.Find(id);
                if (absencePolicyEntitlement == null)
                    return;
                context.AbsencePolicyEntitlements.Remove(absencePolicyEntitlement);
                context.SaveChanges();
            }
        }

        public void DeletePublicHoliday(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var publicHoliday = context.PublicHolidays.Find(id);
                if (publicHoliday == null)
                    return;
                context.PublicHolidays.Remove(publicHoliday);
                context.SaveChanges();
            }
        }

        public void DeletePublicHolidayPolicy(int organisationId, int id)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var publicHolidayPolicy = context.PublicHolidayPolicies.Find(id);
                if (publicHolidayPolicy == null)
                    return;
                var publicHolidays = context.PublicHolidays.Where(e => e.PublicHolidayPolicyId == id);
                context.PublicHolidays.RemoveRange(publicHolidays);
                context.PublicHolidayPolicies.Remove(publicHolidayPolicy);
                context.SaveChanges();
            }
        }


        public void DeletePersonnelAbsenceEntitlementsForAbsencePolicyPeriod(int organisationId, int absencePolicyPeriodId)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var personnelAbsenceEntitlement = context
                  .PersonnelAbsenceEntitlements
                  .Where(e => e.AbsencePolicyPeriodId == absencePolicyPeriodId);
                if (!personnelAbsenceEntitlement.Any())
                    return;

                context.PersonnelAbsenceEntitlements.RemoveRange(personnelAbsenceEntitlement);
                context.SaveChanges();
            }
        }

        public void DeletePersonnelAbsenceEntitlementsForAbsencePolicyAbsenceType(int organisationId, int absencePolicyId, int absenceTypeId)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var personnelAbsenceEntitlement = context
                  .PersonnelAbsenceEntitlements
                  .Where(e => e.AbsenceTypeId == absenceTypeId && e.AbsencePolicyPeriod.AbsencePolicyId == absencePolicyId);
                if (!personnelAbsenceEntitlement.Any())
                    return;

                var employmentPersonnelAbsenceEntitlements = new List<EmploymentPersonnelAbsenceEntitlement>();
                foreach (var item in personnelAbsenceEntitlement)
                {
                    var employmentPersonnelAbsenceEntitlement =
                        context.EmploymentPersonnelAbsenceEntitlements.Where(
                            s => s.PersonnelAbsenceEntitlementId == item.PersonnelAbsenceEntitlementId).ToList();
                    employmentPersonnelAbsenceEntitlements.AddRange(employmentPersonnelAbsenceEntitlement);


                }

                context.EmploymentPersonnelAbsenceEntitlements.RemoveRange(employmentPersonnelAbsenceEntitlements);
                context.PersonnelAbsenceEntitlements.RemoveRange(personnelAbsenceEntitlement);
                context.SaveChanges();
            }
        }


        public void DeleteJobTitleJobGrade(int organisationId, int jobTitleId, int jobGradeId)
        {
            using (var context = _databaseFactory.Create(organisationId))
            {
                var jobTitleJobGrade = context
                  .JobTitleJobGrades
                  .Where(e => e.JobTitleId == jobTitleId && e.JobGrade.JobGradeId == jobGradeId);
                if (!jobTitleJobGrade.Any())
                    return;

                context.JobTitleJobGrades.RemoveRange(jobTitleJobGrade);
                context.SaveChanges();
            }
        }

        public IEnumerable<Employment> RetrieveActiveEmploymentsByAbsencePolicy(int organisationId, int absencePolicyId)
        {
            using (ReadUncommitedTransactionScope)
            using (var context = _databaseFactory.Create(organisationId))
            {
                return context
                    .Employments
                    .Where(e => e.AbsencePolicyId == absencePolicyId &&
                        !e.TerminationDate.HasValue && (!e.EndDate.HasValue || DbFunctions.TruncateTime(e.EndDate.Value) >= Today))
                    .Include(e => e.Building)
                    .Include(e => e.Building.Site)
                    .AsNoTracking()
                    .ToList();
            }
        }
    }
}
