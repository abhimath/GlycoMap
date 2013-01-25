using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycoMap_Align
{
    class Merge
    {
        List<GlycoRecord> merg;

        public Merge(List<double> keys, Dictionary<double, List<GlycoRecord>> refc_buck, Dictionary<double, List<GlycoRecord>> targ_buck, List<List<int>> traceback)
        {
            merg = copyData(keys, refc_buck, targ_buck, traceback);
        }

        private List<GlycoRecord> copyData(List<double> keys, Dictionary<double, List<GlycoRecord>> refc_buck, Dictionary<double, List<GlycoRecord>> targ_buck, List<List<int>> traceback)
        {
            List<GlycoRecord> tempmap = new List<GlycoRecord>();
            int i = keys.Count - 1;
            int j = keys.Count - 1;

            while (i >= 0 || j >= 0)
            {
                if (traceback[i][j] == 0)
                {
                    foreach (GlycoRecord outs in targ_buck[keys[i]])
                    {
                        int check = 0;
                        foreach (GlycoRecord ins in refc_buck[keys[j]])
                        {
                            if (Utilities.check(outs.mass, ins.mass, outs.net, ins.net))
                            {
                                tempmap.Add(ins);
                                check++;
                                break;
                            }
                        }
                        if (check == 0)
                        {
                            tempmap.Add(outs);
                        }
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
                    foreach (GlycoRecord outs in targ_buck[keys[i]])
                    {
                        tempmap.Add(outs);
                    }
                    i--;
                }
            }
            return tempmap;
        }

        public List<GlycoRecord> getMergMap()
        {
            return merg;
        }
    }
}
