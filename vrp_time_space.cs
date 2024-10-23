
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using OptimizationIntegrationSystem.zhenghao;
using OptimizationIntegrationSystem.zhenghao.ClassName;
namespace OptimizationIntegrationSystem.zhenghao
{
    class vrp_time_space //这个类读取的是VRP文件
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
        int capacity;
        List<IndividualID> VehList;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex;
        List<SpaceTimeVertices> ListSpaceTimeVertices;
        List<IndividualID> pasList = new List<IndividualID>();
        Dictionary<IndividualID, int> indiID_TS_int;
        Dictionary<IndividualID, int> indiID_RS_int;
        public vrp_time_space(Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            int capacity,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex, 
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            List<SpaceTimeVertices> ListSpaceTimeVertices,
            Dictionary<IndividualID, int> indiID_TS_int,
            Dictionary<IndividualID, int> indiID_RS_int)
        {
            this.arcDic = arcDic;
            this.arcDicIndiTypeIDArcType = arcDicIndiTypeIDArcType;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.capacity = capacity;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.ListSpaceTimeVertices = ListSpaceTimeVertices;
            this.indiID_TS_int = indiID_TS_int;
            this.indiID_RS_int = indiID_RS_int;
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

            pasList.AddRange(arcDic[pasUp].Keys.ToList());
            pasList.AddRange(arcDic[pasDown].Keys.ToList());

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
            pointVO.individualID = "VO"; pointVO.individualID.O_or_D_or_M_none = "none";pointVO.individualID.OutOrBack = "none";
            pointVO.pointType = "VO";
            pointVD.individualID = "VD"; pointVD.individualID.O_or_D_or_M_none = "none";pointVD.individualID.OutOrBack = "none";
            pointVD.pointType = "VD";
            pointTsD.pointType = "TsD"; pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
        }
        //Generating_Vars
        //方案1：list；方案二：dictionary；方案三：建立类
        //原方案：GRBVar[,,,,]这是数组的意思

        public void Generating_Vars_Obj(GRBModel myModel,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>> VarY_TypeIDArc_Dic,
            out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
            out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
            out Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
            out Dictionary<IndividualID, Dictionary<IndividualID, IndexGRBVar>> VarZDic_pas_Z)
        {

            string FilePath = "file220501_12";
            string filepath_FixedRouteTransit_veh1 = @"" + FilePath + "\\" + "FixedRouteTransit_veh1.txt";
            string filepath_FixedRouteTransit_veh2 = @"" + FilePath + "\\" + "FixedRouteTransit_veh2.txt";
            string filepath_FixedRouteTransit_veh = @"" + FilePath + "\\" + "FixedRouteTransit_veh.txt";
            string[] corLines_veh1 = File.ReadAllLines(filepath_FixedRouteTransit_veh1);
            string[] corLines_veh2 = File.ReadAllLines(filepath_FixedRouteTransit_veh2);
            string[] corLines_veh = File.ReadAllLines(filepath_FixedRouteTransit_veh);
            GRBLinExpr objLineExpr = new GRBLinExpr();
            VarYDic = new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();
            VarY_TypeIDArc_Dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>>();
            ArcGRBVarY_ID_ijtts = new Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>>();
            ArcGRBVarYListijtts = new Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>>();
            foreach (var IndiTypeID_arc_KV in arcDicIndiTypeID)
            {
                IndividualType indiType = IndiTypeID_arc_KV.Key;
                Dictionary<IndividualID, List<Arc>> arcDic_ID = IndiTypeID_arc_KV.Value;
                Dictionary<IndividualID, List<ArcGRBVar>> VarDic_ID
                    = new Dictionary<IndividualID, List<ArcGRBVar>>();
                Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>> VarDic_IDtypeListY
                    = new Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>();
                Dictionary<IndividualID, int> CostID_Dic = new Dictionary<IndividualID, int>();
                foreach (var ID_Arc_KV in arcDic_ID)
                {
                    List<Arc> arcList = ID_Arc_KV.Value;
                    List<ArcGRBVar> arcGRBVarYList = new List<ArcGRBVar>();
                    Dictionary<ArcType, List<ArcGRBVar>> type_YList_Dic = new Dictionary<ArcType, List<ArcGRBVar>>();
                    IndividualID individualID = ID_Arc_KV.Key;
                    foreach (Arc arc in arcList)
                    {
                        GRBVar VarY = new GRBVar();
                        ArcGRBVar arcGRBVarY = new ArcGRBVar();
                        string t_str;
                        string tt_str;
                        if (arc.timeVertices[0] < 10)
                        {
                            t_str = "0" + arc.timeVertices[0].ToString();
                        }
                        else
                        {
                            t_str = arc.timeVertices[0].ToString();
                        }
                        if (arc.timeVertices[1] < 10)
                        {
                            tt_str = "0" + arc.timeVertices[1].ToString();
                        }
                        else
                        {
                            tt_str = arc.timeVertices[1].ToString();
                        }
                        string VarName = "Y_" + individualID.ToString() + "_t_" + t_str + "_tt_" + tt_str
                            + "_" + arc.arcType.ToString() + "_i_" + arc.spaceVertices[0].individualID.ToString() + "_j_" + arc.spaceVertices[1].individualID.ToString();
                        VarY = myModel.AddVar(0.0, 1.0, 0, GRB.BINARY, VarName);
                        arcGRBVarY.arc = arc;
                        arcGRBVarY.GRBV = VarY;
                        if (corLines_veh.Contains(VarName)&& arc.arcType.ToString()!=typeOper.ToString())
                        {
                            GRBLinExpr fixedRouteTransit = new GRBLinExpr();
                            fixedRouteTransit += VarY;
                                myModel.AddConstr(fixedRouteTransit, GRB.EQUAL, 1, "Constr_fixedRouteTransit_" + VarName.ToString());
                        }
                        if (!type_YList_Dic.ContainsKey(arc.arcType))
                        {
                            List<ArcGRBVar> arcGRBVarY_type_List = new List<ArcGRBVar>();
                            type_YList_Dic.Add(arc.arcType, arcGRBVarY_type_List);
                        }
                        type_YList_Dic[arc.arcType].Add(arcGRBVarY);

                        if (!ArcGRBVarY_ID_ijtts.ContainsKey(arc.spaceTimeVertices))
                        {
                            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> keyValuePairs2 = new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();
                            ArcGRBVarY_ID_ijtts.Add(arc.spaceTimeVertices, keyValuePairs2);
                            foreach (IndividualType type in individualTypeList)
                            {
                                Dictionary<IndividualID, List<ArcGRBVar>> keyValuePairs = new Dictionary<IndividualID, List<ArcGRBVar>>();
                                ArcGRBVarY_ID_ijtts[arc.spaceTimeVertices].Add(type, keyValuePairs);
                                foreach (var IndiID_dic_KV in arcDic[type])
                                {
                                    IndividualID indiID = IndiID_dic_KV.Key;
                                    List<ArcGRBVar> arcGRBVarYListijtt = new List<ArcGRBVar>();
                                    ArcGRBVarY_ID_ijtts[arc.spaceTimeVertices][type].Add(indiID, arcGRBVarYListijtt);
                                }
                            }
                        }
                        ArcGRBVarY_ID_ijtts[arc.spaceTimeVertices][indiType][individualID].Add(arcGRBVarY);

                        if (!ArcGRBVarYListijtts.ContainsKey(arc.spaceTimeVertices))
                        {
                            Dictionary<IndividualType, List<ArcGRBVar>> keyValuePairs2 = new Dictionary<IndividualType, List<ArcGRBVar>>();
                            ArcGRBVarYListijtts.Add(arc.spaceTimeVertices, keyValuePairs2);
                            foreach (IndividualType type in individualTypeList)
                            {
                                List<ArcGRBVar> arcGRBVarYListijtt = new List<ArcGRBVar>();
                                ArcGRBVarYListijtts[arc.spaceTimeVertices].Add(type, arcGRBVarYListijtt);
                            }
                        }
                        ArcGRBVarYListijtts[arc.spaceTimeVertices][indiType].Add(arcGRBVarY);
                        arcGRBVarYList.Add(arcGRBVarY);
                        if (individualID.individualType.ToString()=="visveh"
                             && arc.arcType.ToString() != typeStart.ToString()
                             && arc.arcType.ToString() != typeEnd.ToString())
                        {
                            objLineExpr += VarY * 99999;//使用visveh需要在visveh出站时付出很大代价

                        }
                        else
                        {
                            objLineExpr += VarY * arc.value;
                        }
                    }
                    VarDic_ID.Add(individualID, arcGRBVarYList);
                    VarDic_IDtypeListY.Add(individualID, type_YList_Dic);
                }
                VarYDic.Add(indiType, VarDic_ID);
                VarY_TypeIDArc_Dic.Add(indiType, VarDic_IDtypeListY);
            }


            VarZDic = new Dictionary<IndividualType, List<IndexGRBVar>>();
            VarZDic_pas_Z = new Dictionary<IndividualID, Dictionary<IndividualID, IndexGRBVar>>();
            Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicVeh = arcDic[veh];
            List<IndexGRBVar> VarZList = new List<IndexGRBVar>();

            foreach (var IndIDArctypeIDTime_KV in arcDic)
            {
                IndividualType individualTypePas = IndIDArctypeIDTime_KV.Key;
                if (!(individualTypePas == pasUp || individualTypePas == pasDown))
                { continue; }
                Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndIDArctypeIDTime = IndIDArctypeIDTime_KV.Value;

                foreach (var IDArcTypeIDTime_KV in arcDicIndIDArctypeIDTime)
                {
                    IndividualID individualIDPas = IDArcTypeIDTime_KV.Key;
                    GRBLinExpr SumOfIndiVehZ = new GRBLinExpr();
                    Dictionary<IndividualID, IndexGRBVar> VarZDic_ID_listVar = new Dictionary<IndividualID, IndexGRBVar>();
                    foreach (var IndIDArctypeIDTime2_KV in arcDicVeh)
                    {
                        IndividualID individualIDVeh = IndIDArctypeIDTime2_KV.Key;
                        GRBVar VarZveh = new GRBVar();
                        VarZveh = myModel.AddVar(0, 1, 0, GRB.CONTINUOUS,
                            "Z_"
                            + individualIDPas.ToString() + "_"
                            + individualIDVeh.ToString() + "_");
                        IndexGRBVar IndexGRBVarZ = new IndexGRBVar();
                        IndexGRBVarZ.GRBV = VarZveh;
                        IndexGRBVarZ.IndividualIDVeh = individualIDVeh;
                        IndexGRBVarZ.IndividualIDPas = individualIDPas;
                        IndexGRBVarZ.IndividualTypePas = individualTypePas;
                        IndexGRBVarZ.IndividualTypeVeh = veh;
                        VarZList.Add(IndexGRBVarZ);
                        VarZDic_ID_listVar.Add(individualIDVeh, IndexGRBVarZ);
                        SumOfIndiVehZ += IndexGRBVarZ.GRBV;
                    }
                    objLineExpr += (1 - SumOfIndiVehZ) * 99999;
                    VarZDic_pas_Z.Add(individualIDPas, VarZDic_ID_listVar);
                }
            }
            VarZDic.Add(veh, VarZList);
            myModel.SetObjective(objLineExpr, GRB.MINIMIZE);
        }

        public GRBModel Generating_Constr(
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic, GRBModel myModel,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>> VarY_TypeIDArc_Dic,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic
            )
        {
            List<GRBLinExpr> gRBLinExprs;
            List<GRBQuadExpr> gRBQuadExprs;
            gRBLinExprs = new List<GRBLinExpr>();
            gRBQuadExprs = new List<GRBQuadExpr>();
            ////Y constr
            ///

            //foreach (var vehID_Y_KV in VarYDic[veh])
            //{
            //    foreach (ArcGRBVar Y_veh in vehID_Y_KV.Value)
            //    {
            //        if (Y_veh.arc.arcType.ToString() == typeOper.ToString())
            //        {
            //            GRBLinExpr constrOper = new GRBLinExpr();
            //            constrOper += Y_veh.GRBV;
            //            if (Y_veh.arc.spaceTimeVertices.spaceTimeVertex2.spaceVertex.pointType.ToString() == "Up")
            //            {
            //                foreach (ArcGRBVar Y_PasUp in VarYDic[pasUp][Y_veh.arc.spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID])
            //                {
            //                    constrOper -= Y_PasUp.GRBV;
            //                }
            //            }
            //            if (Y_veh.arc.spaceTimeVertices.spaceTimeVertex1.spaceVertex.pointType.ToString() == "Down")
            //            {
            //                foreach (ArcGRBVar Y_PasUp in VarYDic[pasDown][Y_veh.arc.spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID])
            //                {
            //                    constrOper -= Y_PasUp.GRBV;
            //                }
            //            }
            //            myModel.AddConstr(constrOper, GRB.LESS_EQUAL, 0, "");
            //        }
            //    }
            //}




            foreach (var IndiTypeIDTime_KV in arcDic)
            {
                Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndiTypeIDTime = IndiTypeIDTime_KV.Value;
                IndividualType individualType = IndiTypeIDTime_KV.Key;
                Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDFirstSTV = DicFirstSpaceTimeVertex[individualType];
                Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDSecondSTV = DicSecondSpaceTimeVertex[individualType];
                Point startPoint = new Point();
                Point endPoint = new Point();
                if (individualType == pasUp)//应该是存储路径不一致
                {
                    startPoint = pointO;
                    endPoint = pointTsD;
                }
                else if (individualType == pasDown)
                {
                    startPoint = pointTsO;
                    endPoint = pointD;
                }
                else if (individualType == veh)
                {
                    startPoint = pointVO;
                    endPoint = pointVD;
                }
                Dictionary<IndividualID, List<ArcGRBVar>> DicIDVarY = VarYDic[individualType];
                foreach (var IDVarY_KV in DicIDVarY)
                {
                    IndividualID individualID = IDVarY_KV.Key;
                    List<ArcGRBVar> ArcGRBVarList = IDVarY_KV.Value;
                    List<ArcGRBVar> processedArcGRBVarList = new List<ArcGRBVar>();
                    List<SpaceTimeVertex> ListFirstSpaceTimeVertex = DicIDFirstSTV[individualID];
                    List<SpaceTimeVertex> ListSecondSpaceTimeVertex = DicIDSecondSTV[individualID];
                    List<SpaceTimeVertex> ListAllSpaceTimeVertex = ListFirstSpaceTimeVertex.Union(ListSecondSpaceTimeVertex).ToList<SpaceTimeVertex>();
                    GRBLinExpr constr1FirstY_start = new GRBLinExpr();
                    GRBLinExpr constr1SecondY_start = new GRBLinExpr();
                    GRBLinExpr constr1FirstYSecondY_start = new GRBLinExpr();
                    GRBLinExpr constr1Z_start = new GRBLinExpr();
                    GRBLinExpr constr1FirstY_end = new GRBLinExpr();
                    GRBLinExpr constr1SecondY_end = new GRBLinExpr();
                    GRBLinExpr constr1FirstYSecondY_end = new GRBLinExpr();
                    GRBLinExpr constr1Z_end = new GRBLinExpr();
                    foreach (SpaceTimeVertex spaceTimeVertex1 in ListAllSpaceTimeVertex)
                    {
                        //选定i,t
                        //起点和终点按理说如果不立即出发的话可以dwell，但是好像没给dwell的余地
                        Point point_I = spaceTimeVertex1.spaceVertex;
                        int arcTimeVertex_T = spaceTimeVertex1.timeVertex;
                        GRBLinExpr constr1FirstY = new GRBLinExpr();
                        GRBLinExpr constr1SecondY = new GRBLinExpr();
                        foreach (ArcGRBVar arcGRBVar2 in ArcGRBVarList)
                        {
                            Arc arc2 = arcGRBVar2.arc;
                            Point point_J_0 = arc2.spaceVertices[0];
                            if (point_J_0.individualID == point_I.individualID &&
                                arc2.timeVertices[0] == arcTimeVertex_T)
                            {
                                constr1FirstY += arcGRBVar2.GRBV;
                            }
                            Point point_J_1 = arc2.spaceVertices[1];
                            if (point_J_1.individualID == point_I.individualID &&
                                arc2.timeVertices[1] == arcTimeVertex_T)
                            {
                                constr1SecondY += arcGRBVar2.GRBV;
                            }
                        }
                        GRBLinExpr constr1FirstYSecondY = constr1FirstY - constr1SecondY;
                        if (point_I.pointType == startPoint.pointType)
                        {
                            constr1FirstY_start += constr1FirstY;
                            constr1SecondY_start += constr1SecondY;
                        }
                        else if (point_I.pointType == endPoint.pointType)
                        {
                            constr1FirstY_end += constr1FirstY;
                            constr1SecondY_end += constr1SecondY;
                        }
                        else
                        {
                            myModel.AddConstr(constr1FirstYSecondY, GRB.EQUAL, 0, "Constr1_flow_balance_IndiID_" + individualID.ToString() + "_" + point_I.individualID.ToString() + "_" + arcTimeVertex_T.ToString());
                            gRBLinExprs.Add(constr1FirstYSecondY);
                        }
                    }
                    if (individualID.ToString().Contains("pas"))
                    {
                        List<IndexGRBVar> indexGRBVarZList = VarZDic[veh].Where(
                            t => t.IndividualIDPas == individualID).ToList();
                        foreach (IndexGRBVar indexGRBVarZ in indexGRBVarZList)
                        {
                            constr1Z_start += indexGRBVarZ.GRBV;
                            constr1Z_end += indexGRBVarZ.GRBV;
                            //constr1Z_start += 1;
                            //constr1Z_end += 1;
                        }
                    }
                    else if (individualID.ToString().Contains("veh"))
                    {
                        //强制出车，不出的话就直接out and back
                        constr1Z_start += 1;
                        constr1Z_end += 1;
                    }
                    constr1FirstYSecondY_start = constr1FirstY_start - constr1SecondY_start;
                    constr1FirstYSecondY_end = constr1FirstY_end - constr1SecondY_end;
                    myModel.AddConstr(constr1FirstYSecondY_start - constr1Z_start, GRB.EQUAL, 0, "Constr1_flow_out_IndiID_" + individualID.ToString() + "_" + startPoint.individualID.ToString());
                    gRBLinExprs.Add(constr1FirstYSecondY_start - constr1Z_start);
                    myModel.AddConstr(constr1FirstYSecondY_end + constr1Z_end, GRB.EQUAL, 0, "Constr1_flow_back_IndiID_" + individualID.ToString() + "_" + endPoint.individualID.ToString());
                    gRBLinExprs.Add(constr1FirstYSecondY_end + constr1Z_end);
                }
            }

            //生成新的字典Z
            List<IndexGRBVar> GRBVarZVehList = VarZDic[veh];
            Dictionary<IndividualID, List<IndexGRBVar>> GRBVarZVehDic = new Dictionary<IndividualID, List<IndexGRBVar>>();
            List<IndexGRBVar> GRBVarZVehListProcessed = new List<IndexGRBVar>();
            foreach (IndexGRBVar IndexGRBVarZ1 in GRBVarZVehList)
            {
                bool exsitOrNot = GRBVarZVehListProcessed.Contains(IndexGRBVarZ1);
                if (exsitOrNot) { continue; }
                List<IndexGRBVar> GRBVarZList = new List<IndexGRBVar>();
                foreach (IndexGRBVar IndexGRBVarZ2 in GRBVarZVehList)
                {
                    if (IndexGRBVarZ2.IndividualIDVeh == IndexGRBVarZ1.IndividualIDVeh)
                    {
                        GRBVarZList.Add(IndexGRBVarZ2);
                        GRBVarZVehListProcessed.Add(IndexGRBVarZ2);
                    }
                }
                GRBVarZVehDic.Add(IndexGRBVarZ1.IndividualIDVeh, GRBVarZList);
            }
            //生成约束
            //Y-Z约束
            foreach (var ListArcVarYKV in ArcGRBVarYListijtts)
            {
                SpaceTimeVertices spaceTimeVertices = ListArcVarYKV.Key;
                SpaceTimeVertex sTV1 = spaceTimeVertices.spaceTimeVertex1;
                SpaceTimeVertex sTV2 = spaceTimeVertices.spaceTimeVertex2;
                if (sTV1.spaceVertex.individualID.ToString().Contains("veh") || sTV2.spaceVertex.individualID.ToString().Contains("veh"))
                {
                    continue;
                }
                Dictionary<IndividualType, List<ArcGRBVar>> DicTypeArcVarY = ListArcVarYKV.Value;
                if (spaceTimeVertices.spaceTimeVertex2.spaceVertex == pointD || spaceTimeVertices.spaceTimeVertex1.spaceVertex == pointO)
                {
                    //乘客自己走
                }
                else if (spaceTimeVertices.spaceTimeVertex2.spaceVertex == pointTsD || spaceTimeVertices.spaceTimeVertex1.spaceVertex == pointTsO)
                {

                }
                else
                {
                    foreach (var GRBVarZListDic in GRBVarZVehDic)
                    {
                        List<IndexGRBVar> GRBVarZList = GRBVarZListDic.Value;
                        IndividualID IndividualIDVeh = GRBVarZListDic.Key;
                        GRBQuadExpr constr3 = new GRBQuadExpr();
                        GRBQuadExpr constr3Left = new GRBQuadExpr();
                        GRBQuadExpr constr3Right = new GRBQuadExpr();
                        foreach (var Type_ListArcGRBVar in DicTypeArcVarY)
                        {
                            IndividualType individualType = Type_ListArcGRBVar.Key;
                            List<ArcGRBVar> ArcGRBVarYList = Type_ListArcGRBVar.Value;
                            if (individualType == pasUp || individualType == pasDown)
                            {
                                foreach (ArcGRBVar ArcGRBVarYPasUp in ArcGRBVarYList)
                                {
                                    GRBVar GRBVarY = ArcGRBVarYPasUp.GRBV;
                                    double RS_expr = new double();
                                    IndividualID individualIDPas = ArcGRBVarYPasUp.arc.individualID;
                                    GRBVar GRBVarZ = GRBVarZList.Find(t => t.IndividualIDPas == individualIDPas).GRBV;
                                    bool ExistOrNot = GRBVarZList.Exists(s => s.IndividualIDPas == individualIDPas);
                                    if (!ExistOrNot) { continue; }
                                    RS_expr = (1 - indiID_RS_int[individualIDPas.ToString().Replace("Down", "Up")]) * (capacity - 1) + 1;
                                    constr3Left += RS_expr * GRBVarY * GRBVarZ;
                                }
                            }
                            if (individualType == veh)
                            {
                                foreach (ArcGRBVar ArcGRBVarYVeh in ArcGRBVarYList)
                                {
                                    IndividualID individualIDVeh = ArcGRBVarYVeh.arc.individualID;
                                    if (individualIDVeh != IndividualIDVeh)
                                    {
                                        continue;
                                    }
                                    GRBVar GRBVarY = ArcGRBVarYVeh.GRBV;
                                    constr3Right += GRBVarY * capacity;
                                }
                            }
                        }
                        constr3 = constr3Left - constr3Right;
                        myModel.AddQConstr(-constr3, GRB.GREATER_EQUAL, 0, "Constr3_Z_Y_" + IndividualIDVeh.ToString() + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString());
                    }
                }
            }

            foreach (var ListArcVarYKV in ArcGRBVarYListijtts)
            {
                SpaceTimeVertices spaceTimeVertices = ListArcVarYKV.Key;
                Dictionary<IndividualType, List<ArcGRBVar>> DicTypeArcVarY = ListArcVarYKV.Value;
                foreach (var ListArcGRBVarKV in DicTypeArcVarY)
                {
                    List<ArcGRBVar> ListArcGRBVar = ListArcGRBVarKV.Value;
                    IndividualType individualType = ListArcGRBVarKV.Key;
                    if (!(individualType == veh)) { continue; }
                    GRBLinExpr constr4Capacity = new GRBLinExpr();
                    foreach (ArcGRBVar ArcGRBVarY in ListArcGRBVar)
                    {
                        constr4Capacity += ArcGRBVarY.GRBV;
                    }
                    if (ListArcGRBVar.Count == 0) { continue; }
                    myModel.AddConstr(-constr4Capacity + capacity, GRB.GREATER_EQUAL, 0, "Constr4_RoadCapacity_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString());
                }
            }

            //PasUp与PasDown的首尾相接约束
            Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicPasUpTypeIDTime = arcDicIndiTypeIDArcType[pasUp];
            Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicPasDownTypeIDTime = arcDicIndiTypeIDArcType[pasDown];

            Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDFirstSTVpasUp = DicFirstSpaceTimeVertex[pasUp];
            Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDSecondSTVpasDown = DicSecondSpaceTimeVertex[pasDown];

            foreach (var arcDicTypeIDTimeKV in arcDicPasUpTypeIDTime)
            {
                IndividualID individualIDUp = arcDicTypeIDTimeKV.Key;
                //选定individual
                List<ArcGRBVar> VarYListpasUpindividualID = VarYDic[pasUp][individualIDUp];
                //选定某个p对应的P+ varList
                List<Arc> arcListPasUpEndIDTime = arcDicTypeIDTimeKV.Value[typeEnd];
                //选定某个p对应的p+的所有end arc
                IndividualID individualIDDown = individualIDUp.ToString().Replace("Up", "Down");
                List<ArcGRBVar> VarYListpasDownindividualID = VarYDic[pasDown][individualIDDown];
                // 选定某个p对应的P- varList
                List<Arc> arcListPasDownStartIDTime = arcDicPasDownTypeIDTime[individualIDDown][typeStart];
                //选定某个p对应的p-的所有start arc

                //针对每个t，每个i都有一个

                List<SpaceTimeVertex> ListFirstSpaceTimeVertex = DicIDFirstSTVpasUp[individualIDUp];
                List<SpaceTimeVertex> ListSecondSpaceTimeVertex = DicIDSecondSTVpasDown[individualIDDown];
                List<SpaceTimeVertex> ListAllSpaceTimeVertex = ListFirstSpaceTimeVertex.Union(ListSecondSpaceTimeVertex).ToList<SpaceTimeVertex>();
                foreach (SpaceTimeVertex spaceTimeVertex in ListAllSpaceTimeVertex)
                {
                    GRBLinExpr constr5Flow = new GRBLinExpr();
                    GRBLinExpr constr5First = new GRBLinExpr();
                    GRBLinExpr constr5Second = new GRBLinExpr();

                    List<ArcGRBVar> ArcGRBVarYPasUpList = VarY_TypeIDArc_Dic[pasUp][individualIDUp][typeEnd].Where(
                        t => t.arc.arcID.arcSpaceVertices[0].individualID.ToString() == spaceTimeVertex.spaceVertex.individualID.ToString()
                        && t.arc.timeVertices[1].ToString().Equals(spaceTimeVertex.timeVertex.ToString())
                        ).ToList();//不知道为什么就是需要这样子写
                    List<ArcGRBVar> ArcGRBVarYPasDownList = VarY_TypeIDArc_Dic[pasDown][individualIDDown][typeStart].Where(
                        t => t.arc.arcID.arcSpaceVertices[1].individualID.ToString() == spaceTimeVertex.spaceVertex.individualID.ToString() &&
                        t.arc.timeVertices[0].ToString().Equals(spaceTimeVertex.timeVertex.ToString())
                        ).ToList();//不知道为什么就是需要这样子写

                    if (ArcGRBVarYPasUpList.Count == 0 && ArcGRBVarYPasDownList.Count == 0) { continue; }
                    foreach (ArcGRBVar ArcGRBVarUp in ArcGRBVarYPasUpList)
                    {
                        constr5First += ArcGRBVarUp.GRBV;
                    }

                    foreach (ArcGRBVar ArcGRBVarDown in ArcGRBVarYPasDownList)
                    {
                        constr5Second += ArcGRBVarDown.GRBV;
                    }
                    constr5Flow = constr5First - constr5Second;
                    myModel.AddConstr(constr5Flow, GRB.EQUAL, 0, "Constr5_Flow_" + individualIDUp.ToString() + "_j_" + spaceTimeVertex.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertex.timeVertex.ToString());
                }

                List<ArcGRBVar> ArcGRBVarYPasUp_startFromPasUp_List = VarY_TypeIDArc_Dic[pasUp][individualIDUp][typeEnd].Where(
                        t => t.arc.arcID.arcSpaceVertices[0].individualID.IDIndex != individualIDUp.IDIndex
                        || t.arc.arcID.arcSpaceVertices[0].individualID.UpOrDown.ToString()!="Up"
                        || t.arc.arcID.arcSpaceVertices[0].individualID.O_or_D_or_M_none.ToString()!="none").ToList();
                GRBLinExpr constr_ts_preference = new GRBLinExpr();
                GRBLinExpr pasUp_startNotFromPasUp = new GRBLinExpr();
                foreach (ArcGRBVar arcGRBVar in ArcGRBVarYPasUp_startFromPasUp_List)
                {
                    pasUp_startNotFromPasUp += arcGRBVar.GRBV;
                }
                constr_ts_preference = pasUp_startNotFromPasUp * (1- indiID_TS_int[individualIDUp]);
                myModel.AddConstr(constr_ts_preference, GRB.EQUAL, 0, "Constr_TS_preference" + individualIDUp.ToString());
            }

            foreach (var arcDicIndiTypeIDTimeKV in arcDic)
            {
                Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndiTypeIDTime = arcDicIndiTypeIDTimeKV.Value;
                IndividualType individualTypePas = arcDicIndiTypeIDTimeKV.Key;
                if (!(individualTypePas == pasUp || individualTypePas == pasDown))
                {
                    continue;
                }
                foreach (var arcDicIDTimeKV in arcDicIndiTypeIDTime)
                {
                    IndividualID individualIDPas = arcDicIDTimeKV.Key;
                    GRBLinExpr constr6_Z = new GRBLinExpr();
                    foreach (IndividualID individualIDVeh in VehList)
                    {
                        IndexGRBVar indexGRBVarZ = VarZDic[veh].Find(
                            t => t.IndividualIDPas == individualIDPas && t.IndividualIDVeh == individualIDVeh);
                        constr6_Z += indexGRBVarZ.GRBV;
                    }
                    myModel.AddConstr(-constr6_Z, GRB.GREATER_EQUAL, -1, "Constr6_Z_veh_" + individualIDPas.ToString());
                }
            }



            foreach (IndividualID pasID in pasList)
            {
                foreach (var vehID_Y_KV in VarY_TypeIDArc_Dic[veh])
                {
                    GRBLinExpr constrOper = new GRBLinExpr();
                    foreach (ArcGRBVar Y_veh in vehID_Y_KV.Value[typeOper])
                    {
                        if (Y_veh.arc.SpaceTimeVertex1.spaceVertex.individualID == pasID ||  
                            Y_veh.arc.SpaceTimeVertex2.spaceVertex.individualID == pasID)
                        {
                            constrOper += Y_veh.GRBV;
                        }
                    }
                    List<IndexGRBVar> indexGRBVarZList = VarZDic[veh].Where(t => t.IndividualIDPas == pasID &&
                        t.IndividualIDVeh == vehID_Y_KV.Key).ToList();
                    foreach (IndexGRBVar indexGRBVarZ in indexGRBVarZList)
                    {
                        constrOper -= indexGRBVarZ.GRBV;
                    }
                    myModel.AddConstr(constrOper, GRB.GREATER_EQUAL, 0, "constrOper_" + pasID + "_" + vehID_Y_KV.Key);
                }
            }

            

            return myModel;
        }
        //求解类
        public GRBModel SetMyModel()
        {
            GRBEnv myEnv = new GRBEnv();//构建环境
            GRBModel myModel = new GRBModel(myEnv);
            return myModel;
        }
        public void StartStaticModelOptimization(
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>> VarY_TypeIDArc_Dic,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
            GRBModel myModel,
            out List<IndividualID> pasList)
        {
            pasList = this.pasList;
            Generating_Constr(arcDic, myModel, VarYDic, VarY_TypeIDArc_Dic, ArcGRBVarY_ID_ijtts, ArcGRBVarYListijtts, VarZDic);
            myModel.Set(GRB.IntParam.NonConvex, 2);
            myModel.Optimize();
            //var b = myModel.GetVars();

            //foreach(GRBVar variable in b )
            //{
            //    Console.WriteLine(variable.X);
            //}

            //var c = myModel.GetConstrs();

            //foreach (GRBConstr constr in c )
            //{
            //    try
            //    {
            //        Console.WriteLine(constr.Pi);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}
            //int firstLayerofVarYDic = VarYDic.Count;
            //for (int i = 0; i < firstLayerofVarYDic; i++)
            //{
            //    IndividualType key1 = VarYDic.Keys.ToList()[i];
            //    for (int j = 0; j < VarYDic[key1].Count; j++)
            //    {
            //        IndividualID key2 = VarYDic[key1].Keys.ToList()[j];
            //        for (int u_ = 0; u_ < VarYDic[key1][key2].Count; u_++)
            //        {
            //            if (VarYDic[key1][key2][u_].GRBV.VarName == "Y_pasUp1_travel_i_pasUp2_j_pasDown1_t_8_tt_13")
            //            {
            //                Console.WriteLine(VarYDic[key1][key2][u_].GRBV.VarName);
            //            }
            //        }
            //    }
            //}
        }
    }
}