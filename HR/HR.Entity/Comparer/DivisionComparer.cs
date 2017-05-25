using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class DivisionComparer : IEqualityComparer<Division>
    {
        public bool Equals(Division x, Division y)
        {
            return x.DivisionId == y.DivisionId && x.Name == y.Name;
        }

        public int GetHashCode(Division obj)
        {
            if (obj == null)
                return 0;

            return (obj.DivisionId << 16) ^ (obj.Name.GetHashCode() << 8);
        }
    }
    
}
