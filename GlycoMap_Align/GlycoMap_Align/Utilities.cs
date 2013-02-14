using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Data;
using System.Drawing;

namespace GlycoMap_Align
{
    class Utilities
    {
        public static void parseDB()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://ggdb.informatics.indiana.edu/glycomap/data.php");
            request.ContentType = "application/xml";
            DataSet ds = new DataSet();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            using (StreamWriter writer = new StreamWriter("database.xml"))
            {
                ds.ReadXml(stream);
                writer.Write(ds.GetXml());
            }
        }

        public static bool check(double fm, double gm, double fn, double gn)
        {
            if ((Math.Abs((fm - gm) / (fm + gm)) < GlobalVar.TOLMAS) && (Math.Abs(fn - gn) < GlobalVar.TOLNET))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static double assignMasbin(double err)
        {
            for (double i = (-GlobalVar.TOLMAS * 1000000); i < (GlobalVar.TOLMAS * 1000000); i += 0.25)
            {
                if (((err * 1000000) >= Math.Round(i, 3)) && ((err * 1000000) < Math.Round((i + 0.25), 3)))
                {
                    return Math.Round(i, 3);
                }
            }
            return 20.0;
        }

        public static double assignNetbin(double err)
        {
            for (double i = (-GlobalVar.TOLNET * 100); i < (GlobalVar.TOLNET * 100); i += 0.1)
            {
                if (((err * 100) >= Math.Round(i, 1)) && ((err * 100) < Math.Round((i + 0.1), 1)))
                {
                    return Math.Round(i, 1);
                }
            }
            return 20.0;
        }

        public static double deviation(List<double> values)
        {
            double std;
            double avg = values.Average();
            double sum = values.Sum(d => Math.Pow(d - avg, 2));
            std = Math.Sqrt((sum) / (values.Count() - 1));
            return std;
        }

        public static path findMax(double s, double d, double l, double u)
        {
            path temp;
            double sd = d + (2.1 * s);
            double sl = l + s;
            double su = u + s;

            if (sd >= sl && sd >= su)
            {
                temp.upd = sd;
                temp.pos = 0;
            }
            else if (sl > sd && sl >= su)
            {
                temp.upd = sl;
                temp.pos = 1;
            }
            else
            {
                temp.upd = su;
                temp.pos = 2;
            }

            return temp;
        }

        public static Dictionary<double, List<GlycoRecord>> assignBucket(List<GlycoRecord> map)
        {
            Dictionary<double, List<GlycoRecord>> tempdict = new Dictionary<double, List<GlycoRecord>>();

            for (int i = 0; i < map.Count; i++)
            {
                GlycoRecord tempglyrec;
                tempglyrec = map[i];
                List<GlycoRecord> templist;
                double ind = Math.Round((((int)((map[i].net / GlobalVar.BIN) + 1.0) * GlobalVar.BIN)), 2);

                if (tempdict.ContainsKey(ind))
                {
                    templist = tempdict[ind];
                }
                else
                {
                    templist = new List<GlycoRecord>();
                }

                templist.Add(tempglyrec);
                tempdict[ind] = templist;
            }
            return tempdict;
        }

        public static List<double> assignKeys()
        {
            List<double> keys = new List<double>();

            for (double i = GlobalVar.BIN; i < 1.05; i += GlobalVar.BIN)
            {
                keys.Add(Math.Round(i, 2));
            }
            return keys;
        }

        public static Color binColor(double maxscore, double minscore, double score)
        {
            int index = Convert.ToInt32(((score - minscore) / (maxscore - minscore)) * 100);
            return GlobalVar.GRADIENT[index];
        }
    }
}
