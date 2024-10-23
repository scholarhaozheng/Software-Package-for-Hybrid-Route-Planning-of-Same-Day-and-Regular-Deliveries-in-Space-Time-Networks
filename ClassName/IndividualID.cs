using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class IndividualID
    {

        public string m_name;

        public IndividualID(string name)
        {
            this.m_name = name;
        }

        public static implicit operator IndividualID(string name)
        {
            return new IndividualID(name);
        }

        public override string ToString()
        {
            return m_name;
        }

        public override bool Equals(object obj)
        {
            return obj is IndividualID iD &&
                   m_name == iD.m_name;
        }

        public override int GetHashCode()
        {
            return 1904378486 + EqualityComparer<string>.Default.GetHashCode(m_name);
        }

        public static bool operator ==(IndividualID i1, IndividualID i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(IndividualID i1, IndividualID i2)
        {
            return !Equals(i1, i2);
        }

        public int IDIndex;
        public int subTime;
        public IndividualType individualType;
        public string UpOrDown;
        public string OutOrBack;
        public string O_or_D_or_M_none;
        public Point if_vis_veh_then_the_other_half;
        public IndividualID Copying_info(IndividualID old_ID)
        {
            old_ID.IDIndex= this.IDIndex;
            old_ID.subTime= this.subTime;
            old_ID.individualType= this.individualType;
            old_ID.UpOrDown= this.UpOrDown;
            old_ID.OutOrBack= this.OutOrBack;
            return old_ID;
        }
    }
}



