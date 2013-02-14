using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GlycoMap_Align
{
    class Align
    {
        private Dictionary<double, List<GlycoRecord>> refc_buck, targ_buck;
        private List<double> keys;
        private Dictionary<double, int> maserr, neterr;
        private List<List<double>> score, matrix;
        private List<List<int>> traceback;
        private double masstddev, netstddev;
        private double maxscore = 0.0, minscore = 100.0;

        public Align(List<GlycoRecord> refc, List<GlycoRecord> targ)
        {
            refc_buck = Utilities.assignBucket(refc);
            targ_buck = Utilities.assignBucket(targ);
            keys = Utilities.assignKeys();
            score = new List<List<double>>();
            matrix = new List<List<double>>();
            traceback = new List<List<int>>();

            foreach (double key in keys)
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

            for (double i = (-GlobalVar.TOLMAS * 1000000); i < (GlobalVar.TOLMAS * 1000000); i += 0.25)
            {
                maserr[Math.Round(i, 3)] = 0;
            }

            for (double i = (-GlobalVar.TOLNET * 100); i < (GlobalVar.TOLNET * 100); i += 0.1)
            {
                neterr[Math.Round(i, 1)] = 0;
            }

            foreach (double okey in keys)
            {
                foreach (double ikey in keys)
                {
                    int count = 0;
                    foreach (GlycoRecord outs in targ_buck[okey])
                    {
                        foreach (GlycoRecord ins in refc_buck[ikey])
                        {
                            if (Utilities.check(outs.mass, ins.mass, outs.net, ins.net))
                            {
                                masdiff.Add(Math.Abs((outs.mass - ins.mass) / (outs.mass + ins.mass)));
                                netdiff.Add(Math.Abs(outs.net - ins.net));
                                maserr[Utilities.assignMasbin(Math.Abs((outs.mass - ins.mass) / (outs.mass + ins.mass)))] += 1;
                                neterr[Utilities.assignNetbin(Math.Abs(outs.net - ins.net))] += 1;
                                count++;
                                break;
                            }
                        }
                    }
                    int left = (targ_buck[okey].Count - count) + (refc_buck[ikey].Count - count);
                    for (int i = 0; i < left; i++)
                    {
                        masdiff.Add(GlobalVar.TOLMAS);
                        netdiff.Add(GlobalVar.TOLNET);
                    }
                }
            }
            masstddev = Utilities.deviation(masdiff);
            netstddev = Utilities.deviation(netdiff);
            System.Console.WriteLine(masstddev);
            System.Console.WriteLine(netstddev);
        }

        private void calculateScore()
        {
            foreach (double okey in keys)
            {
                List<double> temp = new List<double>();
                foreach (double ikey in keys)
                {
                    int count = 0;
                    double s = 0.0, dm = 0.0, dn = 0.0;
                    foreach (GlycoRecord outs in targ_buck[okey])
                    {
                        foreach (GlycoRecord ins in refc_buck[ikey])
                        {
                            if (Utilities.check(outs.mass, ins.mass, outs.net, ins.net))
                            {
                                dm = Math.Abs((outs.mass - ins.mass) / (outs.mass + ins.mass));
                                dn = Math.Abs(outs.net - ins.net);
                                s -= Math.Log(masstddev * netstddev * 2 * Math.PI) - ((Math.Pow(dm, 2)) / (2 * Math.Pow(masstddev, 2))) - ((Math.Pow(dn, 2)) / (2 * Math.Pow(netstddev, 2)));
                                count++;
                                break;
                            }
                        }
                    }
                    int left = (targ_buck[okey].Count - count) + (refc_buck[ikey].Count - count);
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
                    temp.Add(s);
                }
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
        }

        private void calculateMatrix()
        {
            List<double> tempupd = new List<double>();
            
            for (int i = 0; i <= keys.Count; i++)
            {
                tempupd.Add(i * minscore);
            }
            matrix.Add(tempupd);

            for (int i = 1; i <= keys.Count; i++)
            {
                tempupd = new List<double> { i * minscore };
                List<int> temppos = new List<int>();
                matrix.Add(tempupd);
                for (int j = 1; j <= keys.Count; j++)
                {
                    path cur = Utilities.findMax(score[i - 1][j - 1], matrix[i - 1][j - 1], matrix[i][j - 1], matrix[i - 1][j]);
                    tempupd.Add(cur.upd);
                    temppos.Add(cur.pos);
                }
                matrix.RemoveAt(i);
                matrix.Insert(i, tempupd);
                traceback.Add(temppos);
            }
        }

        public List<double> getKeys()
        {
            return keys;
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