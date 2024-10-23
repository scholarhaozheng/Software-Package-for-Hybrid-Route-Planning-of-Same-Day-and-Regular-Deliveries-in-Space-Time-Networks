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
    internal class GeneratingSubProblem
    {
        GRBModel myModel;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices;
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
        List<IndividualID> pasList;
        Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta;
        Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic;
        int ID_sequence_veh;
        int ID_sequence_pas;
        Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all;
        int odd_veh_even_pas;
        List<SpaceTimeVertices> ListSpaceTimeVertices;
        List<RouteID> vehRID_list;
        BB_node BB_node_processing;
        public GeneratingSubProblem(GRBModel myModel,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts,
            Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta,
            Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic,
            List<IndividualID> pasList, 
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc,
            int ID_sequence_veh, int ID_sequence_pas, Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all,
            int odd_veh_even_pas, List<SpaceTimeVertices> ListSpaceTimeVertices, List<RouteID> vehRID_list, BB_node BB_node_processing)
        {
            this.myModel = myModel;
            this.arcDicIndiTypeIDArcType = arcDicIndiTypeIDArcType;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.DicSpaceTimeVertices = DicSpaceTimeVertices;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.arcGRBVarY_ID_ijtts = arcGRBVarY_ID_ijtts;
            this.u_dic_A_theta = u_dic_A_theta;
            this.u_Oth_dic = u_Oth_dic;
            this.arcDic = arcDic;
            this.pasList = pasList;
            this.DicTypeRID_Arc = DicTypeRID_Arc;
            this.ID_sequence_veh = ID_sequence_veh;
            this.ID_sequence_pas = ID_sequence_pas;
            this.CostDic_all = CostDic_all;
            this.odd_veh_even_pas = odd_veh_even_pas;
            this.ListSpaceTimeVertices = ListSpaceTimeVertices;
            this.vehRID_list = vehRID_list;
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
            pointVO.individualID = "VO"; pointVO.individualID.O_or_D_or_M_none = "none";pointVO.individualID.OutOrBack = "none";
            pointVO.pointType = "VO";
            pointVD.individualID = "VD"; pointVD.individualID.O_or_D_or_M_none = "none";pointVD.individualID.OutOrBack = "none";
            pointVD.pointType = "VD";
            pointTsO.individualID = "TsO"; pointTsO.individualID.O_or_D_or_M_none = "none";pointTsO.individualID.OutOrBack = "none";
            pointTsO.pointType = "TsO";
            pointTsD.individualID = "TsD";pointTsD.individualID.O_or_D_or_M_none = "none";pointTsD.individualID.OutOrBack = "none";
            pointTsD.pointType = "TsD";pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
            this.BB_node_processing = BB_node_processing;
        }


        public void GeneratingSub_Solution(out Dictionary<RouteID, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> VarYDic_out,
            out int ZeroOrOne_OneIsOptimal, out int odd_veh_even_pas)
        {
            //w_b_a_theta_q_0
            //w_al_theta_constr
            //constr_sum_bq_dq_x_dl_w_1_constr
            List<BB_info> veh_BB_info_subpro = BB_node_processing.veh_BB_info_list;
            List<BB_info> pas_BB_info_subpro = BB_node_processing.pas_BB_info_list;
            ZeroOrOne_OneIsOptimal = 1;
            odd_veh_even_pas = this.odd_veh_even_pas;
            odd_veh_even_pas += 1;
            VarYDic_out = new Dictionary<RouteID, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>>();
            Dictionary<IndividualType, Dictionary<RouteID, double>> TypeRID_reduced_dic = new Dictionary<IndividualType, Dictionary<RouteID, double>>();
            Dictionary<IndividualID, List<ArcGRBVar>> VarDic_ID_veh;
            VarDic_ID_veh = new Dictionary<IndividualID, List<ArcGRBVar>>();
            Dictionary<IndividualID, List<Arc>> arcDic_ID_Veh = arcDicIndiTypeID[veh];
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_Type_ID_pas;
            VarDic_Type_ID_pas = new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts = new Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>>();
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts = new Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>>();


            int ID_sequence_veh_out = new int();
            ID_sequence_veh_out = ID_sequence_veh;
            Dictionary<RouteID, double> aaa = new Dictionary<RouteID, double>();
            foreach (var RID_Arc_KV in DicTypeRID_Arc[veh])
            {
                VarDic_ID_veh = new Dictionary<IndividualID, List<ArcGRBVar>>();
                RouteID vehRID = RID_Arc_KV.Key;
                Dictionary<IndividualID, int> CostID_Dic_veh = new Dictionary<IndividualID, int>();
                GRBModel myModel_dy_veh = new GRBModel(myModel);

                foreach (var ID_Arc_KV in arcDic_ID_Veh)
                {
                    List<Arc> arcList = ID_Arc_KV.Value;
                    List<ArcGRBVar> arcGRBVarYList = new List<ArcGRBVar>();
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
                        VarY = myModel_dy_veh.AddVar(0.0, 1.0, 0, GRB.BINARY, VarName);
                        arc.arcVarName = VarName;
                        arcGRBVarY.arc = arc;
                        arcGRBVarY.GRBV = VarY;
                        arcGRBVarYList.Add(arcGRBVarY);

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
                        ArcGRBVarY_ID_ijtts[arc.spaceTimeVertices][veh][individualID].Add(arcGRBVarY);

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
                        ArcGRBVarYListijtts[arc.spaceTimeVertices][veh].Add(arcGRBVarY);
                    }
                    VarDic_ID_veh.Add(individualID, arcGRBVarYList);
                }

                SubProblemGeneratorVeh subProblemGeneratorVeh = new SubProblemGeneratorVeh(
                    myModel_dy_veh, vehRID, u_dic_A_theta, u_Oth_dic, VarDic_ID_veh, veh, arcDic, DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex, 
                    ListSpaceTimeVertices, pasList, DicTypeRID_Arc, ID_sequence_veh_out, veh_BB_info_subpro);
                subProblemGeneratorVeh.Generating_Route_Y_veh_arc_ID_Dic(out Dictionary<RouteID, List<Arc>> Route_Y_veh_arc_ID_Dic,
                    out Dictionary<IndividualID, List<ArcGRBVar>> VarY_bar_ID_Dic,
                    out ID_sequence_veh_out,
                    out IndividualID whichVeh);
                Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarY_bar_veh =
                    new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();
                VarY_bar_veh.Add(veh, VarY_bar_ID_Dic);
                RouteID routeIDveh = Route_Y_veh_arc_ID_Dic.Keys.ToList()[0];
                Console.WriteLine(routeIDveh.ToString() + "_reduced_cost:" + myModel_dy_veh.ObjVal.ToString());
                if (myModel_dy_veh.ObjVal < 0)
                {
                    ZeroOrOne_OneIsOptimal = 0;
                    Console.WriteLine(routeIDveh.ToString() + " not satisfied!!!");
                    Program.Generating_Sub_Results(myModel_dy_veh, VarY_bar_veh);
                }
                aaa.Add(routeIDveh, myModel_dy_veh.ObjVal);
                VarYDic_out.Add(routeIDveh, VarY_bar_veh);
            }
            Console.WriteLine("=================================end: veh sub pro===========================");
            foreach (var aaKV in aaa)
            {
                Console.WriteLine(aaKV.Key.ToString() + " reduced_cost:" + aaKV.Value.ToString());
            }


            GRBModel myModel_dy_pas = new GRBModel(myModel);
            VarDic_ID_veh = new Dictionary<IndividualID, List<ArcGRBVar>>();

            foreach (var ID_Arc_KV in arcDic_ID_Veh)
            {
                List<Arc> arcList = ID_Arc_KV.Value;
                List<ArcGRBVar> arcGRBVarYList = new List<ArcGRBVar>();
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
                    VarY = myModel_dy_pas.AddVar(0.0, 1.0, 0, GRB.BINARY, VarName);
                    arc.arcVarName = VarName;
                    arcGRBVarY.arc = arc;
                    arcGRBVarY.GRBV = VarY;
                    arcGRBVarYList.Add(arcGRBVarY);


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
                    ArcGRBVarY_ID_ijtts[arc.spaceTimeVertices][veh][individualID].Add(arcGRBVarY);

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
                    ArcGRBVarYListijtts[arc.spaceTimeVertices][veh].Add(arcGRBVarY);
                }
                VarDic_ID_veh.Add(individualID, arcGRBVarYList);
            }


            Dictionary<RouteID, double> RID_reduced_dic_pas = new Dictionary<RouteID, double>();
            foreach (var TypeID_Arc_KV in arcDicIndiTypeID)
            {
                if (TypeID_Arc_KV.Key == veh)
                {
                    continue;
                }
                Dictionary<IndividualID, List<ArcGRBVar>> VarDic_ID_pas;
                VarDic_ID_pas = new Dictionary<IndividualID, List<ArcGRBVar>>();
                foreach (var ID_Arc_KV in TypeID_Arc_KV.Value)
                {
                    List<Arc> arcList = ID_Arc_KV.Value;
                    List<ArcGRBVar> arcGRBVarYList = new List<ArcGRBVar>();
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
                        VarY = myModel_dy_pas.AddVar(0.0, 1.0, 0, GRB.BINARY, VarName);
                        arc.arcVarName = VarName;
                        arcGRBVarY.arc = arc;
                        arcGRBVarY.GRBV = VarY;
                        arcGRBVarYList.Add(arcGRBVarY);
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
                        ArcGRBVarY_ID_ijtts[arc.spaceTimeVertices][TypeID_Arc_KV.Key][individualID].Add(arcGRBVarY);

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
                        ArcGRBVarYListijtts[arc.spaceTimeVertices][TypeID_Arc_KV.Key].Add(arcGRBVarY);
                    }
                    VarDic_ID_pas.Add(individualID, arcGRBVarYList);
                }
                VarDic_Type_ID_pas.Add(TypeID_Arc_KV.Key, VarDic_ID_pas);
            }

            Console.WriteLine("======================constructing: pas sub pro======================");
            SubProblemGeneratorPas subProblemGeneratorPas = new SubProblemGeneratorPas(myModel_dy_pas, u_dic_A_theta, u_Oth_dic, VarDic_Type_ID_pas, VarDic_ID_veh,ArcGRBVarY_ID_ijtts, ArcGRBVarYListijtts,
                arcDic, arcDicIndiTypeID,DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex, DicSpaceTimeVertices, ListSpaceTimeVertices, pasList, DicTypeRID_Arc, arcDicIndiTypeIDArcType, ID_sequence_pas
                ,pas_BB_info_subpro);
            subProblemGeneratorPas.Generating_Route_Y_pas_arc_ID_Dic(out Dictionary<RouteID, Dictionary<IndividualType, List<Arc>>> Route_Y_pas_arc_ID_Dic,
                out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarDic_pasType_ID_pas, out ID_sequence_pas);
            Console.WriteLine("============================generating results: pas sub pro solution============================");
            //Program.Generating_Sub_Results(myModel_dy_pas, VarDic_pasType_ID_pas);
            Console.WriteLine("reduced_cost_pas:" + myModel_dy_pas.ObjVal.ToString());
            RID_reduced_dic_pas.Add(DicTypeRID_Arc[pasUp].Keys.ToList()[0], myModel_dy_pas.ObjVal);
            if (myModel_dy_pas.ObjVal < 0)
            {
                ZeroOrOne_OneIsOptimal = 0;
            }
            RouteID routeIDpas = "routeID";
            VarYDic_out.Add(routeIDpas, VarDic_pasType_ID_pas);
            myModel_dy_pas.Write("result_sub_pas" + ".lp");

        }

        public void StartSubProOptimization(out Dictionary<RouteID, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> VarYDic_out, out int ZeroOrOne_OneIsOptimal,out int odd_veh_even_pas)
        {
            GeneratingSub_Solution(out VarYDic_out, out ZeroOrOne_OneIsOptimal, out odd_veh_even_pas);
        }

        public GRBModel SetMyModel()
        {
            GRBEnv myEnv = new GRBEnv();//构建环境
            GRBModel myModel = new GRBModel(myEnv);
            return myModel;
        }
    }
}
