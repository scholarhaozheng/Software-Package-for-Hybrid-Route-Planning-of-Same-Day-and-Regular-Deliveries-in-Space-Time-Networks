using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class PointType
    {

        private string m_name;

        public PointType(string name)
        {
            this.m_name = name;
        }

        public static implicit operator PointType(string name)
        {
            return new PointType(name);
        }//　加入 implicit operator 后可以直接给类型调用方法赋值,不用实例化

        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is PointType type &&
                   m_name == type.m_name;//如果obj是PointType且它的m_name == type.m_name
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
            //该 Default 属性检查类型 T 是否实现 System.IEquatable<T> 接口，如果是，则返回使用该实现的一个 EqualityComparer<T> 。
        }

        public static bool operator ==(PointType i1, PointType i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(PointType i1, PointType i2)
        {
            return !Equals(i1, i2);
        }
    }
}



