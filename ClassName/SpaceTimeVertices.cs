using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao.ClassName
{
    [Serializable]
    public class SpaceTimeVertices
    {
        public SpaceTimeVertex spaceTimeVertex1;
        public SpaceTimeVertex spaceTimeVertex2;
        public SpaceTimeVertices(SpaceTimeVertex spaceTimeVertex1, SpaceTimeVertex spaceTimeVertex2)
        {
            this.spaceTimeVertex1 = spaceTimeVertex1;
            this.spaceTimeVertex2 = spaceTimeVertex2;
        }

        public override bool Equals(object obj)
        {
            return obj is SpaceTimeVertices vertices &&
                   EqualityComparer<SpaceTimeVertex>.Default.Equals(spaceTimeVertex1, vertices.spaceTimeVertex1) &&
                   EqualityComparer<SpaceTimeVertex>.Default.Equals(spaceTimeVertex2, vertices.spaceTimeVertex2);
        }

        public override int GetHashCode()
        {
            int hashCode = 1807588833;
            hashCode = hashCode * -1521134295 + EqualityComparer<SpaceTimeVertex>.Default.GetHashCode(spaceTimeVertex1);
            hashCode = hashCode * -1521134295 + EqualityComparer<SpaceTimeVertex>.Default.GetHashCode(spaceTimeVertex2);
            return hashCode;
        }

        public static bool operator ==(SpaceTimeVertices i1, SpaceTimeVertices i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(SpaceTimeVertices i1, SpaceTimeVertices i2)
        {
            return !Equals(i1, i2);
        }

        public string GetSTVName()
        {
            string t_str;
            string tt_str;
            if (spaceTimeVertex1.timeVertex < 10)
            {
                t_str = "0" + spaceTimeVertex1.timeVertex.ToString();
            }
            else
            {
                t_str = spaceTimeVertex1.timeVertex.ToString();
            }
            if (spaceTimeVertex2.timeVertex < 10)
            {
                tt_str = "0" + spaceTimeVertex2.timeVertex.ToString();
            }
            else
            {
                tt_str = spaceTimeVertex2.timeVertex.ToString();
            }
            string STV_Name = "t_" + t_str + "_tt_" + tt_str
                + "_i_" + spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertex2.spaceVertex.individualID.ToString();
            return STV_Name;
        }

    }
}
