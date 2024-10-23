using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Gurobi;
using OptimizationIntegrationSystem.zhenghao;
using OptimizationIntegrationSystem.zhenghao.ClassName;

namespace OptimizationIntegrationSystem.zhenghao
{
    public class RoadNetwork//class下面的内容没有动作和先后顺序，function里面有动作
    {
        public int tsTime;
        public int opTime;
        Dictionary<IndividualID, List<PointTW>> coorTwDicPas_static;
        Dictionary<IndividualID, List<PointTW>> coorTwDicVeh;
        Dictionary<IndividualID, List<PointTW>> coordinatesDicVehCor;
        Dictionary<IndividualID, List<PointTW>> coordinatesDicPasVeh;
        List<IndividualType> individualTypeList;
        List<ArcType> arcTypesList;
        IndividualType pasUp;
        IndividualType pasDown;
        IndividualType veh; IndividualType visveh;
        ArcType typeTravel;
        ArcType typeDwell;
        ArcType typeStart;
        ArcType typeEnd;
        ArcType typeOper;
        ArcType typeTsOpDown;
        ArcType typeTsOpUp;
        Point pointO;
        Point pointD;
        Point pointVO;
        Point pointVD;
        Point pointTsO;
        Point pointTsD;

        public RoadNetwork(Dictionary<IndividualID, List<PointTW>> coorTwDicPas_static,
            Dictionary<IndividualID, List<PointTW>> coordinatesDicVehCor,
            Dictionary<IndividualID, List<PointTW>> coorTwDicVeh,
            int tsTime, int opTime)
        {
            this.tsTime = tsTime;
            this.opTime = opTime;
            this.coorTwDicPas_static = coorTwDicPas_static;
            this.coorTwDicVeh = coorTwDicVeh;
            this.coordinatesDicVehCor = coordinatesDicVehCor;
            coordinatesDicPasVeh = coorTwDicPas_static.Concat(coordinatesDicVehCor).ToDictionary(kv => kv.Key, kv => kv.Value);
            individualTypeList = new List<IndividualType>();
            pasUp = "pasUp";
            pasDown = "pasDown";
            veh = "veh";
            individualTypeList.Add(pasUp);
            individualTypeList.Add(pasDown);
            individualTypeList.Add(veh);
            //this.individualTypeList = individualTypeList; 如果我在构造器里面声明一个新的，我就需要this.XX=XX
            arcTypesList = new List<ArcType>();
            typeTravel = "travel";
            typeDwell = "dwell";
            typeStart = "start";
            typeEnd = "end";
            typeOper = "oper";
            typeTsOpDown = "tsopdown";
            typeTsOpUp = "tsopup";
            arcTypesList.Add(typeTravel);
            arcTypesList.Add(typeDwell);
            arcTypesList.Add(typeStart);
            arcTypesList.Add(typeEnd);
            arcTypesList.Add(typeOper);
            arcTypesList.Add(typeTsOpDown);
            arcTypesList.Add(typeTsOpUp);
            pointO.individualID = "O";pointO.individualID.O_or_D_or_M_none = "none"; pointO.individualID.OutOrBack = "none";
            pointO.pointType = "O";
            pointD.individualID = "D"; pointD.individualID.O_or_D_or_M_none = "none";pointD.individualID.OutOrBack = "none"; 
            pointD.pointType = "D";
            pointVO.individualID = "VO"; pointVO.individualID.O_or_D_or_M_none = "none";pointVO.individualID.OutOrBack = "none";
            pointVO.pointType = "VO";
            pointVD.individualID = "VD"; pointVD.individualID.O_or_D_or_M_none = "none";pointVD.individualID.OutOrBack = "none";
            pointVD.pointType = "VD";
            pointTsO.individualID = "TsO"; pointTsO.individualID.O_or_D_or_M_none = "none";pointTsO.individualID.OutOrBack = "none";
            pointTsO.pointType = "TsO";
            pointTsD.individualID = "TsD";pointTsD.individualID.O_or_D_or_M_none = "none";pointTsD.individualID.OutOrBack = "none";
            pointTsD.pointType = "TsD";pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";

        }
        public Arc Generating_arc(IndividualType individualType, IndividualID individualID, ArcType arcType, ArcID arcID, List<Point> spaceVertices, List<int> timeVertices, double value, ArcTime arcTime)
        {
            SpaceTimeVertex SpaceTimeVertex1 = new SpaceTimeVertex(spaceVertices[0], timeVertices[0]);
            SpaceTimeVertex SpaceTimeVertex2 = new SpaceTimeVertex(spaceVertices[1], timeVertices[1]);
            SpaceTimeVertices spaceTimeVertices = new SpaceTimeVertices(SpaceTimeVertex1, SpaceTimeVertex2);
            Arc ArcThis = new Arc(individualType, individualID, arcType, arcID, spaceVertices, timeVertices, value, SpaceTimeVertex1, SpaceTimeVertex2, arcTime, spaceTimeVertices);
            ArcThis.SpaceTimeVertex1 = SpaceTimeVertex1;
            ArcThis.SpaceTimeVertex2 = SpaceTimeVertex2;
            return ArcThis;
        }
        public void GeneratingTravellingCost(Point coorPoint1, Point coorPoint2, out List<Point> spaceVertices, out double value, out int distance, out int TimeSpent)
        {
            distance = (int)System.Math.Sqrt(Math.Pow((coorPoint1.X - coorPoint2.X), 2.0) + Math.Pow((coorPoint1.Y - coorPoint2.Y), 2.0));
            spaceVertices = new List<Point>();
            spaceVertices.Add(coorPoint1);
            spaceVertices.Add(coorPoint2);
            value = distance;
            TimeSpent = distance;
        }
        public void GeneratingTimeVertex(int TimeSpent, IndividualType individualType, IndividualID individualID, ArcType arcType, ArcID arcID, List<Point> spaceVertices, int k,
            out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices)
        {
            List<int> timeVertices = new List<int>();
            timeVertices.Add(k);
            timeVertices.Add(k + TimeSpent);
            if (k + TimeSpent < 0 || k < 0)
            {
                Console.WriteLine();
            }
            int value = new int();
            value = TimeSpent;
            ArcTime = k.ToString() + "s";
            ArcThis = Generating_arc(individualType, individualID, arcType, arcID, spaceVertices, timeVertices, value, ArcTime);
            FirstSpaceTimeVertex = new SpaceTimeVertex(spaceVertices[0], timeVertices[0]);
            SecondSpaceTimeVertex = new SpaceTimeVertex(spaceVertices[1], timeVertices[1]);
            SpaceTimeVertices = new SpaceTimeVertices(FirstSpaceTimeVertex, SecondSpaceTimeVertex);
        }
        public void GetArc(
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            out Dictionary<IndividualType, List<Arc>> arcDicIndividualType,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
            out List<SpaceTimeVertices> ListSpaceTimeVertices)
        {
            arcDic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>>();
            //
            arcDicIndiTypeID = new Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>>();
            arcDicIndiTypeIDArcType = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>>();
            arcDicIndividualType = new Dictionary<IndividualType, List<Arc>>();
            DicFirstSpaceTimeVertex = new Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>>();
            DicSecondSpaceTimeVertex = new Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>>();
            DicSpaceTimeVertices = new Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>>();
            ListSpaceTimeVertices = new List<SpaceTimeVertices>();

            foreach (IndividualType individualType in individualTypeList)
            {
                //乘客+
                if (individualType.ToString() == pasUp.ToString())//这里其实不tostring也可以
                {
                    Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDFirstSTV = new Dictionary<IndividualID, List<SpaceTimeVertex>>();
                    Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDSecondSTV = new Dictionary<IndividualID, List<SpaceTimeVertex>>();
                    Dictionary<IndividualID, List<SpaceTimeVertices>> DicIDSTV = new Dictionary<IndividualID, List<SpaceTimeVertices>>();
                    List<Arc> listArcIndividual = new List<Arc>();
                    Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndIDArctypeIDTime = new Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>();
                    //
                    Dictionary<IndividualID, List<Arc>> arcDicIndIDArctypeNew = new Dictionary<IndividualID, List<Arc>>(); Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicIDArcType = new Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>();
                    foreach (var indiInfo_KV in coorTwDicPas_static)
                    {
                        List<SpaceTimeVertex> ListFirstSpaceTimeVertex = new List<SpaceTimeVertex>();
                        List<SpaceTimeVertex> ListSecondSpaceTimeVertex = new List<SpaceTimeVertex>(); List<SpaceTimeVertices> ListSTV = new List<SpaceTimeVertices>();
                        List<Arc> listArc = new List<Arc>(); IndividualID chosenInDi_ID = indiInfo_KV.Key;
                        string individualIDToString = chosenInDi_ID.ToString();
                        TimeWindow tWIndividual = indiInfo_KV.Value[0].timeWindow;
                        if (!(chosenInDi_ID.UpOrDown.ToString() == "Up"))
                        {
                            continue;
                        }
                        //添加弧
                        Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>> arcDicTypeIDTime = new Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>(); Dictionary<ArcType, List<Arc>> arcDicArcType = new Dictionary<ArcType, List<Arc>>();
                        foreach (ArcType arcType in arcTypesList)
                        {
                            //travel 弧
                            if (arcType.ToString() == typeTravel.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                    TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                    IndividualID KeyPInfo1 = PointInfo1.Key;
                                    if ((coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                        && (PointInfo1.Key.ToString().Replace("Down", "Up") == indiInfo_KV.Key.ToString()))
                                    {
                                        continue;//pasUp不能从自己的DpasDown出发
                                    }
                                    if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                    {
                                        coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                        coorPoint1.individualID.O_or_D_or_M_none = "D"; 
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                    }
                                    foreach (var PointInfo2 in coorTwDicPas_static)
                                    {
                                        if (PointInfo2.Key.ToString() == indiInfo_KV.Key.ToString())
                                        {
                                            continue;//不能从任意地方travel到自己还未上车的状态
                                        }
                                        Point coorPoint2 = Program.DeepCopyByBin(PointInfo2.Value[0].point); Point coor_copied2 = coorPoint2;
                                        if (coorPoint2.individualID.UpOrDown.ToString() == "Up" && coorPoint2.individualID.individualType.ToString() == "pas")
                                        {
                                            coorPoint2.individualID = "O" + coorPoint2.individualID.ToString();
                                            coorPoint2.individualID.O_or_D_or_M_none = "O"; 
                                            coorPoint2.individualID = coor_copied2.individualID.Copying_info(coorPoint2.individualID);
                                            //"Y_pasUp1_t_07_tt_12_travel_i_pasUp2_j_pasDown1"
                                        }

                                        IndividualID KeyPInfo2 = PointInfo2.Key;
                                        TimeWindow tWPoint2 = PointInfo2.Value[0].timeWindow;
                                        if (KeyPInfo1.ToString() == KeyPInfo2.ToString())
                                        {
                                            continue;
                                        }
                                        GeneratingTravellingCost(coorPoint1, coorPoint2, out List<Point> spaceVertices, out double value, out int distance, out int TimeSpent);
                                        ArcID arcID = chosenInDi_ID.ToString() + typeTravel.ToString() + KeyPInfo1 + KeyPInfo2;
                                        arcID.arcSpaceVertices = spaceVertices;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        GeneratingTravellingCost(indiInfo_KV.Value[0].point, coorPoint1, out List<Point> spaceVertices1, out double value1, out int distance1, out int from_start_to_this_point);
                                        GeneratingTravellingCost(coorPoint2, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices2, out double value2, out int distance2, out int from_this_point_to_end);
                                        int lower_limit = Math.Max(tWPoint1.lowerLimit, indiInfo_KV.Value[0].timeWindow.lowerLimit+ from_start_to_this_point);
                                        int upper_limit = Math.Min(tWPoint2.upperLimit - TimeSpent  + tsTime + 1, indiInfo_KV.Value[0].otherTimeWindow.upperLimit- TimeSpent - from_this_point_to_end + 1);
                                        if (coorPoint1.individualID.UpOrDown == "Up"
                                                && indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex)//Y_pasUp2_t_07_tt_08_dwell_i_pasUp2_j_pasUp2
                                        {
                                            //需要op再到这个点
                                            lower_limit = tWIndividual.lowerLimit + opTime;
                                        }
                                        for (int k = lower_limit; k < upper_limit; k++)
                                        {//k最早出发时间：起始点的lowerLimit与这个乘客travel到这个地方的最早时间中更小的那个
                                         //k最晚出发时间：max(起始点的upperLimit,终止点的upperlimit-traveltime+tsTime）
                                         //dwell之后还是在原来的点，没有入点和出点
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex);
                                            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> Type_ID_ListArcVar = new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();

                                            ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeDwell.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    int iterNum = 0;
                                    if (PointInfo1.Key.ToString() == indiInfo_KV.Key.ToString())
                                    {
                                        iterNum = 3;//不能从自己的OpasUp dwell到自己的pasUp,那叫operational time
                                    }
                                    else if (PointInfo1.Key.ToString().Replace("Down", "Up") == indiInfo_KV.Key.ToString())
                                    {//pasUp不能到PasDown与DpasDown
                                        iterNum = 0;
                                    }
                                    else
                                    {
                                        iterNum = 4;
                                    }
                                    
                                    for (int i = 1; i < iterNum; i++)
                                    {
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        IndividualID KeyPInfo1 = PointInfo1.Key;
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                        TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                        Point coorPoint2;
                                        
                                        if (i == 1)//OpasUp-OpasUp DpasDown-DpasDown
                                        {

                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "D"; coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);

                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O"; coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            coorPoint2 = coorPoint1;
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            if (tWIndividual.lowerLimit > tWPoint1.upperLimit)
                                            {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                continue;
                                            }
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                            int lower_limit = tWIndividual.lowerLimit + from_out_to_this_point;
                                            int upper_limit = Math.Min(tWPoint1.upperLimit + tsTime + 2, indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_back);
                                            if (coorPoint1.individualID.UpOrDown == "Up"
                                                && indiInfo_KV.Key.IDIndex== coorPoint1.individualID.IDIndex)//Y_pasUp2_t_07_tt_08_dwell_i_pasUp2_j_pasUp2
                                            {
                                                upper_limit = tWPoint1.upperLimit + opTime - 1;
                                                //upperlimit开始dwell，则upperlimit+1 dwell结束
                                                //upperlimit+1处必须已经开始上车，但不必在车上，所以理论上可以upperlimit+1才开始上车，所以+opTime-1
                                            }
                                            //if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            //{
                                            //    lower_limit = tWIndividual.lowerLimit + from_out_to_this_point + 1;
                                            //}
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//在某个点Dwell，如果Dwell到了这个点的upperLimit（上车点则是上车点的upperLimit，下车点则是下车点的upperLimit）之后，直接出发，要dwell去下一个地方dwell
                                             //由于每个客人都享有index各不相同的上下车点位，从而不存在[在原地等待某一乘客上车]的情况
                                             //其它情况下应当符合将等待时间尽量延后的法则
                                             //对于p+而言，是从自己的timetable的lowerlimit开始可以dwell
                                             //需要考虑换乘的时候也得dwell，所以dwell的时间需要更长一点
                                             //可以为了等待其他乘客换乘而Dwell到这个点时间窗的上限之后,等待上限其实就是2而已
                                             //但是乘客不能在任何点dwell到自己的下车时间窗之上。
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                        }
                                        else if (i == 2)//pasup-pasup pasdown-pasdown
                                        {
                                            coorPoint2 = coorPoint1;
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            if (tWIndividual.lowerLimit > tWPoint1.upperLimit)
                                            {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                continue;
                                            }
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                            int lower_limit = tWIndividual.lowerLimit + from_out_to_this_point;
                                            int upper_limit = Math.Min(tWPoint1.upperLimit + tsTime +2, indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_back);
                                            if (coorPoint1.individualID.UpOrDown == "Up"
                                                && indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex)//Y_pasUp2_t_07_tt_08_dwell_i_pasUp2_j_pasUp2
                                            {
                                                lower_limit = tWIndividual.lowerLimit + from_out_to_this_point + opTime;
                                                //需要op再到这个点
                                            }
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//在某个点Dwell，如果Dwell到了这个点的upperLimit（上车点则是上车点的upperLimit，下车点则是下车点的upperLimit）之后，直接出发，要dwell去下一个地方dwell
                                             //由于每个客人都享有index各不相同的上下车点位，从而不存在[在原地等待某一乘客上车]的情况
                                             //其它情况下应当符合将等待时间尽量延后的法则
                                             //对于p+而言，是从自己的timetable的lowerlimit开始可以dwell
                                             //需要考虑换乘的时候也得dwell，所以dwell的时间需要更长一点
                                             //可以为了等待其他乘客换乘而Dwell到这个点时间窗的上限之后,等待上限其实就是2而已
                                             //但是乘客不能在任何点dwell到自己的下车时间窗之上。
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                        }
                                        else
                                        {//pasDown-DpasDown OpasUp-pasUp
                                            coorPoint2 = coorPoint1;
                                            Point coorPoint3;
                                            coorPoint3 = coorPoint1;
                                            coorPoint3.individualID = "M" + coorPoint1.individualID.ToString();
                                            coorPoint3.individualID.O_or_D_or_M_none = "M";coorPoint3.individualID = coor_copied1.individualID.Copying_info(coorPoint3.individualID);
                                            if (coorPoint2.individualID.UpOrDown.ToString() == "Down")
                                            {
                                                coorPoint2.individualID = "D" + coorPoint2.individualID.ToString();
                                                coorPoint2.individualID.O_or_D_or_M_none = "D"; coorPoint2.individualID = coor_copied1.individualID.Copying_info(coorPoint2.individualID);
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O"; coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            if (tWIndividual.lowerLimit > tWPoint1.upperLimit)
                                            {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                continue;
                                            }
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                            int lower_limit = tWIndividual.lowerLimit + from_out_to_this_point;
                                            int upper_limit = Math.Min(tWPoint1.upperLimit + tsTime+2, indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_back);
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//在某个点Dwell，如果Dwell到了这个点的upperLimit（上车点则是上车点的upperLimit，下车点则是下车点的upperLimit）之后，直接出发，要dwell去下一个地方dwell
                                             //由于每个客人都享有index各不相同的上下车点位，从而不存在[在原地等待某一乘客上车]的情况
                                             //其它情况下应当符合将等待时间尽量延后的法则
                                             //对于p+而言，是从自己的timetable的lowerlimit开始可以dwell
                                             //需要考虑换乘的时候也得dwell，所以dwell的时间需要更长一点
                                             //可以为了等待其他乘客换乘而Dwell到这个点时间窗的上限之后,等待上限其实就是2而已
                                             //但是乘客不能在任何点dwell到自己的下车时间窗之上。
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                            ////////

                                            Dictionary<ArcTime, Arc> arcDicTime2 = new Dictionary<ArcTime, Arc>();
                                            arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint3.individualID;
                                            spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint3);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            if (tWIndividual.lowerLimit > tWPoint1.upperLimit)
                                            {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                continue;
                                            }
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//在某个点Dwell，如果Dwell到了这个点的upperLimit（上车点则是上车点的upperLimit，下车点则是下车点的upperLimit）之后，直接出发，要dwell去下一个地方dwell
                                             //由于每个客人都享有index各不相同的上下车点位，从而不存在[在原地等待某一乘客上车]的情况
                                             //其它情况下应当符合将等待时间尽量延后的法则
                                             //对于p+而言，是从自己的timetable的lowerlimit开始可以dwell
                                             //需要考虑换乘的时候也得dwell，所以dwell的时间需要更长一点
                                             //可以为了等待其他乘客换乘而Dwell到这个点时间窗的上限之后,等待上限其实就是2而已
                                             //但是乘客不能在任何点dwell到自己的下车时间窗之上。
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime2.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime2);

                                            Dictionary<ArcTime, Arc> arcDicTime3 = new Dictionary<ArcTime, Arc>();
                                            arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint3.individualID + coorPoint2.individualID;
                                            spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint3);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            if (tWIndividual.lowerLimit > tWPoint1.upperLimit)
                                            {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                continue;
                                            }
                                            for (int k = lower_limit + 1; k < upper_limit + 1; k++)
                                            {//在某个点Dwell，如果Dwell到了这个点的upperLimit（上车点则是上车点的upperLimit，下车点则是下车点的upperLimit）之后，直接出发，要dwell去下一个地方dwell
                                             //由于每个客人都享有index各不相同的上下车点位，从而不存在[在原地等待某一乘客上车]的情况
                                             //其它情况下应当符合将等待时间尽量延后的法则
                                             //对于p+而言，是从自己的timetable的lowerlimit开始可以dwell
                                             //需要考虑换乘的时候也得dwell，所以dwell的时间需要更长一点
                                             //可以为了等待其他乘客换乘而Dwell到这个点时间窗的上限之后,等待上限其实就是2而已
                                             //但是乘客不能在任何点dwell到自己的下车时间窗之上。
                                                int TimeSpent = 0;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime3.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime3);
                                        }

                                    }

                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeStart.ToString())
                            {
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                    IndividualID PointID1 = PointInfo1.Key;
                                    if (PointID1.ToString() == chosenInDi_ID.ToString())
                                    {
                                        ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + PointID1;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                        coorPoint1.individualID = "O" + PointInfo1.Value[0].point.individualID.ToString();
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                        TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                        List<Point> spaceVertices = new List<Point>();
                                        spaceVertices.Add(pointO);
                                        spaceVertices.Add(coorPoint1);
                                        arcID.arcSpaceVertices = spaceVertices;
                                        for (int k = tWPoint1.lowerLimit; k < tWPoint1.upperLimit + 1; k++)
                                        {
                                            int TimeSpent = 0;
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                                }
                            }
                            if (arcType.ToString() == typeEnd.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    int iterNum = 0;
                                    if (PointInfo1.Key.ToString() == indiInfo_KV.Key.ToString())
                                    {
                                        iterNum = 2;//pasUp不能从自己的OpasUp结束
                                    }
                                    else if (PointInfo1.Key.UpOrDown.ToString() == "Down" &&
                                        PointInfo1.Key.ToString().Replace("Down", "Up") == indiInfo_KV.Key.ToString())
                                    {//pasUp不能到DpasDown 可以到pasDown吗？不可以，首先如果不换乘的话应该在PasUp换乘，如果换乘，都到了下车点了来不及换乘了：
                                     ////也未必，可以换乘到其他车辆上等着自己的下车时间窗
                                        iterNum = 2;
                                    }
                                    else
                                    {
                                        iterNum = 3;
                                    }
                                    int TimeSpent;
                                    for (int i = 1; i < iterNum; i++)
                                    {
                                        IndividualID PointID1 = PointInfo1.Key;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                        TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                        List<Point> spaceVertices = new List<Point>();
                                        if (PointID1.ToString() == chosenInDi_ID.ToString())
                                        {
                                            TimeSpent = 0;
                                        }
                                        else
                                        {
                                            TimeSpent = tsTime / 2;//换乘时长
                                        }
                                        if (i == 2)
                                        {
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "D"; coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID); coorPoint1.individualID.UpOrDown = "Down"; ;
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O"; coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                        }
                                        spaceVertices.Add(coorPoint1);
                                        //上车、换乘、下车这三件事的顺序应该如何分配？先下后上，上车后换乘？
                                        //先下后上，然后换乘的下车，然后换乘的上车。
                                        //是否是既可以从DpasDown1 end，也可以从pasDown1 end：是的，也可以从Opasdown end
                                        //先下车再换乘还是先换乘再下车？先换乘再下车的话，就可以直接连接到pasDown1，先下车再换乘就是连接到
                                        //从道理上来讲，也可以从非Dend，也许先换乘之后才会到下车时间窗呢
                                        spaceVertices.Add(pointTsD);
                                        ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID.ToString();
                                        arcID.arcSpaceVertices = spaceVertices;
                                        GeneratingTravellingCost(indiInfo_KV.Value[0].point, coorPoint1, out List<Point> spaceVertices1, out double value1, out int distance1, out int from_start_to_this_point);
                                        GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> DumpspaceVertices, out double Dumpvalue, out int distance, out int from_this_point_to_end);
                                        int lower_limit= indiInfo_KV.Value[0].timeWindow.lowerLimit + opTime + from_start_to_this_point;
                                        int upper_limit = indiInfo_KV.Value[0].otherTimeWindow.upperLimit + 1 - opTime - from_this_point_to_end + tsTime;

                                        //if (coorPoint1.individualID.UpOrDown.ToString() == "Down"
                                        //    && coorPoint1.individualID.individualType.ToString() == "pas"
                                        //    && coorPoint1.individualID.O_or_D_or_M_none.ToString() == "D")
                                        //{//D
                                        //    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + 2 * opTime + from_start_to_this_point;
                                        //}
                                        //if (coorPoint1.individualID.UpOrDown.ToString() == "Up"
                                        //    && coorPoint1.individualID.individualType.ToString() == "pas"
                                        //    && coorPoint1.individualID.IDIndex != indiInfo_KV.Key.IDIndex
                                        //    && coorPoint1.individualID.O_or_D_or_M_none.ToString() == "none")
                                        //{
                                        //    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + 2 * opTime + from_start_to_this_point;
                                        //}

                                        for (int k = lower_limit;k < upper_limit; k++)
                                        //任意一点均可以end，但是需要上车之后再end，且不能超过p-时间窗的upper limit
                                        //Pas整体从任意一点end，那么从这个点出发的时间+从这个点到终止点的时间不能超过pas终止点的upperLimit
                                        {
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeOper.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>();
                                List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                Point coorPoint1 = indiInfo_KV.Value[0].point; Point coor_copied1 = coorPoint1;
                                Point coorPoint2 = coorPoint1;
                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                coorPoint1.individualID.O_or_D_or_M_none = "O";
                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                TimeWindow tWPoint1 = indiInfo_KV.Value[0].timeWindow;
                                List<Point> spaceVertices = new List<Point>();
                                spaceVertices.Add(coorPoint1);
                                spaceVertices.Add(coorPoint2);
                                ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + chosenInDi_ID;
                                arcID.arcSpaceVertices = spaceVertices;
                                int TimeSpent = opTime;
                                for (int k = indiInfo_KV.Value[0].timeWindow.lowerLimit; k < indiInfo_KV.Value[0].timeWindow.upperLimit + opTime; k++)
                                //oper
                                {
                                    GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                    listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                    ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                    arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                }
                                arcDicIDTime.Add(arcID, arcDicTime);
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                        }
                        arcDicIndIDArctypeIDTime.Add(chosenInDi_ID, arcDicTypeIDTime); arcDicIndIDArctypeNew.Add(chosenInDi_ID, listArc);
                        var distinctItemsFirst = ListFirstSpaceTimeVertex.Distinct(new DistinctItemComparer());
                        ListFirstSpaceTimeVertex = distinctItemsFirst.ToList();
                        DicIDFirstSTV.Add(chosenInDi_ID, ListFirstSpaceTimeVertex);
                        var distinctItemsSecond = ListSecondSpaceTimeVertex.Distinct(new DistinctItemComparer());
                        ListSecondSpaceTimeVertex = distinctItemsSecond.ToList();
                        var distinctFSItems = ListSTV.Distinct(new DistinctItemComparerSTVs());
                        ListSTV = distinctFSItems.ToList(); DicIDSTV.Add(chosenInDi_ID, ListSTV);
                        DicIDSecondSTV.Add(chosenInDi_ID, ListSecondSpaceTimeVertex); arcDicIDArcType.Add(chosenInDi_ID, arcDicArcType);
                    }
                    arcDic.Add(individualType, arcDicIndIDArctypeIDTime);
                    arcDicIndiTypeID.Add(individualType, arcDicIndIDArctypeNew); arcDicIndividualType.Add(individualType, listArcIndividual);
                    DicFirstSpaceTimeVertex.Add(individualType, DicIDFirstSTV);
                    DicSecondSpaceTimeVertex.Add(individualType, DicIDSecondSTV); DicSpaceTimeVertices.Add(individualType, DicIDSTV);
                    arcDicIndiTypeIDArcType.Add(individualType, arcDicIDArcType);
                }
                else if (individualType.ToString() == pasDown.ToString())
                {
                    Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDFirstSTV = new Dictionary<IndividualID, List<SpaceTimeVertex>>();
                    Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDSecondSTV = new Dictionary<IndividualID, List<SpaceTimeVertex>>(); Dictionary<IndividualID, List<SpaceTimeVertices>> DicIDSTV = new Dictionary<IndividualID, List<SpaceTimeVertices>>();
                    List<Arc> listArcIndividual = new List<Arc>();
                    Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndIDArctypeIDTime = new Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>();
                    //
                    Dictionary<IndividualID, List<Arc>> arcDicIndIDArctypeNew = new Dictionary<IndividualID, List<Arc>>();
                    Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicIDArcType = new Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>();
                    foreach (var indiInfo_KV in coorTwDicPas_static)
                    {
                        List<SpaceTimeVertex> ListFirstSpaceTimeVertex = new List<SpaceTimeVertex>();
                        List<SpaceTimeVertex> ListSecondSpaceTimeVertex = new List<SpaceTimeVertex>(); List<SpaceTimeVertices> ListSTV = new List<SpaceTimeVertices>();
                        List<Arc> listArc = new List<Arc>(); IndividualID chosenInDi_ID = indiInfo_KV.Key;
                        string individualIDToString = chosenInDi_ID.ToString();
                        TimeWindow tWIndividual = indiInfo_KV.Value[0].timeWindow;
                        if (!(chosenInDi_ID.UpOrDown.ToString() == "Down"))
                        {
                            continue;
                        }
                        Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>> arcDicTypeIDTime = new Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>();
                        Dictionary<ArcType, List<Arc>> arcDicArcType = new Dictionary<ArcType, List<Arc>>();
                        foreach (ArcType arcType in arcTypesList)
                        {
                            if (arcType.ToString() == typeTravel.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1; TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                    IndividualID KeyPInfo1 = PointInfo1.Key;
                                    if (PointInfo1.Key.ToString() == indiInfo_KV.Key.ToString())
                                    {
                                        continue;//不能从自己已经下车的状态travel出去
                                    }
                                    if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                    {
                                        coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                        coorPoint1.individualID.O_or_D_or_M_none = "D";
                                    }
                                    foreach (var PointInfo2 in coorTwDicPas_static)
                                    {
                                        Point coorPoint2 = Program.DeepCopyByBin(PointInfo2.Value[0].point); Point coor_copied2 = coorPoint2;
                                        TimeWindow tWPoint2 = PointInfo2.Value[0].timeWindow;
                                        IndividualID KeyPInfo2 = PointInfo2.Key;
                                        if (coorPoint2.individualID.UpOrDown.ToString() == "Up" && coorPoint2.individualID.individualType.ToString() == "pas")
                                        {
                                            if (coorPoint2.individualID.ToString().Replace("Up", "Down") == indiInfo_KV.Key.ToString())
                                            {
                                                continue;
                                            }
                                            coorPoint2.individualID = "O" + coorPoint2.individualID.ToString();
                                            coorPoint2.individualID.O_or_D_or_M_none = "O";
                                            coorPoint2.individualID = coor_copied2.individualID.Copying_info(coorPoint2.individualID);
                                        }
                                        if (KeyPInfo1.ToString() == KeyPInfo2.ToString())
                                        {
                                            continue;
                                        }
                                        GeneratingTravellingCost(coorPoint1, coorPoint2, out List<Point> spaceVertices, out double value, out int distance, out int TimeSpent);
                                        ArcID arcID = chosenInDi_ID.ToString() + typeTravel.ToString() + KeyPInfo1 + KeyPInfo2;
                                        arcID.arcSpaceVertices = spaceVertices;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        GeneratingTravellingCost(indiInfo_KV.Value[0].otherPoint, coorPoint1, out List<Point> spaceVertices1, out double value1, out int distance1, out int from_start_to_this_point);
                                        int lower_limit= Math.Max(PointInfo1.Value[0].timeWindow.lowerLimit, indiInfo_KV.Value[0].otherTimeWindow.lowerLimit+ from_start_to_this_point);
                                        GeneratingTravellingCost(indiInfo_KV.Value[0].point, coorPoint2, out spaceVertices1, out value1, out distance1, out int from_this_point_to_back);
                                        int upper_limit = Math.Min(PointInfo2.Value[0].timeWindow.upperLimit - TimeSpent - opTime + tsTime + 1, indiInfo_KV.Value[0].timeWindow.upperLimit - from_this_point_to_back- TimeSpent + 1);

                                        for (int k = lower_limit; k < upper_limit; k++)
                                        {
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeDwell.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    int iterNum = 0;
                                    if (PointInfo1.Key.ToString() == indiInfo_KV.Key.ToString())
                                    {//存在DpasDown-DpasDown弧
                                        iterNum = 3;
                                    }
                                    else if (PointInfo1.Key.ToString().Replace("Up", "Down") == indiInfo_KV.Key.ToString())
                                    {//individual:pasDown1 point:pasUp1 不存在OpasUp1-pasUp1弧，存在pasUp1-pasUp1弧 不存在OpasUp1-OpasUp1弧
                                        iterNum = 2;
                                    }
                                    else
                                    {
                                        iterNum = 4;
                                    }
                                    for (int i = 1; i < iterNum; i++)
                                    {
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        IndividualID KeyPInfo1 = PointInfo1.Key;
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                        TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                        Point coorPoint2;
                                        if (i == 1)//pasup-pasup pasdown-pasdown
                                        {
                                            coorPoint2 = coorPoint1;
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                            int lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                            int upper_limit = Math.Min(PointInfo1.Value[0].timeWindow.upperLimit + tsTime +1, indiInfo_KV.Value[0].timeWindow.upperLimit- from_this_point_to_back) + 1;
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//对于p-而言，从p+的lowerlimit开始可以dwell,dwell到p-的upperlimit为止
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                        }
                                        else if (i == 2)//OpasUp-OpasUp DpasDown-DpasDown
                                        {
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "D";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            coorPoint2 = coorPoint1;
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                            int lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                            int upper_limit = Math.Min(PointInfo1.Value[0].timeWindow.upperLimit + tsTime +1, indiInfo_KV.Value[0].timeWindow.upperLimit - from_this_point_to_back) + 1;
                                            //if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            //{
                                            //    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point + 1;
                                            //}
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//对于p-而言，从p+的lowerlimit开始可以dwell,dwell到p-的upperlimit为止
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                        }
                                        else
                                        {//pasDown-DpasDown OpasUp-pasUp

                                            Dictionary<ArcTime, Arc> arcDicTime2 = new Dictionary<ArcTime, Arc>();
                                            coorPoint2 = coorPoint1;
                                            Point coorPoint3;
                                            coorPoint3 = coorPoint1;
                                            coorPoint3.individualID = "M" + coorPoint1.individualID.ToString();
                                            coorPoint3.individualID.O_or_D_or_M_none = "M";coorPoint3.individualID = coor_copied1.individualID.Copying_info(coorPoint3.individualID);
                                            if (coorPoint2.individualID.UpOrDown.ToString() == "Down")
                                            {
                                                coorPoint2.individualID = "D" + coorPoint2.individualID.ToString();
                                                coorPoint2.individualID.O_or_D_or_M_none = "D";
                                                coorPoint2.individualID = coor_copied1.individualID.Copying_info(coorPoint2.individualID);
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                            int lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                            int upper_limit = Math.Min(PointInfo1.Value[0].timeWindow.upperLimit + tsTime+1, indiInfo_KV.Value[0].timeWindow.upperLimit - from_this_point_to_back) + 1;
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//对于p-而言，从p+的lowerlimit开始可以dwell,dwell到p-的upperlimit为止
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime2.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime2);
                                            //////
                                            Dictionary<ArcTime, Arc> arcDicTime3 = new Dictionary<ArcTime, Arc>();
                                            arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint3.individualID;
                                            spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint3);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//对于p-而言，从p+的lowerlimit开始可以dwell,dwell到p-的upperlimit为止
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime3.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime3);

                                            arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint3.individualID + coorPoint2.individualID;
                                            spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint3);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            for (int k = lower_limit + 1; k < upper_limit + 1; k++)
                                            {//对于p-而言，从p+的lowerlimit开始可以dwell,dwell到p-的upperlimit为止
                                                int TimeSpent = 0;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                            ///
                                        }

                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeEnd.ToString())
                            {
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                    IndividualID PointID1 = PointInfo1.Key;
                                    if (PointID1.ToString() == chosenInDi_ID.ToString())
                                    {
                                        ArcID arcID = individualIDToString + arcType + PointID1;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1; TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                        coorPoint1.individualID = "D" + PointInfo1.Value[0].point.individualID.ToString();
                                        coorPoint1.individualID.O_or_D_or_M_none = "D";
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                        List<Point> spaceVertices = new List<Point>();
                                        spaceVertices.Add(coorPoint1);
                                        spaceVertices.Add(pointD);
                                        arcID.arcSpaceVertices = spaceVertices;
                                        for (int k = PointInfo1.Value[0].timeWindow.lowerLimit; k < PointInfo1.Value[0].timeWindow.upperLimit + 1; k++)
                                        {//如果到了地方还没有到lower limit可以dwell
                                            int TimeSpent = 0;
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                                }
                            }
                            if (arcType.ToString() == typeStart.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    int iterNum = 0;
                                    if ((PointInfo1.Key.UpOrDown.ToString() == "Up" &&
                                        PointInfo1.Key.ToString().Replace("Up", "Down") == indiInfo_KV.Key.ToString()) ||
                                        //pasDown不能从自己的OpasUp开始
                                        (PointInfo1.Key.UpOrDown.ToString() == "Down" &&
                                        PointInfo1.Key.ToString() == indiInfo_KV.Key.ToString())
                                        //pasDown不能从自己的DpasDown开始
                                        )
                                    {
                                        iterNum = 2;
                                    }
                                    else
                                    {
                                        iterNum = 3;
                                    }
                                    for (int i = 1; i < iterNum; i++)
                                    {
                                        IndividualID PointID1 = PointInfo1.Key;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1; TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;

                                        if (i == 2)
                                        {
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "D";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                        }
                                        ArcID arcID = individualIDToString + arcType + coorPoint1.individualID.ToString();
                                        int TimeSpent;
                                        if (PointID1.ToString() == chosenInDi_ID.ToString())
                                        {
                                            TimeSpent = tsTime / 2;
                                        }
                                        else if (PointID1.ToString().Replace("Up", "Down") == chosenInDi_ID.ToString())
                                        {
                                            TimeSpent = 0;
                                        }
                                        else
                                        {
                                            TimeSpent = tsTime / 2;//换乘时长
                                        }
                                        List<Point> spaceVertices = new List<Point>();
                                        spaceVertices.Add(pointTsO);
                                        spaceVertices.Add(coorPoint1);
                                        arcID.arcSpaceVertices = spaceVertices;
                                        GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> DumpspaceVertices, out double Dumpvalue, out int distance, out int from_this_point_to_end);
                                        GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> DumpspaceVertices1, out double Dumpvalue1, out int distance1, out int from_start_to_this_point);
                                        int lower_limit= indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + opTime+ from_start_to_this_point;
                                        //int lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + opTime ;
                                        int upper_limit = indiInfo_KV.Value[0].timeWindow.upperLimit + 1 - opTime - from_this_point_to_end + tsTime;
                                        for (int k = lower_limit;
                                            k < upper_limit; k++)
                                        {//得在p+上车之后p-才能start,逻辑上不可能从自己的下车点start，且不是从自己的时间窗下限start
                                         //从TsO出来之后，已经完成了换乘，此时回到原来的点，其实什么时刻回去都是可以的
                                         //时间上需要在pasUp的上车时间窗下限或PasDown的下车时间窗下限之后才可以，所以都是下限
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeOper.ToString())
                            {

                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>();
                                List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                Point coorPoint1 = indiInfo_KV.Value[0].point;
                                Point coorPoint2 = coorPoint1;
                                Point coor_copied2 = coorPoint2;
                                coorPoint2.individualID = "D" + coorPoint2.individualID.ToString();
                                coorPoint2.individualID.O_or_D_or_M_none = "D";
                                coorPoint2.individualID = coor_copied2.individualID.Copying_info(coorPoint2.individualID);
                                TimeWindow tWPoint1 = indiInfo_KV.Value[0].timeWindow;
                                List<Point> spaceVertices = new List<Point>();
                                spaceVertices.Add(coorPoint1);
                                spaceVertices.Add(coorPoint2);
                                ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + chosenInDi_ID;
                                arcID.arcSpaceVertices = spaceVertices;
                                int TimeSpent = opTime;
                                for (int k = indiInfo_KV.Value[0].timeWindow.lowerLimit - opTime; k < indiInfo_KV.Value[0].timeWindow.upperLimit; k++)
                                //oper
                                {
                                    GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                    listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                    ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                    arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                }
                                arcDicIDTime.Add(arcID, arcDicTime);
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                        }
                        arcDicIndIDArctypeIDTime.Add(chosenInDi_ID, arcDicTypeIDTime); arcDicIndIDArctypeNew.Add(chosenInDi_ID, listArc);
                        var distinctItemsFirst = ListFirstSpaceTimeVertex.Distinct(new DistinctItemComparer());
                        ListFirstSpaceTimeVertex = distinctItemsFirst.ToList();
                        DicIDFirstSTV.Add(chosenInDi_ID, ListFirstSpaceTimeVertex);
                        var distinctItemsSecond = ListSecondSpaceTimeVertex.Distinct(new DistinctItemComparer());
                        ListSecondSpaceTimeVertex = distinctItemsSecond.ToList(); var distinctFSItems = ListSTV.Distinct(new DistinctItemComparerSTVs()); ListSTV = distinctFSItems.ToList(); DicIDSTV.Add(chosenInDi_ID, ListSTV);
                        DicIDSecondSTV.Add(chosenInDi_ID, ListSecondSpaceTimeVertex); arcDicIDArcType.Add(chosenInDi_ID, arcDicArcType);
                    }
                    arcDic.Add(individualType, arcDicIndIDArctypeIDTime);
                    arcDicIndiTypeID.Add(individualType, arcDicIndIDArctypeNew); arcDicIndividualType.Add(individualType, listArcIndividual);
                    DicFirstSpaceTimeVertex.Add(individualType, DicIDFirstSTV);
                    DicSecondSpaceTimeVertex.Add(individualType, DicIDSecondSTV); DicSpaceTimeVertices.Add(individualType, DicIDSTV); arcDicIndiTypeIDArcType.Add(individualType, arcDicIDArcType);
                }
                else if (individualType.ToString() == veh.ToString())
                {
                    Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDFirstSTV = new Dictionary<IndividualID, List<SpaceTimeVertex>>();
                    Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDSecondSTV = new Dictionary<IndividualID, List<SpaceTimeVertex>>(); Dictionary<IndividualID, List<SpaceTimeVertices>> DicIDSTV = new Dictionary<IndividualID, List<SpaceTimeVertices>>();
                    List<Arc> listArcIndividual = new List<Arc>();
                    Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndIDArctypeIDTime = new Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>();
                    //
                    Dictionary<IndividualID, List<Arc>> arcDicIndIDArctypeNew = new Dictionary<IndividualID, List<Arc>>(); Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicIDArcType = new Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>();
                    foreach (var indiInfo_KV in coorTwDicVeh)
                    {
                        List<SpaceTimeVertex> ListFirstSpaceTimeVertex = new List<SpaceTimeVertex>();
                        List<SpaceTimeVertex> ListSecondSpaceTimeVertex = new List<SpaceTimeVertex>(); List<SpaceTimeVertices> ListSTV = new List<SpaceTimeVertices>();
                        List<Arc> listArc = new List<Arc>(); IndividualID chosenInDi_ID = indiInfo_KV.Key;

                        Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>> arcDicTypeIDTime = new Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>(); Dictionary<ArcType, List<Arc>> arcDicArcType = new Dictionary<ArcType, List<Arc>>();
                        foreach (ArcType arcType in arcTypesList)
                        {
                            if (arcType.ToString() == typeTravel.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coordinatesDicPasVeh)
                                {
                                    Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                    TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                    if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                    {
                                        coorPoint1.individualID.O_or_D_or_M_none = "D";
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                    }
                                    if (chosenInDi_ID.individualType == veh)
                                    {
                                        if (coorPoint1.individualID.individualType.ToString() == "vehCor")
                                        {
                                            if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex)
                                            {
                                                continue;
                                            }
                                        }
                                        else if (coorPoint1.individualID.individualType.ToString() == "visveh")
                                        {
                                            continue;
                                        }
                                    }
                                    else if (chosenInDi_ID.individualType.ToString() == "visveh")
                                    {
                                        if (coorPoint1.individualID.individualType.ToString() == "vehCor")
                                        {
                                            continue;//虚拟车辆不经过车辆点
                                        }
                                        //if (chosenInDi_ID.UpOrDown.ToString() == "Up" &&
                                        //    coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                        //    coorPoint1.individualID.O_or_D_or_M_none.ToString() == "D")//visvehpasUp不从对应的DpasUp出发
                                        //{
                                        //    continue;
                                        //}
                                        if (coorPoint1.individualID.individualType.ToString() == "visveh")//如果点是虚拟点
                                        {
                                            if (chosenInDi_ID.UpOrDown.ToString() == "Up")//如果是VisUp
                                                                                          //visUp服务其他pasDown
                                            {
                                                if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex &&
                                                     coorPoint1.individualID.individualType == "visveh" &&
                                                     coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                //某个乘客的visUp仅经过这个乘客的visUp
                                                {
                                                    continue;
                                                }
                                                if (coorPoint1.individualID.individualType == "visveh" &&
                                                     coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                //某个乘客的visUp不经过任何乘客vispasDown
                                                {
                                                    //Y_visvehpasDown4_t_21_tt_21_travel_i_visvehBackpasDown2_j_visvehBackpasDown4
                                                    continue;
                                                }
                                                //某个乘客的visUp可以经过这个乘客的pasDown，在换乘的情况下
                                            }
                                            else//如果是visDown
                                            {
                                                if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex &&
                                                       coorPoint1.individualID.individualType == "visveh" &&
                                                       coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                //visvehDown仅去对应的visDown点
                                                {
                                                    continue;
                                                }
                                                if (coorPoint1.individualID.individualType == "visveh" &&
                                                       coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                //visvehDown不去任何visUp
                                                {
                                                    continue;
                                                }
                                                if (coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                        coorPoint1.individualID.individualType == "visveh" &&
                                                        coorPoint1.individualID.UpOrDown.ToString() == "Down" &&
                                                        coorPoint1.individualID.OutOrBack.ToString() == "Back")//对应的visvehBackpasDown
                                                {
                                                    //不从对应的visvehBackpasDown出发
                                                    continue;
                                                }
                                            }

                                        }

                                    }
                                    
                                    if (coorPoint1.individualID.UpOrDown == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                    {
                                        coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                        coorPoint1.individualID.O_or_D_or_M_none = "D";
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                    }
                                    Point coorPoint2 = new Point();
                                    TimeWindow tWPoint2;
                                    IndividualID KeyPInfo1 = PointInfo1.Key;
                                    IndividualID KeyPInfo2;
                                    List<Point> spaceVertices;
                                    foreach (var PointInfo2 in coordinatesDicPasVeh)
                                    {
                                        coorPoint2 = Program.DeepCopyByBin(PointInfo2.Value[0].point); Point coor_copied2 = coorPoint2;
                                        tWPoint2 = PointInfo2.Value[0].timeWindow;
                                        KeyPInfo2 = PointInfo2.Key;
                                        if (coorPoint2.individualID.UpOrDown.ToString() == "Up" && coorPoint2.individualID.individualType.ToString() == "pas")
                                        {
                                            coorPoint2.individualID.O_or_D_or_M_none = "O";
                                            coorPoint2.individualID = coor_copied2.individualID.Copying_info(coorPoint2.individualID);
                                        }
                                        if (KeyPInfo1.ToString() == KeyPInfo2.ToString())
                                        {
                                            continue;
                                        }

                                        if (chosenInDi_ID.individualType == veh)//如果运动的主体是车辆
                                        {
                                            if (coorPoint2.individualID.individualType.ToString() == "vehCor")
                                            {//仅经过自己的车辆起始和终止点
                                                if (coorPoint2.individualID.IDIndex != chosenInDi_ID.IDIndex)
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (coorPoint2.individualID.individualType.ToString() == "visveh")
                                            {
                                                continue;
                                            }
                                        }
                                        else if (chosenInDi_ID.individualType.ToString() == "visveh")
                                        {
                                            if (coorPoint2.individualID.individualType.ToString() == "vehCor")
                                            {
                                                continue;
                                            }
                                            if (coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                coorPoint1.individualID.individualType == "visveh")
                                            {
                                                continue;
                                            }
                                            if (chosenInDi_ID.UpOrDown.ToString() == "Up" &&
                                                (coorPoint2.individualID.IDIndex != chosenInDi_ID.IDIndex ||
                                                 coorPoint2.individualID.O_or_D_or_M_none.ToString() != "O") &&
                                                 coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                 coorPoint1.individualID.individualType == "visveh" &&
                                                 coorPoint1.individualID.UpOrDown.ToString() == "Up" &&
                                                 coorPoint1.individualID.OutOrBack.ToString() == "Out")//对应的visvehOutpasUp
                                            {
                                                //只有对应的OpasUp才可以被visvehOutpasUp连
                                                continue;
                                            }

                                            if (chosenInDi_ID.UpOrDown.ToString() == "Down" &&
                                                (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex ||
                                                coorPoint1.individualID.O_or_D_or_M_none.ToString() != "D") &&
                                                coorPoint2.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                coorPoint2.individualID.UpOrDown == "Down" &&
                                                coorPoint2.individualID.OutOrBack.ToString() == "Back")//对应的visvehBackpasDown
                                            {
                                                //只有对应的DpasDown才可以连到visvehBackpasDown
                                                continue;
                                            }
                                            // Y_visvehpasDown1_t_14_tt_17_travel_i_DpasDown1_j_pasDown2
                                            //if (chosenInDi_ID.UpOrDown.ToString() == "Down" &&
                                            //    coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                            //    coorPoint1.individualID.UpOrDown.ToString() == "Down" &&
                                            //    !(coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                            //    coorPoint2.individualID.UpOrDown == "Down" &&
                                            //    coorPoint2.individualID.OutOrBack.ToString() == "Back")
                                            //    )//对应的visvehBackpasDown
                                            //{
                                            //    //只有对应的DpasDown才可以连到visvehBackpasDown
                                            //    continue;
                                            //}

                                            //if (chosenInDi_ID.UpOrDown.ToString() == "Down" &&
                                            //    coorPoint2.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                            //    coorPoint2.individualID.O_or_D_or_M_none.ToString() == "O")//visvehpasDown不到达对应的OpasUp
                                            //{
                                            //    continue;
                                            //}

                                            if (coorPoint2.individualID.individualType.ToString() == "visveh")
                                            {
                                                if (chosenInDi_ID.UpOrDown.ToString() == "Up")//如果是visUp
                                                {
                                                    if (coorPoint2.individualID.OutOrBack.ToString() == "Out")
                                                    {
                                                        continue;
                                                        //没有其他点可以回到visvehOut
                                                    }
                                                    if (coorPoint2.individualID.IDIndex != chosenInDi_ID.IDIndex &&
                                                         coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                         coorPoint2.individualID.UpOrDown.ToString() == "Up")
                                                    //某个乘客的visUp仅经过这个乘客的visUp
                                                    {
                                                        continue;
                                                    }
                                                    if (coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                         coorPoint2.individualID.UpOrDown == "Down")
                                                    //某个乘客的visUp不经过任何乘客vispasDown
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else//如果是visDown
                                                {
                                                    if (coorPoint2.individualID.IDIndex != chosenInDi_ID.IDIndex &&
                                                           coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                           coorPoint2.individualID.UpOrDown == "Down")
                                                    //visvehDown仅去对应的visDown点
                                                    {
                                                        continue;
                                                    }
                                                    if (coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                           coorPoint2.individualID.UpOrDown.ToString() == "Up")
                                                    //visvehDown不去任何visUp
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                        }

                                        int iter = 0;
                                        if ((indiInfo_KV.Key.individualType.ToString() == "visveh") &&
                                            (indiInfo_KV.Key.UpOrDown.ToString() == "Down") &&//visvehDown
                                            PointInfo1.Key.individualType.ToString() == "visveh" &&
                                            PointInfo1.Key.OutOrBack.ToString() == "Out" &&
                                            PointInfo1.Key.UpOrDown.ToString() == "Down" &&//vispasDownOut-pasUp, vispasDownOut-DpasDown
                                            (// Y_visvehpasDown1_t_08_tt_08_travel_i_visvehOutpasDown1_j_OpasUp1
                                            (PointInfo2.Key.UpOrDown.ToString() == "Up" &&
                                            PointInfo2.Key.individualType.ToString() == "pas") ||
                                            (PointInfo2.Key.UpOrDown.ToString() == "Down" &&
                                            PointInfo2.Key.individualType.ToString() == "pas"&&
                                            PointInfo2.Key.IDIndex!= indiInfo_KV.Key.IDIndex)
                                            )
                                            ||
                                            (indiInfo_KV.Key.individualType.ToString() == "visveh") &&
                                            (indiInfo_KV.Key.UpOrDown.ToString() == "Up") &&//visvehUp
                                            PointInfo2.Key.individualType.ToString() == "visveh" &&
                                            PointInfo2.Key.OutOrBack.ToString() == "Back" &&
                                            PointInfo2.Key.UpOrDown.ToString() == "Up" &&//pasDown-vispasUpBack, OpasUp-vispasUpBack
                                            (
                                            (PointInfo1.Key.UpOrDown.ToString() == "Down" &&
                                            PointInfo1.Key.individualType.ToString() == "pas")||
                                            (PointInfo1.Key.UpOrDown.ToString() == "Up" &&
                                            PointInfo1.Key.individualType.ToString() == "pas" &&
                                            PointInfo1.Key.IDIndex != indiInfo_KV.Key.IDIndex)
                                            )
                                            )
                                        {
                                            iter = 3;
                                        }
                                        else
                                        {
                                            iter = 2;
                                        }
                                        for(int iii=1; iii < iter; iii++)
                                        {
                                            
                                            if (iii == 2)
                                            {
                                                if (indiInfo_KV.Key.UpOrDown.ToString() == "Down")
                                                {
                                                    if (PointInfo2.Key.UpOrDown.ToString() == "Up")
                                                    {
                                                        coorPoint2 = Program.DeepCopyByBin(PointInfo2.Value[0].point);
                                                    }
                                                    else if (PointInfo2.Key.UpOrDown.ToString() == "Down")
                                                    {
                                                        coorPoint2 = Program.DeepCopyByBin(PointInfo2.Value[0].point);
                                                        coorPoint2.individualID = "D" + coorPoint2.individualID.ToString();
                                                        coorPoint2.individualID.O_or_D_or_M_none = "D";
                                                        coorPoint2.individualID = coor_copied2.individualID.Copying_info(coorPoint2.individualID);
                                                    }
                                                }
                                                else if (indiInfo_KV.Key.UpOrDown.ToString() == "Up")
                                                {
                                                    if(PointInfo1.Key.UpOrDown.ToString() == "Up")
                                                    {
                                                        coorPoint1 = PointInfo1.Value[0].point;
                                                        coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                        coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                                    }
                                                    else if (PointInfo1.Key.UpOrDown.ToString() == "Down")
                                                    {
                                                        coorPoint1 = PointInfo1.Value[0].point;
                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }
                                            else
                                            {
                                                if ((indiInfo_KV.Key.individualType.ToString() == "visveh") &&
                                            (indiInfo_KV.Key.UpOrDown.ToString() == "Up") &&//visvehUp
                                            (//e.g. Y_visvehpasUp1_t_10_tt_10_travel_i_DpasDown1_j_XXXX不可以出现
                                            PointInfo1.Key.UpOrDown.ToString() == "Down" &&
                                            PointInfo1.Key.individualType.ToString() == "pas" &&
                                            PointInfo1.Key.IDIndex == indiInfo_KV.Key.IDIndex)
                                            )
                                                {
                                                    continue;
                                                }
                                                if ((indiInfo_KV.Key.individualType.ToString() == "visveh") &&
                                            (indiInfo_KV.Key.UpOrDown.ToString() == "Down") &&//visvehDown
                                            PointInfo1.Key.individualType.ToString() == "visveh" &&
                                            PointInfo1.Key.OutOrBack.ToString() == "Out" &&
                                            PointInfo1.Key.UpOrDown.ToString() == "Down" &&//vispasDownOut-OpasUp(自己的不行)
                                            (// Y_visvehpasDown1_t_08_tt_08_travel_i_visvehOutpasDown1_j_OpasUp1
                                            (PointInfo2.Key.UpOrDown.ToString() == "Up" &&
                                            PointInfo2.Key.individualType.ToString() == "pas") &&
                                            PointInfo2.Key.IDIndex == indiInfo_KV.Key.IDIndex))
                                                {
                                                    continue;
                                                }
                                                if (coorPoint2.individualID.UpOrDown.ToString() == "Up" && coorPoint2.individualID.individualType.ToString() == "pas")
                                                {
                                                    coorPoint2.individualID = "O" + coorPoint2.individualID.ToString();
                                                    coorPoint2.individualID.O_or_D_or_M_none = "O";
                                                    coorPoint2.individualID = coor_copied2.individualID.Copying_info(coorPoint2.individualID);
                                                }
                                            }
                                            
                                            GeneratingTravellingCost(coorPoint1, coorPoint2, out spaceVertices, out double value, out int distance, out int TimeSpent);
                                            ArcID arcID = chosenInDi_ID.ToString() + typeTravel.ToString() + coorPoint1.individualID.ToString() + coorPoint2.individualID.ToString();
                                            arcID.arcSpaceVertices = spaceVertices;
                                            Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                            if (coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                           coorPoint1.individualID.individualType == "visveh" &&
                                                           coorPoint1.individualID.UpOrDown.ToString() == "Down" &&
                                                           chosenInDi_ID.UpOrDown.ToString() == "Down")
                                            //下车段车辆的出发位置需要与任意节点的距离都是0,但是不能有圈
                                            {
                                                TimeSpent = 0;
                                                distance = 0;
                                            }
                                            if (coorPoint2.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                           coorPoint2.individualID.individualType.ToString() == "visveh" &&
                                                           coorPoint2.individualID.UpOrDown.ToString() == "Up" &&
                                                           chosenInDi_ID.UpOrDown.ToString() == "Up")
                                            //上车段车辆的结束位置也需要与任意节点的距离都是0,但是不能有圈
                                            {
                                                TimeSpent = 0;
                                                distance = 0;
                                            }
                                            //如果是一辆车将pasUp放在pasDown，此时还没有到pasDown的服务时间，乘客可以等等再走
                                            //但车辆总归是需要从pasDown出发的，现有设定下车辆是无法出发的
                                            int from_this_point_to_end;
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices2, out double value2, out int from_out_to_this_point, out int TimeSpent2);
                                            int lower_limit = Math.Max(tWPoint1.lowerLimit, indiInfo_KV.Value[0].timeWindow.lowerLimit+ from_out_to_this_point);
                                            int upper_limit = tWPoint2.upperLimit - TimeSpent + 1 + opTime+1;
                                            if (chosenInDi_ID.individualType=="veh")
                                            {
                                                GeneratingTravellingCost(coorPoint2, indiInfo_KV.Value[1].point, out List<Point> spaceVertices3, out double value3, out from_this_point_to_end, out int TimeSpent3);
                                                upper_limit = Math.Min(tWPoint2.upperLimit - TimeSpent + 1 + opTime + 1, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_end + 1);
                                            }
                                            if (chosenInDi_ID.ToString().Contains("visvehpasDown"))
                                            {
                                                GeneratingTravellingCost(indiInfo_KV.Value[0].point.individualID.if_vis_veh_then_the_other_half, coorPoint1, out List<Point> spaceVertices1, out double value1, out int distance1, out int from_start_to_this_point);
                                                lower_limit = Math.Max(tWPoint1.lowerLimit, indiInfo_KV.Value[0].timeWindow.lowerLimit + from_start_to_this_point);

                                                GeneratingTravellingCost(coorPoint2, indiInfo_KV.Value[0].point, out spaceVertices2, out value2, out int distance2, out from_this_point_to_end);
                                                upper_limit = Math.Min(tWPoint2.upperLimit - TimeSpent + 1 + tsTime -opTime, indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_end- TimeSpent + 1);
                                            }
                                            if (chosenInDi_ID.ToString().Contains("visvehpasUp") &&
                                                coorPoint1.individualID.individualType == "pas" &&
                                                coorPoint2.individualID.individualType == "pas")
                                            {
                                                GeneratingTravellingCost(indiInfo_KV.Value[0].point, coorPoint1, out List<Point> spaceVertices1, out double value1, out int distance1, out int from_start_to_this_point);
                                                lower_limit = Math.Max(PointInfo1.Value[0].timeWindow.lowerLimit, indiInfo_KV.Value[0].timeWindow.lowerLimit + from_start_to_this_point);
                                                GeneratingTravellingCost(coorPoint2, indiInfo_KV.Value[0].point.individualID.if_vis_veh_then_the_other_half, out spaceVertices2, out value2, out int distance2, out from_this_point_to_end);
                                                upper_limit = Math.Min(tWPoint2.upperLimit - TimeSpent + 1 + tsTime, indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_end- TimeSpent + 1);
                                                if (coorPoint1.individualID.UpOrDown == "Up"
                                                && indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex)//Y_pasUp2_t_07_tt_08_dwell_i_pasUp2_j_pasUp2
                                                {
                                                    //需要op再到这个点
                                                    lower_limit = PointInfo1.Value[0].timeWindow.lowerLimit + opTime;
                                                }
                                            }
                                            if (chosenInDi_ID.ToString().Contains("visvehpasDown") &&
                                                coorPoint1.individualID.ToString().Contains("visvehOutpasDown"))
                                            {
                                                //从其他点走的话pasDown的TsTime是1，记得+1???好像+1就不对了
                                                GeneratingTravellingCost(coorPoint2, indiInfo_KV.Value[0].point.individualID.if_vis_veh_then_the_other_half, out List<Point> spaceVertices1, out double value1, out int distance11, out from_out_to_this_point);
                                                GeneratingTravellingCost(coorPoint2, indiInfo_KV.Value[0].point, out List<Point> DumpspaceVertices, out double Dumpvalue, out int distance1, out from_this_point_to_end); 
                                                lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + opTime + from_out_to_this_point + 1; // Y_visvehpasDown1_t_08_tt_08_travel_i_visvehOutpasDown1_j_pasUp2
                                                upper_limit = indiInfo_KV.Value[0].otherTimeWindow.upperLimit + 1 - opTime - from_this_point_to_end + tsTime+1;

                                                // Y_pasDown2_t_08_tt_08_start_i_TsO_j_pasUp2
                                                //GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].otherPoint, out List<Point> DumpspaceVertices1, out double Dumpvalue1, out int distance1, out int from_start_to_this_point);
                                                //GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> DumpspaceVertices, out double Dumpvalue, out int distance, out int from_this_point_to_end);
                                                //int lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + opTime + from_start_to_this_point;
                                                //int upper_limit = indiInfo_KV.Value[0].timeWindow.upperLimit + 1 - opTime - from_this_point_to_end + tsTime;

                                                if (coorPoint2.individualID.UpOrDown == "Up" &&
                                                coorPoint2.individualID.individualType == "pas" &&
                                                coorPoint2.individualID.IDIndex == chosenInDi_ID.IDIndex)// Y_visvehpasDown1_t_04_tt_04_travel_i_visvehOutpasDown1_j_pasUp1
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + opTime;
                                                    upper_limit = indiInfo_KV.Value[0].otherTimeWindow.upperLimit + 1 - opTime - from_this_point_to_end + tsTime;
                                                }
                                            }

                                            if (chosenInDi_ID.ToString().Contains("visvehpasUp") &&
                                                coorPoint2.individualID.ToString().Contains("visvehBackpasUp"))
                                            {
                                                GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices1, out double value1, out from_out_to_this_point, out int TimeSpent1);
                                                GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point.individualID.if_vis_veh_then_the_other_half, out List<Point> DumpspaceVertices, out double Dumpvalue, out int distance1, out from_this_point_to_end);
                                                lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + TimeSpent1 + opTime;
                                                if (chosenInDi_ID.ToString() == "visvehpasUp1" && coorPoint2.individualID.ToString() == "pasDown1")
                                                {

                                                }
                                                upper_limit = indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_end + 2;
                                                if (chosenInDi_ID.IDIndex == coorPoint1.individualID.IDIndex
                                                    && coorPoint1.individualID.individualType == "pas"
                                                    && coorPoint1.individualID.UpOrDown == "Up")// Y_visvehpasUp1_t_04_tt_04_travel_i_pasUp1_j_visvehBackpasUp1
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + opTime;
                                                }


                                                //if (coorPoint1.individualID.UpOrDown.ToString() == "Down"
                                                //&& coorPoint1.individualID.individualType.ToString() == "pas"
                                                //&& coorPoint1.individualID.O_or_D_or_M_none.ToString() == "D"
                                                //)
                                                //{//D
                                                //    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + 2 * opTime + from_out_to_this_point;
                                                //}
                                                //if (coorPoint1.individualID.UpOrDown.ToString() == "Up"
                                                //    && coorPoint1.individualID.individualType.ToString() == "pas"
                                                //    && coorPoint1.individualID.IDIndex != indiInfo_KV.Key.IDIndex
                                                //    && coorPoint1.individualID.O_or_D_or_M_none.ToString() == "none")
                                                //{
                                                //    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + 2 * opTime + from_out_to_this_point;
                                                //}

                                            }
                                            

                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {//从起始点或终止点的lowerLimit到起始点或终止点的upperLimit
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                        }
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeDwell.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coordinatesDicPasVeh)
                                {
                                    IndividualID KeyPInfo1 = PointInfo1.Key;
                                    TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                    Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                    if (chosenInDi_ID.individualType == veh)
                                    {
                                        if (coorPoint1.individualID.individualType.ToString() == "vehCor")
                                        {
                                            if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex)
                                            {
                                                continue;
                                            }
                                        }
                                        else if (coorPoint1.individualID.individualType == "visveh")
                                        {
                                            continue;
                                        }
                                    }
                                    else if (chosenInDi_ID.individualType.ToString() == "visveh")
                                    {
                                        if (coorPoint1.individualID.individualType.ToString() == "vehCor")
                                        {
                                            continue;
                                        }
                                        if (chosenInDi_ID.UpOrDown.ToString() == "Up")//如果是VisUp
                                        {
                                            if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex &&
                                                 coorPoint1.individualID.individualType == "visveh" &&
                                                 coorPoint1.individualID.UpOrDown.ToString() == "Up")

                                            //某个乘客的visUp仅经过这个乘客的visUp
                                            {
                                                continue;
                                            }
                                            if (coorPoint1.individualID.individualType == "visveh" &&
                                                 coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                            //某个乘客的visUp不经过任何乘客vispasDown
                                            {
                                                continue;
                                            }
                                            if (coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                 coorPoint1.individualID.individualType == "pas" &&
                                                 coorPoint1.individualID.UpOrDown.ToString() == "Down")

                                            {
                                                continue;
                                            }
                                        }
                                        else//如果是visDown
                                        {
                                            if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex &&
                                                   coorPoint1.individualID.individualType == "visveh" &&
                                                   coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                            //visvehDown仅去对应的visDown点
                                            {
                                                continue;
                                            }
                                            if (coorPoint1.individualID.individualType == "visveh" &&
                                                   coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                            //visvehDown不去任何visUp
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    int times;
                                    if (PointInfo1.Key.individualType.ToString() == "vehCor" || PointInfo1.Key.individualType.ToString() == "visveh")
                                    {
                                        times = 2;
                                    }
                                    else if (PointInfo1.Key.individualType=="pas"&& PointInfo1.Key.UpOrDown=="Up"
                                        && PointInfo1.Key.IDIndex== indiInfo_KV.Key.IDIndex&&
                                        indiInfo_KV.Key.individualType=="visveh"&& indiInfo_KV.Key.UpOrDown == "Down"
                                        )
                                    {//individual:pasDown1 point:pasUp1 不存在OpasUp1-pasUp1弧，存在pasUp1-pasUp1弧 不存在OpasUp1-OpasUp1弧
                                        times = 2;
                                    }
                                    else
                                    {
                                        times = 4;
                                    }
                                    for (int i = 1; i < times; i++)
                                    {
                                        if (indiInfo_KV.Key.ToString() == "visvehpasUp2")
                                        {

                                        }
                                        coorPoint1 = PointInfo1.Value[0].point;
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        Point coorPoint2 = new Point();
                                        GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out List<Point> spaceVertices1, out double value1, out int from_out_to_this_point, out int TimeSpent1);
                                        GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[1].point, out List<Point> spaceVertices2, out double value2, out int from_this_point_to_back, out int TimeSpent2);
                                        if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Up")
                                        {
                                            from_out_to_this_point = 0;
                                            from_this_point_to_back = 0;
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out spaceVertices1, out value1, out from_out_to_this_point, out TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point.individualID.if_vis_veh_then_the_other_half, out spaceVertices2, out value2, out from_this_point_to_back, out TimeSpent2);
                                        }
                                        else if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Down")
                                        {
                                            from_out_to_this_point = 0;
                                            from_this_point_to_back = 0;
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point.individualID.if_vis_veh_then_the_other_half, out spaceVertices1, out value1, out from_out_to_this_point, out TimeSpent1);
                                            GeneratingTravellingCost(coorPoint1, indiInfo_KV.Value[0].point, out spaceVertices2, out value2, out from_this_point_to_back, out TimeSpent2);
                                        }
                                        if (i == 1)//pasup-pasup pasdown-pasdown
                                        {
                                            coorPoint2 = coorPoint1;
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            
                                            //pas
                                            //int lower_limit = tWIndividual.lowerLimit + from_out_to_this_point;
                                            //int upper_limit = Math.Min(tWPoint1.upperLimit + tsTime, indiInfo_KV.Value[0].otherTimeWindow.upperLimit - from_this_point_to_back);

                                            int lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit+ from_out_to_this_point;
                                            int upper_limit;
                                            upper_limit = Math.Max(tWPoint1.upperLimit, indiInfo_KV.Value[1].timeWindow.upperLimit- from_this_point_to_back + 1);
                                            if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Up")
                                            {
                                                if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    upper_limit = Math.Max(tWPoint1.upperLimit + tsTime, PointInfo1.Value[0].otherTimeWindow.upperLimit);
                                                }
                                                else if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                {
                                                    upper_limit = Math.Max(tWPoint1.upperLimit, PointInfo1.Value[0].otherTimeWindow.upperLimit + tsTime);
                                                }
                                                if (indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    upper_limit = Math.Min(tWPoint1.upperLimit + tsTime, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back);
                                                }
                                                if (indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex 
                                                    && coorPoint1.individualID.UpOrDown.ToString() == "Up"
                                                    && coorPoint1.individualID.individualType.ToString() == "pas"
                                                    && coorPoint1.individualID.O_or_D_or_M_none.ToString() == "none")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + from_out_to_this_point + opTime;
                                                }
                                                upper_limit = Math.Min(upper_limit + 2, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back);
                                                arcID.arcSpaceVertices = spaceVertices;
                                                if (indiInfo_KV.Value[0].timeWindow.lowerLimit > tWPoint1.upperLimit)
                                                {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                    continue;
                                                }
                                            }
                                            else if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Down")
                                            {
                                                if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                                    upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + tsTime;
                                                }
                                                else if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                                    upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + tsTime;
                                                }
                                                upper_limit = Math.Min(upper_limit + 2, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back +1);
                                            }
                                            

                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                            if (chosenInDi_ID.individualType.ToString() == "visveh")
                                            {
                                                if (coorPoint1.individualID.IDIndex != chosenInDi_ID.IDIndex 
                                                    //||(coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex && coorPoint1.individualID.UpOrDown.ToString() != chosenInDi_ID.UpOrDown.ToString())
                                                    )
                                                {
                                                    Dictionary<ArcTime, Arc> arcDicTime0 = new Dictionary<ArcTime, Arc>();
                                                    if (coorPoint2.individualID.UpOrDown.ToString() == "Down" && coorPoint2.individualID.individualType.ToString() == "pas")
                                                    {
                                                        coorPoint2.individualID = "D" + coorPoint2.individualID.ToString();
                                                        coorPoint2.individualID.O_or_D_or_M_none = "D";
                                                        coorPoint2.individualID = coor_copied1.individualID.Copying_info(coorPoint2.individualID);
                                                    }
                                                    if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                                    {
                                                        coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                        coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                                    }
                                                    arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                                    spaceVertices = new List<Point>();
                                                    spaceVertices.Add(coorPoint1);
                                                    spaceVertices.Add(coorPoint2);
                                                    arcID.arcSpaceVertices = spaceVertices;
                                                    
                                                    for (int k = lower_limit; k < upper_limit; k++)
                                                    {
                                                        int TimeSpent = 1;
                                                        GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                        listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                        ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                        arcDicTime0.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                                    }
                                                    arcDicIDTime.Add(arcID, arcDicTime0);
                                                }

                                            }

                                        }
                                        else if (i == 2)//OpasUp-OpasUp DpasDown-DpasDown
                                        {
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "D" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "D";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            coorPoint2 = coorPoint1;
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint2.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            int lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + from_out_to_this_point;
                                            int upper_limit;
                                            upper_limit = Math.Max(tWPoint1.upperLimit, PointInfo1.Value[0].otherTimeWindow.upperLimit);
                                            if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Up")
                                            {
                                                if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    upper_limit = Math.Max(tWPoint1.upperLimit + tsTime, PointInfo1.Value[0].otherTimeWindow.upperLimit);
                                                }
                                                else if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                {
                                                    upper_limit = Math.Max(tWPoint1.upperLimit, PointInfo1.Value[0].otherTimeWindow.upperLimit + tsTime);
                                                }
                                                if (indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {//Y_visvehpasUp2_t_07_tt_08_dwell_i_OpasUp2_j_OpasUp2
                                                    upper_limit = tWPoint1.upperLimit + opTime - 1;
                                                    //upperlimit开始dwell，则upperlimit+1 dwell结束
                                                    //upperlimit+1处必须已经开始上车，但不必在车上，所以理论上可以upperlimit+1才开始上车，所以+opTime-1
                                                }
                                                upper_limit = Math.Min(upper_limit + 2, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back);
                                                if (indiInfo_KV.Value[0].timeWindow.lowerLimit > tWPoint1.upperLimit)
                                                {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                    continue;
                                                }
                                                if (coorPoint1.individualID.UpOrDown == "Up"
                                                && indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex)//Y_pasUp2_t_07_tt_08_dwell_i_pasUp2_j_pasUp2
                                                {
                                                    upper_limit = tWPoint1.upperLimit + opTime - 1;
                                                    //upperlimit开始dwell，则upperlimit+1 dwell结束
                                                    //upperlimit+1处必须已经开始上车，但不必在车上，所以理论上可以upperlimit+1才开始上车，所以+opTime-1
                                                }
                                                if (indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex
                                                    && coorPoint1.individualID.UpOrDown.ToString() == "Down"
                                                    && coorPoint1.individualID.individualType.ToString() == "pas"
                                                    && coorPoint1.individualID.O_or_D_or_M_none.ToString() == "D")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + from_out_to_this_point + opTime;
                                                }
                                            }
                                            else if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Down")
                                            {
                                                if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                                    upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + tsTime;
                                                }
                                                else if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                                    upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + tsTime;
                                                }
                                                upper_limit = Math.Min(upper_limit + 2, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back +1);
                                            }

                                            //if (coorPoint1.individualID.UpOrDown.ToString() == "Down" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            //{
                                            //    lower_limit = lower_limit + 1;
                                            //}
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);
                                        }
                                        else
                                        {//pasDown-MpasDown-DpasDown OpasUp-MpasUp-pasUp times = 4;
                                            if (chosenInDi_ID.individualType.ToString() == "visveh"
                                                && chosenInDi_ID.IDIndex == coorPoint1.individualID.IDIndex
                                                && chosenInDi_ID.UpOrDown.ToString() == coorPoint1.individualID.UpOrDown.ToString()
                                                )
                                            {
                                                continue;
                                            }
                                            coorPoint2 = coorPoint1;
                                            Point coorPoint3 = new Point();
                                            coorPoint3 = coorPoint1;
                                            coorPoint3.individualID = "M" + coorPoint2.individualID.ToString();
                                            coorPoint3.individualID.O_or_D_or_M_none = "M";coorPoint3.individualID = coor_copied1.individualID.Copying_info(coorPoint3.individualID);
                                            if (coorPoint2.individualID.UpOrDown.ToString() == "Down" && coorPoint2.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint2.individualID = "D" + coorPoint2.individualID.ToString();
                                                coorPoint2.individualID.O_or_D_or_M_none = "D";
                                                coorPoint2.individualID = coor_copied1.individualID.Copying_info(coorPoint2.individualID);
                                            }
                                            if (coorPoint1.individualID.UpOrDown.ToString() == "Up" && coorPoint1.individualID.individualType.ToString() == "pas")
                                            {
                                                coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                                coorPoint1.individualID.O_or_D_or_M_none = "O";
                                                coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                            }
                                            ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID + coorPoint3.individualID;
                                            List<Point> spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint1);
                                            spaceVertices.Add(coorPoint3);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            int lower_limit = indiInfo_KV.Value[0].timeWindow.lowerLimit + from_out_to_this_point;
                                            int upper_limit;
                                            upper_limit = Math.Max(tWPoint1.upperLimit, PointInfo1.Value[0].otherTimeWindow.upperLimit);
                                            if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Up")
                                            {
                                                if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    upper_limit = Math.Max(tWPoint1.upperLimit + tsTime, PointInfo1.Value[0].otherTimeWindow.upperLimit);
                                                }
                                                else if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                {
                                                    upper_limit = Math.Max(tWPoint1.upperLimit, PointInfo1.Value[0].otherTimeWindow.upperLimit + tsTime);
                                                }
                                                if (indiInfo_KV.Key.IDIndex == coorPoint1.individualID.IDIndex && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    upper_limit = Math.Min(tWPoint1.upperLimit + tsTime, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back);
                                                }
                                                upper_limit = Math.Min(upper_limit + 2, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back);
                                                if (indiInfo_KV.Value[0].timeWindow.lowerLimit > tWPoint1.upperLimit)
                                                {//即乘客的上车服务时间窗下限比这个点的上限还晚，也就是说p+不可能从这个弧travel
                                                    continue;
                                                }
                                            }
                                            else if (indiInfo_KV.Key.individualType.ToString() == "visveh" && indiInfo_KV.Key.UpOrDown.ToString() == "Down")
                                            {
                                                if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Up")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                                    upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + tsTime;
                                                }
                                                else if (coorPoint1.individualID.individualType.ToString() == "pas" && coorPoint1.individualID.UpOrDown.ToString() == "Down")
                                                {
                                                    lower_limit = indiInfo_KV.Value[0].otherTimeWindow.lowerLimit + from_out_to_this_point;
                                                    upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + tsTime;
                                                }
                                                upper_limit = Math.Min(upper_limit + 2, indiInfo_KV.Value[1].timeWindow.upperLimit - from_this_point_to_back +1);
                                            }
                                            
                                            
                                            for (int k = lower_limit; k < upper_limit; k++)
                                            {
                                                int TimeSpent = 1;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime);

                                            Dictionary<ArcTime, Arc> arcDicTime2 = new Dictionary<ArcTime, Arc>();
                                            arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint3.individualID + coorPoint2.individualID;
                                            spaceVertices = new List<Point>();
                                            spaceVertices.Add(coorPoint3);
                                            spaceVertices.Add(coorPoint2);
                                            arcID.arcSpaceVertices = spaceVertices;
                                            for (int k = lower_limit + 1; k < upper_limit + 1; k++)
                                            {
                                                int TimeSpent = 0;
                                                GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                                listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                                ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                                arcDicTime2.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                            }
                                            arcDicIDTime.Add(arcID, arcDicTime2);
                                        }
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeEnd.ToString())
                            {
                                foreach (var PointInfo1 in coorTwDicVeh)
                                {
                                    Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                    IndividualID PointID1 = PointInfo1.Key;
                                    ArcID arcID;
                                    Dictionary<ArcTime, Arc> arcDicTime;
                                    Point thiscoordinatesBack;
                                    Point thiscoordinatesOut;
                                    TimeWindow IndiPoint1;
                                    TimeWindow IndiPoint2;
                                    List<Point> spaceVertices;
                                    if (PointID1.ToString() == chosenInDi_ID.ToString())
                                    {
                                        if (PointID1.UpOrDown.ToString() == "Up")
                                        {
                                            arcID = chosenInDi_ID.ToString() + arcType + PointID1;
                                        }
                                        else
                                        {
                                            arcID = chosenInDi_ID.ToString() + arcType + "D" + PointID1;
                                        }
                                        arcDicTime = new Dictionary<ArcTime, Arc>();
                                        thiscoordinatesBack = PointInfo1.Value[1].point;//车辆返回地点
                                        IndiPoint1 = indiInfo_KV.Value[0].timeWindow;//车辆出发时间窗
                                        IndiPoint2 = indiInfo_KV.Value[1].timeWindow;//车辆返回时间窗
                                        spaceVertices = new List<Point>();
                                        spaceVertices.Add(thiscoordinatesBack);
                                        spaceVertices.Add(pointVD);
                                        arcID.arcSpaceVertices = spaceVertices;
                                        for (int k = IndiPoint1.lowerLimit; k < IndiPoint2.upperLimit + 1; k++)
                                        {
                                            int TimeSpent = 0;
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    //不出车end
                                    arcDicTime = new Dictionary<ArcTime, Arc>();
                                    thiscoordinatesOut = PointInfo1.Value[0].point;//车辆出发地点
                                    IndiPoint1 = indiInfo_KV.Value[0].timeWindow;//车辆出发时间窗
                                    IndiPoint2 = indiInfo_KV.Value[1].timeWindow;//车辆返回时间窗
                                    arcID = chosenInDi_ID.ToString() + arcType + thiscoordinatesOut.individualID;
                                    spaceVertices = new List<Point>();
                                    spaceVertices.Add(thiscoordinatesOut);
                                    spaceVertices.Add(pointVD);
                                    arcID.arcSpaceVertices = spaceVertices;
                                    for (int k = IndiPoint1.lowerLimit; k < IndiPoint1.upperLimit + 1; k++)
                                    {
                                        int TimeSpent = 0;
                                        GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                        listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                        ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                        arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                    }
                                    arcDicIDTime.Add(arcID, arcDicTime);
                                    arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                                }
                            }
                            if (arcType.ToString() == typeStart.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>(); List<Arc> ArcTypeComfirmedList = new List<Arc>();
                                foreach (var PointInfo1 in coorTwDicVeh)
                                {
                                    IndividualID PointID1 = PointInfo1.Key;
                                    Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                    if (PointID1.ToString() == chosenInDi_ID.ToString())
                                    {
                                        ArcID arcID = chosenInDi_ID.ToString() + arcType + PointID1;
                                        Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1; TimeWindow tWPoint1 = PointInfo1.Value[0].timeWindow;
                                        List<Point> spaceVertices = new List<Point>();
                                        spaceVertices.Add(pointVO);
                                        spaceVertices.Add(coorPoint1);
                                        arcID.arcSpaceVertices = spaceVertices;
                                        int lower_limit = tWPoint1.lowerLimit;
                                        int upper_limit = PointInfo1.Value[1].timeWindow.upperLimit + 1;
                                        if (indiInfo_KV.Key.individualType.ToString() == "visveh"&& indiInfo_KV.Key.UpOrDown.ToString()=="Up")
                                        {
                                            upper_limit = PointInfo1.Value[0].timeWindow.upperLimit + 1;
                                        }
                                        for (int k = tWPoint1.lowerLimit; k < upper_limit; k++)
                                        {
                                            int TimeSpent = 0;
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeOper.ToString())
                            {
                                Dictionary<ArcID, Dictionary<ArcTime, Arc>> arcDicIDTime = new Dictionary<ArcID, Dictionary<ArcTime, Arc>>();
                                List<Arc> ArcTypeComfirmedList = new List<Arc>();

                                foreach (var PointInfo1 in coorTwDicPas_static)
                                {
                                    Point coorPoint1 = Program.DeepCopyByBin(PointInfo1.Value[0].point); Point coor_copied1 = coorPoint1;
                                    Point coorPoint2 = coorPoint1;

                                    if (PointInfo1.Value[0].point.pointType.ToString() == "Up")
                                    {
                                        if (chosenInDi_ID.individualType.ToString() == "visveh" &&
                                            !(coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                 chosenInDi_ID.UpOrDown.ToString() == "Up"))
                                        //某个乘客的visUp仅服务这个乘客的visUp
                                        {
                                            continue;
                                        }
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        coorPoint1.individualID = "O" + coorPoint1.individualID.ToString();
                                        coorPoint1.individualID.O_or_D_or_M_none = "O";
                                        coorPoint1.individualID = coor_copied1.individualID.Copying_info(coorPoint1.individualID);
                                        TimeWindow tWPoint1 = indiInfo_KV.Value[0].timeWindow;
                                        List<Point> spaceVertices = new List<Point>();
                                        spaceVertices.Add(coorPoint1);
                                        spaceVertices.Add(coorPoint2);
                                        ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID.ToString();
                                        arcID.arcSpaceVertices = spaceVertices;
                                        int TimeSpent = opTime;
                                        for (int k = PointInfo1.Value[0].timeWindow.lowerLimit; k < PointInfo1.Value[0].timeWindow.upperLimit + 1; k++)
                                        //oper
                                        {
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }
                                    else
                                    {
                                        if (chosenInDi_ID.individualType.ToString() == "visveh" &&
                                            !(coorPoint1.individualID.IDIndex == chosenInDi_ID.IDIndex &&
                                                 chosenInDi_ID.UpOrDown.ToString() == "Down"))
                                        //某个乘客的visDown仅服务这个乘客的visDown
                                        {
                                            continue;
                                        }
                                        Dictionary<ArcTime, Arc> arcDicTime = new Dictionary<ArcTime, Arc>();
                                        coorPoint2.individualID = "D" + coorPoint1.individualID.ToString();
                                        coorPoint2.individualID.O_or_D_or_M_none = "D";
                                        TimeWindow tWPoint1 = indiInfo_KV.Value[0].timeWindow;
                                        List<Point> spaceVertices = new List<Point>();
                                        spaceVertices.Add(coorPoint1);
                                        spaceVertices.Add(coorPoint2);
                                        ArcID arcID = chosenInDi_ID.ToString() + arcType.ToString() + coorPoint1.individualID.ToString();
                                        arcID.arcSpaceVertices = spaceVertices;
                                        int TimeSpent = opTime;
                                        for (int k = PointInfo1.Value[0].timeWindow.lowerLimit - opTime; k < PointInfo1.Value[0].timeWindow.upperLimit; k++)
                                        //oper
                                        {
                                            GeneratingTimeVertex(TimeSpent, individualType, chosenInDi_ID, arcType, arcID, spaceVertices, k, out Arc ArcThis, out ArcTime ArcTime, out SpaceTimeVertex FirstSpaceTimeVertex, out SpaceTimeVertex SecondSpaceTimeVertex, out SpaceTimeVertices SpaceTimeVertices);
                                            listArc.Add(ArcThis); listArcIndividual.Add(ArcThis);
                                            ListSpaceTimeVertices.Add(SpaceTimeVertices); ListFirstSpaceTimeVertex.Add(FirstSpaceTimeVertex); ListSecondSpaceTimeVertex.Add(SecondSpaceTimeVertex); ListSTV.Add(SpaceTimeVertices);
                                            arcDicTime.Add(ArcTime, ArcThis); ArcTypeComfirmedList.Add(ArcThis);
                                        }
                                        arcDicIDTime.Add(arcID, arcDicTime);
                                    }

                                }
                                arcDicTypeIDTime.Add(arcType, arcDicIDTime); arcDicArcType.Add(arcType, ArcTypeComfirmedList);
                            }
                            if (arcType.ToString() == typeTsOpDown.ToString())
                            {

                            }
                            if (arcType.ToString() == typeTsOpUp.ToString())
                            {

                            }
                        }
                        arcDicIndIDArctypeIDTime.Add(chosenInDi_ID, arcDicTypeIDTime); arcDicIndIDArctypeNew.Add(chosenInDi_ID, listArc);
                        var distinctItemsFirst = ListFirstSpaceTimeVertex.Distinct(new DistinctItemComparer());
                        ListFirstSpaceTimeVertex = distinctItemsFirst.ToList();
                        DicIDFirstSTV.Add(chosenInDi_ID, ListFirstSpaceTimeVertex);
                        var distinctItemsSecond = ListSecondSpaceTimeVertex.Distinct(new DistinctItemComparer());
                        ListSecondSpaceTimeVertex = distinctItemsSecond.ToList(); var distinctFSItems = ListSTV.Distinct(new DistinctItemComparerSTVs()); ListSTV = distinctFSItems.ToList(); DicIDSTV.Add(chosenInDi_ID, ListSTV);
                        DicIDSecondSTV.Add(chosenInDi_ID, ListSecondSpaceTimeVertex); arcDicIDArcType.Add(chosenInDi_ID, arcDicArcType);
                    }
                    arcDic.Add(individualType, arcDicIndIDArctypeIDTime);
                    arcDicIndiTypeID.Add(individualType, arcDicIndIDArctypeNew); arcDicIndividualType.Add(individualType, listArcIndividual);
                    DicFirstSpaceTimeVertex.Add(individualType, DicIDFirstSTV);
                    DicSecondSpaceTimeVertex.Add(individualType, DicIDSecondSTV); DicSpaceTimeVertices.Add(individualType, DicIDSTV); arcDicIndiTypeIDArcType.Add(individualType, arcDicIDArcType);
                }
            }
            var distinctItems = ListSpaceTimeVertices.Distinct(new DistinctItemComparerSTVs());
            ListSpaceTimeVertices = distinctItems.ToList();
        }
    }
}
