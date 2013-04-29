using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GlycoMap_Align
{
    class Align
    {
        private Dictionary<double, List<GlycoRecord>> refc_buck, targ_buck;
        private Dictionary<double, int> maserr, neterr;
        private List<List<double>> score, matrix;
        private List<List<int>> traceback;
        private double masstddev, netstddev;
        private double maxscore = 0.0, minscore = 100.0;

        public Align(List<GlycoRecord> refc, List<GlycoRecord> targ)
        {
            /*
            for (int i = 0; i < refc.Count; i++)
            {
                foreach (float key in GlobalVar.MZKEYS)
                {
                    if (!refc[i].cidspec.ContainsKey(key))
                    {
                        refc[i].cidspec[key] = 0f;
                    }
                }
            }

            for (int i = 0; i < targ.Count; i++)
            {
                foreach (float key in GlobalVar.MZKEYS)
                {
                    if (!targ[i].cidspec.ContainsKey(key))
                    {
                        targ[i].cidspec[key] = 0f;
                    }
                }
            }
            */ 

            refc_buck = Utilities.assignNetBucket(refc);
            targ_buck = Utilities.assignNetBucket(targ);
            score = new List<List<double>>();
            matrix = new List<List<double>>();
            traceback = new List<List<int>>();

            foreach (double key in GlobalVar.NETKEYS)
            {
                if (!refc_buck.ContainsKey(key))
                {
                    refc_buck.Add(key, new List<GlycoRecord>());
                }
                if (!targ_buck.ContainsKey(key))
                {
                    targ_buck.Add(key, new List<GlycoRecord>());
                }
            }

            calculateStdDev();
            calculateScore();
            calculateMatrix();
        }

        private void calculateStdDev()
        {
            List<double> masdiff = new List<double>();
            List<double> netdiff = new List<double>();
            maserr = new Dictionary<double, int>();
            neterr = new Dictionary<double, int>();

            for (double i = (-GlobalVar.TOLMAS * 1000000); i < (GlobalVar.TOLMAS * 1000000); i += GlobalVar.ERRBINMAS)
            {
                maserr[Math.Round(i, 3)] = 0;
            }
            for (double i = (-GlobalVar.TOLNET * 100); i < (GlobalVar.TOLNET * 100); i += GlobalVar.ERRBINNET)
            {
                neterr[Math.Round(i, 1)] = 0;
            }
            string str = "TargetID,TargetMass,TargetNET,ReferenceID,ReferenceMass,ReferenceNET,Protein,Site,Peptide,Glycan,Type\n";//
            foreach (double okey in GlobalVar.NETKEYS)
            {
                foreach (double ikey in GlobalVar.NETKEYS)
                {
                    int count = 0;
                    foreach (GlycoRecord outs in targ_buck[okey])
                    {
                        double dm = 0.0, dn = 0.0, euc = 100.0;
                        GlycoRecord temprec = new GlycoRecord();//
                        int chk = 0;
                        foreach (GlycoRecord ins in refc_buck[ikey])
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
                            }
                        }
                        if (chk == 1)
                        {
                            masdiff.Add(dm);
                            netdiff.Add(dn);
                            maserr[Utilities.assignMasbin((outs.mass - temprec.mass) / (outs.mass + temprec.mass))] += 1;
                            neterr[Utilities.assignNetbin(outs.net - temprec.net)] += 1;
                            str += outs.id + "," + outs.mass + "," + outs.net + "," +
                                   temprec.id + "," + temprec.mass + "," + temprec.net + "," +
                                   temprec.protein + "," + temprec.site + "," + temprec.peptide +
                                   "," + temprec.glycan + "," + temprec.type + "\n";//
                            count++;
                        }
                    }
                    ///*
                    int left;
                    if (targ_buck[okey].Count == 0)
                    {
                        left = (refc_buck[ikey].Count - count);
                    }
                    else if (refc_buck[ikey].Count == 0)
                    {
                        left = (targ_buck[okey].Count - count);
                    }
                    else if ((targ_buck[okey].Count - count) < (refc_buck[ikey].Count - count))
                    {
                        left = (targ_buck[okey].Count - count);
                    }
                    else
                    {
                        left = (refc_buck[ikey].Count - count);
                    }
                    //*/
                    //int left = (targ_buck[okey].Count - count) + (refc_buck[ikey].Count - count);
                    for (int i = 0; i < left; i++)
                    {
                        masdiff.Add(GlobalVar.TOLMAS);
                        netdiff.Add(GlobalVar.TOLNET);
                    }
                }
            }
            System.IO.File.WriteAllText("hits.csv", str);
            masstddev = Utilities.deviation(masdiff);
            netstddev = Utilities.deviation(netdiff);
        }

        private void calculateScore()
        {
            string str="";
            foreach (double okey in GlobalVar.NETKEYS)
            {
                List<double> temp = new List<double>();
                foreach (double ikey in GlobalVar.NETKEYS)
                {
                    int count = 0;
                    double s = 0.0, dm = 0.0, dn = 0.0, cosine = 0.0;
                    foreach (GlycoRecord outs in targ_buck[okey])
                    {
                        double euc = 100.0;
                        int chk = 0;
                        foreach (GlycoRecord ins in refc_buck[ikey])
                        {
                            if (Utilities.check(outs.mass, ins.mass, outs.net, ins.net))
                            {
                                double tempmd = Math.Abs((outs.mass - ins.mass) / (outs.mass + ins.mass));
                                double tempnd = Math.Abs(outs.net - ins.net);
                                double tempeuc = Math.Sqrt(Math.Pow(tempmd, 2) + Math.Pow(tempnd, 2));
                                //double tempcos = Utilities.correlation(outs.cidspec, ins.cidspec);
                                if (euc > tempeuc)
                                {
                                    dm = tempmd;
                                    dn = tempnd;
                                    euc = tempeuc;
                                    //cosine = tempcos;
                                }
                                chk = 1;
                            }
                        }
                        if (chk == 1)
                        {
                            dm = Math.Abs(dm);
                            dn = Math.Abs(dn);
                            //s -= Math.Log(cosine) + Math.Log(masstddev * netstddev * 2 * Math.PI) - ((Math.Pow(dm, 2)) / (2 * Math.Pow(masstddev, 2))) - ((Math.Pow(dn, 2)) / (2 * Math.Pow(netstddev, 2)));
                            s -= Math.Log(masstddev * netstddev * 2 * Math.PI) - ((Math.Pow(dm, 2)) / (2 * Math.Pow(masstddev, 2))) - ((Math.Pow(dn, 2)) / (2 * Math.Pow(netstddev, 2)));
                            count++;
                        }
                    }
                    ///*
                    int left;
                    if (targ_buck[okey].Count == 0)
                    {
                        left = (refc_buck[ikey].Count - count);
                    }
                    else if (refc_buck[ikey].Count == 0)
                    {
                        left = (targ_buck[okey].Count - count);
                    }
                    else if ((targ_buck[okey].Count - count) < (refc_buck[ikey].Count - count))
                    {
                        left = (targ_buck[okey].Count - count);
                    }
                    else
                    {
                        left = (refc_buck[ikey].Count - count);
                    }
                    //*/
                    //int left = (targ_buck[okey].Count - count) + (refc_buck[ikey].Count - count);
                    for (int i = 0; i < left; i++)
                    {
                        dm = GlobalVar.TOLMAS;
                        dn = GlobalVar.TOLNET;
                        s -= Math.Log(masstddev * netstddev * 2 * Math.PI) - ((Math.Pow(dm, 2)) / (2 * Math.Pow(masstddev, 2))) - ((Math.Pow(dn, 2)) / (2 * Math.Pow(netstddev, 2)));
                    }
                    if (s != 0.0)
                    {
                        s = 1.0 / s;
                        if (s > maxscore)
                        {
                            maxscore = s;
                        }
                        if (s < minscore)
                        {
                            minscore = s;
                        }
                    }
                    else
                    {
                        s = 10.0;
                    }
                    str += s.ToString() + ",";
                    temp.Add(s);
                }
                str += "\n";
                score.Add(temp);
            }
            for (int i = 0; i < score.Count; i++)
            {
                for (int j = 0; j < score[i].Count; j++)
                {
                    if (score[i][j] == 10.0)
                    {
                        score[i][j] = maxscore;
                    }
                }
            }
            System.IO.File.WriteAllText("score.csv", str);
        }

        private void calculateMatrix()
        {
            List<double> tempupd = new List<double>();
            string str = "", str2 = "";

            /*
            for (int i = 0; i <= GlobalVar.NETKEYS.Count; i++)
            {
                tempupd.Add(i * minscore);
                str2 += (i * minscore).ToString() + ",";
            }*/
            for (int i = GlobalVar.NETKEYS.Count; i >= 0; i--)
            {
                tempupd.Add(i * minscore);
                str2 += (i * minscore).ToString() + ",";
            }
            str2 += "\n";
            matrix.Add(tempupd);

            int k = GlobalVar.NETKEYS.Count - 1;
            for (int i = 1; i <= GlobalVar.NETKEYS.Count; i++)
            {
                //tempupd = new List<double> { i * minscore };
                tempupd = new List<double> { k * minscore };
                //str2 += (i * minscore).ToString() + ",";
                str2 += (k * minscore).ToString() + ",";
                k--;
                List<int> temppos = new List<int>();
                matrix.Add(tempupd);
                for (int j = 1; j <= GlobalVar.NETKEYS.Count; j++)
                {
                    double d = matrix[i - 1][j - 1] + (2.1 * score[i - 1][j - 1]);
                    double l = matrix[i][j - 1] + minscore;
                    double u = matrix[i - 1][j] + minscore;
                    Path cur = Utilities.findMax(d, l, u);
                    str += cur.pos.ToString() + ",";
                    str2 += cur.upd.ToString() + ",";
                    tempupd.Add(cur.upd);
                    temppos.Add(cur.pos);
                }
                str += "\n";
                str2 += "\n";
                matrix.RemoveAt(i);
                matrix.Insert(i, tempupd);
                traceback.Add(temppos);
            }
            System.IO.File.WriteAllText("traceback.csv", str);
            System.IO.File.WriteAllText("matrix.csv", str2);
        }

        public Dictionary<double, int> getMaserr()
        {
            return maserr;
        }

        public Dictionary<double, int> getNeterr()
        {
            return neterr;
        }

        public Dictionary<double, List<GlycoRecord>> getRefcBuck()
        {
            return refc_buck;
        }

        public Dictionary<double, List<GlycoRecord>> getTargBuck()
        {
            return targ_buck;
        }

        public List<List<double>> getScore()
        {
            return score;
        }

        public List<List<int>> getTraceback()
        {
            return traceback;
        }

        public double getMaxscore()
        {
            return maxscore;
        }

        public double getMinscore()
        {
            return minscore;
        }
    }
}