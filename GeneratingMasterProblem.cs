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
    internal class GeneratingMasterProblem
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
        double capacity = 4;
        List<IndividualType> pasTypeList;
        List<SpaceTimeVertices> pasSTV_List;
        List<RouteID> vehRID_list;
        public GeneratingMasterProblem(GRBModel myModel,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts,
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc,
            List<SpaceTimeVertices> pasSTV_List, List<RouteID> vehRID_list)
        {
            this.myModel = myModel;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.DicSpaceTimeVertices = DicSpaceTimeVertices;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.arcGRBVarY_ID_ijtts = arcGRBVarY_ID_ijtts;
            this.DicTypeRID_Arc = DicTypeRID_Arc;
            this.pasSTV_List = pasSTV_List;
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


            //foreach (RouteID vehRID in vehRID_list)
            //{
            //    VarTheta varTheta = VarThetaDic[veh][vehRID];
            //    GRBLinExpr constr_theta_have_to_be_chosen = new GRBLinExpr();
            //    constr_theta_have_to_be_chosen = varTheta.GRBV;
            //    myModel_dy_out.AddConstr(constr_theta_have_to_be_chosen, GRB.EQUAL, 1, "constr_theta_have_to_be_chosen" + vehRID.ToString());
            //}

            VarWDic2 = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>>();
            VarWDic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>>();

            Dictionary<RouteID, List<Arc>> Dic_vehID_STV = DicTypeRID_Arc[veh];
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
                        RouteID routeID_L = vehID_ListSTV_KV.Key;
                        Dictionary<SpaceTimeVertices, VarW> W_STV_Dic2 = new Dictionary<SpaceTimeVertices, VarW>();
                        foreach (var STV_pas_ijtt in pasSTV_List)
                        {
                            if (STV_pas_ijtt.spaceTimeVertex1.spaceVertex.individualID.ToString()== pointO.individualID.ToString()||
                                STV_pas_ijtt.spaceTimeVertex1.spaceVertex.individualID.ToString() == pointTsO.individualID.ToString()||
                                STV_pas_ijtt.spaceTimeVertex2.spaceVertex.individualID.ToString() == pointD.individualID.ToString()||
                                STV_pas_ijtt.spaceTimeVertex2.spaceVertex.individualID.ToString() == pointTsD.individualID.ToString())
                            {
                                continue;
                            }

                            VarW varW = new VarW();
                            varW.IndividualIDpas = IndividualIDPas;
                            varW.RouteID_W_L = routeID_L;
                            varW.GRBV = new GRBVar();
                            varW.SpaceTimeVertices = STV_pas_ijtt;
                            varW.GRBV = myModel_dy_out.AddVar(0, 1, 0.0, GRB.CONTINUOUS,
                                    "W_"
                                    + IndividualIDPas.ToString() + "_"
                                    + routeID_L.ToString()
                                    + "_i_" + STV_pas_ijtt.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + STV_pas_ijtt.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + STV_pas_ijtt.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + STV_pas_ijtt.spaceTimeVertex2.timeVertex.ToString());
                            if (!W_STV_ID_Dic.ContainsKey(STV_pas_ijtt))
                            {
                                Dictionary<RouteID, VarW> KV = new Dictionary<RouteID, VarW>();
                                W_STV_ID_Dic.Add(STV_pas_ijtt, KV);
                            }
                            W_STV_ID_Dic[STV_pas_ijtt].Add(routeID_L, varW);
                            W_STV_Dic2.Add(STV_pas_ijtt, varW);
                        }
                        W_ID_STV_Dic2.Add(routeID_L, W_STV_Dic2);
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
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>> VarWDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, string>> Dic_typeID_str_A_theta,
            out Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>> Dic_typetypeIDSTVRID_str)
        {
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
                            myModel_dy_out.AddConstr(constrA_Theta, GRB.EQUAL, 1, "ConstrA_theta_veh_" + AIDIDKV.Key.ToString());
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
                        //if (VarThetaDic[veh].Keys.ToList().Count == 5)
                        //{
                        //    constrA_Theta = new GRBLinExpr();
                        //    constrA_Theta += 1;
                        //    myModel_dy_out.AddConstr(constrA_Theta, GRB.GREATER_EQUAL, 1, constr_name);
                        //}
                        //else
                        //{
                            myModel_dy_out.AddConstr(constrA_Theta, GRB.EQUAL, 1, constr_name);
                        //}
                        DicCstr_ID_str.Add(AIDIDKV.Key, constr_name);
                    }
                    Dic_typeID_str_A_theta.Add(individualType, DicCstr_ID_str);
                }
            }

            //w_b_a_theta_q_0constr
            Dic_typetypeIDSTVRID_str = new Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>>();
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>> Dic_typeID_str_w_a_b_theta = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>();
            foreach (var Type_IDSTVRIDW_KV in VarWDic)
            {
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>> DicCstr_IDSTVID_str = new Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>();
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>> Dic_IDSTWRID_W = Type_IDSTVRIDW_KV.Value;
                foreach (var IDSTVRID_W_KV in Dic_IDSTWRID_W)
                {
                    IndividualID whichIndi_r = IDSTVRID_W_KV.Key;
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>> DicCstr_STVID_str = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>();
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>> Dic_IDSTW = IDSTVRID_W_KV.Value;
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>> Bq_STVIDDic = BDic[Type_IDSTVRIDW_KV.Key];
                    Dictionary<RouteID, VarTheta> VarThetaIDDic = VarThetaDic[Type_IDSTVRIDW_KV.Key];
                    Dictionary<IndividualID, Dictionary<RouteID, double>> AIDIDDic = ADic[Type_IDSTVRIDW_KV.Key];
                    foreach (var STVW_IDL_KV in Dic_IDSTW)
                    {
                        Dictionary<RouteID, string> DicCstr_ID_str = new Dictionary<RouteID, string>();
                        SpaceTimeVertices spaceTimeVertices = STVW_IDL_KV.Key;
                        foreach (var L_W_KV in STVW_IDL_KV.Value)
                        {
                            GRBLinExpr constr_w_b_a_theta_q_0 = new GRBLinExpr();
                            GRBLinExpr sum_bq_dq_thetaq = new GRBLinExpr();
                            foreach (var RIDA_r_KV in AIDIDDic[whichIndi_r])
                            {
                                if (!Bq_STVIDDic.ContainsKey(STVW_IDL_KV.Key))
                                {
                                    continue;
                                }
                                if (!Bq_STVIDDic[STVW_IDL_KV.Key].ContainsKey(RIDA_r_KV.Key))
                                {
                                    continue;
                                }
                                double b = Bq_STVIDDic[STVW_IDL_KV.Key][RIDA_r_KV.Key];
                                double a = AIDIDDic[whichIndi_r][RIDA_r_KV.Key];
                                sum_bq_dq_thetaq += b * a * VarThetaIDDic[RIDA_r_KV.Key].GRBV;
                            }
                            constr_w_b_a_theta_q_0 = Dic_IDSTW[STVW_IDL_KV.Key][L_W_KV.Key].GRBV - sum_bq_dq_thetaq;
                            string cstrName = "cstr_w_b_a_theta_q_0_" + L_W_KV.Key.ToString() + "serving" + whichIndi_r.ToString() + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString();
                            myModel_dy_out.AddConstr(-constr_w_b_a_theta_q_0, GRB.GREATER_EQUAL, 0, cstrName);
                            DicCstr_ID_str.Add(L_W_KV.Key, cstrName);
                        }
                        DicCstr_STVID_str.Add(STVW_IDL_KV.Key, DicCstr_ID_str);
                    }
                    DicCstr_IDSTVID_str.Add(whichIndi_r, DicCstr_STVID_str);
                }
                Dic_typeID_str_w_a_b_theta.Add(Type_IDSTVRIDW_KV.Key, DicCstr_IDSTVID_str);
            }
            MasterConstrType masterConstrType_w_b_a_theta_q_0 = "w_b_a_theta_q_0";
            Dic_typetypeIDSTVRID_str.Add(masterConstrType_w_b_a_theta_q_0, Dic_typeID_str_w_a_b_theta);


            //w_al_theta_constr
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>> Dic_typeID_str_w_al_theta_constr = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>();
            foreach (var Type_IDIDSTVW_KV in VarWDic)
            {
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>> DicCstr_IDSTVID_str = new Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>();
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>> Dic_IDSTWID = Type_IDIDSTVW_KV.Value;
                foreach (var Type_IDSTVW_KV in Dic_IDSTWID)
                {
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>> DicCstr_STVID_str = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>();
                    IndividualID whichIndi_r = Type_IDSTVW_KV.Key;
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>> Dic_IDSTW = Type_IDSTVW_KV.Value;
                    Dictionary<RouteID, VarTheta> VarThetaIDDic = VarThetaDic[veh];
                    Dictionary<IndividualID, Dictionary<RouteID, double>> AIDIDDic = ADic[Type_IDIDSTVW_KV.Key];
                    foreach (var STVW_IDL_KV in Dic_IDSTW)
                    {
                        Dictionary<RouteID, string> DicCstr_ID_str = new Dictionary<RouteID, string>();
                        SpaceTimeVertices spaceTimeVertices = STVW_IDL_KV.Key;
                        foreach (var L_W_KV in STVW_IDL_KV.Value)
                        {
                            GRBLinExpr constrW_a_theta = new GRBLinExpr();
                            GRBLinExpr dl_thetal = new GRBLinExpr();
                            RouteID RouteID_L = L_W_KV.Key;
                            if (!AIDIDDic[whichIndi_r].ContainsKey(RouteID_L))
                            {
                                continue;
                            }
                            dl_thetal += AIDIDDic[whichIndi_r][RouteID_L] * VarThetaIDDic[RouteID_L].GRBV;
                            constrW_a_theta = Dic_IDSTW[STVW_IDL_KV.Key][L_W_KV.Key].GRBV - dl_thetal;
                            myModel_dy_out.AddConstr(-constrW_a_theta, GRB.GREATER_EQUAL, 0, "constrW_al_theta_" + L_W_KV.Key.ToString() + "serving" + whichIndi_r.ToString() + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString());
                            DicCstr_ID_str.Add(L_W_KV.Key, "constrW_al_theta_" + L_W_KV.Key.ToString() + "serving" + whichIndi_r.ToString() + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString());
                        }
                        DicCstr_STVID_str.Add(STVW_IDL_KV.Key, DicCstr_ID_str);
                    }
                    DicCstr_IDSTVID_str.Add(whichIndi_r, DicCstr_STVID_str);
                }
                Dic_typeID_str_w_al_theta_constr.Add(Type_IDIDSTVW_KV.Key, DicCstr_IDSTVID_str);
            }
            MasterConstrType masterConstrType_w_al_theta_constr = "w_al_theta_constr";
            Dic_typetypeIDSTVRID_str.Add(masterConstrType_w_al_theta_constr, Dic_typeID_str_w_al_theta_constr);


            //constr_sum_bq_dq_x_dl_w_1_constr
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>> sum_bq_dq_x_dl_w_1_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>();
            foreach (var Type_IDIDSTVW_KV in VarWDic)
            {
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>> DicCstr_IDSTVID_str = new Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>();
                Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>> Dic_IDSTWID = Type_IDIDSTVW_KV.Value;
                foreach (var Type_IDSTVW_KV in Dic_IDSTWID)
                {
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>> DicCstr_STVID_str = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>();
                    IndividualID whichIndi_r = Type_IDSTVW_KV.Key;
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>> Dic_IDSTW = Type_IDSTVW_KV.Value;
                    Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>> BSTVIDDic = BDic[Type_IDIDSTVW_KV.Key];
                    Dictionary<RouteID, VarTheta> VarThetaIDDic = VarThetaDic[Type_IDIDSTVW_KV.Key];
                    Dictionary<RouteID, VarTheta> VarThetaIDDic_L = VarThetaDic[veh];
                    Dictionary<IndividualID, Dictionary<RouteID, double>> AIDIDDic = ADic[Type_IDIDSTVW_KV.Key];
                    foreach (var STVW_IDL_KV in Dic_IDSTW)
                    {
                        Dictionary<RouteID, string> DicCstr_ID_str = new Dictionary<RouteID, string>();
                        SpaceTimeVertices spaceTimeVertices = STVW_IDL_KV.Key;
                        foreach (var L_W_KV in STVW_IDL_KV.Value)
                        {
                            GRBLinExpr constr_sum_bq_dq_x_dl_w_1 = new GRBLinExpr();
                            GRBLinExpr sum_bq_dq_thetaq = new GRBLinExpr();
                            string sum_bq_dq_thetaq_string = "";
                            GRBLinExpr dl_thetal = new GRBLinExpr();
                            RouteID RouteID_L = L_W_KV.Key;
                            if (!AIDIDDic[whichIndi_r].ContainsKey(RouteID_L))
                            {
                                continue;
                            }
                            //Console.WriteLine("r:"+whichIndi_r+",RouteID_L:" +RouteID_L.ToString() + "===============");
                            dl_thetal += AIDIDDic[whichIndi_r][RouteID_L] * VarThetaIDDic_L[RouteID_L].GRBV;
                            foreach (var RIDA_r_KV in AIDIDDic[whichIndi_r])
                            {
                                if (RIDA_r_KV.Key.ToString().Contains("veh"))
                                {
                                    continue;
                                }
                                if (!BSTVIDDic.ContainsKey(STVW_IDL_KV.Key))
                                {
                                    continue;
                                }
                                if (!BSTVIDDic[STVW_IDL_KV.Key].ContainsKey(RIDA_r_KV.Key))
                                {
                                    continue;
                                }
                                var BB = BSTVIDDic[STVW_IDL_KV.Key];
                                double B = BB[RIDA_r_KV.Key];
                                double A = AIDIDDic[whichIndi_r][RIDA_r_KV.Key];
                                GRBVar GRBVarW = VarThetaIDDic[RIDA_r_KV.Key].GRBV;

                                sum_bq_dq_thetaq += BSTVIDDic[STVW_IDL_KV.Key][RIDA_r_KV.Key] * AIDIDDic[whichIndi_r][RIDA_r_KV.Key] * VarThetaIDDic[RIDA_r_KV.Key].GRBV;
                                sum_bq_dq_thetaq_string = sum_bq_dq_thetaq_string + "whichIndi_r:" + whichIndi_r.ToString() + ",RIDA_r_KV.Key:" + RIDA_r_KV.Key.ToString()+"____";
                                //Console.WriteLine("which pas route:" + RIDA_r_KV.Key.ToString());
                                //Console.WriteLine(RouteID_L.ToString()+":serving it or not--" + AIDIDDic[whichIndi_r][RouteID_L].ToString());
                            }
                            constr_sum_bq_dq_x_dl_w_1 = -Dic_IDSTW[STVW_IDL_KV.Key][L_W_KV.Key].GRBV + sum_bq_dq_thetaq + dl_thetal - 1;
                            string constr_name = "constr_sum_bq_dq_x_dl_w_1_RID_" + L_W_KV.Key.ToString() + "_serving_" + whichIndi_r.ToString() + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString();
                            myModel_dy_out.AddConstr(-constr_sum_bq_dq_x_dl_w_1, GRB.GREATER_EQUAL, 0, constr_name);
                            DicCstr_ID_str.Add(L_W_KV.Key, constr_name);
                            //Console.WriteLine(constr_name.ToString());
                        }
                        DicCstr_STVID_str.Add(STVW_IDL_KV.Key, DicCstr_ID_str);
                    }
                    DicCstr_IDSTVID_str.Add(whichIndi_r, DicCstr_STVID_str);
                }
                sum_bq_dq_x_dl_w_1_dic.Add(Type_IDIDSTVW_KV.Key, DicCstr_IDSTVID_str);
            }
            MasterConstrType masterConstrType_constr_sum_bq_dq_x_dl_w_1_constr = "constr_sum_bq_dq_x_dl_w_1_constr";
            Dic_typetypeIDSTVRID_str.Add(masterConstrType_constr_sum_bq_dq_x_dl_w_1_constr, sum_bq_dq_x_dl_w_1_dic);

            Dictionary<SpaceTimeVertices, Dictionary<RouteID, GRBLinExpr>> w_constr = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, GRBLinExpr>>();
            foreach (var TypeIDSTVID_W_KV in VarWDic)
            {
                foreach (var IDSTVID_W_KV in TypeIDSTVID_W_KV.Value)
                {
                    foreach (var STVID_W_KV in IDSTVID_W_KV.Value)
                    {
                        if (!w_constr.ContainsKey(STVID_W_KV.Key))
                        {
                            Dictionary<RouteID, GRBLinExpr> keyValuePairs = new Dictionary<RouteID, GRBLinExpr>();
                            w_constr.Add(STVID_W_KV.Key, keyValuePairs);
                        }
                        SpaceTimeVertices STV = STVID_W_KV.Key;
                        foreach (var ID_W_KV in STVID_W_KV.Value)
                        {
                            if (!w_constr[STVID_W_KV.Key].ContainsKey(ID_W_KV.Key))
                            {
                                GRBLinExpr w_expr = new GRBLinExpr();
                                w_constr[STVID_W_KV.Key].Add(ID_W_KV.Key, w_expr);
                            }
                            w_constr[STVID_W_KV.Key][ID_W_KV.Key] += VarWDic[TypeIDSTVID_W_KV.Key][IDSTVID_W_KV.Key][STVID_W_KV.Key][ID_W_KV.Key].GRBV;
                        }
                    }
                }
            }
            foreach (var STVID_W in w_constr)
            {
                foreach (var ID_W in STVID_W.Value)
                {
                    SpaceTimeVertices spaceTimeVertices = STVID_W.Key;
                    GRBLinExpr w_expr = new GRBLinExpr();
                    double b_ijtt_l = 0;
                    if (BDic[veh].ContainsKey(STVID_W.Key))
                    {
                        if (BDic[veh][STVID_W.Key].ContainsKey(ID_W.Key))
                        {
                            b_ijtt_l = BDic[veh][STVID_W.Key][ID_W.Key];
                        }
                    }
                    w_expr = ID_W.Value - b_ijtt_l * capacity;
                    myModel_dy_out.AddConstr(-w_expr, GRB.GREATER_EQUAL, 0, "w_cap_expr_" + ID_W.Key + "_i_" + spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString() + "_j_" + spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString() + "_t_" + spaceTimeVertices.spaceTimeVertex1.timeVertex.ToString() + "_tt_" + spaceTimeVertices.spaceTimeVertex2.timeVertex.ToString());
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

        public void Generating_Mas_Problem_u(Dictionary<IndividualType, Dictionary<IndividualID, string>> Dic_typeID_str_A_theta,
            Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, string>>>>> Dic_typetypeIDSTVRID_str,
            out Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta,
            out Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic)
        {
            Dictionary<IndividualType, Dictionary<IndividualID, GRBConstr>> GRBCstr_A_theta = new Dictionary<IndividualType, Dictionary<IndividualID, GRBConstr>>();
            foreach (var type_ID_str_KV in Dic_typeID_str_A_theta)
            {
                IndividualType individualType = type_ID_str_KV.Key;
                Dictionary<IndividualID, GRBConstr> GRBIDCstr = new Dictionary<IndividualID, GRBConstr>();
                foreach (var ID_str_KV in type_ID_str_KV.Value)
                {
                    IndividualID individualID = ID_str_KV.Key;
                    GRBIDCstr.Add(individualID, myModel.GetConstrByName(ID_str_KV.Value));
                }
                GRBCstr_A_theta.Add(individualType, GRBIDCstr);
            }
            u_dic_A_theta = new Dictionary<IndividualType, Dictionary<IndividualID, double>>();
            foreach (var typeConstrKV in GRBCstr_A_theta)
            {
                Dictionary<IndividualID, double> w_ID_dic = new Dictionary<IndividualID, double>();
                foreach (var ConstrKV in typeConstrKV.Value)
                {
                    double u_ = ConstrKV.Value.Pi;
                    w_ID_dic.Add(ConstrKV.Key, u_);
                }
                u_dic_A_theta.Add((IndividualType)typeConstrKV.Key, w_ID_dic);
                //if (typeConstrKV.Key==pasUp)
                //{
                //    u_dic_A_theta.Add(pasDown, w_ID_dic);
                //}
            }


            u_Oth_dic = new Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>>();
            foreach (var typetypeIDSTVID_cstr in Dic_typetypeIDSTVRID_str)
            {
                MasterConstrType masterConstrType = typetypeIDSTVID_cstr.Key;
                Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>> u_Oth_dic_typeIDSTVID =
                new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>();
                foreach (var type_ID_STV_ID_str_KV in typetypeIDSTVID_cstr.Value)
                {
                    IndividualType individualType = type_ID_STV_ID_str_KV.Key;
                    Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> u_Oth_dic_IDSTVID =
                new Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>();
                    foreach (var ID_STV_ID_str_KV in type_ID_STV_ID_str_KV.Value)
                    {
                        IndividualID individualID = ID_STV_ID_str_KV.Key;
                        Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>> u_Oth_dic_STVID =
                new Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>();
                        foreach (var STV_ID_str_KV in ID_STV_ID_str_KV.Value)
                        {
                            SpaceTimeVertices spaceTimeVertices = STV_ID_str_KV.Key;
                            Dictionary<RouteID, double> u_Oth_dic_ID = new Dictionary<RouteID, double>();
                            foreach (var ID_str_KV in STV_ID_str_KV.Value)
                            {
                                double u_ = myModel.GetConstrByName(ID_str_KV.Value).Pi;
                                u_Oth_dic_ID.Add(ID_str_KV.Key, u_);
                            }
                            u_Oth_dic_STVID.Add(spaceTimeVertices, u_Oth_dic_ID);
                        }
                        u_Oth_dic_IDSTVID.Add(individualID, u_Oth_dic_STVID);
                    }
                    u_Oth_dic_typeIDSTVID.Add(individualType, u_Oth_dic_IDSTVID);
                }
                u_Oth_dic.Add(masterConstrType, u_Oth_dic_typeIDSTVID);
            }
        }
        public void StartMasterProOptimization(
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic2,
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic2,
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta,
            out Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic,
            out bool IntOrNot,out BB_info BB_info_new,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic2,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_timestring_dic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_combined_string_dic)
        {
            u_dic_A_theta = new Dictionary<IndividualType, Dictionary<IndividualID, double>>();
            u_Oth_dic = new Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>>();
            IntOrNot = true;
            BB_info_new = new BB_info();
            Generating_Master_Problem_Var_Obj(
            out Dictionary<IndividualType, Dictionary<RouteID, VarTheta>> VarThetaDic,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, Dictionary<SpaceTimeVertices, VarW>>>> VarWDic2,
            out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>> VarWDic,
            out GRBModel myModel_dy_out,
            CostDic);
            myModel_dy_out = Generating_Master_Constr(myModel_dy_out, ADic, ADic2, BDic, BDic2, VarThetaDic, VarWDic2, VarWDic, 
                out Dictionary<IndividualType, Dictionary<IndividualID, string>> Dic_typeID_str_A_theta,
                out Dictionary < MasterConstrType, Dictionary < IndividualType, Dictionary < IndividualID, Dictionary < SpaceTimeVertices, Dictionary<RouteID, string> >>>> Dic_typetypeIDSTVRID_str);
            myModel_dy_out.Optimize();
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_string_dic;
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_string_dic2;
            Y_string_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_string_dic2 = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_string_dic=new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_string_dic2 = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_timestring_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_combined_string_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            if (myModel_dy_out.Status == GRB.Status.OPTIMAL)
            {
                Console.WriteLine("Master_Results:");
                Console.WriteLine(myModel_dy_out.ObjVal);
                Console.WriteLine("Master_Results:");
                Program.Generating_Master_Results(myModel_dy_out, VarThetaDic, VarWDic, DicTypeRID_Arc, 
                    out Y_typeIDRID_string_dic,out Y_typeIDRID_string_dic2,out Y_typeIDRID_timestring_dic,out Y_typeIDRID_combined_string_dic);
                foreach (var c in VarThetaDic)
                {
                    foreach (var d in c.Value)
                    {
                        if (d.Value.GRBV.X != 0&& d.Value.GRBV.X != 1)
                        {
                            IntOrNot=false;
                            continue;
                        }
                    }
                }
                Console.WriteLine(myModel_dy_out.ObjVal);
                Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<Arc, double>>> Y_solution_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<Arc, double>>>();
                foreach (var TypeRID_theta_KV in VarThetaDic)
                {
                    foreach (var RID_theta_KV in TypeRID_theta_KV.Value)
                    {
                        if (RID_theta_KV.Value.GRBV.X != 0)
                        {
                            List<Arc> ArcYList = DicTypeRID_Arc[TypeRID_theta_KV.Key][RID_theta_KV.Key];
                            ArcYList=ArcYList.OrderBy(a => (a.timeVertices[1]+ a.timeVertices[0])).ToList();
                            Arc startArc= ArcYList.Where(x =>x.arcType.ToString()== "start").ToList()[0];
                            Arc endArc = ArcYList.Where(x => x.arcType.ToString() == "end").ToList()[0];
                            int startArc_index = ArcYList.IndexOf(startArc);
                            int endArc_index = ArcYList.IndexOf(endArc);
                            ArcYList.RemoveAt(endArc_index);
                            ArcYList.RemoveAt(startArc_index);
                            ArcYList.Insert(0, startArc);
                            ArcYList.Add(endArc);
                            foreach (Arc arc in ArcYList)
                            {
                                if (!Y_solution_dic.ContainsKey(arc.individualType))
                                {
                                    Dictionary<IndividualID, Dictionary<Arc, double>> keyValuePairs = new Dictionary<IndividualID, Dictionary<Arc, double>>();
                                    Y_solution_dic.Add(arc.individualType, keyValuePairs);
                                }
                                if (!Y_solution_dic[arc.individualType].ContainsKey(arc.individualID))
                                {
                                    Dictionary<Arc, double> keyValuePairs2 = new Dictionary<Arc, double>();
                                    Y_solution_dic[arc.individualType].Add(arc.individualID, keyValuePairs2);
                                }
                                if (!Y_solution_dic[arc.individualType][arc.individualID].ContainsKey(arc))
                                {
                                    Y_solution_dic[arc.individualType][arc.individualID].Add(arc, 0);
                                }
                                Y_solution_dic[arc.individualType][arc.individualID][arc] += RID_theta_KV.Value.GRBV.X;
                            } 
                        }
                    }
                }
                if (myModel_dy_out.Status == GRB.Status.OPTIMAL)
                {
                    foreach (var c in Y_solution_dic)
                    {
                        Dictionary<IndividualID,Dictionary<RouteID, string>> KeyValuePair_RID = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                        Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID2 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                        foreach (var ID_STV_double in c.Value)
                        {
                            Dictionary<Arc, double> STV_double_dic = ID_STV_double.Value;
                            string route_string = "";
                            string route_string2 = "";
                            int howmanyarcs = STV_double_dic.Keys.Count;
                            int howmanyarcshaveprocessed = 0;
                            //o_(p,m)^r
                            //o_
                            RouteID RID_all = "RID_all";
                            Dictionary<RouteID, string> KeyValuePair = new Dictionary<RouteID, string>();
                            Dictionary<RouteID, string> KeyValuePair2 = new Dictionary<RouteID, string>();
                            foreach (var STV_double_KV in STV_double_dic)
                            {
                                string individualID = ID_STV_double.Key.ToString();
                                string i = STV_double_KV.Key.SpaceTimeVertex1.spaceVertex.individualID.ToString();
                                string j = STV_double_KV.Key.SpaceTimeVertex2.spaceVertex.individualID.ToString();
                                string t_str;
                                string tt_str;
                                route_string += i + "_";
                                route_string2 += Program.Y_to_notation(STV_double_KV.Key.SpaceTimeVertex1.spaceVertex.individualID);
                                route_string2 += ";";
                                howmanyarcshaveprocessed += 1;
                                if (howmanyarcshaveprocessed == howmanyarcs)
                                {
                                    route_string += j;
                                    route_string2 += Program.Y_to_notation(STV_double_KV.Key.SpaceTimeVertex2.spaceVertex.individualID);
                                }
                                if (STV_double_KV.Key.SpaceTimeVertex1.timeVertex < 10)
                                {
                                    t_str = "0" + STV_double_KV.Key.SpaceTimeVertex1.timeVertex;
                                }
                                else
                                {
                                    t_str = STV_double_KV.Key.SpaceTimeVertex1.timeVertex.ToString();
                                }
                                if (STV_double_KV.Key.SpaceTimeVertex2.timeVertex < 10)
                                {
                                    tt_str = "0" + STV_double_KV.Key.SpaceTimeVertex2.timeVertex;
                                }
                                else
                                {
                                    tt_str = STV_double_KV.Key.SpaceTimeVertex2.timeVertex.ToString();
                                }
                                string output = "Y_" + individualID.ToString() + "_t_" + t_str + "_tt_" + tt_str + "_i_" + i + "_j_" + j + ":" + STV_double_KV.Value;
                                Console.WriteLine(output);
                                RID_all.route_lambda = STV_double_KV.Value;
                            }
                            Console.WriteLine(route_string);
                            Console.WriteLine(route_string2);
                            KeyValuePair.Add(RID_all, route_string);
                            KeyValuePair2.Add(RID_all, route_string2);
                            KeyValuePair_RID.Add(ID_STV_double.Key, KeyValuePair);
                            KeyValuePair_RID2.Add(ID_STV_double.Key, KeyValuePair2);

                        }
                        Y_string_dic.Add(c.Key, KeyValuePair_RID);
                        Y_string_dic2.Add(c.Key, KeyValuePair_RID2);
                    }
                }
                double min_num = 1;
                foreach (var typeIDSTV_double in Y_solution_dic)
                {
                    foreach (var IDSTV_double in typeIDSTV_double.Value)
                    {
                        foreach (var STV_double in IDSTV_double.Value)
                        {
                            if (STV_double.Value< min_num)
                            {
                                min_num = STV_double.Value;
                                BB_info_new.BB_indiType = typeIDSTV_double.Key;
                                BB_info_new.BB_indiID = IDSTV_double.Key;
                                BB_info_new.BB_type_ID_index = DicSpaceTimeVertices[typeIDSTV_double.Key][IDSTV_double.Key].IndexOf(STV_double.Key.spaceTimeVertices);
                            }
                        }
                    }
                }
                Generating_Mas_Problem_u(Dic_typeID_str_A_theta, Dic_typetypeIDSTVRID_str,
            out u_dic_A_theta,
            out u_Oth_dic);
                Console.WriteLine(myModel_dy_out.ObjVal);
                foreach (GRBConstr c in myModel_dy_out.GetConstrs())
                {
                    try
                    {
                        if (c.Pi == 0)
                        {
                            continue;
                        }
                        Console.WriteLine(c.ConstrName + "-shadow_cost:" + c.Pi);
                    }
                    catch (GRBException gex)
                    {
                        Console.WriteLine(gex.HelpLink);
                    }
                }
            }
            else
            {
                //foreach (var STV_double_KV in BDic2[veh][vehRID_list[0]])
                //{
                //    if (STV_double_KV.Value==0)
                //    {
                //        continue;
                //    }
                //    string i = STV_double_KV.Key.spaceTimeVertex1.spaceVertex.individualID.ToString();
                //    string j = STV_double_KV.Key.spaceTimeVertex2.spaceVertex.individualID.ToString();
                //    string t_str;
                //    string tt_str;
                //    if (STV_double_KV.Key.spaceTimeVertex1.timeVertex < 10)
                //    {
                //        t_str = "0" + STV_double_KV.Key.spaceTimeVertex1.timeVertex.ToString();
                //    }
                //    else
                //    {
                //        t_str = STV_double_KV.Key.spaceTimeVertex1.timeVertex.ToString();
                //    }
                //    if (STV_double_KV.Key.spaceTimeVertex2.timeVertex < 10)
                //    {
                //        tt_str = "0" + STV_double_KV.Key.spaceTimeVertex2.timeVertex.ToString();
                //    }
                //    else
                //    {
                //        tt_str = STV_double_KV.Key.spaceTimeVertex2.timeVertex.ToString();
                //    }
                //    string output = string.Format("Y_[{0}]_t[{1}]_tt[{2}]_i[{3}]_j[{4}]", t_str, tt_str, i, j);
                //    Console.WriteLine(output);
                //}
            }
            

            //myModel_dy_out.Dualize();
            //for(double i = 0; i < myModel_dy_out.GetVars().Length; i++)
            //{
            //    Console.WriteLine(myModel_dy_out.GetVars()[i].X); 
            //}

            
        }
    }
}
