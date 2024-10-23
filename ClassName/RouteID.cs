using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class RouteID
    {
        private string m_name;
        public int ID_Index;
        public IndividualID whoIsServing;
        public List<IndividualID> servedWhom;
        public double route_lambda;
        public RouteID(string name)
        {
            this.m_name = name;
        }

        public static implicit operator RouteID(string name)
        {
            return new RouteID(name);
        }
        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is RouteID iD &&
                   m_name == iD.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }

        public List<Point> arcSpaceVertices;

        public static bool operator ==(RouteID i1, RouteID i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(RouteID i1, RouteID i2)
        {
            return !Equals(i1, i2);
        }
    }
}



