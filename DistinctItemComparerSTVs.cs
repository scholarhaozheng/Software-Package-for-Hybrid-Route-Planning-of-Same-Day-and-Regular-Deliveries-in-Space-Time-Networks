
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimizationIntegrationSystem.zhenghao.ClassName;

namespace OptimizationIntegrationSystem.zhenghao
{
    class DistinctItemComparerSTVs : IEqualityComparer<SpaceTimeVertices>
    {
        public bool Equals(SpaceTimeVertices x, SpaceTimeVertices y)
        {
            return x.spaceTimeVertex1.Equals(y.spaceTimeVertex1) && x.spaceTimeVertex2.Equals(y.spaceTimeVertex2);
        }

        public int GetHashCode(SpaceTimeVertices obj)
        {
            return obj.spaceTimeVertex1.GetHashCode() ^
                obj.spaceTimeVertex2.GetHashCode();
        }
    }
}
