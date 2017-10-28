namespace HR.Data.Models
{
    using Entity;
    using System.Data.Entity;

    public partial class HRDatabase : OrganisationDbContext
    {
        public HRDatabase() : base("name=HRDatabase")
        {
        }

        public virtual DbSet<Absence> Absences { get; set; }
        public virtual DbSet<AbsenceDay> AbsenceDays { get; set; }
        public virtual DbSet<AbsencePeriod> AbsencePeriods { get; set; }
        public virtual DbSet<AbsencePolicy> AbsencePolicies { get; set; }
        public virtual DbSet<AbsencePolicyEntitlement> AbsencePolicyEntitlements { get; set; }
        public virtual DbSet<AbsencePolicyPeriod> AbsencePolicyPeriods { get; set; }
        //public virtual DbSet<AbsenceStatus> AbsenceStatus { get; set; }
        public virtual DbSet<AbsenceType> AbsenceTypes { get; set; }
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<ApprovalEntityType> ApprovalEntities { get; set; }
        public virtual DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        public virtual DbSet<ApprovalLevelUser> ApprovalLevelUsers { get; set; }
        public virtual DbSet<ApprovalModel> ApprovalModels { get; set; }
        public virtual DbSet<ApprovalState> ApprovalStates { get; set; }
        public virtual DbSet<AspNetUsersAlertSchedule> AspNetUsersAlertSchedules { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Colour> Colours { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyBuilding> CompanyBuildings { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public virtual DbSet<Employment> Employments { get; set; }
        public virtual DbSet<EmploymentDepartment> EmploymentDepartments { get; set; }
        public virtual DbSet<EmploymentTeam> EmploymentTeams { get; set; }
        public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }
        public virtual DbSet<Frequency> Frequencies { get; set; }
        public virtual DbSet<Host> Hosts { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }
        public virtual DbSet<Overtime> Overtimes { get; set; }
        public virtual DbSet<OvertimeState> OvertimeStates { get; set; }
        public virtual DbSet<OvertimePreference> OvertimePreferences { get; set; }
        public virtual DbSet<Personnel> Personnels { get; set; }
        public virtual DbSet<PersonnelAbsenceEntitlement> PersonnelAbsenceEntitlements { get; set; }
        public virtual DbSet<PersonnelApprovalModel> PersonnelApprovalModels { get; set; }
        public virtual DbSet<PublicHoliday> PublicHolidays { get; set; }
        public virtual DbSet<PublicHolidayPolicy> PublicHolidayPolicies { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<WorkingPattern> WorkingPatterns { get; set; }
        public virtual DbSet<WorkingPatternDay> WorkingPatternDays { get; set; }
        public virtual DbSet<PersonnelSearchField> PersonnelSearchFields { get; set; }
        public virtual DbSet<UserAuthorisationFilter> UserAuthorisationFilters { get; set; }
        public virtual DbSet<JobGrade> JobGrades { get; set; }
        public virtual DbSet<JobTitle> JobTitles { get; set; }
        public virtual DbSet<EmploymentPersonnelAbsenceEntitlement> EmploymentPersonnelAbsenceEntitlements { get; set; }
        public virtual DbSet<JobTitleJobGrade> JobTitleJobGrades { get; set; }
        public virtual DbSet<Template> Templates { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Absence>()
                .HasMany(e => e.AbsenceDays)
                .WithRequired(e => e.Absence)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsencePeriod>()
                .HasMany(e => e.AbsencePolicyPeriods)
                .WithRequired(e => e.AbsencePeriod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsencePolicy>()
                .HasMany(e => e.AbsencePolicyEntitlements)
                .WithRequired(e => e.AbsencePolicy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsencePolicy>()
                .HasMany(e => e.AbsencePolicyPeriods)
                .WithRequired(e => e.AbsencePolicy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsencePolicy>()
                .HasMany(e => e.Employments)
                .WithRequired(e => e.AbsencePolicy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsencePolicyPeriod>()
                .HasMany(e => e.PersonnelAbsenceEntitlements)
                .WithRequired(e => e.AbsencePolicyPeriod)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<AbsenceStatus>()
            //    .Property(e => e.Name)
            //    .IsUnicode(false);

            //modelBuilder.Entity<AbsenceStatus>()
            //    .HasMany(e => e.Absences)
            //    .WithRequired(e => e.AbsenceStatus)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsenceType>()
                .HasMany(e => e.Absences)
                .WithRequired(e => e.AbsenceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AbsenceType>()
                .HasMany(e => e.AbsencePolicyEntitlements)
                .WithRequired(e => e.AbsenceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Alert>()
                .HasMany(e => e.AspNetUsersAlertSchedules)
                .WithRequired(e => e.Alert)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalEntityType>()
                .HasMany(e => e.Approvals)
                .WithRequired(e => e.ApprovalEntity)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalEntityType>()
                .HasMany(e => e.PersonnelApprovalModels)
                .WithRequired(e => e.ApprovalEntity)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalLevel>()
                .HasMany(e => e.Approvals)
                .WithRequired(e => e.ApprovalLevel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalLevel>()
                .HasMany(e => e.ApprovalLevelUsers)
                .WithRequired(e => e.ApprovalLevel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalModel>()
                .HasMany(e => e.ApprovalLevels)
                .WithRequired(e => e.ApprovalModel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalModel>()
                .HasMany(e => e.PersonnelApprovalModels)
                .WithRequired(e => e.ApprovalModel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApprovalState>()
                .HasMany(e => e.Approvals)
                .WithRequired(e => e.ApprovalState)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Building>()
                .HasMany(e => e.CompanyBuildings)
                .WithRequired(e => e.Building)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Building>()
                .HasMany(e => e.Employments)
                .WithRequired(e => e.Building)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Colour>()
                .Property(e => e.Hex)
                .IsUnicode(false);

            modelBuilder.Entity<Colour>()
                .HasMany(e => e.AbsenceTypes)
                .WithRequired(e => e.Colour)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Colour>()
                .HasMany(e => e.Teams)
                .WithRequired(e => e.Colour)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Colour>()
                .HasMany(e => e.Companies)
                .WithRequired(e => e.Colour)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Colour>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Colour)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.CompanyBuildings)
                .WithRequired(e => e.Company)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.Employments)
                .WithRequired(e => e.Company)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.Personnels)
                .WithRequired(e => e.Country)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.Sites)
                .WithRequired(e => e.Country)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.EmploymentDepartments)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EmergencyContact>()
                .Property(e => e.Relationship)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyContact>()
                .Property(e => e.Telephone)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyContact>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<Employment>()
                .HasMany(e => e.EmploymentDepartments)
                .WithRequired(e => e.Employment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employment>()
                .HasMany(e => e.EmploymentTeams)
                .WithRequired(e => e.Employment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Frequency>()
                .HasMany(e => e.AbsencePolicyEntitlements)
                .WithRequired(e => e.Frequency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Frequency>()
                .HasMany(e => e.PersonnelAbsenceEntitlements)
                .WithRequired(e => e.Frequency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Absences)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AbsenceDays)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AbsencePeriods)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AbsencePolicies)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AbsencePolicyEntitlements)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AbsencePolicyPeriods)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AbsenceTypes)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Alerts)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.ApprovalLevels)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.ApprovalModels)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Companies)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Countries)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.EmergencyContacts)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Employments)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.EmploymentDepartments)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.EmploymentTeams)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.EmploymentTypes)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Hosts)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Teams)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Personnels)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.PersonnelAbsenceEntitlements)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.PersonnelApprovalModels)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.PublicHolidays)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.PublicHolidayPolicies)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.Sites)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.WorkingPatterns)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.JobGrades)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.JobTitles)
                .WithRequired(e => e.Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.Telephone)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.NINumber)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.BankAccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.BankSortCode)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.BankTelephone)
                .IsUnicode(false);
            
            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.PersonnelApprovalModels)
                .WithRequired(e => e.Personnel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Personnel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.EmergencyContacts)
                .WithRequired(e => e.Personnel)
                .WillCascadeOnDelete(false);

            PersonnelModelCreating(modelBuilder);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.PersonnelAbsenceEntitlements)
                .WithRequired(e => e.Personnel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonnelAbsenceEntitlement>()
                .HasMany(e => e.Absences)
                .WithRequired(e => e.PersonnelAbsenceEntitlement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PublicHolidayPolicy>()
                .HasMany(e => e.Employments)
                .WithRequired(e => e.PublicHolidayPolicy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PublicHolidayPolicy>()
                .HasMany(e => e.PublicHolidays)
                .WithRequired(e => e.PublicHolidayPolicy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Site>()
                .HasMany(e => e.Buildings)
                .WithRequired(e => e.Site)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(e => e.EmploymentTeams)
                .WithRequired(e => e.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkingPattern>()
                .HasMany(e => e.WorkingPatternDays)
                .WithRequired(e => e.WorkingPattern)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonnelSearchField>()
                .Property(e => e.Telephone)
                .IsUnicode(false);

            modelBuilder.Entity<PersonnelSearchField>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<PersonnelSearchField>()
                .Property(e => e.NINumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonnelSearchField>()
                .Property(e => e.BankAccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonnelSearchField>()
                .Property(e => e.BankSortCode)
                .IsUnicode(false);

            modelBuilder.Entity<PersonnelSearchField>()
                .Property(e => e.BankTelephone)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
               .Property(e => e.Telephone)
               .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.NINumber)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.BankAccountNumber);

            modelBuilder.Entity<JobTitle>()
               .HasMany(e => e.JobTitleJobGrades)
               .WithRequired(e => e.JobTitle)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<JobGrade>()
               .HasMany(e => e.JobTitleJobGrades)
               .WithRequired(e => e.JobGrade)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.BankSortCode)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.BankTelephone)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.Employments)
                .WithRequired(e => e.Personnel)
                .HasForeignKey(e => e.PersonnelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.ManageEmployments)
                .WithOptional(e => e.ReportsToPersonnel)
                .HasForeignKey(e => e.ReportsToPersonnelId);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.PersonnelAbsenceEntitlements)
                .WithRequired(e => e.Personnel)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}