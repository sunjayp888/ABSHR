using HR.Entity.Dto;
using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class DivisionCountryComparer : IEqualityComparer<DivisionCountry>
    {
        public bool Equals(DivisionCountry x, DivisionCountry y)
        {
            return x.DivisionId == y.DivisionId && x.CountryId == y.CountryId;
        }

        public int GetHashCode(DivisionCountry obj)
        {
            if (obj == null)
                return 0;

            return (obj.DivisionId << 16) ^ (obj.CountryId << 8);
        }
    }
    
}
