using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OptimizationIntegrationSystem.zhenghao;
using OptimizationIntegrationSystem.zhenghao.ClassName;
using Gurobi;

namespace OptimizationIntegrationSystem.zhenghao
{
    [Serializable]
    public struct Point
    {
        public IndividualID individualID;
        public PointType pointType;
        public IndividualType indiType;
        public double X;
        public double Y;
        public override bool Equals(object obj)
        {
            return obj is Point point &&
                   EqualityComparer<IndividualID>.Default.Equals(individualID, point.individualID) &&
                   EqualityComparer<PointType>.Default.Equals(pointType, point.pointType) &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = -813443585;
            hashCode = hashCode * -1521134295 + EqualityComparer<IndividualID>.Default.GetHashCode(individualID);
            hashCode = hashCode * -1521134295 + EqualityComparer<PointType>.Default.GetHashCode(pointType);
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Point i1, Point i2)
        {
            return Equals(i1, i2);
        }

        public static bool operator !=(Point i1, Point i2)
        {
            return !Equals(i1, i2);
        }
    }
    [Serializable]
    public struct TimeWindow
    {
        public IndividualID individualID;
        public PointType pointType;
        public int lowerLimit;
        public int upperLimit;
    }
    [Serializable]
    public struct PointTW
    {
        public Point point;
        public TimeWindow timeWindow;
        public Point otherPoint;
        public TimeWindow otherTimeWindow;
        public int subTime;
    }
    [Serializable]
    public class ArcGRBVar
    {
        public Arc arc;
        public GRBVar GRBV;
    }
    [Serializable]
    public class IndexGRBVar
    {
        public IndividualType IndividualTypePas;
        public IndividualType IndividualTypeVeh;
        public IndividualID IndividualIDPas;
        public IndividualID IndividualIDVeh;
        public GRBVar GRBV;
    }
    [Serializable]
    public class VarTheta
    {
        public IndividualType IndividualTypeTheta;
        public RouteID RouteIDTheta;
        public GRBVar GRBV;
    }
    [Serializable]
    public class VarW
    {
        public RouteID RouteID_W_L;
        public IndividualID IndividualIDpas;
        public RouteID RouteIDW;
        public SpaceTimeVertices SpaceTimeVertices;
        public GRBVar GRBV;
    }
    [Serializable]
    public class VarW_NOW
    {
        public IndividualID IndividualIDpas;
        public RouteID RouteIDW;
        public SpaceTimeVertices SpaceTimeVertices;
    }

}
