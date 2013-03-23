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
                    if (values[9].Equals("verified", StringComparison.OrdinalIgnoreCase))
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
                        record.hcdscore = double.Parse(values[11]);
                        record.cidscore = double.Parse(values[12]);
                        record.etdscore = double.Parse(values[13]);
                        if (GlobalVar.RMAXMAS < record.mass)
                        {
                            GlobalVar.RMAXMAS = record.mass;
                        }
                        if (GlobalVar.RMINMAS > record.mass)
                        {
                            GlobalVar.RMINMAS = record.mass;
                        }
                        if (GlobalVar.RMAXNET < record.net)
                        {
                            GlobalVar.RMAXNET = record.net;
                        }
                        if (GlobalVar.RMINNET > record.net)
                        {
                            GlobalVar.RMINNET = record.net;
                        }
                    }
                }
                else
                {
                    record.id = int.Parse(values[0]);
                    record.mass = double.Parse(values[2]);
                    record.net = double.Parse(values[3]);
                }
                map.Add(record);
            }
            if (flag)
            {
                if ((GlobalVar.RMAXNET + GlobalVar.TOLNET) < 1.0)
                {
                    GlobalVar.RMAXNET += GlobalVar.TOLNET;
                    GlobalVar.RMAXNET = (Convert.ToInt32(GlobalVar.RMAXNET / GlobalVar.BIN) + 1) * GlobalVar.BIN;
                }
                else
                {
                    GlobalVar.RMAXNET = 1.0;
                }
                if ((GlobalVar.RMINNET - GlobalVar.TOLNET) > 0.0)
                {
                    GlobalVar.RMINNET -= GlobalVar.TOLNET;
                    GlobalVar.RMINNET = (Convert.ToInt32(GlobalVar.RMINNET / GlobalVar.BIN) + 1) * GlobalVar.BIN;
                }
                else
                {
                    GlobalVar.RMINNET = 0.0;
                }
                GlobalVar.RMAXMAS = ((GlobalVar.RMAXMAS * (1 - GlobalVar.TOLMAS)) / (1 + GlobalVar.TOLMAS));
                GlobalVar.RMAXMAS = (Convert.ToInt32(GlobalVar.RMAXMAS / GlobalVar.BIN) + 1) * GlobalVar.BIN;
                GlobalVar.RMINMAS = ((GlobalVar.RMINMAS * (1 + GlobalVar.TOLMAS)) / (1 - GlobalVar.TOLMAS));
                GlobalVar.RMINMAS = (Convert.ToInt32(GlobalVar.RMINMAS / GlobalVar.BIN) + 1) * GlobalVar.BIN;

                GlobalVar.assignKeys();
            }
            file.Close();
        }

        public List<GlycoRecord> getMap()
        {
            return map;
        }
    }
}
