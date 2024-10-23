using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class ArcID
    {
        private string m_name;
        public ArcID(string name)
        {
            this.m_name = name;
        }

        public static implicit operator ArcID(string name)
        {
            return new ArcID(name);
        }
        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is ArcID iD &&
                   m_name == iD.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }

        public List<Point> arcSpaceVertices;

        public static bool operator ==(ArcID i1, ArcID i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(ArcID i1, ArcID i2)
        {
            return !Equals(i1, i2);
        }
    }
}



