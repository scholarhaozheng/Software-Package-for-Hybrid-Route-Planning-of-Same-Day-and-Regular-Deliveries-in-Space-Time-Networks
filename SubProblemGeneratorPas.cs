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
    internal class SubProblemGeneratorPas
    {
        GRBModel myModel;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices;
        List<SpaceTimeVertices> ListSpaceTimeVertices;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic;
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_Type_ID_pas;
        Dictionary<IndividualID, List<ArcGRBVar>> VarDic_ID_veh;
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts;
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts;
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
        List<IndividualID> pasList;
        Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta;
        Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType;
        int ID_sequence_pas;
        List<BB_info> pas_BB_info_subpro;
        public SubProblemGeneratorPas(GRBModel myModel, Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta,
            Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_Type_ID_pas,
            Dictionary<IndividualID, List<ArcGRBVar>> VarDic_ID_veh,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
            List<SpaceTimeVertices> ListSpaceTimeVertices,
            List<IndividualID> pasList, Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
            int ID_sequence_pas, List<BB_info> pas_BB_info_subpro)
        {
            this.myModel = myModel;
            this.u_dic_A_theta = u_dic_A_theta;
            this.u_Oth_dic = u_Oth_dic;
            this.VarDic_Type_ID_pas = VarDic_Type_ID_pas;
            this.VarDic_ID_veh = VarDic_ID_veh;
            this.ArcGRBVarY_ID_ijtts = ArcGRBVarY_ID_ijtts;
            this.ArcGRBVarYListijtts = ArcGRBVarYListijtts;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.DicSpaceTimeVertices = DicSpaceTimeVertices;
            this.ListSpaceTimeVertices = ListSpaceTimeVertices;
            this.arcDic = arcDic;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.pasList = pasList;
            this.arcDicIndiTypeIDArcType = arcDicIndiTypeIDArcType;
            this.ID_sequence_pas = ID_sequence_pas;
            individualTypeList = new List<IndividualType>();
            pasTypeList = new List<IndividualType>();
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
            pointTsO.individualID = "TsO"; pointTsO.individualID.O_or_D_or_M_none = "none";pointTsO.individualID.OutOrBack = "none";
            pointTsO.pointType = "TsO";
            pointTsD.individualID = "TsD";pointTsD.individualID.O_or_D_or_M_none = "none";pointTsD.individualID.OutOrBack = "none";
            pointVO.individualID = "VO"; pointVO.individualID.O_or_D_or_M_none = "none";pointVO.individualID.OutOrBack = "none";
            pointVO.pointType = "VO";
            pointVD.individualID = "VD"; pointVD.individualID.O_or_D_or_M_none = "none";pointVD.individualID.OutOrBack = "none";
            pointVD.pointType = "VD";
            pointTsD.pointType = "TsD"; pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
            this.pas_BB_info_subpro = pas_BB_info_subpro;
        }

        public void Generating_Sub_Problem_Var_Obj(out GRBModel myModel_dy_out)
        {
            myModel_dy_out = this.myModel;
            GRBQuadExpr expr_sub_pas = new GRBQuadExpr();
            MasterConstrType masterConstrType_w_al_theta_constr = "w_al_theta_constr";
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>> u_dic_w_a_theta_pas = u_Oth_dic[masterConstrType_w_al_theta_constr];
            MasterConstrType masterConstrType_constr_sum_bq_dq_x_dl_w_1_constr = "constr_sum_bq_dq_x_dl_w_1_constr";
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>> u_dic_w_a_b_theta_pas = u_Oth_dic[masterConstrType_constr_sum_bq_dq_x_dl_w_1_constr];
            foreach (var typeRID_ListVar_KV in VarDic_Type_ID_pas)
            {
                GRBQuadExpr expr_sub_pas_half = new GRBQuadExpr();
                IndividualType pasType = typeRID_ListVar_KV.Key;
                if (pasType==veh)
                {
                    continue;
                }
                foreach (var ID_Y_KV in typeRID_ListVar_KV.Value)
                {
                    List<ArcGRBVar> arcYList = ID_Y_KV.Value;
                    foreach (ArcGRBVar arcGRBVar in ID_Y_KV.Value)
                    {
                        expr_sub_pas_half += arcGRBVar.GRBV * arcGRBVar.arc.value;
                    }
                    GRBLinExpr sum_u_Y_A_theta = new GRBLinExpr();
                    foreach (IndividualID individualIDpas in pasList)
                    {
                        if (individualIDpas.ToString().Contains(typeRID_ListVar_KV.Key.ToString()))
                        {
                            GRBLinExpr sumY_pas = new GRBLinExpr();
                            foreach (ArcGRBVar arcGRBVar in arcYList)
                            {
                                if (pasType == pasUp)
                                {
                                    if (arcGRBVar.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == "O" + individualIDpas.ToString() &&
                                        arcGRBVar.arc.SpaceTimeVertex1.spaceVertex.pointType.ToString() == pointO.pointType.ToString())
                                    {
                                        sumY_pas += arcGRBVar.GRBV;
                                    }
                                }
                                else
                                {
                                    if (arcGRBVar.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == "D" + individualIDpas.ToString() &&
                                        arcGRBVar.arc.SpaceTimeVertex2.spaceVertex.pointType.ToString() == pointD.pointType.ToString())
                                    {
                                        sumY_pas += arcGRBVar.GRBV;
                                    }
                                }
                            }
                            sum_u_Y_A_theta += sumY_pas * u_dic_A_theta[typeRID_ListVar_KV.Key][individualIDpas];
                        }
                    }
                    expr_sub_pas_half = expr_sub_pas_half - sum_u_Y_A_theta;

                    GRBQuadExpr w_Y_sum_r = new GRBQuadExpr();

                    foreach (var IDSTVRID_u_KV in u_dic_w_a_theta_pas[pasType])
                    {
                        IndividualID IndiID = IDSTVRID_u_KV.Key;
                        GRBQuadExpr w_y_multiple_Y = new GRBQuadExpr();
                        double w_addedUp = new double();
                        GRBLinExpr w_addedUp_Y = new GRBLinExpr();
                        foreach (var STVRID_u_KV in IDSTVRID_u_KV.Value)
                        {
                            foreach (var RID_u_KV in STVRID_u_KV.Value)
                            {
                                RouteID RID_veh = RID_u_KV.Key;
                                double w_a_theta_u_value = STVRID_u_KV.Value[RID_veh];
                                double w_a_b_theta_Combined_u_value = u_dic_w_a_b_theta_pas[pasType][IndiID][STVRID_u_KV.Key][RID_veh];
                                w_addedUp -= w_a_theta_u_value;
                                w_addedUp += w_a_b_theta_Combined_u_value;
                            }
                            GRBLinExpr y_determinedSTV=new GRBLinExpr();
                            int y_determinedSTV_num = 0;
                            foreach (var ID_ListVar in VarDic_Type_ID_pas[pasType])
                            {
                                ArcGRBVar y_determinedSTV_ = VarDic_Type_ID_pas[pasType][ID_ListVar.Key].Find(x => x.arc.spaceTimeVertices == STVRID_u_KV.Key);
                                if (y_determinedSTV_ == null)
                                {
                                    y_determinedSTV = 0;
                                }
                                else
                                {
                                    y_determinedSTV += y_determinedSTV_.GRBV;
                                    y_determinedSTV_num = y_determinedSTV_num + 1;
                                }
                            }
                            w_addedUp_Y = w_addedUp * y_determinedSTV;
                            if (w_addedUp!=0)
                            {

                            }
                        }
                        GRBLinExpr Y_addedUp = new GRBLinExpr();
                        int Y_addedUp_num = 0;
                        foreach (var TypeID_Y_KV in VarDic_Type_ID_pas)
                        {
                            foreach (var ID_Y_out_KV in TypeID_Y_KV.Value)
                            {
                                List<ArcGRBVar> ArcGRBVarList = ID_Y_out_KV.Value;
                                foreach (ArcGRBVar varY in ArcGRBVarList)
                                {
                                    if (pasType==pasUp)
                                    {
                                        if (varY.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == "O"+ IndiID.ToString() &&
                                            varY.arc.SpaceTimeVertex1.spaceVertex.pointType.ToString() == pointO.pointType.ToString())
                                        {
                                            Y_addedUp += varY.GRBV;
                                            Y_addedUp_num += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (varY.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == "D" + IndiID.ToString() &&
                                            varY.arc.SpaceTimeVertex2.spaceVertex.pointType.ToString() == pointD.pointType.ToString())
                                        {
                                            Y_addedUp += varY.GRBV;
                                            Y_addedUp_num += 1;
                                        }
                                    }
                                }
                            }
                        }
                        w_y_multiple_Y = w_addedUp_Y * Y_addedUp;
                        //myModel_dy_out.AddQConstr(w_y_multiple_Y, GRB.LESS_EQUAL, 1000000, "SEE_whereIsQuad" + IndiID.ToString());
                        w_Y_sum_r += w_y_multiple_Y;
                    }
                    expr_sub_pas_half += w_Y_sum_r;
                }
                expr_sub_pas+= expr_sub_pas_half;
            }
            myModel_dy_out.SetObjective(expr_sub_pas, GRB.MINIMIZE);
        }

        public GRBModel Generating_Sub_Constr(GRBModel myModel_dy_out, 
            out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_veh_pas)
        {
            List<GRBLinExpr> gRBLinExprs;
            List<GRBQuadExpr> gRBQuadExprs;
            gRBLinExprs = new List<GRBLinExpr>();
            gRBQuadExprs = new List<GRBQuadExpr>();
            ////Y constr
            Dictionary<IndividualID,Dictionary<int, GRBLinExpr>> DicIDPoint_expr = new Dictionary<IndividualID, Dictionary<int, GRBLinExpr>>();
            GRBLinExpr constr_pas_added_out_2 = new GRBLinExpr();
            GRBLinExpr constr_pas_added_back_2 = new GRBLinExpr();
            foreach (var TypeIDpasY_KV in VarDic_Type_ID_pas)
            {
                IndividualType individualType = TypeIDpasY_KV.Key;
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
                List<Point> startEndPoint = new List<Point>();
                startEndPoint.Add(startPoint);
                startEndPoint.Add(endPoint);
                Dictionary<IndividualID, List<ArcGRBVar>> DicIDVarY = VarDic_Type_ID_pas[individualType];
                foreach (var ID_ListVarY_KV in DicIDVarY)
                {
                    Dictionary<int, GRBLinExpr> DicPoint_expr = new Dictionary<int, GRBLinExpr>();
                    IndividualID individualID = ID_ListVarY_KV.Key;
                    List<ArcGRBVar> ArcGRBVarList = ID_ListVarY_KV.Value;
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
                            myModel_dy_out.AddConstr(constr1FirstYSecondY, GRB.EQUAL, 0, "Constr_Y_bar_flow_balance_IndiID_" + individualID.ToString() + "_" + point_I.individualID.ToString() + "_" + arcTimeVertex_T.ToString());
                            gRBLinExprs.Add(constr1FirstYSecondY);
                        }
                    }
                    constr1FirstYSecondY_start = constr1FirstY_start - constr1SecondY_start;
                    constr1FirstYSecondY_end = constr1FirstY_end - constr1SecondY_end;
                    constr_pas_added_out_2 += constr1FirstYSecondY_start;
                    constr_pas_added_back_2 += constr1FirstYSecondY_end;
                    //起始点为1 终止点为0
                    if (individualType == pasUp)
                    {
                        DicPoint_expr.Add(1, constr1FirstYSecondY_start);
                        DicPoint_expr.Add(0, constr1FirstYSecondY_end);
                    }
                    else
                    {
                        DicIDPoint_expr[individualID.ToString().Replace("Down", "Up")][1] -= constr1FirstYSecondY_start;
                        DicIDPoint_expr[individualID.ToString().Replace("Down", "Up")][0] -= constr1FirstYSecondY_end;
                    }
                    if (individualType == pasUp)
                    {
                        DicIDPoint_expr.Add(individualID, DicPoint_expr);
                    }
                }

            }
            foreach (var IDPoint_Y_KV in DicIDPoint_expr)
            {
                foreach (var Point_Y_KV in IDPoint_Y_KV.Value)
                {
                    int StartEnd = Point_Y_KV.Key;
                    GRBLinExpr a_completed_pas_route = Point_Y_KV.Value;
                    myModel_dy_out.AddConstr(a_completed_pas_route, GRB.EQUAL, 0, "Constr_a_completed_pas_route" + IDPoint_Y_KV.Key.ToString() + "_" + StartEnd.ToString());
                }
            }

            //myModel_dy_out.AddConstr(constr_pas_added_out_2, GRB.EQUAL, 2, "Constr_Y_bar_pas_added_out_2_");
            //myModel_dy_out.AddConstr(constr_pas_added_back_2, GRB.EQUAL, -2, "Constr_Y_bar_pas_added_2_back");
            myModel_dy_out.AddConstr(constr_pas_added_back_2- constr_pas_added_back_2, GRB.EQUAL, 0, "Constr_Y_bar_pas_out_equals_back");

            //PasUp与PasDown的首尾相接约束
            Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicPasUpTypeIDTime = arcDicIndiTypeIDArcType[pasUp];
            Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>> arcDicPasDownTypeIDTime = arcDicIndiTypeIDArcType[pasDown];

            Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDFirstSTVpasUp = DicFirstSpaceTimeVertex[pasUp];
            Dictionary<IndividualID, List<SpaceTimeVertex>> DicIDSecondSTVpasDown = DicSecondSpaceTimeVertex[pasDown];

            foreach (var arcDicTypeIDTimeKV in arcDicPasUpTypeIDTime)
            {
                IndividualID individualIDUp = arcDicTypeIDTimeKV.Key;
                //选定individual
                List<ArcGRBVar> VarYListpasUpindividualID = VarDic_Type_ID_pas[pasUp][individualIDUp];
                //选定某个p对应的P+ varList
                List<Arc> arcListPasUpEndIDTime = arcDicTypeIDTimeKV.Value[typeEnd];
                //选定某个p对应的p+的所有end arc
                IndividualID individualIDDown = individualIDUp.ToString().Replace("Up", "Down");
                List<ArcGRBVar> VarYListpasDownindividualID = VarDic_Type_ID_pas[pasDown][individualIDDown];
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
                    List<ArcGRBVar> ArcGRBVarYPasUpList = VarYListpasUpindividualID.Where(
                        t => t.arc.arcType.ToString() == typeEnd.ToString()
                        && t.arc.arcID.arcSpaceVertices[0].individualID.ToString() == spaceTimeVertex.spaceVertex.individualID.ToString()
                        && t.arc.timeVertices[1].ToString().Equals(spaceTimeVertex.timeVertex.ToString())
                        ).ToList();//不知道为什么就是需要这样子写
                    List<ArcGRBVar> ArcGRBVarYPasDownList = VarYListpasDownindividualID.Where(
                        t => t.arc.arcType.ToString() == typeStart.ToString() &&
                        t.arc.arcID.arcSpaceVertices[1].individualID.ToString() == spaceTimeVertex.spaceVertex.individualID.ToString() &&
                        t.arc.timeVertices[0].ToString().Equals(spaceTimeVertex.timeVertex.ToString())).ToList();//不知道为什么就是需要这样子写
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
                    myModel_dy_out.AddConstr(constr5Flow, GRB.EQUAL, 0, "Constr5_Flow_" + individualIDUp.ToString() + "_j_" + spaceTimeVertex.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertex.timeVertex.ToString());
                }
            }

            VarDic_veh_pas = new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();
            VarDic_veh_pas.Add(pasUp, VarDic_Type_ID_pas[pasUp]);
            VarDic_veh_pas.Add(pasDown, VarDic_Type_ID_pas[pasDown]);
            VarDic_veh_pas.Add(veh, VarDic_ID_veh);
            //生成新的字典
            //将原字典打开
            foreach (var BB_info_pas in pas_BB_info_subpro)
            {
                SpaceTimeVertices STV = DicSpaceTimeVertices[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index];
                ArcGRBVar Y_var = VarDic_Type_ID_pas[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index];
                List<ArcGRBVar> arcGRBVarYs_list = VarDic_Type_ID_pas[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID].Where(x => x.arc.arcType.ToString() == "start").ToList();
                //如果选择了这个乘客则必须经过或不经过这条路径
                GRBLinExpr Y_branch = new GRBLinExpr();
                GRBLinExpr Y_start_added = new GRBLinExpr();
                foreach (ArcGRBVar arc_var in arcGRBVarYs_list)
                {
                    Y_start_added += arc_var.GRBV;
                }
                if (BB_info_pas.BB_zero_or_one==1)
                {
                    Y_branch = Y_var.GRBV - Y_start_added;
                    myModel_dy_out.AddConstr(Y_branch, GRB.EQUAL, 0, "Y_added_" + Y_var.arc.arcVarName + "_" + BB_info_pas.BB_zero_or_one);
                }
                else
                {
                    Y_branch = Y_var.GRBV;
                    myModel_dy_out.AddConstr(Y_branch, GRB.EQUAL, 0, "Y_added_" + Y_var.arc.arcVarName + "_" + BB_info_pas.BB_zero_or_one);
                }
            }


            return myModel_dy_out;
        }

        public void StartSubProOptimization_pas(
            out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_veh_pas)
        {
            Generating_Sub_Problem_Var_Obj(out GRBModel myModel_dy_out);
            myModel_dy_out = Generating_Sub_Constr(myModel_dy_out,out VarDic_veh_pas);
            myModel_dy_out.Optimize();
            Program.Generating_Sub_Results(myModel_dy_out, VarDic_veh_pas);
            Console.WriteLine("============================end: pas sub pro solution============================");
            GeneratingVehRoutes(arcDic, VarDic_veh_pas, ArcGRBVarY_ID_ijtts, myModel_dy_out);
            myModel_dy_out.Optimize();
            Console.WriteLine("============================end: pas sub pro-visveh solution============================");
        }

        public void Generating_Route_Y_pas_arc_ID_Dic(out Dictionary<RouteID, Dictionary<IndividualType, List<Arc>>> Route_Y_veh_arc_ID_Dic, 
            out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_Type_ID_pas,
            out int ID_sequence_pas_out)
        {
            StartSubProOptimization_pas(out VarDic_Type_ID_pas);
            Route_Y_veh_arc_ID_Dic = new Dictionary<RouteID, Dictionary<IndividualType, List<Arc>>>();
            Dictionary<IndividualType,List<Arc>> chosenArcDic = new Dictionary<IndividualType, List<Arc>>();
            ID_sequence_pas_out = ID_sequence_pas + 1;
            string whichPas = "";
            foreach (var typeIDListVar_KV in VarDic_Type_ID_pas)
            {
                List<Arc> chosenArcList = new List<Arc>();
                foreach (var IDListVar_KV in typeIDListVar_KV.Value)
                {
                    List<ArcGRBVar> arcGRBVarY_pas_List = IDListVar_KV.Value;
                    foreach (ArcGRBVar arcGRBVar in arcGRBVarY_pas_List)
                    {
                        if (arcGRBVar.GRBV.X == 1)
                        {
                            chosenArcList.Add(arcGRBVar.arc);
                            if (typeIDListVar_KV.Key!=veh)
                            {
                                whichPas = arcGRBVar.arc.individualID.ToString();
                            }
                        }
                    }
                }
                chosenArcDic.Add(typeIDListVar_KV.Key, chosenArcList);
            }
            RouteID routeID = "R_pas_No_" + ID_sequence_pas_out.ToString() + "_serving_" + whichPas;
            Console.WriteLine(routeID.ToString());
            Route_Y_veh_arc_ID_Dic.Add(routeID, chosenArcDic);
        }

        public void GeneratingVehRoutes(
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_veh_pas,
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
        GRBModel myModel)
        {
            GRBLinExpr objLineExpr = new GRBLinExpr();
            foreach (var typeID_listArcVar in VarDic_veh_pas)
            {
                foreach (var ID_listArcVar in typeID_listArcVar.Value)
                {
                    IndividualID pasID = ID_listArcVar.Key;
                    foreach (ArcGRBVar Y_arcVar in ID_listArcVar.Value)
                    {
                        if (typeID_listArcVar.Key == veh)
                        {
                            objLineExpr+= Y_arcVar.GRBV* Y_arcVar.arc.value*999;
                        }
                        else
                        {
                            if (Y_arcVar.GRBV.X == 1)
                            {
                                SpaceTimeVertices STV = Y_arcVar.arc.spaceTimeVertices;
                                GRBLinExpr pas_arcY_value = new GRBLinExpr();
                                pas_arcY_value += Y_arcVar.GRBV;
                                myModel.AddConstr(pas_arcY_value, GRB.EQUAL, 1, "vispas_Y_" + pasID.ToString() + "_i_" + STV.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + STV.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + STV.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + STV.spaceTimeVertex2.timeVertex.ToString());
                            }
                        }
                    }
                }
            }
            myModel.SetObjective(objLineExpr, GRB.MINIMIZE);

            foreach (var STV_type_ID_ListVar in ArcGRBVarY_ID_ijtts)
            {
                SpaceTimeVertices STV = STV_type_ID_ListVar.Key;
                foreach (var type_ID_ListVar_KV in STV_type_ID_ListVar.Value)
                {
                    if (type_ID_ListVar_KV.Key == veh)
                    {
                        continue;
                    }
                    foreach (var ID_ListVar_KV in type_ID_ListVar_KV.Value)
                    {
                        IndividualID pasID = ID_ListVar_KV.Key;
                        IndividualID visvehID = "visveh" + pasID.ToString();
                        List<ArcGRBVar> vehID_listVar = STV_type_ID_ListVar.Value[veh][visvehID];
                        if (ID_ListVar_KV.Value.ToArray().Length == 0 ||
                            vehID_listVar.ToArray().Length == 0)//这个时空节点是无法被veh访问的
                        {
                            continue;
                        }
                        ArcGRBVar pas_arcY = ID_ListVar_KV.Value[0];
                        if (pas_arcY.GRBV.X == 0)
                        {
                            continue;
                        }
                        //按理说每一个individualID_pas的每一个时空坐标只对应一个arc
                        //每一个individualID_veh的每一个时空坐标可能会对应两个arc：oper和dwell
                        GRBLinExpr veh_arcY_value = new GRBLinExpr();
                        ArcGRBVar veh_arcY;
                        if (pas_arcY.arc.arcType.ToString() == typeOper.ToString())
                        {
                            veh_arcY = vehID_listVar.Find(
                                t => t.arc.arcType.ToString() == typeOper.ToString());
                        }
                        else
                        {
                            veh_arcY = vehID_listVar.Find(
                                t => t.arc.arcType.ToString() != typeOper.ToString());
                        }
                        veh_arcY_value += veh_arcY.GRBV;
                        myModel.AddConstr(veh_arcY_value, GRB.EQUAL, 1, "visveh_Y_" + pasID.ToString() + "_i_" + STV.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + STV.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + STV.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + STV.spaceTimeVertex2.timeVertex.ToString());
                    }
                }
            }

            foreach (var IndiTypeIDTime_KV in arcDic)
            {
                Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>> arcDicIndiTypeIDTime = IndiTypeIDTime_KV.Value;
                IndividualType individualType = IndiTypeIDTime_KV.Key;
                if (individualType != veh)
                {
                    continue;
                }
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
                Dictionary<IndividualID, List<ArcGRBVar>> DicIDVarY = VarDic_veh_pas[individualType];
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
                        }
                    }
                    if (individualID.ToString().Contains("pas"))
                    {
                        //初始解强制有解
                        constr1Z_start += 1;
                        constr1Z_end += 1;
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
                    myModel.AddConstr(constr1FirstYSecondY_end + constr1Z_end, GRB.EQUAL, 0, "Constr1_flow_back_IndiID_" + individualID.ToString() + "_" + endPoint.individualID.ToString());
                }
            }
        }
    }
}
