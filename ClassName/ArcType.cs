using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class ArcType
    {

        private string m_name;

        public ArcType(string name)
        {
            this.m_name = name;
        }

        public static implicit operator ArcType(string name)
        {
            return new ArcType(name);
        }

        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is ArcType type &&
                   m_name == type.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }
    }
}



