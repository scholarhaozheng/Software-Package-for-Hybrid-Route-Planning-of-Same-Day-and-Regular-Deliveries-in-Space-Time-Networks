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
    internal class A_B_generator
    {
        GRBModel myModel;
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic;
        int ID_sequence_veh_input;
        int ID_sequence_pas_input;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex;
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts;
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
        List<IndividualID> pasList;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all;
        Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all;
        Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2;
        Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2;
        Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all;
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all;
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all;
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_one_iter;
        bool vis_pas_or_not;
        bool ini_or_not;
        public A_B_generator(GRBModel myModel,int ID_sequence_veh_input, int ID_sequence_pas_input,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
            Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
            Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> arcGRBVarY_ID_ijtts,
            List<IndividualID> pasList,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all,
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2,
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2,
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all,
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all,
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all,
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> dicTypeRID_Arc_one_iter,
            bool vis_pas_or_not, bool ini_or_not
            //,Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> dicTypeRID_Arc_one_iter
            )
        {
            this.myModel = myModel;
            this.ID_sequence_veh_input = ID_sequence_veh_input;
            this.ID_sequence_pas_input = ID_sequence_pas_input;
            this.VarYDic = VarYDic;
            this.DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex;
            this.DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex;
            this.DicSpaceTimeVertices = DicSpaceTimeVertices;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.arcGRBVarY_ID_ijtts = arcGRBVarY_ID_ijtts;
            this.pasList = pasList;
            this.ADic_all = ADic_all;
            this.BDic_all = BDic_all;
            this.ADic_all2 = ADic_all2;
            this.BDic_all2 = BDic_all2;
            this.CostDic_all = CostDic_all;
            this.DicTypeRID_Arc_all = DicTypeRID_Arc_all;
            this.DicTypeRID_Arc_combined_all = DicTypeRID_Arc_combined_all;
            this.vis_pas_or_not = vis_pas_or_not;
            this.ini_or_not = ini_or_not;
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
            DicTypeRID_Arc_one_iter = dicTypeRID_Arc_one_iter;
            this.DicTypeRID_Arc_one_iter = dicTypeRID_Arc_one_iter;
        }
        public void Generating_a_and_b(
          out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all,
          out Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2,
          out Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all,
          out Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2,
          out Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all,
          out Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all,
          out Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all,
          out int ID_sequence_veh, out int ID_sequence_pas, out List<SpaceTimeVertices> pasSTV_List,
          out Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_one_iter
            )
        {
            ID_sequence_veh = ID_sequence_veh_input;
            ID_sequence_pas = ID_sequence_pas_input;
            CostDic_all = this.CostDic_all;
            DicTypeRID_Arc_all = this.DicTypeRID_Arc_all;
            DicTypeRID_Arc_combined_all = this.DicTypeRID_Arc_combined_all;
            ADic_all = this.ADic_all;
            ADic_all2 = this.ADic_all2;
            BDic_all = this.BDic_all;
            BDic_all2 = this.BDic_all2;
            DicTypeRID_Arc_one_iter = this.DicTypeRID_Arc_one_iter;

            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>();
            Dictionary<IndividualID, List<Arc>> DicTypeRID_Arc_pas_this_iter_combined = new Dictionary<IndividualID, List<Arc>>();

            Dictionary<IndividualID, RouteID> pasUp_RID_name_dic = new Dictionary<IndividualID, RouteID>();

            Dictionary<IndividualType, Dictionary<IndividualID, double>> Route_cost_Dic = new Dictionary<IndividualType, Dictionary<IndividualID, double>>();
            foreach (var YTypeID_KV in VarYDic)
            {
                IndividualType route_belong_type = YTypeID_KV.Key;
                Dictionary<IndividualID, List<ArcGRBVar>> YID_Dic = YTypeID_KV.Value;
                foreach (var YDic_KV in YID_Dic)
                {
                    List<ArcGRBVar> YList = YDic_KV.Value;
                    double cost = 0;
                    foreach (ArcGRBVar ArcGRBVarY in YList)
                    {
                        if (ArcGRBVarY.arc.individualID.individualType.ToString()=="visveh"
                            && ArcGRBVarY.arc.arcType.ToString() != typeStart.ToString()
                            && ArcGRBVarY.arc.arcType.ToString() != typeEnd.ToString())
                        {
                            cost += ArcGRBVarY.GRBV.X * 99999; //使用visveh需要付出很大代价
                        }
                        else if (vis_pas_or_not)
                        {
                            cost += ArcGRBVarY.GRBV.X * 9999; //使用vispas需要付出很大代价
                        }
                        else
                        {
                            cost += ArcGRBVarY.GRBV.X * ArcGRBVarY.arc.value;
                        }
                    }
                    if (cost == 0)
                    {
                        continue;
                    }
                    if (!Route_cost_Dic.ContainsKey(route_belong_type))
                    {
                        Dictionary<IndividualID, double> CostDic_type1 = new Dictionary<IndividualID, double>();
                        Route_cost_Dic.Add(route_belong_type, CostDic_type1);
                    }
                    Route_cost_Dic[route_belong_type].Add(YDic_KV.Key, cost);
                }
            }

            foreach (var YTypeID_KV in VarYDic)
            {
                IndividualType route_belong_type = YTypeID_KV.Key;
                Dictionary<IndividualID, List<ArcGRBVar>> YID_Dic = YTypeID_KV.Value;
                Dictionary<RouteID, List<Arc>> DicID_Arc = new Dictionary<RouteID, List<Arc>>();
                foreach (var YDic_KV in YID_Dic)
                {
                    List<ArcGRBVar> YList = YDic_KV.Value;
                    List<Arc> List_Arc_Y = new List<Arc>();
                    double cost = 0;
                    foreach (ArcGRBVar ArcGRBVarY in YList)
                    {
                        if (ArcGRBVarY.arc.individualID.individualType.ToString() == "visveh"
                            && ArcGRBVarY.arc.arcType.ToString() != typeStart.ToString()
                            && ArcGRBVarY.arc.arcType.ToString() != typeEnd.ToString())
                        {
                            cost += ArcGRBVarY.GRBV.X * 99999; //使用visveh需要付出很大代价
                        }
                        else if (vis_pas_or_not)
                        {
                            cost += ArcGRBVarY.GRBV.X * 9999; //使用vispas需要付出很大代价
                        }
                        else
                        {
                            cost += ArcGRBVarY.GRBV.X * ArcGRBVarY.arc.value;
                        }
                        if (ArcGRBVarY.GRBV.X == 1)
                        {
                            List_Arc_Y.Add(ArcGRBVarY.arc);
                        }
                    }
                    if (!CostDic_all.ContainsKey(route_belong_type))
                    {
                        Dictionary<RouteID, double> CostDic_type = new Dictionary<RouteID, double>();
                        CostDic_all.Add(route_belong_type, CostDic_type);
                        Dictionary<RouteID, List<Arc>> DicTypeRID_Arc_type = new Dictionary<RouteID, List<Arc>>();
                        DicTypeRID_Arc_all.Add(route_belong_type, DicTypeRID_Arc_type);
                        if (route_belong_type.ToString() == "pasUp")
                        {
                            Dictionary<RouteID, List<Arc>> DicTypeRID_Arc_type1 = new Dictionary<RouteID, List<Arc>>();
                            DicTypeRID_Arc_combined_all.Add(route_belong_type, DicTypeRID_Arc_type1);
                        }
                    }
                    if (cost == 0)
                    {
                        continue;
                    }
                    if (route_belong_type.ToString()=="pasUp")
                    {
                        DicTypeRID_Arc_pas_this_iter_combined.Add(YDic_KV.Key, List_Arc_Y);
                    }
                    else if (route_belong_type.ToString() == "pasDown")
                    {
                        DicTypeRID_Arc_pas_this_iter_combined[YDic_KV.Key.ToString().Replace("Down", "Up")] = Program.Clone(DicTypeRID_Arc_pas_this_iter_combined[YDic_KV.Key.ToString().Replace("Down", "Up")]) as List<Arc>;
                        DicTypeRID_Arc_pas_this_iter_combined[YDic_KV.Key.ToString().Replace("Down", "Up")].AddRange(List_Arc_Y);
                    }
                    ///使得重复路径不会被加入进来
                    if (CostDic_all["pasUp"].Keys.ToList().Count > 4 && CostDic_all.ContainsKey("veh") && CostDic_all["veh"].Keys.ToList().Count > 2)
                    //{
                    //}

                    //if ((!vis_pas_or_not)&&(!ini_or_not))
                    {
                        int if_one_then_repeated_and_delete = 1;
                        RouteID RID_same_Indi_STVs_with_new = new RouteID("");
                        //Console.WriteLine("new_routeID:" + routeID.ToString());
                        List<Arc> List_Arc_Y_see_if_duplicated = Program.Clone(List_Arc_Y) as List<Arc>;
                        if (route_belong_type.ToString() == "pasDown" || route_belong_type.ToString() == "veh")
                        {
                            Dictionary<RouteID, List<Arc>> ID_Routes_Dic;
                            if (route_belong_type.ToString() == "pasDown")
                            {
                                List_Arc_Y_see_if_duplicated = DicTypeRID_Arc_pas_this_iter_combined[YDic_KV.Key.ToString().Replace("Down", "Up")];
                                ID_Routes_Dic = DicTypeRID_Arc_combined_all[pasUp];
                            }
                            else
                            {
                                ID_Routes_Dic = DicTypeRID_Arc_all[route_belong_type];
                            }
                            if (DicTypeRID_Arc_all[route_belong_type].Count != 0)
                            {
                                foreach (var RID_GBRVAR_list_existed in ID_Routes_Dic)
                                {
                                    List<Arc> one_route_existed = RID_GBRVAR_list_existed.Value;
                                    //Console.WriteLine(RID_GBRVAR_list_existed.Key.ToString());
                                    if (one_route_existed.Count != List_Arc_Y_see_if_duplicated.Count)
                                    {
                                        if_one_then_repeated_and_delete = 0;
                                        continue;
                                        //如果两个arc都不一样长，那么它们肯定不一样
                                    }
                                    int howmanyarcs = 0;
                                    //Console.WriteLine("existed_routeID:" + RID_GBRVAR_list_existed.Key.ToString());
                                    foreach (Arc arc_new in List_Arc_Y_see_if_duplicated)
                                    {
                                        howmanyarcs += 1;
                                        //Console.WriteLine("new:" + arc_new.arcVarName.ToString());
                                        int if_one_then_this_arc_repeated = 0;
                                        foreach (Arc arc_existed in one_route_existed)
                                        {
                                            //Console.WriteLine("existed:" + arc_existed.arcVarName.ToString());
                                            if (arc_new.arcVarName.ToString().Equals(arc_existed.arcVarName.ToString()))
                                            //如果是属性的话就需要ToString()
                                            {
                                                //Console.WriteLine("Arc_name_same");
                                                if_one_then_this_arc_repeated = 1;
                                                break;
                                                //如果这两个Y的ARCName一样，那么这两个arc是同一个arc
                                            }
                                        }
                                        //如果循环结束这个旧路径之后，发现没有一个arc和新生成的路径中的这个arc一样，
                                        //那么可知这个新路径和这一条选中的旧路径不一样，继续循环
                                        if (if_one_then_this_arc_repeated == 0)
                                        {
                                            //Console.WriteLine("Arc_name:"+ arc_new.arcVarName+"_all_different_from_existed:"+ RID_GBRVAR_list_existed.Key.ToString());
                                            if_one_then_repeated_and_delete = 0;
                                            break;
                                            //可以不继续尝试这条新路径中的其他arc是否与选中的旧路径一样了，因为已经有arc不一样了;
                                            //直接去验证其他的旧Route会不会一样
                                        }
                                        //如果循环结束这个旧路径之后，发现有一个arc和新生成的路径中的这个arc一样，
                                        //那么可知这个新路径和可能和这个选中的Route一样
                                        if (if_one_then_this_arc_repeated == 1)
                                        {
                                            //Console.WriteLine("Arc_name_same_as_existed:" + routeID.ToString());
                                            if_one_then_repeated_and_delete = 1;
                                        }
                                    }
                                    if (if_one_then_repeated_and_delete == 0)
                                    {
                                        //Console.WriteLine("Arc_name_all_different_from_all_existed");
                                    }
                                    else
                                    {
                                        RID_same_Indi_STVs_with_new = RID_GBRVAR_list_existed.Key;
                                        break;
                                        //Console.WriteLine("Arc_name_same"+ arc_new.arcVarName.ToString());
                                    }
                                }
                            }
                            else
                            {
                                if_one_then_repeated_and_delete = 0;
                            }
                            if (if_one_then_repeated_and_delete == 1)
                            {
                                if (route_belong_type.ToString() == "pasDown" && CostDic_all["pasUp"][RID_same_Indi_STVs_with_new.ToString()] < cost*100)
                                {
                                    RouteID routeID_pas_tobe_removed = pasUp_RID_name_dic[YDic_KV.Key.ToString().Replace("Down", "Up")];
                                    CostDic_all[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc_all[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc_one_iter[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc_combined_all[pasUp].Remove(routeID_pas_tobe_removed);
                                    continue;
                                }
                                else if (route_belong_type.ToString() == "pasDown" && CostDic_all["pasUp"][RID_same_Indi_STVs_with_new] > cost*100)
                                {
                                    RouteID routeID_pas_tobe_removed = pasUp_RID_name_dic[RID_same_Indi_STVs_with_new.ToString().Replace("Down", "Up")];
                                    CostDic_all[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc_all[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc_one_iter[pasUp].Remove(routeID_pas_tobe_removed);
                                    DicTypeRID_Arc_combined_all[pasUp].Remove(routeID_pas_tobe_removed);
                                    CostDic_all[pasDown].Remove(RID_same_Indi_STVs_with_new);
                                    DicTypeRID_Arc[pasDown].Remove(RID_same_Indi_STVs_with_new);
                                    DicTypeRID_Arc_all[pasDown].Remove(RID_same_Indi_STVs_with_new);
                                    DicTypeRID_Arc_one_iter[pasDown].Remove(RID_same_Indi_STVs_with_new);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }


                    RouteID routeID;
                    if (route_belong_type.ToString() == "veh")
                    {
                        ID_sequence_veh = ID_sequence_veh + 1;
                        routeID = "R_" + route_belong_type.ToString() + "_No_" + ID_sequence_veh.ToString()+"_served_by_"+ YDic_KV.Key.ToString().Replace("vis","vs").Replace("pas","ps");
                        if (routeID.servedWhom == null)
                        {
                            routeID.servedWhom = new List<IndividualID>();
                        }
                        foreach (ArcGRBVar arcGRBVar in YList)
                        {
                            if (arcGRBVar.GRBV.X == 0) { continue; }
                            if (arcGRBVar.arc.arcType.ToString() != typeOper.ToString()) { continue;
                                                                                          }
                            SpaceTimeVertices spaceTimeVertices = arcGRBVar.arc.spaceTimeVertices;
                            if (spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.O_or_D_or_M_none.ToString()=="O"
                                && (spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString().Replace("Op", "p") ==
                                spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString()))
                            {
                                routeID.servedWhom.Add(spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID);
                            }
                            else if (spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.O_or_D_or_M_none.ToString()=="D"
                                && (spaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString().Replace("Dp", "p") ==
                                spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString()))
                            {
                                routeID.servedWhom.Add(spaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID);
                            }
                        }
                    }
                    else if (route_belong_type.ToString() == "pasUp")
                    {
                        ID_sequence_pas = ID_sequence_pas + 1;
                        routeID = "R_pas_No_" + ID_sequence_pas.ToString() +"_serving_"+ YDic_KV.Key;
                        routeID.ID_Index = ID_sequence_pas;
                        if (routeID.servedWhom == null)
                        {
                            routeID.servedWhom = new List<IndividualID>();
                            routeID.servedWhom.Add(YDic_KV.Key);
                        }
                        else
                        {
                            routeID.servedWhom.Add(YDic_KV.Key);
                        }
                        pasUp_RID_name_dic.Add(YDic_KV.Key, routeID);

                    }
                    else
                    {
                        routeID = pasUp_RID_name_dic[YDic_KV.Key.ToString().Replace("Down", "Up")].ToString().Replace("Up", "Down");
                        routeID.ID_Index = pasUp_RID_name_dic[YDic_KV.Key.ToString().Replace("Down", "Up")].ID_Index;
                        if (routeID.servedWhom == null)
                        {
                            routeID.servedWhom = new List<IndividualID>();
                            routeID.servedWhom.Add(YDic_KV.Key);
                        }
                        else
                        {
                            routeID.servedWhom.Add(YDic_KV.Key);
                        }
                    }
                    routeID.whoIsServing = YDic_KV.Key;
                    CostDic_all[route_belong_type].Add(routeID, cost);
                    DicTypeRID_Arc_all[route_belong_type].Add(routeID, List_Arc_Y);
                    if (route_belong_type.ToString() == "pasUp")
                    {
                        DicTypeRID_Arc_combined_all[route_belong_type].Add(routeID, List_Arc_Y);
                    }
                    else if (route_belong_type.ToString() == "pasDown")
                    {
                        DicTypeRID_Arc_combined_all["pasUp"][routeID.ToString().Replace("serving_pasDown", "serving_pasUp")] = Program.Clone(DicTypeRID_Arc_combined_all["pasUp"][routeID.ToString().Replace("serving_pasDown", "serving_pasUp")]) as List<Arc>;
                        DicTypeRID_Arc_combined_all["pasUp"][routeID.ToString().Replace("serving_pasDown", "serving_pasUp")].AddRange(List_Arc_Y);
                    }
                    DicID_Arc.Add(routeID, List_Arc_Y);
                    if (!DicTypeRID_Arc_one_iter.ContainsKey(route_belong_type))
                    {
                        Dictionary<RouteID, List<Arc>> RID_ListArc = new Dictionary<RouteID, List<Arc>>();
                        DicTypeRID_Arc_one_iter.Add(route_belong_type, RID_ListArc);
                    }
                    DicTypeRID_Arc_one_iter[route_belong_type].Add(routeID, List_Arc_Y);
                }
                DicTypeRID_Arc.Add(route_belong_type, DicID_Arc);
            }

            foreach (IndividualType indiType in individualTypeList)
            {
                if (indiType== visveh)
                {
                    continue;
                }
                Dictionary<IndividualID, Dictionary<RouteID, double>> A_ID_ID_Dic;
                Dictionary<RouteID, Dictionary<IndividualID, double>> A_ID_ID_Dic2;
                if (!ADic_all2.ContainsKey(indiType))
                {
                    A_ID_ID_Dic2 = new Dictionary<RouteID, Dictionary<IndividualID, double>>();
                }
                else
                {
                    A_ID_ID_Dic2 = ADic_all2[indiType];
                }
                if (!ADic_all.ContainsKey(indiType))
                {
                    A_ID_ID_Dic = new Dictionary<IndividualID, Dictionary<RouteID, double>>();
                }
                else
                {
                    A_ID_ID_Dic = ADic_all[indiType];
                }
                foreach (var Type_ID_ListSTV_KV in DicTypeRID_Arc)
                {
                    IndividualType route_belong_type = Type_ID_ListSTV_KV.Key;
                    Dictionary<RouteID, List<Arc>> ID_STV = Type_ID_ListSTV_KV.Value;
                    if (indiType != veh)
                    {
                        if (route_belong_type!=veh)
                        {
                            if (indiType!= route_belong_type)
                            {
                                continue;
                            }
                            foreach (var ID_ListSTV_KV in ID_STV)
                            {
                                RouteID routeID = ID_ListSTV_KV.Key;
                                Dictionary<RouteID, double> A_ID_Dic;
                                Dictionary<IndividualID, double> A_ID_Dic2;
                                if (!A_ID_ID_Dic.ContainsKey(routeID.whoIsServing))
                                {
                                    A_ID_Dic = new Dictionary<RouteID, double>();
                                }
                                else
                                {
                                    A_ID_Dic = A_ID_ID_Dic[routeID.whoIsServing];
                                }
                                A_ID_Dic2 = new Dictionary<IndividualID, double>();
                                A_ID_Dic.Add(routeID, 1);
                                A_ID_ID_Dic.Remove(routeID.whoIsServing);
                                A_ID_ID_Dic.Add(routeID.whoIsServing, A_ID_Dic);
                                A_ID_Dic2.Add(routeID.whoIsServing, 1);
                                A_ID_ID_Dic2.Add(routeID, A_ID_Dic2);
                            }
                        }
                        else
                        {
                            if (DicTypeRID_Arc.ContainsKey(veh))
                            {
                                Dictionary<RouteID, List<Arc>> veh_IDSTV = DicTypeRID_Arc[veh];
                                foreach (var ID_ListSTV in veh_IDSTV)
                                {
                                    RouteID routeIDveh = ID_ListSTV.Key;
                                    List<IndividualID> veh_served_whom = routeIDveh.servedWhom;
                                    foreach (IndividualID IndividualIDpas in veh_served_whom)
                                    {
                                        if (A_ID_ID_Dic.ContainsKey(IndividualIDpas))
                                        {
                                            A_ID_ID_Dic[IndividualIDpas].Add(routeIDveh, 1);
                                        }
                                    }
                                }
                            }
                        }
                        ADic_all.Remove(indiType);
                        ADic_all.Add(indiType, A_ID_ID_Dic);
                    }
                    else
                    {
                        if (route_belong_type!=veh)
                        {
                            continue;
                        }
                        Dictionary<RouteID, List<Arc>> veh_IDSTV = DicTypeRID_Arc[veh];
                        foreach (var ID_ListSTV in veh_IDSTV)
                        {
                            RouteID routeIDveh = ID_ListSTV.Key;
                            List<IndividualID> veh_served_whom = routeIDveh.servedWhom;
                            Dictionary<IndividualID, double> A_ID_Dic2 = new Dictionary<IndividualID, double>();
                            foreach (IndividualID IndividualIDpas in veh_served_whom)
                            {
                                A_ID_Dic2.Add(IndividualIDpas, 1);
                            }
                            A_ID_ID_Dic2.Add(routeIDveh, A_ID_Dic2);
                        }
                    }
                    ADic_all2.Remove(indiType);
                    ADic_all2.Add(indiType, A_ID_ID_Dic2);
                }
            }


           




            foreach (var Type_RouteID_ListSTV_KV in DicTypeRID_Arc)
            {
                IndividualType route_belong_type = Type_RouteID_ListSTV_KV.Key;
                Dictionary<RouteID, List<Arc>> Dic_ID_STV = Type_RouteID_ListSTV_KV.Value;
                Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>> B_STV_ID_Dic;
                Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>> B_ID_STV_Dic2;
                if (!BDic_all.ContainsKey(route_belong_type))
                {
                    B_STV_ID_Dic = new Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>();
                }
                else
                {
                    B_STV_ID_Dic = BDic_all[route_belong_type];
                }
                if (!BDic_all2.ContainsKey(route_belong_type))
                {
                    B_ID_STV_Dic2 = new Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>();
                }
                else
                {
                    B_ID_STV_Dic2 = BDic_all2[route_belong_type];
                }
                foreach (var ID_ListSTV_KV in Dic_ID_STV)
                {
                    IndividualID IDPas = ID_ListSTV_KV.Key.whoIsServing;
                    RouteID routeID = ID_ListSTV_KV.Key;
                    List<Arc> ListSpaceTimeVertices = ID_ListSTV_KV.Value;
                    Dictionary<SpaceTimeVertices, double> B_STV_Dic2;
                    Dictionary<RouteID, double> B_ID_Dic;
                    if (!B_ID_STV_Dic2.ContainsKey(routeID))
                    {
                        B_STV_Dic2 = new Dictionary<SpaceTimeVertices, double>();
                    }
                    else
                    {
                        B_STV_Dic2 = B_ID_STV_Dic2[routeID];
                    }

                    foreach (Arc arc in ListSpaceTimeVertices)
                    {
                        B_STV_Dic2.Add(arc.spaceTimeVertices, 1);
                        if (B_STV_ID_Dic.ContainsKey(arc.spaceTimeVertices))
                        {
                            B_STV_ID_Dic[arc.spaceTimeVertices].Add(routeID, 1);
                            continue;
                        }
                        else
                        {
                            B_ID_Dic = new Dictionary<RouteID, double>();
                            B_ID_Dic.Add(routeID, 1);
                            B_STV_ID_Dic.Add(arc.spaceTimeVertices, B_ID_Dic);
                        }
                    }
                    B_ID_STV_Dic2.Add(routeID, B_STV_Dic2);
                }

                BDic_all.Remove(route_belong_type);
                BDic_all2.Remove(route_belong_type);
                BDic_all.Add(route_belong_type, B_STV_ID_Dic);
                BDic_all2.Add(route_belong_type, B_ID_STV_Dic2);
            }

            List<SpaceTimeVertices> pasUpSTV_List = BDic_all[pasUp].Keys.ToList();
            List<SpaceTimeVertices> pasDownSTV_List = BDic_all[pasDown].Keys.ToList();
            pasSTV_List = pasUpSTV_List.Union(pasDownSTV_List).ToList<SpaceTimeVertices>();



        }
    }
}
