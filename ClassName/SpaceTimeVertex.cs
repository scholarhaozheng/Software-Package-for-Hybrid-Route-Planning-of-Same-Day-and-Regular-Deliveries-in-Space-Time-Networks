using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao.ClassName
{
    [Serializable]
    public class SpaceTimeVertex
    {
        public Point spaceVertex;
        public int timeVertex;
        public SpaceTimeVertex(Point spaceVertex, int timeVertex)
        {
            this.spaceVertex = spaceVertex;
            this.timeVertex = timeVertex;
        }

        public override bool Equals(object obj)
        {
            return obj is SpaceTimeVertex vertex &&
                   EqualityComparer<Point>.Default.Equals(spaceVertex, vertex.spaceVertex) &&
                   timeVertex == vertex.timeVertex;
        }

        public override int GetHashCode()
        {
            int hashCode = 2091219409;
            hashCode = hashCode * -1521134295 + spaceVertex.GetHashCode();
            hashCode = hashCode * -1521134295 + timeVertex.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(SpaceTimeVertex i1, SpaceTimeVertex i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(SpaceTimeVertex i1, SpaceTimeVertex i2)
        {
            return !Equals(i1, i2);
        }
    }
}
