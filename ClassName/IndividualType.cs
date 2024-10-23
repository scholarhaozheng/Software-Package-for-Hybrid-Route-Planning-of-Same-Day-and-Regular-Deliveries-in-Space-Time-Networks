using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class IndividualType
    {

        public string m_name;

        public IndividualType(string name)
        {
            this.m_name = name;
        }

        public static implicit operator IndividualType(string name)
        {
            return new IndividualType(name);
        }

        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is IndividualType type &&
                   m_name == type.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }
        public static bool operator ==(IndividualType i1, IndividualType i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(IndividualType i1, IndividualType i2)
        {
            return !Equals(i1, i2);
        }
    }
}



