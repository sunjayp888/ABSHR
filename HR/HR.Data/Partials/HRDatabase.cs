using HR.Entity;
using System.Data.Entity;

namespace HR.Data.Models
{
    /// Ensure the generated HRDatabase also references OrganisationDbContext
    /// and the OnModelCreating has the following as its last line of code:  base.OnModelCreating(modelBuilder);
    public partial class HRDatabase : OrganisationDbContext
    {
        public HRDatabase(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Initialise();
        }

        public HRDatabase(string nameOrConnectionString, int organisationId) : base(nameOrConnectionString, organisationId)
        {
            Initialise();
        }

        private void Initialise()
        {
            //Disable initializer
            Database.SetInitializer<HRDatabase>(null);
            Database.CommandTimeout = 300;
            Configuration.ProxyCreationEnabled = false;
        }

        // Ensure this function is called with in the generated HRDatabase

        protected void PersonnelModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employment>()
                   .HasMany(e => e.EmploymentPersonnelAbsenceEntitlements)
                   .WithRequired(e => e.Employment)
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

            modelBuilder.Entity<PersonnelAbsenceEntitlement>()
                .HasMany(e => e.EmploymentPersonnelAbsenceEntitlements)
                .WithRequired(e => e.PersonnelAbsenceEntitlement)
                .WillCascadeOnDelete(false);

        }

    }
}
