using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycoMap_Align
{
    class Merge
    {
        List<GlycoRecord> merg;
        string str = "TargetID,TargetMass,TargetNET,ReferenceID,ReferenceMass,ReferenceNET,Protein,Site,Peptide,Glycan,Type\n";//

        public Merge(Dictionary<double, List<GlycoRecord>> refc_buck, Dictionary<double, List<GlycoRecord>> targ_buck, List<List<int>> traceback)
        {
            merg = new List<GlycoRecord>();
            int i = GlobalVar.NETKEYS.Count - 1;
            int j = GlobalVar.NETKEYS.Count - 1;

            while (i >= 0 || j >= 0)
            {
                if (traceback[i][j] == 0)
                {
                    copyData(targ_buck[GlobalVar.NETKEYS[i]], refc_buck[GlobalVar.NETKEYS[j]]);
                    if (i != 0)
                    {
                        copyData(targ_buck[GlobalVar.NETKEYS[i - 1]], refc_buck[GlobalVar.NETKEYS[j]]);
                    }
                    if (j != 0)
                    {
                        copyData(targ_buck[GlobalVar.NETKEYS[i]], refc_buck[GlobalVar.NETKEYS[j - 1]]);
                    }
                    i--;
                    j--;
                }
                else if (traceback[i][j] == 1)
                {
                    j--;
                }
                else
                {
                    foreach (GlycoRecord outs in targ_buck[GlobalVar.NETKEYS[i]])
                    {
                        //tempmap.Add(outs);
                    }
                    i--;
                }
            }
            System.IO.File.WriteAllText("new.csv", str);
        }

        private void copyData(List<GlycoRecord> targ_ikey, List<GlycoRecord> refc_jkey)
        {
            foreach (GlycoRecord outs in targ_ikey)
            {
                double dm = 0.0, dn = 0.0, euc = 100.0;
                GlycoRecord temprec = new GlycoRecord();//
                int chk = 0;
                foreach (GlycoRecord ins in refc_jkey)
                {
                    if (Utilities.check(outs.mass, ins.mass, outs.net, ins.net))
                    {
                        double tempmd = Math.Abs((outs.mass - ins.mass) / (outs.mass + ins.mass));
                        double tempnd = Math.Abs(outs.net - ins.net);
                        double tempeuc = Math.Sqrt(Math.Pow(tempmd, 2) + Math.Pow(tempnd, 2));
                        if (euc > tempeuc)
                        {
                            dm = tempmd;
                            dn = tempnd;
                            euc = tempeuc;
                            temprec = ins;//
                        }
                        chk = 1;
                        //break;
                    }
                }
                if (chk == 1)
                {
                    merg.Add(temprec);
                    str += outs.id + "," + outs.mass + "," + outs.net + "," +
                           temprec.id + "," + temprec.mass + "," + temprec.net + "," +
                           temprec.protein + "," + temprec.site + "," + temprec.peptide +
                           "," + temprec.glycan + "," + temprec.type + "\n";//
                }
                if (chk == 0)
                {
                    //tempmap.Add(outs);
                }
            }
        }
        /*
        List<List<int>> ijpair = new List<List<int>>() { new List<int>() {(GlobalVar.NETKEYS.Count - 1), (GlobalVar.NETKEYS.Count - 1)},
                                                        new List<int>() {(GlobalVar.NETKEYS.Count - 2), (GlobalVar.NETKEYS.Count - 1)},
                                                        new List<int>() {(GlobalVar.NETKEYS.Count - 1), (GlobalVar.NETKEYS.Count - 2)} };

        foreach (List<int> start in ijpair)
        {
            int i = start[0];
            int j = start[1];

            while (i >= 0 && j >= 0)
            {
                if (traceback[i][j] == 0)
                {
                    foreach (GlycoRecord outs in targ_buck[GlobalVar.NETKEYS[i]])
                    {
                        double dm = 0.0, dn = 0.0, euc = 100.0;
                        GlycoRecord temprec = new GlycoRecord();//
                        int chk = 0;
                        foreach (GlycoRecord ins in refc_buck[GlobalVar.NETKEYS[j]])
                        {
                            if (Utilities.check(outs.mass, ins.mass, outs.net, ins.net))
                            {
                                double tempmd = Math.Abs((outs.mass - ins.mass) / (outs.mass + ins.mass));
                                double tempnd = Math.Abs(outs.net - ins.net);
                                double tempeuc = Math.Sqrt(Math.Pow(tempmd, 2) + Math.Pow(tempnd, 2));
                                if (euc > tempeuc)
                                {
                                    dm = tempmd;
                                    dn = tempnd;
                                    euc = tempeuc;
                                    temprec = ins;//
                                }
                                chk = 1;
                                //break;
                            }
                        }
                        if (chk == 1)
                        {
                            tempmap.Add(temprec);
                            str += outs.id + "," + outs.mass + "," + outs.net + "," +
                                    temprec.id + "," + temprec.mass + "," + temprec.net + "," +
                                    temprec.protein + "," + temprec.site + "," + temprec.peptide +
                                    "," + temprec.glycan + "," + temprec.type + "\n";//
                        }
                        if (chk == 0)
                        {
                            //tempmap.Add(outs);
                        }
                    }
                }
                i--;
                j--;
            }
        }
        */

        public List<GlycoRecord> getMergMap()
        {
            return merg;
        }
    }
}
