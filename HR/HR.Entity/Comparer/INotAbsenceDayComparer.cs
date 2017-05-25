using HR.Entity.Interfaces;
using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class INotAbsenceDayComparer : IEqualityComparer<INotAbsenceDay>
    {
        public bool Equals(INotAbsenceDay x, INotAbsenceDay y)
        {
            return x.Date.Date == y.Date.Date;
        }

        public int GetHashCode(INotAbsenceDay obj)
        {
           return obj.Date.GetHashCode();
        }
    }
    
}
