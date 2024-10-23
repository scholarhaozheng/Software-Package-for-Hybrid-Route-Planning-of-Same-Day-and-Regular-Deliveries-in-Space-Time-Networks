using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimizationIntegrationSystem.zhenghao.ClassName;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public class Arc : IEquatable<Arc>
    {
        public IndividualType individualType;
        public IndividualID individualID;//
        public ArcType arcType;
        public ArcID arcID;
        public ArcTime arcTime;
        public List<Point> spaceVertices;//
        public List<int> timeVertices;//
        public double value;
        public SpaceTimeVertex SpaceTimeVertex1;
        public SpaceTimeVertex SpaceTimeVertex2;
        public SpaceTimeVertices spaceTimeVertices;
        public string arcVarName;
        public Arc(IndividualType individualType, IndividualID individualID, ArcType arcType,ArcID arcID, List<Point> spaceVertices, List<int> timeVertices, double value, SpaceTimeVertex SpaceTimeVertex1, SpaceTimeVertex SpaceTimeVertex2, ArcTime arcTime, SpaceTimeVertices spaceTimeVertices) 
        { 
            this.individualType = individualType;
            this.individualID = individualID;
            this.arcType = arcType;
            this.arcID = arcID;
            this.spaceVertices = spaceVertices;
            this.timeVertices = timeVertices;
            this.value = value;
            this.SpaceTimeVertex2 = SpaceTimeVertex2;
            this.SpaceTimeVertex1= SpaceTimeVertex1;
            this.arcTime= arcTime;
            this.spaceTimeVertices= spaceTimeVertices;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Arc);
        }

        public bool Equals(Arc other)
        {
            return !(other is null) &&
                   EqualityComparer<IndividualType>.Default.Equals(individualType, other.individualType) &&
                   EqualityComparer<IndividualID>.Default.Equals(individualID, other.individualID) &&
                   EqualityComparer<ArcType>.Default.Equals(arcType, other.arcType) &&
                   EqualityComparer<ArcID>.Default.Equals(arcID, other.arcID) &&
                   EqualityComparer<ArcTime>.Default.Equals(arcTime, other.arcTime) &&
                   value == other.value &&
                   EqualityComparer<SpaceTimeVertex>.Default.Equals(SpaceTimeVertex1, other.SpaceTimeVertex1) &&
                   EqualityComparer<SpaceTimeVertex>.Default.Equals(SpaceTimeVertex2, other.SpaceTimeVertex2);
        }

        public override int GetHashCode()
        {
            int hashCode = 1097657807;
            hashCode = hashCode * -1521134295 + EqualityComparer<IndividualType>.Default.GetHashCode(individualType);
            hashCode = hashCode * -1521134295 + EqualityComparer<IndividualID>.Default.GetHashCode(individualID);
            hashCode = hashCode * -1521134295 + EqualityComparer<ArcType>.Default.GetHashCode(arcType);
            hashCode = hashCode * -1521134295 + EqualityComparer<ArcID>.Default.GetHashCode(arcID);
            hashCode = hashCode * -1521134295 + EqualityComparer<ArcTime>.Default.GetHashCode(arcTime);
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<SpaceTimeVertex>.Default.GetHashCode(SpaceTimeVertex1);
            hashCode = hashCode * -1521134295 + EqualityComparer<SpaceTimeVertex>.Default.GetHashCode(SpaceTimeVertex2);
            return hashCode;
        }

        public static bool operator ==(Arc left, Arc right)
        {
            return EqualityComparer<Arc>.Default.Equals(left, right);
        }

        public static bool operator !=(Arc left, Arc right)
        {
            return !(left == right);
        }
    }
}
