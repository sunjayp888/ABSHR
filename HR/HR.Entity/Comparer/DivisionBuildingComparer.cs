using HR.Entity.Dto;
using System.Collections.Generic;

namespace HR.Entity.Comparer
{
    public class DivisionBuildingComparer : IEqualityComparer<DivisionBuilding>
    {
        public bool Equals(DivisionBuilding x, DivisionBuilding y)
        {
            return x.Name.Equals(y.Name) && x.DivisionId == y.DivisionId && x.BuildingId == y.BuildingId;
        }

        public int GetHashCode(DivisionBuilding obj)
        {
            if (obj == null)
                return 0;

            return (obj.DivisionId << 16) ^ (obj.BuildingId << 8);
        }
    }
}
