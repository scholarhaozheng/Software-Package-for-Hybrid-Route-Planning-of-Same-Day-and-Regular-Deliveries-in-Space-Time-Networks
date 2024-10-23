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
    internal class GeneratingMasterProblemQ
    {
        GRBModel myModel;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts;
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc;
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
        double capacity = 4;
        List<IndividualType> pasTypeList;
        public GeneratingMasterProblemQ(GRBModel myModel,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts,
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc)
        {
            this.myModel = myModel;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.DicSpaceTimeVertices = DicSpaceTimeVertices;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.arcGRBVarY_ID_ijtts = arcGRBVarY_ID_ijtts;
            this.DicTypeRID_Arc = DicTypeRID_Arc;
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
            pointTsO.individualID = "TsO";pointTsO.individualID.O_or_D_or_M_none = "none";pointTsO.individualID.OutOrBack = "none";pointTsO.individualID.OutOrBack = "none";
            pointTsO.pointType = "TsO";
            pointTsD.individualID = "TsD";pointTsD.individualID.O_or_D_or_M_none = "none";pointTsD.individualID.OutOrBack = "none";
            pointTsD.pointType = "TsD";pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
        }


        public void Generating_Master_Problem_Var_Obj(
            out Dictionary<IndividualType, Dictionary<RouteID, VarTheta>> VarThetaDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>> VarWDic2,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>> VarWDic,
            out GRBModel myModel_dy_out,
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic)
        {

            myModel_dy_out = this.myModel;
            myModel_dy_out = Program.DeletingConstrs(myModel_dy_out);
            GRBLinExpr objMasterExpr = new GRBLinExpr();
            VarThetaDic = new Dictionary<IndividualType, Dictionary<RouteID, VarTheta>>();
            foreach (var CostID_Dic_KV in CostDic)
            {
                IndividualType individualType = CostID_Dic_KV.Key;

                Dictionary<RouteID, double> CostID_Dic = CostID_Dic_KV.Value;
                Dictionary<RouteID, VarTheta> VarThetaIDDic = new Dictionary<RouteID, VarTheta>();
                foreach (var ID_Dic_KV in CostID_Dic)
                {

                    RouteID routeID = ID_Dic_KV.Key;
                    VarTheta varTheta = new VarTheta();
                    if (individualType == pasDown)
                    {
                        varTheta = VarThetaDic[pasUp][routeID.ToString().Replace("Down", "Up")];
                    }
                    else
                    {
                        varTheta.RouteIDTheta = routeID;
                        varTheta.IndividualTypeTheta = individualType;
                        varTheta.GRBV = new GRBVar();
                        varTheta.GRBV = myModel_dy_out.AddVar(0, 1, 1.0, GRB.CONTINUOUS,
                                "Theta_"
                                + routeID.ToString());
                    }
                    VarThetaIDDic.Add(routeID, varTheta);
                    objMasterExpr += varTheta.GRBV * CostID_Dic[routeID];
                }
                VarThetaDic.Add(individualType, VarThetaIDDic);
            }
            myModel_dy_out.SetObjective(objMasterExpr, GRB.MINIMIZE);

            ////乘客上下车Theta相等constr
            //foreach (var RID_Theta_KV in VarThetaDic[pasUp])
            //{
            //    VarTheta varThetaDown = VarThetaDic[pasDown][RID_Theta_KV.Key.ToString().Replace("Up", "Down")];
            //    VarTheta varThetaUp = RID_Theta_KV.Value;
            //    GRBLinExpr ThetaUpDown = varThetaDown.GRBV - varThetaUp.GRBV;
            //    myModel_dy_out.AddConstr(ThetaUpDown, GRB.EQUAL, 0, "ConstrA_theta_veh_" + RID_Theta_KV.Key.ToString());
            //}


            VarWDic2 = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>>();
            VarWDic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>>();



            Dictionary<RouteID, List<Arc>> Dic_vehID_STV = DicTypeRID_Arc[veh];
            //应该有针对除了O-OpasUp, pasUp-TsD, TsO-, -D所有ijtt的w
            foreach (IndividualType pasType in pasTypeList)
            {
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>> W_ID_STV_ID_Dic = new Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>();
                Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>> W_ID_ID_STV_Dic2 = new Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>();
                foreach (var ID_VAR_KV in arcDicIndiTypeID[pasType])
                {
                    IndividualID IndividualIDPas = ID_VAR_KV.Key;
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>> W_STV_ID_Dic = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>();
                    Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>> W_ID_STV_Dic2 = new Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>();
                    foreach (var vehID_ListSTV_KV in Dic_vehID_STV)
                    {
                        RouteID routeID = vehID_ListSTV_KV.Key;
                        List<Arc> ListSpaceTimeVertices = vehID_ListSTV_KV.Value;
                        Dictionary<SpaceTimeVertices, VarW> W_STV_Dic2 = new Dictionary<SpaceTimeVertices, VarW>();
                        foreach (Arc arc in ListSpaceTimeVertices)
                        {
                            VarW varW = new VarW();
                            varW.IndividualIDpas = IndividualIDPas;
                            varW.RouteID_W_L = routeID;
                            varW.GRBV = new GRBVar();
                            varW.SpaceTimeVertices = arc.spaceTimeVertices;
                            //varW.GRBV = myModel_dy_out.AddVar(0, 1, 0.0, GRB.CONTINUOUS,
                            //        "W_"
                            //        + IndividualIDPas.ToString() + "_"
                            //        + routeID.ToString()+"_"
                            //        + arc.arcVarName.ToString());
                            if (!W_STV_ID_Dic.ContainsKey(arc.spaceTimeVertices))
                            {
                                Dictionary<RouteID, VarW> KV = new Dictionary<RouteID, VarW>();
                                W_STV_ID_Dic.Add(arc.spaceTimeVertices, KV);
                            }
                            W_STV_ID_Dic[arc.spaceTimeVertices].Add(routeID, varW);
                            W_STV_Dic2.Add(arc.spaceTimeVertices, varW);
                        }
                        W_ID_STV_Dic2.Add(routeID, W_STV_Dic2);
                    }
                    W_ID_STV_ID_Dic.Add(IndividualIDPas, W_STV_ID_Dic);
                    W_ID_ID_STV_Dic2.Add(IndividualIDPas, W_ID_STV_Dic2);
                }
                VarWDic2.Add(pasType, W_ID_ID_STV_Dic2);
                VarWDic.Add(pasType, W_ID_STV_ID_Dic);
            }

        }
        public GRBModel Generating_Master_Constr(
            GRBModel myModel_dy_out,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic2,
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic2,
            Dictionary<IndividualType, Dictionary<RouteID, VarTheta>> VarThetaDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>> VarWDic2,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>> VarWDic)            
        {
            Dictionary<IndividualType, Dictionary<IndividualID, string>> Dic_typeID_str_A_theta;
            Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>> Dic_typetypeIDSTVRID_str;
            Dic_typeID_str_A_theta = new Dictionary<IndividualType, Dictionary<IndividualID, string>>();
            //a_theta constr
            foreach (var type_IDTheta_KV in VarThetaDic)
            {
                Dictionary<RouteID, VarTheta> DicIDTheta = type_IDTheta_KV.Value;
                IndividualType individualType = type_IDTheta_KV.Key;
                if (individualType == veh)
                {
                    Dictionary<IndividualID, string> DicCstr_ID_str = new Dictionary<IndividualID, string>();
                    foreach (IndividualType pasType in pasTypeList)
                    {
                        Dictionary<IndividualID, Dictionary<RouteID, double>> ADicIDID = ADic[pasType];
                        foreach (var AIDIDKV in ADicIDID)
                        {
                            GRBLinExpr constrA_Theta = new GRBLinExpr();
                            IndividualID ID = AIDIDKV.Key;
                            foreach (var AIDKV in AIDIDKV.Value)
                            {
                                if (AIDKV.Key.ToString().Contains("pas"))
                                {
                                    continue;
                                }
                                VarTheta varTheta = DicIDTheta[AIDKV.Key];
                                double A = AIDKV.Value;
                                constrA_Theta += varTheta.GRBV * A;
                            }
                            myModel_dy_out.AddConstr(constrA_Theta, GRB.GREATER_EQUAL, 1, "ConstrA_theta_veh_" + AIDIDKV.Key.ToString());
                            DicCstr_ID_str.Add(ID, "ConstrA_theta_veh_" + AIDIDKV.Key.ToString());
                        }
                    }
                    Dic_typeID_str_A_theta.Add(individualType, DicCstr_ID_str);
                }
                else
                {
                    Dictionary<IndividualID, Dictionary<RouteID, double>> ADicIDID = ADic[individualType];
                    Dictionary<IndividualID, string> DicCstr_ID_str = new Dictionary<IndividualID, string>();
                    foreach (var AIDIDKV in ADicIDID)
                    {
                        GRBLinExpr constrA_Theta = new GRBLinExpr();
                        IndividualID ID = AIDIDKV.Key;
                        foreach (var AIDKV in AIDIDKV.Value)
                        {
                            if (AIDKV.Key.ToString().Contains("veh"))
                            {
                                continue;
                            }
                            VarTheta varTheta = DicIDTheta[AIDKV.Key];
                            double A = AIDKV.Value;
                            constrA_Theta += varTheta.GRBV * A;
                        }
                        string constr_name = "ConstrA_theta_pas_" + AIDIDKV.Key.ToString();
                        GRBConstr[] gRBConstrs = myModel_dy_out.GetConstrs();
                        myModel_dy_out.AddConstr(constrA_Theta, GRB.GREATER_EQUAL, 1, constr_name);
                        DicCstr_ID_str.Add(AIDIDKV.Key, constr_name);
                    }
                    Dic_typeID_str_A_theta.Add(individualType, DicCstr_ID_str);
                }
            }

            List<SpaceTimeVertices> pasUpSTV_List = BDic[pasUp].Keys.ToList();
            List<SpaceTimeVertices> pasDownSTV_List = BDic[pasDown].Keys.ToList();
            List<SpaceTimeVertices> pasSTV_List = pasUpSTV_List.Union(pasDownSTV_List).ToList<SpaceTimeVertices>();

            Dictionary<SpaceTimeVertices, Dictionary<RouteID, GRBQuadExpr>> STV_ID_Qconstr_left_dic = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, GRBQuadExpr>>();
            foreach (SpaceTimeVertices STVpas in pasSTV_List)//选择pasDic中的STV
            {
                if (STVpas.spaceTimeVertex1.spaceVertex.individualID.ToString() == pointO.individualID.ToString() ||
                                STVpas.spaceTimeVertex1.spaceVertex.individualID.ToString() == pointTsO.individualID.ToString() ||
                                STVpas.spaceTimeVertex2.spaceVertex.individualID.ToString() == pointD.individualID.ToString() ||
                                STVpas.spaceTimeVertex2.spaceVertex.individualID.ToString() == pointTsD.individualID.ToString())
                {
                    continue;
                }
                Dictionary<RouteID, VarTheta> VarThetaIDDic_L = VarThetaDic[veh];
                Dictionary<RouteID, GRBQuadExpr> ID_Qconstr_left_dic = new Dictionary<RouteID, GRBQuadExpr>();
                foreach (var RID_theta_KV in VarThetaIDDic_L)//选择任意一个L
                {
                    RouteID RouteID_L = RID_theta_KV.Key;
                    GRBVar theta_L = RID_theta_KV.Value.GRBV;
                    GRBQuadExpr constr_bdqdlx_left = new GRBQuadExpr();
                    foreach (var TypeSTVRIC_b_KV in BDic)
                    {
                        IndividualType pasType = TypeSTVRIC_b_KV.Key;//选择了是左边的+段还是-段的计算
                        Dictionary<RouteID, VarTheta> VarThetaIDDic_Q = VarThetaDic[pasType];
                        if (pasType == veh) { continue; }
                        if (!TypeSTVRIC_b_KV.Value.ContainsKey(STVpas))
                            //因为使用的STV是从所有的STV里面循环选择的，所以有可能比如pasDown里面不包含pasUp的内容
                        {
                            continue;
                        }
                        Dictionary<RouteID, double> RID_pas_b = TypeSTVRIC_b_KV.Value[STVpas];//先只考虑左边，不要管右边
                        GRBQuadExpr constr_bdqdlx_left_half = new GRBQuadExpr();
                        foreach (var ID_RID_A in ADic[pasType])
                        {
                            IndividualID pasID_r = ID_RID_A.Key;
                            GRBLinExpr bdq_theta_sum_q = new GRBLinExpr();
                            GRBLinExpr dl_theta = new GRBLinExpr();
                            foreach (var RID_Q_b_KV in RID_pas_b)
                            {
                                RouteID RIDQ = RID_Q_b_KV.Key;
                                double b_ijtt = BDic[pasType][STVpas][RIDQ];
                                GRBVar theta_Q = VarThetaIDDic_Q[RIDQ].GRBV;
                                double d_q_r;
                                if (!ID_RID_A.Value.ContainsKey(RIDQ))
                                {
                                    d_q_r = 0;
                                }
                                else
                                {
                                    d_q_r = ID_RID_A.Value[RIDQ];
                                }
                                GRBLinExpr bdq_theta = new GRBLinExpr();
                                bdq_theta = b_ijtt * d_q_r * theta_Q;
                                bdq_theta_sum_q += bdq_theta;
                            }
                            double d_l_r;
                            if (!ADic[pasType][pasID_r].ContainsKey(RouteID_L))//这条路径不服务这个乘客
                            {
                                d_l_r = 0;
                            }
                            else
                            {
                                d_l_r = ADic[pasType][pasID_r][RouteID_L];
                            }
                            
                            constr_bdqdlx_left_half += bdq_theta_sum_q * d_l_r* theta_L;
                        }
                        constr_bdqdlx_left += constr_bdqdlx_left_half;
                    }
                    ID_Qconstr_left_dic.Add(RouteID_L, constr_bdqdlx_left);
                }
                STV_ID_Qconstr_left_dic.Add(STVpas, ID_Qconstr_left_dic);
            }

            foreach (var STVID_W in STV_ID_Qconstr_left_dic)
            {
                foreach (var ID_W in STVID_W.Value)
                {
                    SpaceTimeVertices spaceTimeVertices = STVID_W.Key;
                    GRBQuadExpr w_expr = new GRBQuadExpr();
                    double b_ijtt_q = 0;
                    if (BDic[veh].ContainsKey(STVID_W.Key))
                    {
                        if (BDic[veh][STVID_W.Key].ContainsKey(ID_W.Key))
                        {
                            b_ijtt_q = BDic[veh][STVID_W.Key][ID_W.Key];
                        }
                    }
                    w_expr = ID_W.Value - b_ijtt_q * capacity;
                    myModel_dy_out.AddQConstr(-w_expr, GRB.GREATER_EQUAL, 0, "w_cap_expr_" + ID_W.Key + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString());
                }
            }

            return myModel_dy_out;
        }
        public GRBModel SetMyModel()
        {
            GRBEnv myEnv = new GRBEnv();//构建环境
            GRBModel myModel = new GRBModel(myEnv);
            return myModel;
        }
        public void StartMasterProOptimization(
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic2,
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic2,
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic)
        {
            Generating_Master_Problem_Var_Obj(
            out Dictionary<IndividualType, Dictionary<RouteID, VarTheta>> VarThetaDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>> VarWDic2,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>> VarWDic,
            out GRBModel myModel_dy_out,
            CostDic);
            myModel_dy_out = Generating_Master_Constr(myModel_dy_out, ADic, ADic2, BDic, BDic2, VarThetaDic, VarWDic2, VarWDic);
            myModel_dy_out.Set(GRB.IntParam.NonConvex, 2);
            myModel_dy_out.Optimize();
            Console.WriteLine("Master_Results_Q");
            //Program.Generating_Master_Results(myModel_dy_out, VarThetaDic, VarWDic);
            myModel_dy_out.Write("result_masterQ_" + ".lp");
            if (myModel_dy_out.Status == GRB.Status.OPTIMAL)
            {
                Console.WriteLine(myModel_dy_out.ObjVal);
                foreach (var TypeRID_theta_KV in VarThetaDic)
                {
                    foreach (var RID_theta_KV in TypeRID_theta_KV.Value)
                    {
                        if (RID_theta_KV.Value.GRBV.X !=0)
                        {
                            Console.WriteLine(RID_theta_KV.Key);
                            List<Arc> ArcYList = DicTypeRID_Arc[TypeRID_theta_KV.Key][RID_theta_KV.Key];
                            ArcYList = ArcYList.OrderBy(a => (a.timeVertices[1] + a.timeVertices[0])).ToList();
                            Arc startArc = ArcYList.Where(x => x.arcType.ToString() == "start").ToList()[0];
                            Arc endArc = ArcYList.Where(x => x.arcType.ToString() == "end").ToList()[0];
                            int startArc_index = ArcYList.IndexOf(startArc);
                            int endArc_index = ArcYList.IndexOf(endArc);
                            ArcYList.RemoveAt(endArc_index);
                            ArcYList.RemoveAt(startArc_index);
                            ArcYList.Insert(0, startArc);
                            ArcYList.Add(endArc);
                            foreach (Arc arc in ArcYList)
                            {
                                Console.WriteLine(arc.arcVarName + " = " + RID_theta_KV.Value.GRBV.X.ToString());
                            }
                        }
                    }
                }
            }
            Program.Generating_Master_Results(myModel_dy_out, VarThetaDic, VarWDic, DicTypeRID_Arc,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic, 
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic2,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_timestring_dic,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_combined_dic);



            //myModel_dy_out.Dualize();
            //for(double i = 0; i < myModel_dy_out.GetVars().Length; i++)
            //{
            //    Console.WriteLine(myModel_dy_out.GetVars()[i].X); 
            //}



            //foreach (GRBConstr c in myModel_dy_out.GetConstrs())
            //{
            //    try
            //    {
            //        Console.WriteLine(c.Pi);
            //        Console.WriteLine(c.ConstrName);
            //    }
            //    catch(GRBException  gex)
            //    {
            //        Console.WriteLine(gex.HelpLink);
            //    }
            //}




        }



    }
}




