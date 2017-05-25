using HR.Entity.Dto;
using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class ScheduleItemTypeComparer : IEqualityComparer<ScheduleItemType>
    {
        public bool Equals(ScheduleItemType x, ScheduleItemType y)
        {
            return x.Name == y.Name && x.Colour == y.Colour;
        }

        public int GetHashCode(ScheduleItemType obj)
        {
            if (obj == null)
                return 0;

            return (obj.Name.GetHashCode() << 16) ^ (obj.Colour.GetHashCode() << 8);
        }
    }
    
}
