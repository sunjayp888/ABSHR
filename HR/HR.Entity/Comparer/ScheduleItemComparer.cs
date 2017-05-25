using HR.Entity.Dto;
using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class ScheduleItemComparer : IEqualityComparer<ScheduleItem>
    {
        public bool Equals(ScheduleItem x, ScheduleItem y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(ScheduleItem obj)
        {
           return obj.Name.GetHashCode();
        }
    }
    
}
