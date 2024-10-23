using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OptimizationIntegrationSystem.zhenghao;
using OptimizationIntegrationSystem.zhenghao.ClassName;
using Gurobi;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;

namespace OptimizationIntegrationSystem.zhenghao
{

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        public static T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }

        public static object Clone(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return formatter.Deserialize(memoryStream);
        }

        public static T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        public static List<BB_node> Generating_new_node(
        BB_info BB_info_new, BB_node BB_node_processing, int BB_sequence, List<BB_node> BB_node_list,
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all,
        Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all,
        Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2,
        Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2,
        Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all,
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all,
        Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all,
        Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices,
        out int BB_sequence_new)
        {
            IndividualType BB_type = BB_info_new.BB_indiType;

            BB_node BB_node_0 = new BB_node();
            BB_node_0 = DeepCopyByBin(BB_node_processing);
            BB_sequence = BB_sequence + 1;
            BB_node_0.BB_sequence = BB_sequence;
            BB_info BB_info_new_0 = new BB_info();
            BB_info_new_0 = DeepCopyByBin(BB_info_new);
            BB_info_new_0.BB_zero_or_one = 0;

            BB_node BB_node_1 = new BB_node();
            BB_node_1 = DeepCopyByBin(BB_node_processing);
            BB_sequence = BB_sequence + 1;
            BB_node_1.BB_sequence = BB_sequence;
            BB_info BB_info_new_1 = new BB_info();
            BB_info_new_1 = DeepCopyByBin(BB_info_new);
            BB_info_new_1.BB_zero_or_one = 1;

            BB_sequence_new=BB_sequence;
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all_0 = Clone(ADic_all) as Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>;
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all_0 = Clone(BDic_all) as Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>;
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2_0 = Clone(ADic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>;
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2_0 = Clone(BDic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>;
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all_0 = Clone(CostDic_all) as Dictionary<IndividualType, Dictionary<RouteID, double>>;
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all_0 = Clone(DicTypeRID_Arc_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all_0 = Clone(DicTypeRID_Arc_combined_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
            //Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_0 = Clone(DicTypeRID_Arc) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;

            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all_1 = Clone(ADic_all) as Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>;
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all_1 = Clone(BDic_all) as Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>;
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2_1 = Clone(ADic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>;
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2_1 = Clone(BDic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>;
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all_1 = Clone(CostDic_all) as Dictionary<IndividualType, Dictionary<RouteID, double>>;
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all_1 = Clone(DicTypeRID_Arc_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all_1 = Clone(DicTypeRID_Arc_combined_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
            //Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_1 = Clone(DicTypeRID_Arc) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;


            List<RouteID> RID_deleted_veh_0 = new List<RouteID>();
            List<RouteID> RID_deleted_pas_0 = new List<RouteID>();//如果是pasUp的路径不能被选择，那么对应的pasDown也不能被选择；pasDown同理
            List<RouteID> RID_deleted_veh_1 = new List<RouteID>();
            List<RouteID> RID_deleted_pas_1 = new List<RouteID>();
            SpaceTimeVertices STV = DicSpaceTimeVertices[BB_info_new.BB_indiType][BB_info_new.BB_indiID][BB_info_new.BB_type_ID_index];
            if (BB_type.ToString() == "veh")
            {
                BB_node_0.veh_BB_info_list.Add(BB_info_new_0);
                BB_node_1.veh_BB_info_list.Add(BB_info_new_1);
                foreach (var RID_STVdoubles in BDic_all2[BB_info_new.BB_indiType])
                {
                    if (ADic_all2[BB_info_new.BB_indiType][RID_STVdoubles.Key].ContainsKey(BB_info_new.BB_indiID))
                    {
                        if (!RID_STVdoubles.Value.ContainsKey(STV))
                        {

                            RID_deleted_veh_1.Add(RID_STVdoubles.Key);
                        }
                        else
                        {
                            RID_deleted_veh_0.Add(RID_STVdoubles.Key);
                        }
                    }
                }
                foreach (RouteID RID_veh in RID_deleted_veh_1)
                {
                    ADic_all2_1["veh"].Remove(RID_veh);
                    foreach (var type_ID_RID_double in ADic_all_1)
                    {
                        foreach (var ID_RID_double in type_ID_RID_double.Value)
                        {
                            if (ID_RID_double.Value.ContainsKey(RID_veh))
                            {
                                ADic_all_1[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_veh);
                            }
                        }
                    }
                    BDic_all2_1["veh"].Remove(RID_veh);
                    CostDic_all_1["veh"].Remove(RID_veh);
                    //DicTypeRID_Arc_1["veh"].Remove(RID_veh);
                    DicTypeRID_Arc_all_1["veh"].Remove(RID_veh);
                    DicTypeRID_Arc_combined_all_1["veh"].Remove(RID_veh);
                }
                foreach (RouteID RID_veh in RID_deleted_veh_0)
                {
                    ADic_all2_0["veh"].Remove(RID_veh);
                    foreach (var type_ID_RID_double in ADic_all_0)
                    {
                        foreach (var ID_RID_double in type_ID_RID_double.Value)
                        {
                            if (ID_RID_double.Value.ContainsKey(RID_veh))
                            {
                                ADic_all_0[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_veh);
                            }
                        }
                    }
                    BDic_all2_0["veh"].Remove(RID_veh);
                    CostDic_all_0["veh"].Remove(RID_veh);
                    //DicTypeRID_Arc_0["veh"].Remove(RID_veh);
                    DicTypeRID_Arc_all_0["veh"].Remove(RID_veh);
                    DicTypeRID_Arc_combined_all_0["veh"].Remove(RID_veh);

                }
            }
            else
            {
                BB_node_0.pas_BB_info_list.Add(BB_info_new_0);
                BB_node_1.pas_BB_info_list.Add(BB_info_new_1);


                foreach (var RID_STVdoubles in BDic_all2[BB_info_new.BB_indiType])
                {
                    if (ADic_all2[BB_info_new.BB_indiType][RID_STVdoubles.Key].ContainsKey(BB_info_new.BB_indiID))
                    {
                        if (!RID_STVdoubles.Value.ContainsKey(STV))
                        {

                            RID_deleted_pas_1.Add(RID_STVdoubles.Key);
                        }
                        else
                        {
                            RID_deleted_pas_0.Add(RID_STVdoubles.Key);
                        }
                    }
                }

                foreach (RouteID RID_pas in RID_deleted_pas_1)
                {
                    foreach (var type_RID_ID_double in ADic_all2_1)
                    {
                        if (type_RID_ID_double.Value.ContainsKey(RID_pas))
                        {
                            ADic_all2_1[type_RID_ID_double.Key].Remove(RID_pas);
                        }
                        if (type_RID_ID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown")))
                        {
                            ADic_all2_1[type_RID_ID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                        }
                        if (type_RID_ID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp")))
                        {
                            ADic_all2_1[type_RID_ID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                        }
                    }
                    foreach (var type_ID_RID_double in ADic_all_1)
                    {
                        foreach (var ID_RID_double in type_ID_RID_double.Value)
                        {
                            if (ID_RID_double.Value.ContainsKey(RID_pas))
                            {
                                ADic_all_1[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_pas);
                            }
                            if (ID_RID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown")))
                            {
                                ADic_all_1[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                            }
                            if (ID_RID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp")))
                            {
                                ADic_all_1[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                            }
                        }
                    }
                    BDic_all2_1["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    CostDic_all_1["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    //DicTypeRID_Arc_1["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    DicTypeRID_Arc_all_1["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    DicTypeRID_Arc_combined_all_1["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    BDic_all2_1["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                    CostDic_all_1["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                    //DicTypeRID_Arc_1["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                    DicTypeRID_Arc_all_1["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                }

                foreach (RouteID RID_pas in RID_deleted_pas_0)
                {
                    foreach (var type_RID_ID_double in ADic_all2_0)
                    {
                        if (type_RID_ID_double.Value.ContainsKey(RID_pas))
                        {
                            ADic_all2_0[type_RID_ID_double.Key].Remove(RID_pas);
                        }
                        if (type_RID_ID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown")))
                        {
                            ADic_all2_0[type_RID_ID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                        }
                        if (type_RID_ID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp")))
                        {
                            ADic_all2_0[type_RID_ID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                        }
                    }
                    foreach (var type_ID_RID_double in ADic_all_0)
                    {
                        foreach (var ID_RID_double in type_ID_RID_double.Value)
                        {
                            if (ID_RID_double.Value.ContainsKey(RID_pas))
                            {
                                ADic_all_0[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_pas);
                            }
                            if (ID_RID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown")))
                            {
                                ADic_all_0[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                            }
                            if (ID_RID_double.Value.ContainsKey(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp")))
                            {
                                ADic_all_0[type_ID_RID_double.Key][ID_RID_double.Key].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                            }
                        }
                    }
                    BDic_all2_0["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    CostDic_all_0["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    //DicTypeRID_Arc_0["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    DicTypeRID_Arc_all_0["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    DicTypeRID_Arc_combined_all_0["pasUp"].Remove(RID_pas.ToString().Replace("serving_pasDown", "serving_pasUp"));
                    BDic_all2_0["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                    CostDic_all_0["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                    //DicTypeRID_Arc_0["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                    DicTypeRID_Arc_all_0["pasDown"].Remove(RID_pas.ToString().Replace("serving_pasUp", "serving_pasDown"));
                }
            }

            BB_node_1.ADic_all = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>(ADic_all_1);
            BB_node_1.BDic_all = new Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>(BDic_all_1);
            BB_node_1.ADic_all2 = new Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>(ADic_all2_1);
            BB_node_1.BDic_all2 = new Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>(BDic_all2_1);
            BB_node_1.CostDic_all = new Dictionary<IndividualType, Dictionary<RouteID, double>>(CostDic_all_1);
            BB_node_1.DicTypeRID_Arc_all = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>(DicTypeRID_Arc_all_1);
            BB_node_1.DicTypeRID_Arc_combined_all = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>(DicTypeRID_Arc_combined_all_1);
            //BB_node_1.DicTypeRID_Arc = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>(DicTypeRID_Arc_1);

            BB_node_0.ADic_all = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>(ADic_all_0);
            BB_node_0.BDic_all = new Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>(BDic_all_0);
            BB_node_0.ADic_all2 = new Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>(ADic_all2_0);
            BB_node_0.BDic_all2 = new Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>(BDic_all2_0);
            BB_node_0.CostDic_all = new Dictionary<IndividualType, Dictionary<RouteID, double>>(CostDic_all_0);
            BB_node_0.DicTypeRID_Arc_all = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>(DicTypeRID_Arc_all_0);
            BB_node_0.DicTypeRID_Arc_combined_all = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>(DicTypeRID_Arc_combined_all_0);
            //BB_node_0.DicTypeRID_Arc = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>(DicTypeRID_Arc_0);


            List<BB_node> BB_node_list_new = Clone(BB_node_list) as List<BB_node>;
            BB_node_list_new.Add(BB_node_1);
            BB_node_list_new.Add(BB_node_0);

            return BB_node_list_new;
        }


        public static string Y_to_notation(IndividualID i_ID)
        {
            string route_string2 = "";
            if (i_ID.ToString()==("vehOut1"))
            {

            }
            if (i_ID.UpOrDown==("Up") && i_ID.individualType==("pas"))//i点为pasUp或OpasUp或MpasUp
            {
                route_string2 += "o_(p,";
                if (i_ID.O_or_D_or_M_none == "none")//pasUp
                {
                    route_string2 += "s";
                }
                else if (i_ID.O_or_D_or_M_none == "O")//OpasUp
                {
                    route_string2 += "u";
                }
                else if (i_ID.O_or_D_or_M_none == "M")//MpasUp
                {
                    route_string2 += "m";
                }
                route_string2 += ")^(";
                //route_string2 += System.Text.RegularExpressions.Regex.Replace(i, @"[^0-9]+", "");
                route_string2 += i_ID.IDIndex.ToString() + ")";
            }
            else if (i_ID.UpOrDown == ("Down") && i_ID.individualType == ("pas"))
            {
                route_string2 += "d_(p,";
                if (i_ID.O_or_D_or_M_none == "none")//paDown
                {
                    route_string2 += "u";
                }
                else if (i_ID.O_or_D_or_M_none == "D")
                {
                    route_string2 += "s";
                }
                else if (i_ID.O_or_D_or_M_none == "M")
                {
                    route_string2 += "m";
                }
                route_string2 += ")^(";
                //route_string2 += System.Text.RegularExpressions.Regex.Replace(i, @"[^0-9]+", "");
                route_string2 += i_ID.IDIndex.ToString() + ")";
            }
            else if (i_ID.individualType.ToString()==("veh")|| i_ID.individualType.ToString() == ("vehCor"))
            {
                if (i_ID.OutOrBack==("Out"))
                {
                    route_string2 += "o_(v";
                }
                else if (i_ID.OutOrBack ==("Back"))
                {
                    route_string2 += "d_(v";
                }
                route_string2 += ")^(";
                //route_string2 += System.Text.RegularExpressions.Regex.Replace(i, @"[^0-9]+", "");
                route_string2 += i_ID.IDIndex.ToString()+")";
            }
            else if (i_ID.individualType.ToString() == ("visveh"))
            {
                if (i_ID.OutOrBack == "Out")
                {
                    route_string2 += "o_(vis";
                }
                else if (i_ID.OutOrBack==("Back"))
                {
                    route_string2 += "d_(vis";
                }
                if (i_ID.UpOrDown == "Up")
                {
                    route_string2 += "^+";
                }
                else if (i_ID.UpOrDown == "Down")
                {
                    route_string2 += "^-";
                }
                route_string2 += ")^(";
                //route_string2 += System.Text.RegularExpressions.Regex.Replace(i, @"[^0-9]+", "");
                route_string2 += i_ID.IDIndex.ToString()+ ")";
            }
            else if (i_ID.ToString() == "O" || i_ID.ToString() == "VO")
            {
                route_string2 += "o";
            }
            else if (i_ID.ToString() == "TsO")
            {
                route_string2 += "ts_o";
            }
            else if (i_ID.ToString() == "D" || i_ID.ToString() == "VD")
            {
                route_string2 += "d";
            }
            else if (i_ID.ToString() == "TsD")
            {
                route_string2 += "ts_d";
            }
            return route_string2;
        }
            public static void GeneratingRoutes(
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_string_dic_all,
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_string_dic_all2,
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_timestring_dic_all,
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_combined_string_dic_all,
            Dictionary<int, double> iter_and_objective,
            Dictionary<int, int> BB_sequence_iter,int now_sequence, List<BB_node> BB_node_list,
            Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID)
        {
            //foreach (var iterTypeIDstring_KV in Y_string_dic_all)
            //{
            //    for (int i_sequence = 1; i_sequence <= now_sequence; i_sequence++)
            //    {
            //        if (BB_sequence_iter[i_sequence] == iterTypeIDstring_KV.Key)
            //        {
            //            Console.WriteLine("BB_node" + now_sequence + ":Branching:");
            //            foreach (BB_info BB_info_pas in BB_node_list[i_sequence].pas_BB_info_list)
            //            {
            //                Console.WriteLine(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].arcVarName);
            //                string i = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID.ToString();
            //                string j = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID.ToString();
            //                string t = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.timeVertex.ToString();
            //                string tt = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.timeVertex.ToString();
            //                string route_string_i = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID);
            //                string route_string_j = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID);
            //                Console.WriteLine("(" + route_string_i + "," + t + ")" + ",(" + route_string_j + "," + tt + ")");
            //            }
            //        }
            //    }

            //    Console.WriteLine("iter_num:" + iterTypeIDstring_KV.Key.ToString());
            //    foreach (var typeIDstring_KV in iterTypeIDstring_KV.Value)
            //    {
            //        foreach (var IDRIDstring_KV in typeIDstring_KV.Value)
            //        {
            //            foreach (var RIDstring_KV in IDRIDstring_KV.Value)
            //            {
            //                Console.WriteLine("IndiID:" + IDRIDstring_KV.Key.ToString()
            //                    + "RID:" + RIDstring_KV.Key + ":" + RIDstring_KV.Value+"; weight:"+ RIDstring_KV.Key.route_lambda.ToString());
            //            }
            //        }
            //    }
            //}
            ///
            //foreach (var iterTypeIDstring_KV in Y_string_dic_all2)
            //{
            //    for (int i_sequence = 1; i_sequence <= now_sequence; i_sequence++)
            //    {
            //        if (BB_sequence_iter[i_sequence] == iterTypeIDstring_KV.Key)
            //        {
            //            string BB_branching =("BB_node" + i_sequence + "; Branching:");
            //            foreach (BB_info BB_info_pas in BB_node_list[i_sequence].pas_BB_info_list)
            //            {
            //                string Individual_string = "";
            //                if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "pas")
            //                {
            //                    if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "p^+";
            //                    }
            //                    else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "p^-";
            //                    }
            //                }
            //                else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "veh")
            //                {
            //                    Individual_string += "v";
            //                }
            //                else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "visveh")
            //                {
            //                    Individual_string += "v^(vis";
            //                    if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "^+)";
            //                    }
            //                    else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "^-)";
            //                    }
            //                }
            //                Individual_string += "_" + arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.IDIndex.ToString()+":";
            //                //Console.WriteLine(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].arcVarName);
            //                string i = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID.ToString();
            //                string j = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID.ToString();
            //                string t = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.timeVertex.ToString();
            //                string tt = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.timeVertex.ToString();
            //                string route_string_i = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID);
            //                string route_string_j = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID);
            //                BB_branching += (Individual_string + "[(" + route_string_i + "," + t + ")" + ",(" + route_string_j + "," + tt + ")]=" + BB_info_pas.BB_zero_or_one + ";");
            //            }
            //            Console.WriteLine(BB_branching);
            //        }
            //    }
            //    Console.WriteLine("iter_num:" + iterTypeIDstring_KV.Key.ToString());
            //    foreach (var typeIDstring_KV in iterTypeIDstring_KV.Value)
            //    {
            //        foreach (var IDRIDstring_KV in typeIDstring_KV.Value)
            //        {
            //            foreach (var RIDstring_KV in IDRIDstring_KV.Value)
            //            {
            //                string Individual_string = "";
            //                if (IDRIDstring_KV.Key.individualType.ToString()=="pas")
            //                {
            //                    if (IDRIDstring_KV.Key.UpOrDown.ToString()=="Up")
            //                    {
            //                        Individual_string += "p^+";
            //                    }
            //                    else if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "p^-";
            //                    }
            //                }
            //                else if (IDRIDstring_KV.Key.individualType.ToString() == "veh")
            //                {
            //                    Individual_string += "v";
            //                }
            //                else if (IDRIDstring_KV.Key.individualType.ToString() == "visveh")
            //                {
            //                    Individual_string += "v^(vis";
            //                    if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "^+)";
            //                    }
            //                    else if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "^-)";
            //                    }
            //                }
            //                Individual_string +="_"+ IDRIDstring_KV.Key.IDIndex.ToString();
            //                Console.WriteLine(Individual_string
            //                    + ":" + RIDstring_KV.Value + "; weight:" + RIDstring_KV.Key.route_lambda.ToString());
            //                //Console.WriteLine(Individual_string
            //                //    + ":" + Y_timestring_dic_all[iterTypeIDstring_KV.Key][typeIDstring_KV.Key][IDRIDstring_KV.Key][RIDstring_KV.Key]);
            //            }
            //        }
            //    }
            //}
            /////
            //foreach (var iterTypeIDstring_KV in Y_timestring_dic_all)
            //{
            //    for (int i_sequence = 1; i_sequence <= now_sequence; i_sequence++)
            //    {
            //        if (BB_sequence_iter[i_sequence] == iterTypeIDstring_KV.Key)
            //        {
            //            string BB_branching = ("BB_node" + i_sequence + "; Branching:");
            //            foreach (BB_info BB_info_pas in BB_node_list[i_sequence].pas_BB_info_list)
            //            {
            //                string Individual_string = "";
            //                if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "pas")
            //                {
            //                    if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "p^+";
            //                    }
            //                    else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "p^-";
            //                    }
            //                }
            //                else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "veh")
            //                {
            //                    Individual_string += "v";
            //                }
            //                else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "visveh")
            //                {
            //                    Individual_string += "v^(vis";
            //                    if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "^+)";
            //                    }
            //                    else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "^-)";
            //                    }
            //                }
            //                Individual_string += "_" + arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.IDIndex.ToString() + ":";
            //                //Console.WriteLine(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].arcVarName);
            //                string i = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID.ToString();
            //                string j = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID.ToString();
            //                string t = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.timeVertex.ToString();
            //                string tt = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.timeVertex.ToString();
            //                string route_string_i = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID);
            //                string route_string_j = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID);
            //                BB_branching += (Individual_string + "[(" + route_string_i + "," + t + ")" + ",(" + route_string_j + "," + tt + ")]=" + BB_info_pas.BB_zero_or_one + ";");
            //            }
            //            Console.WriteLine(BB_branching);
            //        }
            //    }
            //    Console.WriteLine("iter_num:" + iterTypeIDstring_KV.Key.ToString());
            //    foreach (var typeIDstring_KV in iterTypeIDstring_KV.Value)
            //    {
            //        foreach (var IDRIDstring_KV in typeIDstring_KV.Value)
            //        {
            //            foreach (var RIDstring_KV in IDRIDstring_KV.Value)
            //            {
            //                string Individual_string = "";
            //                if (IDRIDstring_KV.Key.individualType.ToString() == "pas")
            //                {
            //                    if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "p^+";
            //                    }
            //                    else if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "p^-";
            //                    }
            //                }
            //                else if (IDRIDstring_KV.Key.individualType.ToString() == "veh")
            //                {
            //                    Individual_string += "v";
            //                }
            //                else if (IDRIDstring_KV.Key.individualType.ToString() == "visveh")
            //                {
            //                    Individual_string += "v^(vis";
            //                    if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Up")
            //                    {
            //                        Individual_string += "^+)";
            //                    }
            //                    else if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Down")
            //                    {
            //                        Individual_string += "^-)";
            //                    }
            //                }
            //                Individual_string += "_" + IDRIDstring_KV.Key.IDIndex.ToString();
            //                Console.WriteLine(Individual_string
            //                    + ":" + RIDstring_KV.Value);
            //            }
            //        }
            //    }
            //}
            ////
            foreach (var iterTypeIDstring_KV in Y_combined_string_dic_all)
            {
                if (BB_sequence_iter.Keys.ToList().Count!=0)
                {
                    for (int i_sequence = 1; i_sequence <= now_sequence; i_sequence++)
                    {
                        if (BB_sequence_iter[i_sequence] == iterTypeIDstring_KV.Key)
                        {
                            string BB_branching = ("BB_node" + i_sequence + "; Branching:");
                            foreach (BB_info BB_info_pas in BB_node_list[i_sequence].pas_BB_info_list)
                            {
                                string Individual_string = "";
                                if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "pas")
                                {
                                    if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Up")
                                    {
                                        Individual_string += "p^+";
                                    }
                                    else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Down")
                                    {
                                        Individual_string += "p^-";
                                    }
                                }
                                else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "veh")
                                {
                                    Individual_string += "v";
                                }
                                else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.individualType.ToString() == "visveh")
                                {
                                    Individual_string += "v^(vis";
                                    if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Up")
                                    {
                                        Individual_string += "^+)";
                                    }
                                    else if (arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.UpOrDown.ToString() == "Down")
                                    {
                                        Individual_string += "^-)";
                                    }
                                }
                                Individual_string += "_" + arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].individualID.IDIndex.ToString() + ":";
                                //Console.WriteLine(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].arcVarName);
                                string i = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID.ToString();
                                string j = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID.ToString();
                                string t = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.timeVertex.ToString();
                                string tt = arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.timeVertex.ToString();
                                string route_string_i = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex1.spaceVertex.individualID);
                                string route_string_j = Y_to_notation(arcDicIndiTypeID[BB_info_pas.BB_indiType][BB_info_pas.BB_indiID][BB_info_pas.BB_type_ID_index].SpaceTimeVertex2.spaceVertex.individualID);
                                BB_branching += (Individual_string + "[(" + route_string_i + "," + t + ")" + ",(" + route_string_j + "," + tt + ")]=" + BB_info_pas.BB_zero_or_one + ";");
                            }
                            Console.WriteLine(BB_branching);
                        }
                    }
                    Console.WriteLine("iter_num:" + iterTypeIDstring_KV.Key.ToString() + ", objective:" + iter_and_objective[iterTypeIDstring_KV.Key]);
                }

                foreach (var typeIDstring_KV in iterTypeIDstring_KV.Value)
                {
                    foreach (var IDRIDstring_KV in typeIDstring_KV.Value)
                    {
                        foreach (var RIDstring_KV in IDRIDstring_KV.Value)
                        {
                            string Individual_string = "";
                            if (IDRIDstring_KV.Key.individualType.ToString() == "pas")
                            {
                                if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Up")
                                {
                                    Individual_string += "p^+";
                                }
                                else if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Down")
                                {
                                    Individual_string += "p^-";
                                }
                            }
                            else if (IDRIDstring_KV.Key.individualType.ToString() == "veh")
                            {
                                Individual_string += "v";
                            }
                            else if (IDRIDstring_KV.Key.individualType.ToString() == "visveh")
                            {
                                Individual_string += "v^(vis";
                                if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Up")
                                {
                                    Individual_string += "^+)";
                                }
                                else if (IDRIDstring_KV.Key.UpOrDown.ToString() == "Down")
                                {
                                    Individual_string += "^-)";
                                }
                            }
                            Individual_string += "_" + IDRIDstring_KV.Key.IDIndex.ToString();
                            Console.WriteLine(Individual_string
                                + ":" + RIDstring_KV.Value + "; weight:" + RIDstring_KV.Key.route_lambda.ToString());
                        }
                    }
                }
            }

        }

        public static string GetOperationalTime(DateTime startTime, int pasNum, int vehNum,double objV)
        {
            DateTime stop = DateTime.Now; //获取代码段执行结束时的时间
            TimeSpan tspan = stop - startTime; //求时间差
            string time = (tspan.TotalMilliseconds / 1000).ToString(); //获取代码段执行时间
            using (StreamWriter w = File.AppendText(@"" + "..\\pointsNumAndTime.txt"))
            {
                w.WriteLine("pasNum:"+ pasNum.ToString()+ "; vehNum:"+ vehNum.ToString()+ "; elapsedTime:"+ time+ "; objV:"+ objV);
            }
            return time;
        }

        static void Main()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            //Try_static_solution.Try_static_solution_method();
            Generating_TS_RS_results.Generating_TS_RS_results_method();
            //String_beads.String_beads_method();
            String_beads0427.String_beads_method();
            string FilePath = "file0728_4";
            //string FilePath = "file0814_5_sta";
            string filepathCorVeh = @"" + FilePath + "\\" + "testCorVeh.txt";
            string filepathTWVeh = @"" + FilePath + "\\" + "testTWVeh.txt";
            string filepathCorPas = @"" + FilePath + "\\" + "testCorPas.txt";
            string filepathTWPas = @"" + FilePath + "\\" + "testTwPas.txt";
            string corfilepath_dy = @"" + FilePath + "\\" + "testCorPas_dy.txt";
            string tWfilepath_dy = @"" + FilePath + "\\" + "testTWPas_dy.txt";
            string filepath_TsOrNotRideShareOrNot = @"" + FilePath + "\\" + "TsOrNotRideShareOrNot.txt";
            IndividualType individualTypePas = "pas";
            IndividualType individualTypeVeh = "veh";
            IndividualType individualTypeVehCor = "vehCor";
            int capacity = 4;
            int pointNum0 = 0;
            int tsTime = 2;
            int optime = 1;
            int pasNum = 4;
            int vehNum = 3;
            int startNumpas = 1;
            int startNumveh = 1;
            Dictionary<IndividualID, List<PointTW>> coorTwDicVeh;
            Dictionary<IndividualID, List<PointTW>> coordTWDic_visveh;
            Dictionary<IndividualID, List<PointTW>> coordTWDic_visvehCor;
            Dictionary<IndividualID, List<PointTW>> coordTWDic_nothing;
            Dictionary<IndividualID, List<PointTW>> coorTWDicVehCor;
            Dictionary<IndividualID, List<PointTW>> coorTwDicPas_static;

            GetCoordTWDic_Dy(filepathCorVeh, filepathTWVeh, individualTypeVeh, pointNum0, startNumveh, vehNum, out coorTwDicVeh, out coordTWDic_nothing, out coordTWDic_nothing, out int pointNumVeh);
            GetCoordTWDic_Dy(filepathCorVeh, filepathTWVeh, individualTypeVehCor, pointNum0, startNumveh, vehNum, out coorTWDicVehCor, out coordTWDic_nothing, out coordTWDic_nothing, out int pointNumVeh_use);
            GetCoordTWDic_Dy(filepathCorPas, filepathTWPas, individualTypePas, pointNum0, startNumpas, pasNum, out coorTwDicPas_static, out coordTWDic_visveh, out coordTWDic_visvehCor, out int pasNum_Sta);
            Program.GetTsOrNotandRideshareOrNot(filepath_TsOrNotRideShareOrNot,
            pointNum0, startNumpas, pasNum,
            out Dictionary<IndividualID, int> indiID_TS_int,
            out Dictionary<IndividualID, int> indiID_RS_int);

            coorTwDicVeh = coorTwDicVeh.Concat(coordTWDic_visveh).ToDictionary(postParK => postParK.Key, PostParV => PostParV.Value);
            coorTWDicVehCor = coorTWDicVehCor.Concat(coordTWDic_visvehCor).ToDictionary(postParK => postParK.Key, PostParV => PostParV.Value);

            RoadNetwork roadNetwork = new RoadNetwork(coorTwDicPas_static, coorTWDicVehCor, coorTwDicVeh, tsTime, optime);
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


            //vrp_time_space vrp_time_space = new vrp_time_space(arcDic, arcDicIndiTypeIDArcType, arcDicIndiTypeID, capacity, DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex, ListSpaceTimeVertices);
            //GRBModel myModel_sta = vrp_time_space.SetMyModel();
            //vrp_time_space.Generating_Vars_Obj(myModel_sta,
            //out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            //out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
            //out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
            //out Dictionary<IndividualType, List<IndexGRBVar>> VarZDic);
            //vrp_time_space.StartStaticModelOptimization(arcDic, VarYDic, ArcGRBVarY_ID_ijtts, ArcGRBVarYListijtts, VarZDic, myModel_sta, out List<IndividualID> pasList);
            //myModel_sta.Write("result_sta_" + FilePath + ".lp");
            //Generating_Results(myModel_sta, VarZDic, VarYDic);


            Console.WriteLine("=========================constructing: initial solution=========================");
            vrp_time_space_ini vrp_time_space_ini = new vrp_time_space_ini(arcDic, arcDicIndiTypeIDArcType, arcDicIndiTypeID, capacity, DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex, ListSpaceTimeVertices);
            GRBModel myModel_sta = vrp_time_space_ini.SetIniModel();
            vrp_time_space_ini.Generating_Vars_Obj(myModel_sta,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts,
            out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts,
            out Dictionary<IndividualType, List<IndexGRBVar>> VarZDic);
            vrp_time_space_ini.StartIniOptimization(arcDic, VarYDic, ArcGRBVarY_ID_ijtts, ArcGRBVarYListijtts, VarZDic, myModel_sta, out List<IndividualID> pasList);
            Console.WriteLine("============================generating results: initial solution============================");
            Generating_Results(myModel_sta, VarZDic, VarYDic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic2,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_timestring_dic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_combined_string_dic);
            myModel_sta.Write("result_ini_" + FilePath + ".lp");
            Console.WriteLine("============================generating results end: initial solution============================");

            Console.WriteLine("=========================constructing: vispas solution=========================");
            vrp_time_space_vis_pas vrp_time_space_vis_pas = new vrp_time_space_vis_pas(arcDic, arcDicIndiTypeIDArcType, arcDicIndiTypeID, capacity, DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex, ListSpaceTimeVertices);
            GRBModel myModel_vis_pas = vrp_time_space_vis_pas.SetIniModel();
            vrp_time_space_vis_pas.Generating_Vars_Obj(myModel_vis_pas,
            out Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic_vis_pas,
            out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> ArcGRBVarY_ID_ijtts_vis_pas,
            out Dictionary<SpaceTimeVertices, Dictionary<IndividualType, List<ArcGRBVar>>> ArcGRBVarYListijtts_vis_pas,
            out Dictionary<IndividualType, List<IndexGRBVar>> VarZDic_vis_pas);
            vrp_time_space_vis_pas.StartIniOptimization(arcDic, VarYDic_vis_pas, ArcGRBVarY_ID_ijtts_vis_pas, ArcGRBVarYListijtts_vis_pas, VarZDic_vis_pas, myModel_vis_pas, out List<IndividualID> pasList_vis_pas);
            Console.WriteLine("============================generating results: vispas solution============================");
            Generating_Results(myModel_vis_pas, VarZDic_vis_pas, VarYDic_vis_pas,
    out Y_typeIDRID_string_dic,
    out Y_typeIDRID_string_dic2,
    out Y_typeIDRID_timestring_dic,
    out Y_typeIDRID_combined_string_dic);
            myModel_vis_pas.Write("myModel_vis_pas_" + FilePath + ".lp");
            Console.WriteLine("============================generating results end: vispas solution============================");

            int ID_sequence_veh = 0;
            int ID_sequence_pas = 0;
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>();
            Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all = new Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>();
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2 = new Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>();
            Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2 = new Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>();
            Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all = new Dictionary<IndividualType, Dictionary<RouteID, double>>();
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>();
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>();
            //Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>();
            Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_one_iter = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>();

            List<SpaceTimeVertices> pasSTV_List = new List<SpaceTimeVertices>();
            Dictionary<RouteID, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>> VarYDic_all = new Dictionary<RouteID, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>>();

            RouteID IniRIDs = "Ini";
            RouteID vispas = "vis_pas";
            VarYDic_all.Add(IniRIDs, VarYDic);
            VarYDic_all.Add(vispas, VarYDic_vis_pas);
            foreach (var VarYDic_KV in VarYDic_all)
            {
                VarYDic = VarYDic_KV.Value;
                bool vis_pas_or_not;
                bool ini_or_not;
                if (VarYDic_KV.Key.ToString() == "vis_pas")
                {
                    vis_pas_or_not = true;
                }
                else
                {
                    vis_pas_or_not = false;
                }
                if (VarYDic_KV.Key.ToString() == "Ini")
                {
                    ini_or_not = true;
                }
                else
                {
                    ini_or_not = false;
                }
                A_B_generator a_B_generator = new A_B_generator(myModel_sta, ID_sequence_veh, ID_sequence_pas, VarYDic, DicFirstSpaceTimeVertex,
DicSecondSpaceTimeVertex, DicSpaceTimeVertices, arcDicIndiTypeID, ArcGRBVarY_ID_ijtts, pasList,
ADic_all, BDic_all, ADic_all2, BDic_all2, CostDic_all, DicTypeRID_Arc_all, DicTypeRID_Arc_combined_all
, DicTypeRID_Arc_one_iter, vis_pas_or_not, ini_or_not
);
                a_B_generator.Generating_a_and_b(
              out ADic_all,
              out ADic_all2,
              out BDic_all,
              out BDic_all2,
              out CostDic_all,
              out DicTypeRID_Arc_all, out DicTypeRID_Arc_combined_all,
              out ID_sequence_veh, out ID_sequence_pas, out pasSTV_List
              , out DicTypeRID_Arc_one_iter
              );
            }

            int iter_num = 0;
            int odd_veh_even_pas = 0;
            List<RouteID> vehRID_list = new List<RouteID>();
            double soluLB = 99999999;
            double best_case_solution = 99999999;
            int bestIter = 0;
            int now_sequence = 0;
            int BB_sequence = 0;
            GRBModel myModel_master;
            List<BB_node> BB_node_list = new List<BB_node>();
            BB_node BB_node_ini = new BB_node();
            BB_node_ini.BB_sequence = now_sequence;
            BB_node_ini.active1_inactive0 = 1;
            BB_node_ini.veh_BB_info_list = new List<BB_info>();
            BB_node_ini.pas_BB_info_list = new List<BB_info>();

            BB_node_ini.ADic_all = Clone(ADic_all) as Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>;
            BB_node_ini.BDic_all = Clone(BDic_all) as Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>;
            BB_node_ini.ADic_all2 = Clone(ADic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>;
            BB_node_ini.BDic_all2 = Clone(BDic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>;
            BB_node_ini.CostDic_all = Clone(CostDic_all) as Dictionary<IndividualType, Dictionary<RouteID, double>>;
            BB_node_ini.DicTypeRID_Arc_all = Clone(DicTypeRID_Arc_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
            BB_node_ini.DicTypeRID_Arc_combined_all = Clone(DicTypeRID_Arc_combined_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
            //BB_node_ini.DicTypeRID_Arc = Clone(DicTypeRID_Arc) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;

            BB_node_list.Add(BB_node_ini);

            bool IntOrNot;
            BB_info BB_info_new;
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_string_dic_all = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_string_dic_all2 = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_timestring_dic_all = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
            Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>> Y_combined_string_dic_all = new Dictionary<int, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>>();
            Dictionary<int, int> BB_sequence_iter = new Dictionary<int, int>();
            Dictionary<int, double> iter_and_objective = new Dictionary<int, double>();
            while (now_sequence <= BB_sequence)
            {
                bool sameOrNot = false;
                int ZeroOrOne_OneIsOptimal = 0;
                BB_node BB_node_processing = BB_node_list[now_sequence];
                BB_sequence_iter.Add(now_sequence, iter_num);
                ADic_all = Clone(BB_node_processing.ADic_all) as Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>>;
                BDic_all = Clone(BB_node_processing.BDic_all) as Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>;
                ADic_all2 = Clone(BB_node_processing.ADic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>>;
                BDic_all2 = Clone(BB_node_processing.BDic_all2) as Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>>;
                CostDic_all = Clone(BB_node_processing.CostDic_all) as Dictionary<IndividualType, Dictionary<RouteID, double>>;
                DicTypeRID_Arc_all = Clone(BB_node_processing.DicTypeRID_Arc_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
                DicTypeRID_Arc_combined_all = Clone(BB_node_processing.DicTypeRID_Arc_combined_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
                //DicTypeRID_Arc = Clone(BB_node_processing.DicTypeRID_Arc) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
                while (true)
                {
                    iter_num = iter_num + 1;
                    Console.WriteLine("====================================iter_num:" + iter_num.ToString() + "====================================");
                    DicTypeRID_Arc_one_iter = new Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>();

                    Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all_old = Clone(CostDic_all) as Dictionary<IndividualType, Dictionary<RouteID, double>>;

                    Console.WriteLine("======================constructing: master problem======================");
                    myModel_master = vrp_time_space_ini.SetIniModel();
                    Dictionary<IndividualType, Dictionary<IndividualID, double>> u_dic_A_theta;
                    Dictionary<MasterConstrType, Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>>>> u_Oth_dic;
                    GeneratingMasterProblem generatingMasterProblem = new GeneratingMasterProblem(myModel_master, DicFirstSpaceTimeVertex,
                      DicSecondSpaceTimeVertex, DicSpaceTimeVertices, arcDicIndiTypeID, ArcGRBVarY_ID_ijtts, DicTypeRID_Arc_all, pasSTV_List, vehRID_list);
                    generatingMasterProblem.StartMasterProOptimization(ADic_all, ADic_all2, BDic_all, BDic_all2, CostDic_all, out u_dic_A_theta, out u_Oth_dic, out IntOrNot, out BB_info_new
                        , out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_string_dic,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_string_dic2,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_timestring_dic,
                    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_combined_dic);
                    Y_string_dic_all.Add(iter_num, Y_string_dic);//第一个iter的结果是inisolution的结果
                    Y_string_dic_all2.Add(iter_num, Y_string_dic2);
                    Y_timestring_dic_all.Add(iter_num, Y_timestring_dic);
                    Y_combined_string_dic_all.Add(iter_num, Y_combined_dic);
                    if (myModel_master.Status == 2)
                    {
                        iter_and_objective.Add(iter_num, myModel_master.ObjVal);
                    }
                    else
                    {
                        iter_and_objective.Add(iter_num, 999999999);
                    }
                    
                    Console.WriteLine("============================generating results: master pro solution============================");
                    myModel_master.Write("result_master_" + FilePath + ".lp");
                    Console.WriteLine("=========================end: master pro solution=========================");
                    if (ZeroOrOne_OneIsOptimal == 1)
                    {
                        if (now_sequence == 0)
                        {
                            GRBModel myModel_masterQ = vrp_time_space_ini.SetIniModel();
                            GeneratingMasterProblemQ generatingMasterProblemQ = new GeneratingMasterProblemQ(myModel_masterQ, DicFirstSpaceTimeVertex,
          DicSecondSpaceTimeVertex, DicSpaceTimeVertices, arcDicIndiTypeID, ArcGRBVarY_ID_ijtts, DicTypeRID_Arc_all);
                            try
                            {
                                generatingMasterProblemQ.StartMasterProOptimization(ADic_all, ADic_all2, BDic_all, BDic_all2, CostDic_all);
                                myModel_masterQ.Write("result_masterQ_" + FilePath + ".lp");
                                soluLB = Math.Min(soluLB, myModel_masterQ.ObjVal);
                                soluLB = 104;
                            }
                            catch (GRBException gex)
                            {

                            }
                            stopwatch.Stop();
                            string timeElapsed = "CG time：" + stopwatch.ElapsedTicks;
                            Console.WriteLine(timeElapsed);
                        }
                        GeneratingRoutes(Y_string_dic_all, Y_string_dic_all2, Y_timestring_dic_all, Y_combined_string_dic_all, iter_and_objective, BB_sequence_iter, now_sequence, BB_node_list, arcDicIndiTypeID);
                        Console.WriteLine("=========================All arcs:=========================");
                        //Generating_Arcs(DicTypeRID_Arc_combined_all);
                        //Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all_no_pas = Clone(DicTypeRID_Arc_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
                        //DicTypeRID_Arc_all_no_pas.Remove("pasUp");
                        //DicTypeRID_Arc_all_no_pas.Remove("pasDown");
                        //Generating_Arcs(DicTypeRID_Arc_all_no_pas);
                        
                        
                        break;
                    }
                    if (sameOrNot)
                    {
                        Console.WriteLine("=========================All arcs:=========================");
                        GeneratingRoutes(Y_string_dic_all, Y_string_dic_all2, Y_timestring_dic_all, Y_combined_string_dic_all, iter_and_objective, BB_sequence_iter, now_sequence, BB_node_list, arcDicIndiTypeID);
                        //Generating_Arcs(DicTypeRID_Arc_combined_all);
                        //Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all_no_pas = Clone(DicTypeRID_Arc_all) as Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>>;
                        //DicTypeRID_Arc_all_no_pas.Remove("pasUp");
                        //DicTypeRID_Arc_all_no_pas.Remove("pasDown");
                        //Generating_Arcs(DicTypeRID_Arc_all_no_pas);
                        break;
                    }

                    if (myModel_master.Status == 2)
                    {
                        if (best_case_solution > myModel_master.ObjVal)
                        {
                            best_case_solution = myModel_master.ObjVal;
                        }
                        GRBModel myModel_sub = vrp_time_space_ini.SetIniModel();
                        GeneratingSubProblem generatingSubProblem = new GeneratingSubProblem(myModel_sub, arcDic, arcDicIndiTypeIDArcType, DicFirstSpaceTimeVertex, DicSecondSpaceTimeVertex,
                            DicSpaceTimeVertices, arcDicIndiTypeID, ArcGRBVarY_ID_ijtts, u_dic_A_theta, u_Oth_dic, pasList, DicTypeRID_Arc_all,
                            ID_sequence_veh, ID_sequence_pas, CostDic_all, odd_veh_even_pas, ListSpaceTimeVertices
                            , vehRID_list, BB_node_processing
                            );
                        generatingSubProblem.StartSubProOptimization(out VarYDic_all, out ZeroOrOne_OneIsOptimal, out odd_veh_even_pas);
                        myModel_sub.Write("result_sub_" + FilePath + ".lp");
                        foreach (var VarYDic_KV in VarYDic_all)
                        {
                            VarYDic = VarYDic_KV.Value;
                            bool vis_pas_or_not;
                            bool ini_or_not;
                            if (VarYDic_KV.Key.ToString() == "vis_pas")
                            {
                                vis_pas_or_not = true;
                            }
                            else
                            {
                                vis_pas_or_not = false;
                            }
                            if (VarYDic_KV.Key.ToString() == "Ini")
                            {
                                ini_or_not = true;
                            }
                            else
                            {
                                ini_or_not = false;
                            }
                            A_B_generator a_B_generator = new A_B_generator(myModel_sta, ID_sequence_veh, ID_sequence_pas, VarYDic, DicFirstSpaceTimeVertex,
            DicSecondSpaceTimeVertex, DicSpaceTimeVertices, arcDicIndiTypeID, ArcGRBVarY_ID_ijtts, pasList,
            ADic_all, BDic_all, ADic_all2, BDic_all2, CostDic_all, DicTypeRID_Arc_all, DicTypeRID_Arc_combined_all
            , DicTypeRID_Arc_one_iter, vis_pas_or_not, ini_or_not
            );
                            a_B_generator.Generating_a_and_b(
                          out ADic_all,
                          out ADic_all2,
                          out BDic_all,
                          out BDic_all2,
                          out CostDic_all,
                          out DicTypeRID_Arc_all, out DicTypeRID_Arc_combined_all,
                          out ID_sequence_veh, out ID_sequence_pas, out pasSTV_List
                          , out DicTypeRID_Arc_one_iter
                          );
                        }
                        sameOrNot = true;
                        foreach (var ID_double_KV in CostDic_all)
                        {
                            if (!Enumerable.SequenceEqual(ID_double_KV.Value.Values.ToList(), CostDic_all_old[ID_double_KV.Key].Values.ToList()))
                            {
                                sameOrNot = false;
                            }
                        }
                    }
                    else if (myModel_master.Status == 3)//无解
                    {
                        break;
                    }
                }
                if (myModel_master.Status == 3)//无解
                {
                    now_sequence = now_sequence + 1;
                }
                else if (myModel_master.Status == 2 && myModel_master.ObjVal > soluLB)
                {
                    now_sequence = now_sequence + 1;
                }
                else if (myModel_master.Status == 2 && myModel_master.ObjVal <= soluLB && IntOrNot)
                {
                    now_sequence = now_sequence + 1;
                    soluLB = myModel_master.ObjVal;
                    bestIter = iter_num;
                    if (soluLB==best_case_solution)
                    {
                        break;
                    }
                }
                else if (myModel_master.Status == 2 && myModel_master.ObjVal <= soluLB && !IntOrNot)
                {
                    now_sequence = now_sequence + 1;
                    ///break;/////////////////////////////////////////
                    BB_node_list = Generating_new_node(
                       BB_info_new, BB_node_processing, BB_sequence, BB_node_list, ADic_all, BDic_all, ADic_all2, BDic_all2,
                       CostDic_all, DicTypeRID_Arc_all, DicTypeRID_Arc_combined_all, DicSpaceTimeVertices,out BB_sequence);
                }
            }









            Dictionary<IndividualID, List<PointTW>> coordTWDic_visveh_all;
            Dictionary<IndividualID, List<PointTW>> coordTWDic_visvehCor_all;

            //全部的需求
            GetCoordTWDic_Dy(corfilepath_dy, tWfilepath_dy, individualTypePas, pasNum_Sta, startNumpas, pasNum,
                out Dictionary<IndividualID, List<PointTW>> coordTWDicPas_dy_all, out coordTWDic_visveh_all, out coordTWDic_visvehCor_all, out int PointNum_dy_all);

            GRBModel myModel_Cmpl = myModel_sta;//GRBModel myModel_Cmpl = new GRBModel(myModel_sta);这样的话model status不等
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic_dy = new Dictionary<IndividualType, List<IndexGRBVar>>();
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic_dy = new Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>>();
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>> VarY_TypeIDArc_Dic_dy = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<ArcGRBVar>>>>();
            Dictionary<IndividualID, List<PointTW>> coordTWDicPas_dy = new Dictionary<IndividualID, List<PointTW>>(coorTwDicPas_static);//Dictionary<IndividualID, List<PointTW>> coordTWDicPas_Cmpl = coordTWDicPas_dy; 这样写的意思是不是它们两个永远相等
            int startWhere = 0;
            int pasPointNum_dy = 2 * pasNum_Sta;



            for (int DcT = 5; DcT < 70; DcT = DcT + 5)//DcT 为decision epoch
            {
                int pasPointNum_dy_new = pasPointNum_dy;
                int ImT = DcT + 1;
                Dictionary<IndividualID, List<PointTW>> coordTWDicPas_Cmpl = new Dictionary<IndividualID, List<PointTW>>(coordTWDicPas_dy);
                for (int cdTW_KV_ID = startWhere; cdTW_KV_ID < coordTWDicPas_dy_all.Count; cdTW_KV_ID++)
                {
                    IndividualID individualID = coordTWDicPas_dy_all.Keys.ToList()[cdTW_KV_ID];
                    if (individualID.subTime > DcT)//此次循环时这个需求还未进入系统
                    {
                        break;
                    }
                    startWhere = cdTW_KV_ID + 1;
                    pasPointNum_dy_new += 1;
                    coordTWDicPas_dy.Add(individualID, coordTWDicPas_dy_all[individualID]);
                }
                if (coordTWDicPas_Cmpl.Count == coordTWDicPas_dy.Count)
                {
                    continue;
                }
                Dictionary<IndividualID, List<PointTW>> coorTWDicVehCor_dy = new Dictionary<IndividualID, List<PointTW>>(coorTWDicVehCor);
                Dictionary<IndividualID, List<PointTW>> coorTwDicVeh_dy = new Dictionary<IndividualID, List<PointTW>>(coorTwDicVeh);
                RoadNetwork roadNetwork_dy = new RoadNetwork(coordTWDicPas_dy, coorTWDicVehCor_dy, coorTwDicVeh_dy, tsTime, optime);
                roadNetwork_dy.GetArc(
                out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic_dy,
                out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType_dy,
                out Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID_dy,
                out Dictionary<IndividualType, List<Arc>> arcDicIndividualType_dy,
                out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicFirstSpaceTimeVertex_dy,
                out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertex>>> DicSecondSpaceTimeVertex_dy,
                out Dictionary<IndividualType, Dictionary<IndividualID, List<SpaceTimeVertices>>> DicSpaceTimeVertices_dy,
                out List<SpaceTimeVertices> ListSpaceTimeVertices_dy);
                vrp_time_space vrp_time_space_dy = new vrp_time_space(arcDic_dy, arcDicIndiTypeIDArcType_dy, arcDicIndiTypeID_dy, capacity, DicFirstSpaceTimeVertex_dy, DicSecondSpaceTimeVertex_dy, ListSpaceTimeVertices_dy,
                    indiID_TS_int, indiID_RS_int);
                GRBModel myModel_dy = vrp_time_space_dy.SetMyModel();

                vrp_time_space_dy.Generating_Vars_Obj(myModel_dy,
                    out VarYDic_dy,
                    out VarY_TypeIDArc_Dic_dy,
                    out ArcGRBVarY_ID_ijtts,
                    out ArcGRBVarYListijtts,
                    out VarZDic_dy,
                    out Dictionary<IndividualID, Dictionary<IndividualID, IndexGRBVar>> VarZDic_pas_Z);//0808把次序改了一下，Generating_Vars_Obj再加动态约束

                OrderManager OrderManager1 = new OrderManager(
                    coordTWDicPas_dy,
                    myModel_Cmpl, myModel_dy, DcT, ImT, VarZDic, VarZDic_dy, VarYDic, VarYDic_dy,
                    arcDic, arcDic_dy, arcDicIndiTypeIDArcType_dy, arcDicIndiTypeID_dy, pasPointNum_dy, pasPointNum_dy_new);

                myModel_dy = OrderManager1.Generating_Ordr_Assgnmnt_Constr();

                DynConstrGenerater DynConstrGenerater1 = new DynConstrGenerater(
                    myModel_Cmpl, myModel_dy, ImT, VarZDic, VarZDic_dy, VarYDic, VarYDic_dy,
                    arcDic, arcDic_dy, arcDicIndiTypeIDArcType_dy, arcDicIndiTypeID_dy, pasNum_Sta);

                myModel_dy = DynConstrGenerater1.Generating_Dy_Constr();

                vrp_time_space_dy.StartStaticModelOptimization(arcDic_dy, VarYDic_dy, VarY_TypeIDArc_Dic_dy, ArcGRBVarY_ID_ijtts, ArcGRBVarYListijtts, VarZDic_dy, myModel_dy, out pasList);
                if (myModel_dy.Status == GRB.Status.OPTIMAL)
                {
                    myModel_Cmpl = myModel_dy;
                    VarZDic = VarZDic_dy;
                    VarYDic = VarYDic_dy;
                    arcDic = arcDic_dy;
                    arcDicIndiTypeIDArcType = arcDicIndiTypeIDArcType_dy;
                    arcDicIndiTypeID = arcDicIndiTypeID_dy;
                    arcDicIndividualType = arcDicIndividualType_dy;
                    DicFirstSpaceTimeVertex = DicFirstSpaceTimeVertex_dy;
                    DicSecondSpaceTimeVertex = DicSecondSpaceTimeVertex_dy;
                    DicSpaceTimeVertices = DicSpaceTimeVertices_dy;
                    ListSpaceTimeVertices = ListSpaceTimeVertices_dy;
                    //myModel_Cmpl.Write("result_dy_step_1" + FilePath + ".lp");
                    //GRBConstr gRBConstr = myModel_Cmpl.GetConstrByName("Constr1_flow_balance_IndiID_veh1_DpasDown1_9");
                    //double u_ = gRBConstr.
                    //Pi;
                    myModel_Cmpl.Write("result_dy_12_5_" + FilePath + ".lp");
                    Generating_Results(myModel_Cmpl, VarZDic_dy, VarYDic_dy,
    out Y_typeIDRID_string_dic,
    out Y_typeIDRID_string_dic2,
    out Y_typeIDRID_timestring_dic,
    out Y_typeIDRID_combined_string_dic);
                }
                else
                {
                    myModel_Cmpl = myModel_dy;
                    myModel_Cmpl.Write("result_dy_error_" + FilePath + ".lp");
                    break;
                }
                pasPointNum_dy = pasPointNum_dy_new;

            }

            Generating_Results(myModel_Cmpl, VarZDic, VarYDic,
    out Y_typeIDRID_string_dic,
    out Y_typeIDRID_string_dic2,
    out Y_typeIDRID_timestring_dic,
    out Y_typeIDRID_combined_string_dic);
        }

        static public GRBModel DeletingConstrs(GRBModel myModel)
        {
            GRBModel myModel_dy_out = myModel;
            GRBConstr[] GRBConstrs = myModel_dy_out.GetConstrs();
            foreach (GRBConstr gRBConstr in GRBConstrs)
            {
                myModel_dy_out.Remove(gRBConstr);
            }
            GRBQConstr[] GRBQConstrs = myModel_dy_out.GetQConstrs();
            foreach (GRBQConstr gRBQConstr in GRBQConstrs)
            {
                myModel_dy_out.Remove(gRBQConstr);
            }
            return myModel_dy_out;
        }

        static public void Generating_Analytical_Results(GRBModel myModel,
         Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
         Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
         Dictionary<IndividualID, int> indiID_TS_int,
         Dictionary<IndividualID, int> indiID_RS_int,
         Dictionary<string, int> ServedOrNot_string_int_dic,
         int served_passengers_all,
         int all_passengers,double total_cost_all,
         out Dictionary<string, int> ServedOrNot_string_int_dic_out,
         out int served_passengers_all_new,
         out int all_passengers_new,out double total_cost_all_new)
        {
            served_passengers_all_new = served_passengers_all;
            all_passengers_new = all_passengers;
            total_cost_all_new = total_cost_all;
            ServedOrNot_string_int_dic_out = Clone(ServedOrNot_string_int_dic) as Dictionary<string, int>;
            int A_TACR = 0;
            int A_RACR = 0;
            int A_T_RACR = 0;
            int A_others = 0;
            double all_passengers_in_this_example_fragment = arcDic["pasUp"].Keys.Count;
            Dictionary<IndividualID, double> ServedOrNot = new Dictionary<IndividualID, double>();
            foreach (IndividualID pasUpID in arcDic["pasUp"].Keys)
            {
                List<IndexGRBVar> indexGRBVarZ_list = VarZDic["veh"].Where(
                t => t.IndividualIDPas == pasUpID).ToList();
                double servedTimes = 0;
                foreach (IndexGRBVar indexGRBVarZ in indexGRBVarZ_list)
                {
                    servedTimes += indexGRBVarZ.GRBV.X;
                    served_passengers_all_new += (int)Math.Round(indexGRBVarZ.GRBV.X);
                }
                ServedOrNot.Add(pasUpID, servedTimes);
            }
            double total_cost = myModel.ObjVal - (all_passengers_in_this_example_fragment - served_passengers_all_new) * 99999 * 2;
            total_cost_all_new += total_cost;
            foreach (IndividualID pasUpID in arcDic["pasUp"].Keys)
            {
                int TsOrNot = indiID_TS_int[pasUpID];
                int RsOrNot = indiID_RS_int[pasUpID];
                if (ServedOrNot[pasUpID] == 0)
                {
                    continue;
                }
                if (TsOrNot == 1 && RsOrNot == 1)
                {
                    A_T_RACR += 1;
                }
                else if (RsOrNot == 1)
                {
                    A_RACR += 1;
                }
                else if (TsOrNot == 1)
                {
                    A_TACR += 1;
                }
                else
                {
                    A_others += 1;
                }
            }
            if (!ServedOrNot_string_int_dic_out.ContainsKey("A_TACR"))
            {
                ServedOrNot_string_int_dic_out.Add("A_TACR", 0);
            }
            if (!ServedOrNot_string_int_dic_out.ContainsKey("A_RACR"))
            {
                ServedOrNot_string_int_dic_out.Add("A_RACR", 0);
            }
            if (!ServedOrNot_string_int_dic_out.ContainsKey("A_T_RACR"))
            {
                ServedOrNot_string_int_dic_out.Add("A_T_RACR", 0);
            }
            if (!ServedOrNot_string_int_dic_out.ContainsKey("A_others"))
            {
                ServedOrNot_string_int_dic_out.Add("A_others", 0);
            }
            ServedOrNot_string_int_dic_out["A_TACR"] += A_TACR;
            ServedOrNot_string_int_dic_out["A_RACR"] += A_RACR;
            ServedOrNot_string_int_dic_out["A_T_RACR"] += A_T_RACR;
            ServedOrNot_string_int_dic_out["A_others"] += A_others;
            
        }

        static public void Generating_TS_RS_Results(int caseNum, string path,
            Dictionary<string, int> ServedOrNot_string_int_dic,
            int served_passengers_all,
            int all_passengers, double total_cost_all)
        {
            string A_TACR_num=ServedOrNot_string_int_dic["A_TACR"].ToString();
            string A_RACR_num = ServedOrNot_string_int_dic["A_RACR"].ToString();
            string A_T_RACR_num = ServedOrNot_string_int_dic["A_T_RACR"].ToString();
            string A_others_num = ServedOrNot_string_int_dic["A_others"].ToString();

            string results = "";
            results += caseNum.ToString() + "\t" + A_TACR_num
                    + "\t" + A_RACR_num + "\t" + A_T_RACR_num + "\t" + A_others_num
                    + "\t" + total_cost_all.ToString() + "\t" + served_passengers_all.ToString()
                    ;
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            //System.IO.File.WriteAllText(@"CorPas_all.txt", strpas);
            using (StreamWriter w = File.AppendText(@"" + path + "\\Result_TS_RS_all.txt"))
            {
                w.WriteLine(results);
            }
        }

        static public void Generating_Passenger_Proportion(GRBModel myModel,
         Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
         Dictionary<IndividualID, Dictionary<IndividualID, IndexGRBVar>> VarZDic_pas_Z,
         Dictionary<string, int> PassengerP_string_int_dic,
         out Dictionary<string, int> PassengerP_string_int_dic_out)
        {
            PassengerP_string_int_dic_out = Clone(PassengerP_string_int_dic) as Dictionary<string, int>;
            int Taxi = 0;
            int fixed_route_transport = 0;
            int Taxi_fixed_route_transport = 0;
            int unserved = 0;
            Dictionary<IndividualID, double> ServedOrNot = new Dictionary<IndividualID, double>();
            foreach (IndividualID pasUpID in arcDic["pasUp"].Keys)
            {
                try
                {
                    IndividualID veh_served_pasUp = VarZDic_pas_Z[pasUpID].Where(t => t.Value.GRBV.X == 1).ToList()[0].Value.IndividualIDVeh;
                    IndividualID veh_served_pasDown = VarZDic_pas_Z[pasUpID.ToString().Replace("Up", "Down")].Where(t => t.Value.GRBV.X == 1).ToList()[0].Value.IndividualIDVeh;
                    int pasUpServedBy_0_fixed_1_taxi = 0;
                    int pasDownServedBy_0_fixed_1_taxi = 0;
                    if (veh_served_pasUp.ToString().Contains("1") || veh_served_pasUp.ToString().Contains("2"))
                    {
                        pasUpServedBy_0_fixed_1_taxi = 0;
                    }
                    if (veh_served_pasUp.ToString().Contains("3") || veh_served_pasUp.ToString().Contains("4"))
                    {
                        pasUpServedBy_0_fixed_1_taxi = 1;
                    }
                    if (veh_served_pasDown.ToString().Contains("1") || veh_served_pasDown.ToString().Contains("2"))
                    {
                        pasDownServedBy_0_fixed_1_taxi = 0;
                    }
                    if (veh_served_pasDown.ToString().Contains("3") || veh_served_pasDown.ToString().Contains("4"))
                    {
                        pasDownServedBy_0_fixed_1_taxi = 1;
                    }
                    if (pasUpServedBy_0_fixed_1_taxi == 1 && pasDownServedBy_0_fixed_1_taxi == 1)
                    {
                        Taxi += 1;
                    }
                    else if (pasUpServedBy_0_fixed_1_taxi == 0 && pasDownServedBy_0_fixed_1_taxi == 0)
                    {
                        fixed_route_transport += 1;
                    }
                    else if (pasUpServedBy_0_fixed_1_taxi == 1 && pasDownServedBy_0_fixed_1_taxi == 0)
                    {
                        Taxi_fixed_route_transport += 1;
                    }
                    else if (pasUpServedBy_0_fixed_1_taxi == 0 && pasDownServedBy_0_fixed_1_taxi == 1)
                    {
                        Taxi_fixed_route_transport += 1;
                    }
                }
                catch
                {
                    unserved += 1;
                }
            }

            if (!PassengerP_string_int_dic_out.ContainsKey("Taxi"))
            {
                PassengerP_string_int_dic_out.Add("Taxi", 0);
            }
            if (!PassengerP_string_int_dic_out.ContainsKey("fixed_route_transport"))
            {
                PassengerP_string_int_dic_out.Add("fixed_route_transport", 0);
            }
            if (!PassengerP_string_int_dic_out.ContainsKey("Taxi_fixed_route_transport"))
            {
                PassengerP_string_int_dic_out.Add("Taxi_fixed_route_transport", 0);
            }
            if (!PassengerP_string_int_dic_out.ContainsKey("unserved"))
            {
                PassengerP_string_int_dic_out.Add("unserved", 0);
            }
            PassengerP_string_int_dic_out["Taxi"] += Taxi;
            PassengerP_string_int_dic_out["fixed_route_transport"] += fixed_route_transport;
            PassengerP_string_int_dic_out["Taxi_fixed_route_transport"] += Taxi_fixed_route_transport;
            PassengerP_string_int_dic_out["unserved"] += unserved;
        }

        static public void Generating_Passenger_Proportion_Results(int caseNum, string path,
            Dictionary<string, int> PassengerP_string_int_dic)
        {
            string Taxi_num = PassengerP_string_int_dic["Taxi"].ToString();
            string fixed_route_transport_num = PassengerP_string_int_dic["fixed_route_transport"].ToString();
            string Taxi_fixed_route_transport_num = PassengerP_string_int_dic["Taxi_fixed_route_transport"].ToString();
            string unserved_num = PassengerP_string_int_dic["unserved"].ToString();

            string results = "";
            results += caseNum.ToString() + "\t" + Taxi_num
                    + "\t" + fixed_route_transport_num + "\t" + Taxi_fixed_route_transport_num + "\t" + unserved_num
                    ;
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            //System.IO.File.WriteAllText(@"CorPas_all.txt", strpas);
            using (StreamWriter w = File.AppendText(@"" + path + "\\Result_Pas_Pro_all.txt"))
            {
                w.WriteLine(results);
            }
        }
        public static void Generating_Results(GRBModel myModel_sta,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic2,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_timestring_dic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_combined_string_dic)
        {
            Y_typeIDRID_string_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_string_dic2 = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_timestring_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_combined_string_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            if (myModel_sta.Status == GRB.Status.OPTIMAL)
            {
                foreach (var c in VarZDic)
                {
                    for (int i = 0; i < c.Value.Count; i++)
                    {
                        if (c.Value[i].GRBV.X == 0)
                        {
                            continue;
                        }
                        string output = string.Format("_[{0}]_[{1}]", c.Value[i].GRBV.VarName, c.Value[i].GRBV.X);//"ZVeh_P_[{0}]V_[{1}]_Z[{2}]"
                        Console.WriteLine(output);
                    }
                }
                foreach (var c in VarYDic)
                {
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
                        RouteID RID_this = d.Key.ToString();
                        RID_this.route_lambda = 1;
                        string route_string = "";
                        string route_string2 = "";
                        string time_string = "";
                        string combined_string = "";
                        int howmanyarcs = 0;
                        int howmanyarcshaveprocessed = 0;
                        if (!Y_typeIDRID_string_dic.ContainsKey(startArc.arc.individualType))
                        {
                            Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                            Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID2 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                            Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID3 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                            Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID4 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                            Y_typeIDRID_string_dic.Add(startArc.arc.individualType, KeyValuePair_RID);
                            Y_typeIDRID_string_dic2.Add(startArc.arc.individualType, KeyValuePair_RID2);
                            Y_typeIDRID_timestring_dic.Add(startArc.arc.individualType, KeyValuePair_RID3);
                            Y_typeIDRID_combined_string_dic.Add(startArc.arc.individualType, KeyValuePair_RID4);
                        }
                        if (!Y_typeIDRID_string_dic[startArc.arc.individualType].ContainsKey(startArc.arc.individualID))
                        {
                            Dictionary<RouteID, string> KeyValuePair = new Dictionary<RouteID, string>();
                            Dictionary<RouteID, string> KeyValuePair2 = new Dictionary<RouteID, string>();
                            Dictionary<RouteID, string> KeyValuePair3 = new Dictionary<RouteID, string>();
                            Dictionary<RouteID, string> KeyValuePair4 = new Dictionary<RouteID, string>();
                            Y_typeIDRID_string_dic[startArc.arc.individualType].Add(startArc.arc.individualID, KeyValuePair);
                            Y_typeIDRID_string_dic2[startArc.arc.individualType].Add(startArc.arc.individualID, KeyValuePair2);
                            Y_typeIDRID_timestring_dic[startArc.arc.individualType].Add(startArc.arc.individualID, KeyValuePair3);
                            Y_typeIDRID_combined_string_dic[startArc.arc.individualType].Add(startArc.arc.individualID, KeyValuePair4);
                        }
                        foreach (ArcGRBVar arcGRBVar in ArcYList)
                        {
                            if (arcGRBVar.GRBV.X == 0)
                            {
                                continue;
                            }
                            howmanyarcs += 1;
                        }
                        foreach (ArcGRBVar arcGRBVar in ArcYList)
                        {
                            if (arcGRBVar.GRBV.X == 0)
                            {
                                continue;
                            }
                            string individualID = arcGRBVar.arc.individualID.ToString();
                            string i = arcGRBVar.arc.spaceVertices[0].individualID.ToString();
                            string j = arcGRBVar.arc.spaceVertices[1].individualID.ToString();
                            string t = arcGRBVar.arc.timeVertices[0].ToString();
                            string tt = arcGRBVar.arc.timeVertices[1].ToString();
                            string output = string.Format(arcGRBVar.GRBV.VarName, individualID, i, j, t, tt, arcGRBVar.GRBV.X.ToString());//"Y_Indi_[{0}]i_[{1}]_j[{2}]_t[{3}]_tt[{4}]_Y[{5}]"
                            Console.WriteLine(output);

                            Arc arc = arcGRBVar.arc;
                            i = arc.SpaceTimeVertex1.spaceVertex.individualID.ToString();
                            j = arc.SpaceTimeVertex2.spaceVertex.individualID.ToString();
                            string t_str;
                            string tt_str;
                            route_string += i + "_";
                            route_string2 += Y_to_notation(arc.SpaceTimeVertex1.spaceVertex.individualID);
                            time_string += arc.SpaceTimeVertex1.timeVertex;
                            combined_string += "[" + Y_to_notation(arc.SpaceTimeVertex1.spaceVertex.individualID) + "," + arc.SpaceTimeVertex1.timeVertex + "]" + ";";
                            time_string += ";";
                            route_string2 += ";";
                            howmanyarcshaveprocessed += 1;
                            if (howmanyarcshaveprocessed == howmanyarcs)
                            {
                                route_string += j;
                                route_string2 += Y_to_notation(arc.SpaceTimeVertex2.spaceVertex.individualID);
                                time_string += arc.SpaceTimeVertex2.timeVertex;
                                combined_string += "[" + Y_to_notation(arc.SpaceTimeVertex2.spaceVertex.individualID) + "," + arc.SpaceTimeVertex2.timeVertex + "]";
                            }
                        }
                        Y_typeIDRID_string_dic[startArc.arc.individualType][startArc.arc.individualID].Add(RID_this, route_string);
                        Y_typeIDRID_string_dic2[startArc.arc.individualType][startArc.arc.individualID].Add(RID_this, route_string2);
                        Y_typeIDRID_timestring_dic[startArc.arc.individualType][startArc.arc.individualID].Add(RID_this, time_string);
                        Y_typeIDRID_combined_string_dic[startArc.arc.individualType][startArc.arc.individualID].Add(RID_this, combined_string);
                    }
                }
            }
        }


        public static void Generating_Sub_Results(GRBModel myModel_sta, Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic)
        {
            if (myModel_sta.Status == GRB.Status.OPTIMAL)
            {
                foreach (var c in VarYDic)
                {
                    foreach (var d in c.Value)
                    {
                        List<ArcGRBVar> ArcYList = d.Value;
                        
                        ArcYList = ArcYList.OrderBy(a => a.arc.timeVertices[1] + a.arc.timeVertices[0]).ToList();
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
                            if ((d.Key.individualType == "visveh" || d.Key.individualType == "veh") && ArcYList.Where(x => x.GRBV.X==1).ToList().Count == 2)
                            {
                                continue;
                            }
                            string individualID = arcGRBVar.arc.individualID.ToString();
                            string i = arcGRBVar.arc.spaceVertices[0].individualID.ToString();
                            string j = arcGRBVar.arc.spaceVertices[1].individualID.ToString();
                            string t_str;
                            string tt_str;
                            if (arcGRBVar.arc.timeVertices[0] < 10)
                            {
                                t_str = "0" + arcGRBVar.arc.timeVertices[0].ToString();
                            }
                            else
                            {
                                t_str = arcGRBVar.arc.timeVertices[0].ToString();
                            }
                            if (arcGRBVar.arc.timeVertices[1] < 10)
                            {
                                tt_str = "0" + arcGRBVar.arc.timeVertices[1].ToString();
                            }
                            else
                            {
                                tt_str = arcGRBVar.arc.timeVertices[1].ToString();
                            }
                            string output = string.Format(arcGRBVar.GRBV.VarName, individualID, i, j, t_str, tt_str, arcGRBVar.GRBV.X.ToString());//"Y_Indi_[{0}]i_[{1}]_j[{2}]_t[{3}]_tt[{4}]_Y[{5}]"
                            Console.WriteLine(output);
                        }
                    }
                }
                //foreach (var c in VarYDic)
                //{
                //    foreach (var d in c.Value)
                //    {
                //        List<ArcGRBVar> ArcYList = d.Value;
                //        ArcYList = ArcYList.OrderBy(a => a.arc.timeVertices[1]+ a.arc.timeVertices[0]).ToList();
                //        foreach (ArcGRBVar arcGRBVar in ArcYList)
                //        {
                //            if (arcGRBVar.GRBV.X == 0)
                //            {
                //                continue;
                //            }
                //            string individualID = arcGRBVar.arc.individualID.ToString();
                //            string i = arcGRBVar.arc.spaceVertices[0].individualID.ToString();
                //            string j = arcGRBVar.arc.spaceVertices[1].individualID.ToString();
                //            string t_str;
                //            string tt_str;
                //            if (arcGRBVar.arc.timeVertices[0] < 10)
                //            {
                //                t_str = "0" + arcGRBVar.arc.timeVertices[0].ToString();
                //            }
                //            else
                //            {
                //                t_str = arcGRBVar.arc.timeVertices[0].ToString();
                //            }
                //            if (arcGRBVar.arc.timeVertices[1] < 10)
                //            {
                //                tt_str = "0" + arcGRBVar.arc.timeVertices[1].ToString();
                //            }
                //            else
                //            {
                //                tt_str = arcGRBVar.arc.timeVertices[1].ToString();
                //            }
                //            string type = arcGRBVar.arc.arcType.ToString();
                //            string output = string.Format("Y_[{0}]_t[{1}]_tt[{2}]_i[{3}]_j[{4}]_Type[{5}]_Y[{6}]", individualID, t_str, tt_str, i, j, type, arcGRBVar.GRBV.X.ToString());//""
                //            Console.WriteLine(output);
                //        }
                //    }
                //}
            }
        }


        public static void Generating_Arcs(Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all)
        {
            foreach (var TypeRID_Arc_KV in DicTypeRID_Arc_all)
            {
                foreach (var RID_Arc_KV in TypeRID_Arc_KV.Value)
                {
                    Console.WriteLine(RID_Arc_KV.Key);
                    List<Arc> ArcYList = RID_Arc_KV.Value;
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
                        string individualID = arc.individualID.ToString();
                        string i = arc.spaceVertices[0].individualID.ToString();
                        string j = arc.spaceVertices[1].individualID.ToString();
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
                        string output = string.Format(arc.arcVarName);//"Y_Indi_[{0}]i_[{1}]_j[{2}]_t[{3}]_tt[{4}]_Y[{5}]"
                        Console.WriteLine(output);
                    }
                }
            }
        }
        public static void GetCoordTWDic_Dy(string corfilepath, string tWfilepath, IndividualType individualType,
            int pasNum_Sta, int startNum, int coorNum, out Dictionary<IndividualID, List<PointTW>> coordTWDic,
            out Dictionary<IndividualID, List<PointTW>> coordTWDic_visveh,
            out Dictionary<IndividualID, List<PointTW>> coordTWDic_visvehCor,
            out int PointNum)
        {
            coordTWDic = new Dictionary<IndividualID, List<PointTW>>();
            coordTWDic_visveh = new Dictionary<IndividualID, List<PointTW>>();
            coordTWDic_visvehCor = new Dictionary<IndividualID, List<PointTW>>();
            // 读入所有行
            string[] corLines = File.ReadAllLines(corfilepath);
            string[] tWLines = File.ReadAllLines(tWfilepath);
            if (coorNum < corLines.Length - 1)
            {
                PointNum = coorNum;
            }
            else
            {
                PointNum = corLines.Length - 1;
            }
            if (startNum < corLines.Length - 1)
            {
                startNum = startNum;
            }
            else
            {
                startNum = 1;
            }
            // 让过第一行，从第二行开始处理
            for (int i = startNum; i < PointNum + 1; i++)
            {
                string corline = corLines[i];
                string tWLine = tWLines[i];
                // 拆分行
                string[] corV = corline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] TWV = tWLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Point p1;
                p1.pointType = "Up";
                p1.X = double.Parse(corV[1]);
                p1.Y = double.Parse(corV[2]);
                p1.indiType = "";
                TimeWindow TW1;
                TW1.pointType = "Up";
                TW1.lowerLimit = (int)double.Parse(TWV[1]);
                TW1.upperLimit = (int)double.Parse(TWV[2]);
                Point p2;
                p2.pointType = "Down";
                p2.X = double.Parse(corV[3]);
                p2.Y = double.Parse(corV[4]);
                p2.indiType = "";
                TimeWindow TW2;
                TW2.pointType = "Down";
                TW2.lowerLimit = (int)double.Parse(TWV[3]);
                TW2.upperLimit = (int)double.Parse(TWV[4]);
                PointTW pTW1 = new PointTW();
                PointTW pTW2 = new PointTW();
                if (individualType.ToString() == "pas")
                {
                    p1.indiType = "pasUp";
                    p2.indiType = "pasDown";
                    List<PointTW> pointTWList1 = new List<PointTW>();
                    List<PointTW> pointTWList2 = new List<PointTW>();
                    IndividualType individualType1;
                    IndividualType individualType2;
                    if (individualType.ToString() == "pas")
                    {
                        individualType1 = individualType.ToString() + "Up";
                        individualType2 = individualType.ToString() + "Down";
                    }
                    else
                    {
                        individualType1 = individualType.ToString().Replace("Cor", "") + "Out";
                        individualType2 = individualType.ToString().Replace("Cor", "") + "Back";
                    }
                    int pSubTime;
                    if (individualType.ToString() == "pas")
                    {
                        pSubTime = (int)double.Parse(TWV[5]);
                    }
                    else
                    {
                        pSubTime = 0;
                    }
                    p1.individualID = individualType1 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    p1.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    p1.individualID.subTime = pSubTime;
                    p1.individualID.UpOrDown = "Up";
                    p1.individualID.individualType = individualType;
                    p1.individualID.O_or_D_or_M_none = "none";
                    p1.individualID.OutOrBack = "none";

                    TW1.individualID = individualType1 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    TW1.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    pTW1.point = p1;
                    pTW1.timeWindow = TW1;
                    pTW1.subTime = pSubTime;

                    p2.individualID = individualType2 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    p2.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    p2.individualID.subTime = pSubTime;
                    p2.individualID.UpOrDown = "Down";
                    p2.individualID.individualType = individualType;
                    p2.individualID.O_or_D_or_M_none = "none";
                    p2.individualID.OutOrBack = "none";

                    TW2.individualID = individualType2 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    TW2.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    pTW2.point = p2;
                    pTW2.timeWindow = TW2;
                    pTW2.subTime = pSubTime;

                    pTW2.otherPoint = p1;
                    pTW1.otherPoint = p2;
                    pTW2.otherTimeWindow = TW1;
                    pTW1.otherTimeWindow = TW2;

                    pointTWList1.Add(pTW1);
                    pointTWList2.Add(pTW2);

                    coordTWDic.Add(p1.individualID, pointTWList1);
                    coordTWDic.Add(p2.individualID, pointTWList2);

                    List<PointTW> two_p_List = new List<PointTW>();
                    two_p_List.Add(pTW1);
                    two_p_List.Add(pTW2);

                    for (int ii = 0; ii < 2; ii++)
                    {
                        PointTW ptw = two_p_List[ii];
                        p1.indiType = "visvehUp";
                        p2.indiType = "visvehDown";
                        pointTWList1 = new List<PointTW>();
                        pointTWList2 = new List<PointTW>();
                        p1 = ptw.point;
                        TW1 = ptw.timeWindow;

                        p1.individualID = "visveh" + p1.individualID;
                        p1.individualID.individualType = "visveh";
                        p1.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                        p1.individualID.O_or_D_or_M_none = "none";

                        p2 = ptw.point;
                        TW2 = ptw.timeWindow;
                        if (ii == 0)
                        {
                            p1.individualID.if_vis_veh_then_the_other_half = two_p_List[1].point;
                            p2.individualID.if_vis_veh_then_the_other_half = two_p_List[1].point;
                        }
                        else
                        {
                            p1.individualID.if_vis_veh_then_the_other_half = two_p_List[0].point;
                            p2.individualID.if_vis_veh_then_the_other_half = two_p_List[0].point;
                        }
                        TW2.lowerLimit = ptw.timeWindow.lowerLimit + 1;
                        //visveh最少也要在上车乘客被服务之后才能停止服务，即到达结束时间窗
                        TW2.upperLimit = ptw.timeWindow.upperLimit + 1;
                        p2.individualID = p1.individualID.ToString();
                        p2.individualID.individualType = "visveh";
                        p2.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                        p2.individualID.O_or_D_or_M_none = "none";

                        IndividualID IDvisveh;
                        if (p1.individualID.ToString().Contains("Up"))//是不是该把冗余的删掉？
                        {
                            IDvisveh = "visveh" + individualType + "Up" + corV[0];
                            IDvisveh.UpOrDown = "Up";
                            p2.individualID.m_name = p2.individualID.ToString().Replace("visveh", "visvehBack");
                            p2.individualID.OutOrBack = "Back";
                            p1.individualID.m_name = p1.individualID.ToString().Replace("visveh", "visvehOut");
                            p1.individualID.OutOrBack = "Out";
                            p1.individualID.UpOrDown = "Up";
                            p2.individualID.UpOrDown = "Up";
                            p1.individualID.individualType = "visveh";
                            p2.individualID.individualType = "visveh";
                            p1.individualID.O_or_D_or_M_none = "none";
                            p2.individualID.O_or_D_or_M_none = "none";
                            TW2.lowerLimit = two_p_List[0].timeWindow.lowerLimit;
                            //如果是上车虚拟车辆，那么它的时间窗下限应该是乘客下车时间窗下限（考虑到换乘的情况）
                            TW2.upperLimit = two_p_List[1].timeWindow.upperLimit;
                        }
                        else
                        {
                            IDvisveh = "visveh" + individualType + "Down" + corV[0];
                            IDvisveh.UpOrDown = "Down";
                            p2.individualID.m_name = p2.individualID.ToString().Replace("visveh", "visvehBack");
                            p2.individualID.OutOrBack = "Back";
                            p1.individualID.m_name = p1.individualID.ToString().Replace("visveh", "visvehOut");
                            p1.individualID.OutOrBack = "Out";
                            p1.individualID.UpOrDown = "Down";
                            p2.individualID.UpOrDown = "Down";
                            p1.individualID.individualType = "visveh";
                            p2.individualID.individualType = "visveh";
                            p1.individualID.O_or_D_or_M_none = "none";
                            p2.individualID.O_or_D_or_M_none = "none";
                            TW2.lowerLimit = two_p_List[0].timeWindow.lowerLimit;
                            //如果是上车虚拟车辆，那么它的时间窗下限应该是乘客下车时间窗下限（考虑到换乘的情况）
                            TW2.upperLimit = two_p_List[1].timeWindow.upperLimit;
                        }
                        IDvisveh.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                        IDvisveh.individualType = "visveh";
                        IDvisveh.O_or_D_or_M_none = "none";
                        IDvisveh.OutOrBack = "none";
                        pTW2.point = p2;
                        pTW2.otherPoint = p1;
                        pTW1.point = p1;
                        pTW1.otherPoint = p2;
                        pTW2.otherTimeWindow = TW1;
                        pTW1.otherTimeWindow = TW2;
                        pointTWList1.Add(pTW1);
                        pointTWList2.Add(pTW2);
                        coordTWDic_visvehCor.Add(p1.individualID, pointTWList1);
                        coordTWDic_visvehCor.Add(p2.individualID, pointTWList2);

                        pointTWList1 = new List<PointTW>();
                        pointTWList1.Add(pTW1);
                        pointTWList1.Add(pTW2);
                        coordTWDic_visveh.Add(IDvisveh, pointTWList1);
                    }
                }
                else if (individualType.ToString() == "vehCor")
                {
                    p1.indiType = "vehCor";
                    p2.indiType = "vehCor";
                    List<PointTW> pointTWList1 = new List<PointTW>();
                    List<PointTW> pointTWList2 = new List<PointTW>();
                    IndividualType individualType1;
                    IndividualType individualType2;
                    if (individualType.ToString() == "pas")
                    {
                        individualType1 = individualType.ToString() + "Up";
                        individualType2 = individualType.ToString() + "Down";
                    }
                    else
                    {
                        individualType1 = individualType.ToString().Replace("Cor", "") + "Out";
                        individualType2 = individualType.ToString().Replace("Cor", "") + "Back";
                    }
                    int pSubTime;
                    if (individualType.ToString() == "pas")
                    {
                        pSubTime = (int)double.Parse(TWV[5]);
                    }
                    else
                    {
                        pSubTime = 0;
                    }

                    p1.individualID = individualType1 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    p1.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    p1.individualID.subTime = pSubTime;
                    p1.individualID.individualType = individualType;
                    p1.individualID.O_or_D_or_M_none = "none";
                    p1.individualID.UpOrDown = "none";
                    p1.individualID.OutOrBack = "Out";
                    TW1.individualID = individualType1 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    TW1.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    pTW1.point = p1;
                    pTW1.timeWindow = TW1;
                    pTW1.subTime = pSubTime;

                    p2.individualID = individualType2 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    p2.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    p2.individualID.subTime = pSubTime;
                    p2.individualID.individualType = individualType;
                    p2.individualID.O_or_D_or_M_none = "none";
                    p2.individualID.UpOrDown = "none";
                    p2.individualID.OutOrBack = "Back";
                    TW2.individualID = individualType2 + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                    TW2.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    pTW2.point = p2;
                    pTW2.timeWindow = TW2;
                    pTW2.subTime = pSubTime;

                    pTW2.otherPoint = p1;
                    pTW1.otherPoint = p2;
                    pTW2.otherTimeWindow = TW1;
                    pTW1.otherTimeWindow = TW2;

                    pointTWList1.Add(pTW1);
                    pointTWList2.Add(pTW2);
                    coordTWDic.Add(p1.individualID, pointTWList1);
                    coordTWDic.Add(p2.individualID, pointTWList2);
                }
                else if (individualType.ToString() == "veh")
                {
                    p1.indiType = "veh";
                    p2.indiType = "veh";
                    List<PointTW> pointTWList1 = new List<PointTW>();
                    IndividualType individualType1 = individualType;
                    IndividualID IDveh = individualType1 + corV[0];
                    IDveh.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    IDveh.individualType = individualType;
                    IDveh.O_or_D_or_M_none = "none";
                    IDveh.OutOrBack = "none";
                    IDveh.UpOrDown = "none";

                    p1.individualID = individualType1 + "Out" + corV[0];
                    p1.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    p1.individualID.individualType = individualType;
                    p1.individualID.O_or_D_or_M_none = "none";
                    p1.individualID.UpOrDown = "none";
                    p1.individualID.OutOrBack = "Out";
                    TW1.individualID = individualType1 + "Out" + corV[0];
                    pTW1.point = p1;
                    pTW1.timeWindow = TW1;
                    p2.individualID = individualType1 + "Back" + corV[0];
                    p2.individualID.individualType = individualType;
                    p2.individualID.IDIndex = int.Parse(corV[0]) + pasNum_Sta;
                    p2.individualID.O_or_D_or_M_none = "none";
                    p2.individualID.UpOrDown = "none";
                    p2.individualID.OutOrBack = "Back";
                    TW2.individualID = individualType1 + "Back" + corV[0];
                    pTW2.point = p2;
                    pTW2.timeWindow = TW2;
                    pointTWList1.Add(pTW1);
                    pointTWList1.Add(pTW2);
                    coordTWDic.Add(IDveh, pointTWList1);
                }
            }
        }

        public static void GetTsOrNotandRideshareOrNot(string filepath_TsOrNotRideShareOrNot,
            int pasNum_Sta, int startNum, int coorNum,
            out Dictionary<IndividualID, int> indiID_TS_int,
            out Dictionary<IndividualID, int> indiID_RS_int)
        {
            indiID_TS_int = new Dictionary<IndividualID, int>();
            indiID_RS_int = new Dictionary<IndividualID, int>();
            // 读入所有行
            string[] corLines = File.ReadAllLines(filepath_TsOrNotRideShareOrNot);
            int PointNum;
            if (coorNum < corLines.Length - 1)
            {
                PointNum = coorNum;
            }
            else
            {
                PointNum = corLines.Length - 1;
            }
            // 让过第一行，从第二行开始处理
            for (int i = startNum; i < PointNum + 1; i++)
            {
                string corline = corLines[i];
                // 拆分行
                //string[] corV = corline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] corV = corline.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                //IndividualID individualID = "pasUp" + Convert.ToString(int.Parse(corV[0]) + pasNum_Sta);
                IndividualID individualID = "pasUp" + (i - startNum + 1).ToString();
                indiID_TS_int.Add(individualID, int.Parse(corV[1]) + pasNum_Sta);
                indiID_RS_int.Add(individualID, int.Parse(corV[2]) + pasNum_Sta);
            }
        }
        public static void Generating_Master_Results(GRBModel myModel_dy_out, Dictionary<IndividualType, Dictionary<RouteID, VarTheta>> VarThetaDic,
    Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<SpaceTimeVertices, Dictionary<RouteID, VarW>>>> VarWDic,
    Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_string_dic2,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_timestring_dic,
    out Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>> Y_typeIDRID_combined_string_dic
)
        {
            Y_typeIDRID_string_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_string_dic2 = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_timestring_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            Y_typeIDRID_combined_string_dic = new Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, string>>>();
            if (myModel_dy_out.Status == GRB.Status.OPTIMAL)
            {
                foreach (var c in VarThetaDic)
                {
                    foreach (var d in c.Value)
                    {
                        if (d.Value.GRBV.X == 0)
                        {
                            continue;
                        }
                        string output = string.Format("{0}:[{1}]", d.Value.GRBV.VarName, d.Value.GRBV.X);//"ZVeh_P_[{0}]V_[{1}]_Z[{2}]"
                        Console.WriteLine(output);
                    }
                }
                
                foreach (var TypeRID_theta_KV in VarThetaDic)
                {
                    foreach (var RID_theta_KV in TypeRID_theta_KV.Value)
                    {
                        if (RID_theta_KV.Value.GRBV.X != 0)
                        {
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
                            if (!Y_typeIDRID_string_dic.ContainsKey(startArc.individualType))
                            {
                                Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                                Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID2 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                                Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID3 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                                Dictionary<IndividualID, Dictionary<RouteID, string>> KeyValuePair_RID4 = new Dictionary<IndividualID, Dictionary<RouteID, string>>();
                                Y_typeIDRID_string_dic.Add(startArc.individualType, KeyValuePair_RID);
                                Y_typeIDRID_string_dic2.Add(startArc.individualType, KeyValuePair_RID2);
                                Y_typeIDRID_timestring_dic.Add(startArc.individualType, KeyValuePair_RID3);
                                Y_typeIDRID_combined_string_dic.Add(startArc.individualType, KeyValuePair_RID4);
                            }
                            if (!Y_typeIDRID_string_dic[startArc.individualType].ContainsKey(startArc.individualID))
                            {
                                Dictionary<RouteID, string> KeyValuePair = new Dictionary<RouteID, string>();
                                Dictionary<RouteID, string> KeyValuePair2 = new Dictionary<RouteID, string>();
                                Dictionary<RouteID, string> KeyValuePair3 = new Dictionary<RouteID, string>();
                                Dictionary<RouteID, string> KeyValuePair4 = new Dictionary<RouteID, string>();
                                Y_typeIDRID_string_dic[startArc.individualType].Add(startArc.individualID, KeyValuePair);
                                Y_typeIDRID_string_dic2[startArc.individualType].Add(startArc.individualID, KeyValuePair2);
                                Y_typeIDRID_timestring_dic[startArc.individualType].Add(startArc.individualID, KeyValuePair3);
                                Y_typeIDRID_combined_string_dic[startArc.individualType].Add(startArc.individualID, KeyValuePair4);
                            }
                            RouteID RID_this = Clone(RID_theta_KV.Key) as RouteID;
                            RID_this.route_lambda = RID_theta_KV.Value.GRBV.X;
                            string route_string = "";
                            string route_string2 = "";
                            string time_string = "";
                            string combined_string = "";
                            int howmanyarcs = ArcYList.Count;
                            int howmanyarcshaveprocessed = 0;
                            foreach (Arc arc in ArcYList)
                            {
                                string i = arc.SpaceTimeVertex1.spaceVertex.individualID.ToString();
                                string j = arc.SpaceTimeVertex2.spaceVertex.individualID.ToString();
                                string t_str;
                                string tt_str;
                                route_string += i + "_";
                                route_string2 += Y_to_notation(arc.SpaceTimeVertex1.spaceVertex.individualID);
                                time_string += arc.SpaceTimeVertex1.timeVertex;
                                combined_string += "[" + Y_to_notation(arc.SpaceTimeVertex1.spaceVertex.individualID) + "," + arc.SpaceTimeVertex1.timeVertex + "]" + ";";
                                time_string += ";";
                                route_string2 += ";";
                                howmanyarcshaveprocessed += 1;
                                if (howmanyarcshaveprocessed == howmanyarcs)
                                {
                                    route_string += j;
                                    route_string2 += Y_to_notation(arc.SpaceTimeVertex2.spaceVertex.individualID);
                                    time_string += arc.SpaceTimeVertex2.timeVertex;
                                    combined_string += "[" + Y_to_notation(arc.SpaceTimeVertex2.spaceVertex.individualID) + "," + arc.SpaceTimeVertex2.timeVertex + "]";
                                }
                            }
                            Y_typeIDRID_string_dic[startArc.individualType][startArc.individualID].Add(RID_this, route_string);
                            Y_typeIDRID_string_dic2[startArc.individualType][startArc.individualID].Add(RID_this, route_string2);
                            Y_typeIDRID_timestring_dic[startArc.individualType][startArc.individualID].Add(RID_this, time_string);
                            Y_typeIDRID_combined_string_dic[startArc.individualType][startArc.individualID].Add(RID_this, combined_string);
                        }
                    }
                }
                //int ZeroNoOneYes = 1;
                //foreach (var c in VarWDic)
                //{
                //    foreach (var d in c.Value)
                //    {
                //        foreach (var STV_ID_varW_KV in d.Value)
                //        {
                //            foreach (var ID_W_KV in STV_ID_varW_KV.Value)
                //            {
                //                if (ZeroNoOneYes == 0)
                //                {
                //                    continue;
                //                }
                //                try
                //                {
                //                    if (ID_W_KV.Value.GRBV.X == 0)
                //                    {
                //                        continue;
                //                    }
                //                    string individualID = ID_W_KV.Value.IndividualIDpas.ToString();
                //                    string routeID = ID_W_KV.Value.RouteID_W_L.ToString();
                //                    string i = ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex1.spaceVertex.individualID.ToString();
                //                    string j = ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex2.spaceVertex.individualID.ToString();
                //                    string t_str;
                //                    string tt_str;
                //                    if (ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex1.timeVertex < 10)
                //                    {
                //                        t_str = "0" + ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex1.timeVertex.ToString();
                //                    }
                //                    else
                //                    {
                //                        t_str = ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex1.timeVertex.ToString();
                //                    }
                //                    if (ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex2.timeVertex < 10)
                //                    {
                //                        tt_str = "0" + ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex2.timeVertex.ToString();
                //                    }
                //                    else
                //                    {
                //                        tt_str = ID_W_KV.Value.SpaceTimeVertices.spaceTimeVertex2.timeVertex.ToString();
                //                    }
                //                    //string output = string.Format("W_[{0}]_RouteID_[{1}]_i[{2}]_j[{3}]_t[{4}]_tt[{5}]_Y[{6}]", individualID, routeID, i, j, t_str, tt_str, ID_W_KV.Value.GRBV.X.ToString());
                //                    //Console.WriteLine(output);
                //                    string output = string.Format("{0}:[{1}]", ID_W_KV.Value.GRBV.VarName, ID_W_KV.Value.GRBV.X);//"ZVeh_P_[{0}]V_[{1}]_Z[{2}]"
                //                    Console.WriteLine(output);
                //                }
                //                catch (GRBException gex)
                //                {
                //                    ZeroNoOneYes = 0;
                //                    //Console.WriteLine(gex.HelpLink);
                //                }
                //            }
                //        }
                //    }
                //}
            }
        }

    }
}

