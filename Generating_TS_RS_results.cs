using Gurobi;
using OptimizationIntegrationSystem.zhenghao.ClassName;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao
{
    static class Generating_TS_RS_results
    {
        public static void Generating_TS_RS_results_method()
        {
            DateTime start = DateTime.Now; //获取代码段执行开始时的时间
            List<string> corLines_all = new List<string>();
            List<string> tWLines_all = new List<string>();
            Dictionary<IndividualID, List<string>> pas_for_specific_veh_CorLines_all = new Dictionary<IndividualID, List<string>>();
            Dictionary<IndividualID, List<string>> pas_for_specific_veh_tWLines_all = new Dictionary<IndividualID, List<string>>();
            Dictionary<IndividualID, List<IndividualID>> pas_for_specific_veh_all = new Dictionary<IndividualID, List<IndividualID>>();
            Dictionary<int,Dictionary<string, int>> ServedOrNot_string_int_dic_all = new Dictionary<int, Dictionary<string, int>>();
            Dictionary<int, Dictionary<string, int>> PassengerP_string_int_dic_all = new Dictionary<int, Dictionary<string, int>>();
            for (int caseIndex = 1; caseIndex < 17; caseIndex++)
            {
                Dictionary<string, int> ServedOrNot_string_int_dic = new Dictionary<string, int>();
                Dictionary<string, int> PassengerP_string_int_dic = new Dictionary<string, int>();
                int served_passengers_all = 0;
                int all_passengers = 0;
                double total_cost_all = 0;
                string FilePath = "file220501_12";
                string filepath_TsOrNotRideShareOrNot = @"" + FilePath + "\\" + "TsOrNotRideShareOrNot0501_12" + "\\" + "TsOrNotRideShareOrNot" + caseIndex.ToString()+ ".txt";
                for (int i = 1; i < 6; i++)
                {
                    int pasNum = i * 24;
                    int vehNum = 4;
                    int startNumpas = 1 + (i - 1) * 24;
                    int startNumveh = 1;
                    string filepathCorVeh = @"" + FilePath + "\\" + "testCorVeh.txt";
                    string filepathTWVeh = @"" + FilePath + "\\" + "testTWVeh.txt";
                    string filepathCorPas = @"" + FilePath + "\\" + "testCorPas.txt";
                    string filepathTWPas = @"" + FilePath + "\\" + "testTWPas.txt";
                    string corfilepath_dy = @"" + FilePath + "\\" + "testCorPas_dy.txt";
                    string tWfilepath_dy = @"" + FilePath + "\\" + "testTWPas_dy.txt";
                    IndividualType individualTypePas = "pas";
                    IndividualType individualTypeVeh = "veh";
                    IndividualType individualTypeVehCor = "vehCor";
                    int capacity = 4;
                    int pointNum0 = 0;
                    int tsTime = 2;
                    int optime = 1;

                    string[] corLines = File.ReadAllLines(filepathCorPas);
                    string[] tWLines = File.ReadAllLines(filepathTWPas);

                    StreamWriter sw = new StreamWriter(@"" + FilePath + "\\" + FilePath + "_ConsoleOutput.txt");
                    Console.SetOut(sw);

                    Dictionary<IndividualID, List<PointTW>> coorTwDicVeh;
                    Dictionary<IndividualID, List<PointTW>> coordTWDic_visveh;
                    Dictionary<IndividualID, List<PointTW>> coordTWDic_visvehCor;
                    Dictionary<IndividualID, List<PointTW>> coordTWDic_nothing;
                    Dictionary<IndividualID, List<PointTW>> coorTWDicVehCor;
                    Dictionary<IndividualID, List<PointTW>> coorTwDicPas_static;

                    Program.GetCoordTWDic_Dy(filepathCorVeh, filepathTWVeh, individualTypeVeh, pointNum0, startNumveh, vehNum, out coorTwDicVeh, out coordTWDic_nothing, out coordTWDic_nothing, out int pointNumVeh);
                    Program.GetCoordTWDic_Dy(filepathCorVeh, filepathTWVeh, individualTypeVehCor, pointNum0, startNumveh, vehNum, out coorTWDicVehCor, out coordTWDic_nothing, out coordTWDic_nothing, out int pointNumVeh_use);
                    Program.GetCoordTWDic_Dy(filepathCorPas, filepathTWPas, individualTypePas, pointNum0, startNumpas, pasNum, out coorTwDicPas_static, out coordTWDic_visveh, out coordTWDic_visvehCor, out int pasNum_Sta);
                    Program.GetTsOrNotandRideshareOrNot(filepath_TsOrNotRideShareOrNot,
                    pointNum0, startNumpas, pasNum,
                    out Dictionary<IndividualID, int> indiID_TS_int,
                    out Dictionary<IndividualID, int> indiID_RS_int);


                    coorTwDicVeh = coorTwDicVeh.Concat(coordTWDic_visveh).ToDictionary(postParK => postParK.Key, PostParV => PostParV.Value);
                    coorTWDicVehCor = coorTWDicVehCor.Concat(coordTWDic_visvehCor).ToDictionary(postParK => postParK.Key, PostParV => PostParV.Value);

                    RoadNetwork_no_vis roadNetwork = new RoadNetwork_no_vis(coorTwDicPas_static, coorTWDicVehCor, coorTwDicVeh, tsTime, optime);
                    //这些应该调用的时候放到main里面去
                    roadNetwork.GetArc(
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
                    out Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID,
                    out Dictionary<IndividualType, List<Arc>> arcDicIndividualType,
                    out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex,
                    out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex,
                    out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
                    out List<SpaceTimeVertices> ListSpaceTimeVertices);


                    vrp_time_space vrp_time_space = new vrp_time_space(arcDic, arcDicIndiTypeIDArcType, arcDicIndiTypeID, capacity, DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex, ListSpaceTimeVertices, indiID_TS_int, indiID_RS_int);
                    GRBModel myModel_sta = vrp_time_space.SetMyModel();
                    vrp_time_space.Generating_Vars_Obj(myModel_sta,
                    out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>> VarY_TypeIDArc_Dic,
                    out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
                    out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
                    out Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
                    out Dictionary<IndividualID, Dictionary<IndividualID, IndexGRBVar>> VarZDic_pas_Z);
                    vrp_time_space.StartStaticModelOptimization(arcDic, VarYDic, VarY_TypeIDArc_Dic, ArcGRBVarY_ID_ijtts, ArcGRBVarYListijtts, VarZDic, myModel_sta, out List<IndividualID> pasList);
                    myModel_sta.Write("result_sta_no_vis" + FilePath + ".lp");
                    string elapsedTime = Program.GetOperationalTime(start, pasNum, vehNum, myModel_sta.ObjVal);
                    Program.Generating_Results(myModel_sta, VarZDic, VarYDic,
        out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic,
        out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic2,
        out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_timestring_dic,
        out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_combined_string_dic);

                    Program.Generating_Analytical_Results(myModel_sta, VarZDic, arcDic, indiID_TS_int, indiID_RS_int,
                         ServedOrNot_string_int_dic,
                         served_passengers_all,
                         all_passengers, total_cost_all,
                         out ServedOrNot_string_int_dic,
                         out served_passengers_all,
                         out all_passengers, out total_cost_all);
                    Program.Generating_Passenger_Proportion(myModel_sta, arcDic, VarZDic_pas_Z, PassengerP_string_int_dic,
         out PassengerP_string_int_dic);
                    Dictionary<int, int> BB_sequence_iter = new Dictionary<int, int>();
                    Dictionary<int, double> iter_and_objective = new Dictionary<int, double>();
                    List<BB_node> BB_node_list = new List<BB_node>();
                    Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_string_dic_all = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
                    Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_string_dic_all2 = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
                    Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_timestring_dic_all = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
                    Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_combined_string_dic_all = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
                    Y_string_dic_all.Add(1, Y_typeIDRID_string_dic);//第一个iter的结果是inisolution的结果
                    Y_string_dic_all2.Add(1, Y_typeIDRID_string_dic2);
                    Y_timestring_dic_all.Add(1, Y_typeIDRID_timestring_dic);
                    Y_combined_string_dic_all.Add(1, Y_typeIDRID_combined_string_dic);
                    Program.GeneratingRoutes(Y_string_dic_all, Y_string_dic_all2, Y_timestring_dic_all, Y_combined_string_dic_all,
                        iter_and_objective, BB_sequence_iter, 1, BB_node_list, arcDicIndiTypeID);

                    List<IndividualID> chosen_pasList = new List<IndividualID>();
                    foreach (var c in VarYDic)
                    {
                        if (c.Key.ToString() == "veh" || c.Key.ToString() == "pasDown")
                        {
                            continue;
                        }
                        foreach (var d in c.Value)
                        {
                            List<ArcGRBVar> ArcYList = d.Value;
                            ArcYList = ArcYList.OrderBy(a => (a.arc.timeVertices[1] + a.arc.timeVertices[0])).ToList();
                            ArcGRBVar startArc = ArcYList.Where(x => x.arc.arcType.ToString() == "start").ToList()[0];
                            ArcGRBVar endArc = ArcYList.Where(x => x.arc.arcType.ToString() == "end").ToList()[0];
                            int startArc_index = ArcYList.IndexOf(startArc);
                            int endArc_index = ArcYList.IndexOf(endArc);
                            ArcYList.RemoveAt(endArc_index);
                            ArcYList.RemoveAt(startArc_index);
                            ArcYList.Insert(0, startArc);
                            ArcYList.Add(endArc);
                            foreach (ArcGRBVar arcGRBVar in ArcYList)
                            {
                                if (arcGRBVar.GRBV.X == 0)
                                {
                                    continue;
                                }
                                chosen_pasList.Add(d.Key);
                                break;
                            }
                        }
                    }
                    Console.WriteLine();
                    sw.Flush();
                    sw.Close();
                }

                Program.Generating_TS_RS_Results(caseIndex, FilePath,
                ServedOrNot_string_int_dic,
                served_passengers_all,
                all_passengers, total_cost_all);
                Program.Generating_Passenger_Proportion_Results(caseIndex, FilePath,
                PassengerP_string_int_dic);
                ServedOrNot_string_int_dic_all.Add(caseIndex, ServedOrNot_string_int_dic);
                PassengerP_string_int_dic_all.Add(caseIndex, PassengerP_string_int_dic);
            }
        }
    }
}
