using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GlycoMap_Align
{
    class ParseCSV
    {
        private List<GlycoRecord> map;

        public ParseCSV(String name, Boolean flag)
        {
            StreamReader file = new StreamReader(name);
            String line;
            String[] values;
            map = new List<GlycoRecord>();
            GlycoRecord record = new GlycoRecord();

            file.ReadLine();
            while ((line = file.ReadLine()) != null)
            {
                values = line.Split(',');
                if (flag)
                {
                    if (!values[3].Contains("decoy"))
                    {
                        if (values[10].Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            record.id = int.Parse(values[0]);
                            record.mass = double.Parse(values[1]);
                            record.net = double.Parse(values[2]);
                            record.protein = values[3];
                            record.peptide = values[4];
                            record.site = values[5];
                            record.glycan = values[6];
                            record.pepmass = double.Parse(values[7]);
                            record.glymass = double.Parse(values[8]);
                            record.type = values[9];
                            record.hcdscore = double.Parse(values[11]);
                            record.cidscore = double.Parse(values[12]);
                            record.etdscore = double.Parse(values[13]);
                            if (GlobalVar.REFCMAXMAS < record.mass)
                            {
                                GlobalVar.REFCMAXMAS = record.mass;
                            }
                            if (GlobalVar.REFCMINMAS > record.mass)
                            {
                                GlobalVar.REFCMINMAS = record.mass;
                            }
                            if (GlobalVar.REFCMAXNET < record.net)
                            {
                                GlobalVar.REFCMAXNET = record.net;
                            }
                            if (GlobalVar.REFCMINNET > record.net)
                            {
                                GlobalVar.REFCMINNET = record.net;
                            }
                        }
                    }
                }
                else
                {
                    record.id = int.Parse(values[0]);
                    record.mass = double.Parse(values[2]);
                    record.net = double.Parse(values[3]);
                    if (GlobalVar.TARGMAXMAS < record.mass)
                    {
                        GlobalVar.TARGMAXMAS = record.mass;
                    }
                    if (GlobalVar.TARGMINMAS > record.mass)
                    {
                        GlobalVar.TARGMINMAS = record.mass;
                    }
                    if (GlobalVar.TARGMAXNET < record.net)
                    {
                        GlobalVar.TARGMAXNET = record.net;
                    }
                    if (GlobalVar.TARGMINNET > record.net)
                    {
                        GlobalVar.TARGMINNET = record.net;
                    }
                }
                map.Add(record);
            }
            /*if (flag)
            {
                if ((GlobalVar.RMAXNET + GlobalVar.TOLNET) < 1.0)
                {
                    GlobalVar.RMAXNET += GlobalVar.TOLNET;
                    GlobalVar.RMAXNET = (Convert.ToInt32(GlobalVar.RMAXNET / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;
                }
                else
                {
                    GlobalVar.RMAXNET = (1.0 + GlobalVar.BINNET);
                }
                if ((GlobalVar.RMINNET - GlobalVar.TOLNET) > 0.0)
                {
                    GlobalVar.RMINNET -= GlobalVar.TOLNET;
                    GlobalVar.RMINNET = (Convert.ToInt32(GlobalVar.RMINNET / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;
                }
                else
                {
                    GlobalVar.RMINNET = GlobalVar.BINNET;
                }
                GlobalVar.RMAXMAS = ((GlobalVar.RMAXMAS * (1 - GlobalVar.TOLMAS)) / (1 + GlobalVar.TOLMAS));
                GlobalVar.RMAXMAS = (Convert.ToInt32(GlobalVar.RMAXMAS / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;
                GlobalVar.RMINMAS = ((GlobalVar.RMINMAS * (1 + GlobalVar.TOLMAS)) / (1 - GlobalVar.TOLMAS));
                GlobalVar.RMINMAS = (Convert.ToInt32(GlobalVar.RMINMAS / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;

                GlobalVar.assignNetKeys();
            }*/
            file.Close();
        }

        public List<GlycoRecord> getMap()
        {
            return map;
        }
    }
}
