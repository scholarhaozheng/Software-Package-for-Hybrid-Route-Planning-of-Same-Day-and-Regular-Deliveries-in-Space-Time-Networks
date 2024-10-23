using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class ArcTime
    {

        private string m_name;

        public ArcTime(string name)
        {
            this.m_name = name;
        }

        public static implicit operator ArcTime(string name)
        {
            return new ArcTime(name);
        }

        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is ArcTime time &&
                   m_name == time.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }

        public static bool operator ==(ArcTime i1, ArcTime i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(ArcTime i1, ArcTime i2)
        {
            return !Equals(i1, i2);
        }
    }
}



