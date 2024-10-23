using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimizationIntegrationSystem.zhenghao.ClassName;

namespace OptimizationIntegrationSystem.zhenghao
{
    class DistinctItemComparer:IEqualityComparer<SpaceTimeVertex>
    {
        public bool Equals(SpaceTimeVertex x, SpaceTimeVertex y)
        {
            return x.spaceVertex.Equals(y.spaceVertex) &&x.timeVertex == y.timeVertex;}

        public int GetHashCode(SpaceTimeVertex obj)
        {
            return obj.spaceVertex.GetHashCode() ^
                obj.timeVertex.GetHashCode();
        }
    }
}
