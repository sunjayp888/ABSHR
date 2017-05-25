using HR.Data.Interfaces;
using System;

namespace HR.Data.Models
{
    public class HRDatabaseFactory : IHRDatabaseFactory
    {
        public string NameOrConnectionString { get; }

        public HRDatabaseFactory(string nameOrConnectionString)
        {
            NameOrConnectionString = nameOrConnectionString;
        }

        public HRDatabase Create()
        {
            ValidateConnectionString();
            return new HRDatabase(NameOrConnectionString);
        }

        public HRDatabase Create(int organisationId)
        {
            ValidateConnectionString();
            return new HRDatabase(NameOrConnectionString, organisationId);
        }

        private void ValidateConnectionString()
        {
            if (string.IsNullOrWhiteSpace(NameOrConnectionString))
                throw new NullReferenceException("OmbrosDatabaseFactory expects a valid NameOrConnectionString");
        }
    }
}
