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
    internal class SubProblemGeneratorVeh
    {
        GRBModel myModel;
        Dictionary<IndividualID, List<Arc>> arcDic_ID_Veh;
        IndividualType individualType;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices;
        List<SpaceTimeVertices> ListSpaceTimeVertices;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic;
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc;
        List<IndividualType> individualTypeList;
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
        int capacity = 4;
        List<IndividualType> pasTypeList;
        Dictionary<IndividualType, Dictionary<IndividualID, string>> Dic_typeID_str_A_theta;
        Dictionary<IndividualID, List<ArcGRBVar>> VarY_bar_ID_Dic;
        List<IndividualID> pasList;
        Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta;
        Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic;
        RouteID vehRID;
        int ID_sequence_veh;
        List<BB_info> veh_BB_info_subpro;

        public SubProblemGeneratorVeh(GRBModel myModel,
            RouteID vehRID,Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta,
            Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic,
            Dictionary<IndividualID, List<ArcGRBVar>> VarY_bar_ID_Dic,
            IndividualType individualType,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            List<SpaceTimeVertices> ListSpaceTimeVertices,
            List<IndividualID> pasList, Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc,
            int ID_sequence_veh, List<BB_info> veh_BB_info_subpro)
        {
            this.myModel = myModel;
            this.u_dic_A_theta = u_dic_A_theta;
            this.u_Oth_dic = u_Oth_dic;
            this.individualType = individualType;
            this.VarY_bar_ID_Dic = VarY_bar_ID_Dic;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.ListSpaceTimeVertices = ListSpaceTimeVertices;
            this.arcDic = arcDic;
            this.pasList = pasList;
            this.DicTypeRID_Arc = DicTypeRID_Arc;
            individualTypeList = new List<IndividualType>();
            pasTypeList = new List<IndividualType>();
            this.vehRID = vehRID;
            this.ID_sequence_veh = ID_sequence_veh;
            this.veh_BB_info_subpro = veh_BB_info_subpro;
            pasUp = "pasUp";
            pasDown = "pasDown";
            veh = "veh";
            individualTypeList.Add(pasUp);
            individualTypeList.Add(pasDown);
            individualTypeList.Add(veh); 
            pasTypeList.Add(pasUp);
            pasTypeList.Add(pasDown);
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
            pointTsO.individualID = "TsO";pointTsO.individualID.O_or_D_or_M_none = "none";pointTsO.individualID.OutOrBack = "none";pointTsO.individualID.OutOrBack = "none";
            pointTsO.pointType = "TsO";
            pointTsD.individualID = "TsD";pointTsD.individualID.O_or_D_or_M_none = "none";pointTsD.individualID.OutOrBack = "none";
            pointTsD.pointType = "TsD";pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
        }

        public void Generating_Sub_Problem_Var_Obj(
            out GRBModel myModel_dy_out)
        {
            myModel_dy_out = this.myModel;

            GRBLinExpr expr_sub_veh = new GRBLinExpr();
            foreach (var ID_ListVar_KV in VarY_bar_ID_Dic)
            {
                foreach (ArcGRBVar arcGRBVar in ID_ListVar_KV.Value)
                {
                    if (arcGRBVar.arc.individualID.ToString().Contains("vis")
                         && arcGRBVar.arc.arcType.ToString() != typeStart.ToString()
                         && arcGRBVar.arc.arcType.ToString() != typeEnd.ToString())
                    {
                        expr_sub_veh += arcGRBVar.GRBV * 99999;//使用visveh需要付出很大代价

                    }
                    else
                    {
                        expr_sub_veh += arcGRBVar.GRBV * arcGRBVar.arc.value;
                    }
                }
            }

            GRBLinExpr sum_u_Y_A_theta = new GRBLinExpr();
            foreach (var ID_Y_KV in VarY_bar_ID_Dic)
            {
                List<ArcGRBVar> arcYList = ID_Y_KV.Value;
                foreach (IndividualID individualIDpas in pasList)
                {
                    if (individualIDpas.ToString().Contains("Up"))
                    {
                        GRBLinExpr sumY_up = new GRBLinExpr();
                        foreach (ArcGRBVar arcGRBVar in arcYList)
                        {
                            if (arcGRBVar.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == "O" + individualIDpas.ToString()&&
                                arcGRBVar.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == individualIDpas.ToString()&&
                                arcGRBVar.arc.arcType.ToString()=="oper")
                            {
                                sumY_up += arcGRBVar.GRBV;
                            }
                        }
                        sum_u_Y_A_theta += sumY_up * u_dic_A_theta[veh][individualIDpas];
                    }
                    else
                    {
                        GRBLinExpr sumY_down = new GRBLinExpr();
                        foreach (ArcGRBVar arcGRBVar in arcYList)
                        {
                            if (arcGRBVar.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == "D" + individualIDpas.ToString()&&
                                arcGRBVar.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == individualIDpas.ToString()&&
                                arcGRBVar.arc.arcType.ToString() == "oper")
                            {
                                sumY_down += arcGRBVar.GRBV;
                            }
                        }
                        sum_u_Y_A_theta += sumY_down * u_dic_A_theta[veh][individualIDpas];
                    }
                }
            }

            expr_sub_veh = expr_sub_veh - sum_u_Y_A_theta;

            MasterConstrType masterConstrType_w_al_theta_constr = "w_al_theta_constr";
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>> u_dic_w_al_theta_constr = u_Oth_dic[masterConstrType_w_al_theta_constr];
            MasterConstrType masterConstrType_constr_sum_bq_dq_x_dl_w_1_constr = "constr_sum_bq_dq_x_dl_w_1_constr";
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>> u_dic_constr_sum_bq_dq_x_dl_w_1_constr = u_Oth_dic[masterConstrType_constr_sum_bq_dq_x_dl_w_1_constr];
            GRBLinExpr w_Y_sum_r = new GRBLinExpr();
            foreach (var typeIDSTVRID_u_KV in u_dic_w_al_theta_constr)
            {
                IndividualType pasType = typeIDSTVRID_u_KV.Key;
                foreach (var IDSTVRID_u_KV in typeIDSTVRID_u_KV.Value)
                {
                    IndividualID IndiID = IDSTVRID_u_KV.Key;
                    GRBLinExpr w_multiple_Y = new GRBLinExpr();
                    double w_addedUp = new double();
                    foreach (var STVRID_u_KV in IDSTVRID_u_KV.Value)
                    {
                        if (!STVRID_u_KV.Value.ContainsKey(vehRID))
                        {
                            continue;
                        }
                        double w_a_theta_u_value = STVRID_u_KV.Value[vehRID];
                        double w_a_b_theta_Combined_u_value = u_dic_constr_sum_bq_dq_x_dl_w_1_constr[pasType][IndiID][STVRID_u_KV.Key][vehRID];
                        w_addedUp -= w_a_theta_u_value;
                        w_addedUp += w_a_b_theta_Combined_u_value;
                    }
                    GRBLinExpr Y_addedUp = new GRBLinExpr();
                    foreach (var ID_ListVar in VarY_bar_ID_Dic)
                    {
                        List<ArcGRBVar> ArcGRBVarList = ID_ListVar.Value;
                        foreach (ArcGRBVar varY in ArcGRBVarList)
                        {
                            if (pasType == pasUp)
                            {
                                if (varY.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == "O" + IndiID.ToString() &&
                                varY.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == IndiID.ToString() &&
                                varY.arc.arcType.ToString() == "oper")
                                {
                                    Y_addedUp += varY.GRBV;
                                }
                            }
                            else
                            {
                                if (varY.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == "D" + IndiID.ToString() &&
                                varY.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == IndiID.ToString() &&
                                varY.arc.arcType.ToString() == "oper")
                                {
                                    Y_addedUp += varY.GRBV;
                                }
                            }
                        }
                    }
                    w_multiple_Y = w_addedUp * Y_addedUp;
                    w_Y_sum_r += w_multiple_Y;
                }
            }
            expr_sub_veh += w_Y_sum_r;
            myModel.SetObjective(expr_sub_veh, GRB.MINIMIZE);
        }

        public GRBModel Generating_Sub_Constr(GRBModel myModel_dy_out)
        {
            List<GRBLinExpr> gRBLinExprs;
            List<GRBQuadExpr> gRBQuadExprs;
            gRBLinExprs = new List<GRBLinExpr>();
            gRBQuadExprs = new List<GRBQuadExpr>();
            ////Y constr
            Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndiTypeIDTime = arcDic[individualType];
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
            Dictionary<IndividualID, List<ArcGRBVar>> DicIDVarY = VarY_bar_ID_Dic;
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
                constr1Z_start += 1;
                constr1Z_end += 1;
                constr1FirstYSecondY_start = constr1FirstY_start - constr1SecondY_start;
                constr1FirstYSecondY_end = constr1FirstY_end - constr1SecondY_end;
                myModel.AddConstr(constr1FirstYSecondY_start - constr1Z_start, GRB.EQUAL, 0, "Constr1_flow_out_IndiID_" + individualID.ToString() + "_" + startPoint.individualID.ToString());
                gRBLinExprs.Add(constr1FirstYSecondY_start - constr1Z_start);
                myModel.AddConstr(constr1FirstYSecondY_end + constr1Z_end, GRB.EQUAL, 0, "Constr1_flow_back_IndiID_" + individualID.ToString() + "_" + endPoint.individualID.ToString());
                gRBLinExprs.Add(constr1FirstYSecondY_end + constr1Z_end);
            }

            return myModel_dy_out;

        }

        public void StartSubProOptimization_veh(out Dictionary<IndividualID, List<ArcGRBVar>> VarY_bar_ID_Dic)
        {
            VarY_bar_ID_Dic = this.VarY_bar_ID_Dic;
            Generating_Sub_Problem_Var_Obj(out GRBModel myModel_dy_out);
            myModel_dy_out = Generating_Sub_Constr(myModel_dy_out);
            myModel_dy_out.Optimize();
        }

        public void Generating_Route_Y_veh_arc_ID_Dic(out Dictionary<RouteID, List<Arc>> Route_Y_veh_arc_ID_Dic,
            out Dictionary<IndividualID, List<ArcGRBVar>> VarY_bar_ID_Dic,
            out int ID_sequence_veh_out,
            out IndividualID whichVeh)
        {
            StartSubProOptimization_veh(out VarY_bar_ID_Dic);
            Route_Y_veh_arc_ID_Dic = new Dictionary<RouteID, List<Arc>>();
            List<Arc> chosenArcList = new List<Arc>();
            List<RouteID> routeIDList = new List<RouteID>();
            ID_sequence_veh_out = ID_sequence_veh+ 1;
            whichVeh ="";
            foreach (var IDListVar_KV in VarY_bar_ID_Dic)
            {
                List<ArcGRBVar> arcGRBVarY_veh_List = IDListVar_KV.Value;
                foreach (ArcGRBVar arcGRBVar in arcGRBVarY_veh_List)
                {
                    if (arcGRBVar.GRBV.X == 1)
                    {
                        chosenArcList.Add(arcGRBVar.arc);
                        if(!(arcGRBVar.arc.spaceVertices[0].individualID.OutOrBack.ToString() == "Out"
                        && arcGRBVar.arc.spaceVertices[1].individualID.ToString() == "VD") &&
                        !(arcGRBVar.arc.spaceVertices[0].individualID.ToString() == "VO"
                        && arcGRBVar.arc.spaceVertices[1].individualID.OutOrBack.ToString() == "Out"))
                        {
                            whichVeh = arcGRBVar.arc.individualID;
                        }
                    }
                }
            }
            RouteID routeID = "R_veh_No_" + ID_sequence_veh_out.ToString() + "_served_by_" + whichVeh.ToString().Replace("vis", "vs").Replace("pas", "ps");
            Route_Y_veh_arc_ID_Dic.Add(routeID, chosenArcList);
        }
    }
}
