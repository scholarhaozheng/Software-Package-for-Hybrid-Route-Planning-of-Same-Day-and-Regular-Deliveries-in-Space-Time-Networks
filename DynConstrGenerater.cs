using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using OptimizationIntegrationSystem.zhenghao;
using OptimizationIntegrationSystem.zhenghao.ClassName;

namespace OptimizationIntegrationSystem.zhenghao
{
    class DynConstrGenerater
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
        GRBModel myModel_sta;
        GRBModel myModel_dy;
        int ImT;
        Dictionary<IndividualType, List<IndexGRBVar>> VarZDic;
        Dictionary<IndividualType, List<IndexGRBVar>> VarZDic_dy;
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic;
        Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic_dy;
        int capacity;
        int pointNumPas;
        List<IndividualID> VehList;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic_dy;
        Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType;
        Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID;
        public DynConstrGenerater(GRBModel myModel_sta, GRBModel myModel_dy, int ImT,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic,
            Dictionary<IndividualType, List<IndexGRBVar>> VarZDic_dy,///0808 比起上一个版本，我将VarZDic_dy也输了进来
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic,
            Dictionary<IndividualType, Dictionary<IndividualID, List<ArcGRBVar>>> VarYDic_dy,///0808 比起上一个版本，我将VarYDic_dy也输了进来
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic,
            Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, Dictionary<ArcID, Dictionary<ArcTime, Arc>>>>> arcDic_dy,
          Dictionary<IndividualType, Dictionary<IndividualID, Dictionary<ArcType, List<Arc>>>> arcDicIndiTypeIDArcType,
          Dictionary<IndividualType, Dictionary<IndividualID, List<Arc>>> arcDicIndiTypeID, int pointNumPas)
        {
            this.arcDic = arcDic;
            this.arcDic_dy = arcDic_dy;
            this.arcDicIndiTypeIDArcType = arcDicIndiTypeIDArcType;
            this.arcDicIndiTypeID = arcDicIndiTypeID;
            this.pointNumPas = pointNumPas;
            this.VarYDic = VarYDic;
            this.VarYDic_dy= VarYDic_dy;
            this.VarZDic_dy = VarZDic_dy;
            this.VarZDic = VarZDic;
            this.ImT = ImT;
            this.myModel_sta = myModel_sta;
            this.myModel_dy = myModel_dy;
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
            pointTsD.pointType = "TsD";pointO.individualID.individualType = "none"; pointD.individualID.individualType = "none"; pointVO.individualID.individualType = "none"; pointVD.individualID.individualType = "none"; pointTsO.individualID.individualType = "none"; pointTsD.individualID.individualType = "none";
            pointVO.individualID = "VO"; pointVO.individualID.O_or_D_or_M_none = "none";pointVO.individualID.OutOrBack = "none";
            pointVO.pointType = "VO";
            pointVD.individualID = "VD"; pointVD.individualID.O_or_D_or_M_none = "none";pointVD.individualID.OutOrBack = "none";
            pointVD.pointType = "VD";
        }


        public GRBModel Generating_Dy_Constr()
        {
            GRBModel myModel_dy_out = this.myModel_dy;
            if (myModel_sta.Status != GRB.Status.OPTIMAL)
            { return null; }

            for (int i = 0; i < this.VarYDic.Count; i++)
            {
                IndividualType individualType = VarYDic.Keys.ToList()[i];
                for (int j = 0; j < VarYDic[individualType].Count; j++)
                {
                    IndividualID individualID = VarYDic[individualType].Keys.ToList()[j];
                    for (int ID = 0; ID < VarYDic[individualType][individualID].Count; ID++)
                    {
                        ArcGRBVar ArcGRBVarY = VarYDic[individualType][individualID][ID];
                        string Y_i = ArcGRBVarY.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString();
                        int Y_t = ArcGRBVarY.arc.SpaceTimeVertex1.timeVertex;
                        int Y_tt = ArcGRBVarY.arc.SpaceTimeVertex2.timeVertex;

                        if (ImT >= Y_tt)
                        {
                            ArcGRBVar ArcGRBVarY_dy = VarYDic_dy[individualType][individualID].Where(t =>
                                t.arc.arcID.ToString() == ArcGRBVarY.arc.arcID.ToString() &&
                                t.arc.arcTime.ToString() == ArcGRBVarY.arc.arcTime.ToString()).ToList()[0];
                            GRBLinExpr constr_dy = new GRBLinExpr();
                            constr_dy = ArcGRBVarY_dy.GRBV - ArcGRBVarY.GRBV.X;////0808 我修改了这里，将新生成的dic中的Y放进constraint中
                            myModel_dy_out.AddConstr(constr_dy, GRB.EQUAL, 0, "Constr_add_" + ArcGRBVarY.GRBV.VarName);

                            if (ArcGRBVarY.GRBV.X==1&&
                                ((ArcGRBVarY.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == "O" + ArcGRBVarY.arc.individualID
                                && ArcGRBVarY.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == ArcGRBVarY.arc.individualID
                                && ArcGRBVarY.arc.individualType == pasUp) ||
                                (ArcGRBVarY.arc.SpaceTimeVertex1.spaceVertex.individualID.ToString() == ArcGRBVarY.arc.individualID
                                && ArcGRBVarY.arc.SpaceTimeVertex2.spaceVertex.individualID.ToString() == "D" + ArcGRBVarY.arc.individualID
                                && ArcGRBVarY.arc.individualType == pasDown)))//如果符合提交时间的arc是start arc或者end arc
                            {
                                List<IndexGRBVar> IndexGRBVarZList = VarZDic[veh].Where(t =>
                                t.IndividualIDPas.ToString() == ArcGRBVarY.arc.individualID.ToString()).ToList();
                                //那么选出z字典中所有与该乘客相关的z
                                foreach (IndexGRBVar IndexGRBVarZ in IndexGRBVarZList)
                                {//对于对应着该乘客的各个车辆
                                    IndexGRBVar IndexGRBVarZ_dy = VarZDic_dy[veh].Where(t =>
                                t.IndividualIDPas.ToString() == ArcGRBVarY.arc.individualID.ToString()&&
                                t.IndividualIDVeh.ToString() == IndexGRBVarZ.IndividualIDVeh.ToString()).ToList()[0];
                                    GRBLinExpr constr_dy_Z = new GRBLinExpr();
                                    constr_dy_Z = IndexGRBVarZ_dy.GRBV - IndexGRBVarZ.GRBV.X;
                                    myModel_dy_out.AddConstr(constr_dy_Z, GRB.EQUAL, 0, "Constr_add_Z_"
                                        + IndexGRBVarZ.IndividualIDPas.ToString()+"_"
                                        + IndexGRBVarZ.IndividualIDVeh.ToString());
                                }

                            }

                        }

                    }
                }

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
                    if (individualIDPas.ToString()=="pasUp5")
                    {

                    }
                    GRBLinExpr constr_Z_HaveToServe = new GRBLinExpr();
                    int ServedOrNot = 0;
                    foreach (IndividualID individualIDVeh in VehList)
                    {
                        IndexGRBVar indexGRBVarZ = VarZDic[veh].Find(
                            t => t.IndividualIDPas == individualIDPas && t.IndividualIDVeh == individualIDVeh);
                        IndexGRBVar indexGRBVarZ_dy = VarZDic_dy[veh].Find(
                            t => t.IndividualIDPas == individualIDPas && t.IndividualIDVeh == individualIDVeh);
                        ServedOrNot += (int)indexGRBVarZ.GRBV.X;
                        constr_Z_HaveToServe += indexGRBVarZ_dy.GRBV;

                    }
                    if (ServedOrNot==1)
                    {
                        myModel_dy_out.AddConstr(constr_Z_HaveToServe, GRB.EQUAL, 1, "constr_Z_HaveToServe_" + individualIDPas.ToString());
                    }
                }

            }

            return myModel_dy_out;

        }

    }
}
