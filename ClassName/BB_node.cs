using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationIntegrationSystem.zhenghao.ClassName
{
    [Serializable]
    public class BB_node
    {
        public int BB_sequence;
        public int active1_inactive0;
        public List<BB_info> pas_BB_info_list;
        public List<BB_info> veh_BB_info_list;
        public Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<RouteID, double>>> ADic_all;
        public Dictionary<IndividualType, Dictionary<SpaceTimeVertices, Dictionary<RouteID, double>>> BDic_all;
        public Dictionary<IndividualType, Dictionary<RouteID, Dictionary<IndividualID, double>>> ADic_all2;
        public Dictionary<IndividualType, Dictionary<RouteID, Dictionary<SpaceTimeVertices, double>>> BDic_all2;
        public Dictionary<IndividualType, Dictionary<RouteID, double>> CostDic_all;
        public Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_all;
        public Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc_combined_all;
        public Dictionary<IndividualType, Dictionary<RouteID, List<Arc>>> DicTypeRID_Arc;
    }
}
