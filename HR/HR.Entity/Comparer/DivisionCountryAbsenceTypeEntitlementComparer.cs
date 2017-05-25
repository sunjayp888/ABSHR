using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class DivisionCountryAbsenceTypeEntitlementComparer : IEqualityComparer<DivisionCountryAbsenceTypeEntitlement>
    {
        public bool Equals(DivisionCountryAbsenceTypeEntitlement x, DivisionCountryAbsenceTypeEntitlement y)
        {
            return x.DivisionId == y.DivisionId && x.CountryAbsenceTypeId == y.CountryAbsenceTypeId;
        }

        public int GetHashCode(DivisionCountryAbsenceTypeEntitlement obj)
        {
            if (obj == null)
                return 0;

            return (obj.DivisionId << 16) ^ (obj.CountryAbsenceTypeId << 8);

        }
    }
    
}
