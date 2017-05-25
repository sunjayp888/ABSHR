using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class CompanyComparer : IEqualityComparer<Company>
    {
        public bool Equals(Company x, Company y)
        {
            return x.CompanyId == y.CompanyId && x.Name == y.Name;
        }

        public int GetHashCode(Company obj)
        {
            if (obj == null)
                return 0;

            return (obj.CompanyId << 16) ^ (obj.Name.GetHashCode() << 8);
        }
    }
    
}
