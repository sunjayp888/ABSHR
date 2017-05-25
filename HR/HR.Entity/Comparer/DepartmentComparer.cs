using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class DepartmentComparer : IEqualityComparer<Department>
    {
        public bool Equals(Department x, Department y)
        {
            return x.DepartmentId == y.DepartmentId && x.Name == y.Name;
        }

        public int GetHashCode(Department obj)
        {
            if (obj == null)
                return 0;

            return (obj.DepartmentId << 16) ^ (obj.Name.GetHashCode() << 8);
        }
    }
    
}
