using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using OptimizationIntegrationSystem.zhenghao;
using OptimizationIntegrationSystem.zhenghao.ClassName;

namespace OptimizationIntegrationSystem.zhenghao
{
    class OrderManager
    {
        List<IndividualType> individualTypeList;
        List<IndividualType> individualTypeListPas;
        List<ArcType> arcTypesList;
        IndividualType pasUp;
        IndividualType pasDown;
        IndividualType veh;IndividualType visveh;
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
        Dictionary<IndividualID, List<PointTW>> coordTWDicPas_dy;
        GRBModel myModel_sta;
        GRBModel myModel_dy;
        int DcT;
        int ImT;
        int pasPointNum_dy;
        int pasPointNum_dy_new;        
        Dictionary<IndividualType, List<IndexGRBVar>> VarZDic;
        Dictionary<IndividualType, List<IndexGRBVar>> VarZDic_dy;
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic;
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic_dy;
        int capacity;
        List<IndividualID> VehList;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic_dy;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        public OrderManager(
            Dictionary<IndividualID, List<PointTW>> coordTWDicPas_dy,
            GRBModel myModel_sta, GRBModel myModel_dy, int DcT, int ImT,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic_dy,///0808 比起上一个版本，我将VarZDic_dy也输了进来
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic_dy,///0808 比起上一个版本，我将VarYDic_dy也输了进来
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic_dy,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID, int pasPointNum_dy, int pasPointNum_dy_new)
        {
            this.coordTWDicPas_dy = coordTWDicPas_dy;
            this.arcDic = arcDic;
            this.arcDic_dy = arcDic_dy;
            this.arcDicIndiTypeIDArcType = arcDicIndiTypeIDArcType;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.pasPointNum_dy = pasPointNum_dy;
            this.pasPointNum_dy_new = pasPointNum_dy_new;
            this.VarYDic = VarYDic;
            this.VarYDic_dy = VarYDic_dy;
            this.VarZDic_dy = VarZDic_dy;
            this.VarZDic = VarZDic;
            this.DcT = DcT;
            this.ImT = ImT;
            this.myModel_sta = myModel_sta;
            this.myModel_dy = myModel_dy;
            individualTypeList = new List<IndividualType>();
            individualTypeListPas = new List<IndividualType>();
            individualTypeList = arcDic.Keys.ToList();

            pasUp = individualTypeList[0];
            pasDown = individualTypeList[1];
            veh = individualTypeList[2];
            individualTypeListPas.Add(pasUp);
            individualTypeListPas.Add(pasDown);

            VehList = new List<IndividualID>();
            VehList = arcDic[veh].Keys.ToList();

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
            pointTsO.individualID = "TsO";pointTsO.individualID.O_or_D_or_M_none = "none";pointTsO.individualID.OutOrBack = "none";pointTsO.individualID.OutOrBack = "none";
            pointTsO.pointType = "TsO";
            pointTsD.individualID = "TsD";pointTsD.individualID.O_or_D_or_M_none = "none";pointTsD.individualID.OutOrBack = "none";
            pointTsD.pointType = "TsD";pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
            pointVO.individualID = "VO"; pointVO.individualID.O_or_D_or_M_none = "none";pointVO.individualID.OutOrBack = "none";
            pointVO.pointType = "VO";
            pointVD.individualID = "VD"; pointVD.individualID.O_or_D_or_M_none = "none";pointVD.individualID.OutOrBack = "none";
            pointVD.pointType = "VD";
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


        public GRBModel Generating_Ordr_Assgnmnt_Constr()
        {
            GRBModel myModel_dy_out = this.myModel_dy;
            for (int pasIndex = 0; pasIndex < coordTWDicPas_dy.Count - pasPointNum_dy; pasIndex++)
            {
                IndividualID individualIDPas = coordTWDicPas_dy.Keys.ToList()[pasIndex + pasPointNum_dy];
                PointTW PointTWpas = coordTWDicPas_dy[individualIDPas][0];
                Point Pointpas = coordTWDicPas_dy[individualIDPas][0].point;
                if (individualIDPas.individualType == pasUp)
                {
                    int tGap = 3;//小于这个范围则难以服务
                    int rho1 = 10;
                    int rho2 = 10;
                    for (int n = 0; n < 10; n++)
                    {
                        int a = PointTWpas.timeWindow.upperLimit-ImT;
                        int b = n * tGap;
                        int c=(n+1) * tGap;
                        if (n == 0 && PointTWpas.timeWindow.upperLimit - ImT<0)
                            //如果时间窗上限（最晚接乘客的时间）比下一次implement时间还早，则无法服务
                        {
                            GRBLinExpr constr_CantServe = new GRBLinExpr();
                            List<IndexGRBVar> ListIndexGRBVar = VarZDic_dy[veh].Where(t =>
                            t.IndividualIDPas == individualIDPas).ToList();
                            foreach (IndexGRBVar IndexGRBVarZ in ListIndexGRBVar)
                            {
                                constr_CantServe += IndexGRBVarZ.GRBV;
                            }
                            myModel_dy_out.AddConstr(constr_CantServe, GRB.EQUAL, 0, "Constr_CantServe" + individualIDPas.ToString());
                        }
                        else if (PointTWpas.timeWindow.upperLimit-ImT >= n * tGap && PointTWpas.timeWindow.upperLimit-ImT < (n+1) * tGap)
                        {
                            if (n == 0)//如果时间窗上限（最晚接乘客的时间）=implement时间，则无法服务
                            {
                                GRBLinExpr constr_CantServe = new GRBLinExpr();
                                List<IndexGRBVar> ListIndexGRBVar = VarZDic_dy[veh].Where(t =>
                                t.IndividualIDPas == individualIDPas).ToList();
                                foreach (IndexGRBVar IndexGRBVarZ in ListIndexGRBVar)
                                {
                                    constr_CantServe += IndexGRBVarZ.GRBV;
                                }
                                myModel_dy_out.AddConstr(constr_CantServe, GRB.EQUAL, 0, "Constr_CantServe" + individualIDPas.ToString());
                            }
                            else//如果时间窗上限（最晚接乘客的时间）与implement时间的差距不大，则……
                            {
                                List<Point> PointInCircle = new List<Point>();
                                for (int pointIndex = 0; pointIndex < coordTWDicPas_dy.Count; pointIndex++)
                                {
                                    IndividualID pointIDPas = coordTWDicPas_dy.Keys.ToList()[pointIndex];
                                    Point PointTWpoint = coordTWDicPas_dy[pointIDPas][0].point;
                                    GeneratingTravellingCost(Pointpas, PointTWpoint, out List<Point> spaceVertices, out double value, out int distance, out int TimeSpent);
                                    if (distance < n * tGap * rho1)
                                    {
                                        PointInCircle.Add(PointTWpoint);
                                        Point PointTWpoint_ = PointTWpoint;
                                        if (PointTWpoint_.individualID.ToString().Contains("Up"))
                                        {
                                            PointTWpoint_.individualID = "O" + PointTWpoint_.individualID.ToString();
                                        }
                                        else if (PointTWpoint_.individualID.ToString().Contains("Down"))
                                        {
                                            PointTWpoint_.individualID = "D" + PointTWpoint_.individualID.ToString();
                                        }
                                        PointInCircle.Add(PointTWpoint_);
                                    }
                                }
                                foreach (var IDY_KV in VarYDic[veh])
                                {
                                    IndividualID IndividualIDveh = IDY_KV.Key;
                                    List<ArcGRBVar> ListArcGRBVarY_veh = IDY_KV.Value;
                                    int PassedOrNot = 0;
                                    foreach (ArcGRBVar ArcGRBVarY_veh in ListArcGRBVarY_veh)
                                    {
                                        if (ArcGRBVarY_veh.GRBV.X == 0)
                                        {
                                            continue;
                                        }
                                        if (PointTWpas.timeWindow.lowerLimit - n * tGap * rho2 <= ArcGRBVarY_veh.arc.SpaceTimeVertex2.timeVertex
                                            && PointTWpas.timeWindow.upperLimit + n * tGap * rho2 >= ArcGRBVarY_veh.arc.SpaceTimeVertex2.timeVertex)
                                        {
                                            List<Point> IsTherePointsInCircle = PointInCircle.Where(t =>
                                            t.individualID.ToString() == ArcGRBVarY_veh.arc.spaceVertices[1].individualID.ToString()).ToList();

                                            if (IsTherePointsInCircle.Count>0)
                                            {
                                                PassedOrNot += 1;
                                                break;
                                            }
                                        }

                                    }
                                    if (PassedOrNot==0)
                                    {
                                        GRBLinExpr constr_veh_CantServe = new GRBLinExpr();
                                        GRBVar Zvehpas = VarZDic_dy[veh].Where(t =>
                                        t.IndividualIDPas == individualIDPas &&
                                        t.IndividualIDVeh == IndividualIDveh).ToList()[0].GRBV;
                                        constr_veh_CantServe += Zvehpas;
                                        myModel_dy_out.AddConstr(constr_veh_CantServe, GRB.EQUAL, 0, "Constr_veh_CantServe_" + IndividualIDveh.ToString() + "_" + individualIDPas.ToString());

                                    }

                                }
                            }
                        }
                    }
                }
                else if (individualIDPas.individualType == pasDown)
                {

                }
            }

            return myModel_dy_out;
        }

    }
}
