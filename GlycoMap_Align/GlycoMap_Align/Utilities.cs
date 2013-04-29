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
        public static Dictionary<float, float> assignMzBucket(float[] mz)
        {
            Dictionary<float, float> tempdict = new Dictionary<float, float>();
            Dictionary<float, List<float>> tempcount = new Dictionary<float, List<float>>();

            for (int i = 0; i < mz.Length; i++)
            {
                if (mz[i] >= GlobalVar.MINMZ && mz[i] < GlobalVar.MAXMZ)
                {
                    float key = (float) Math.Round((Convert.ToInt32((mz[i] / GlobalVar.BINMZ) + 1.0) * GlobalVar.BINMZ), 2);

                    if (tempdict.ContainsKey(key))
                    {
                        tempcount[key].Add(mz[i]);
                    }
                    else
                    {
                        tempcount[key] = new List<float>() { mz[i] };
                    }
                }
            }

            foreach (float key in tempcount.Keys)
            {
                tempdict[key] = tempcount[key].Average();
            }
            return tempdict;
        }

        public static Dictionary<double, List<GlycoRecord>> assignNetBucket(List<GlycoRecord> map)
        {
            Dictionary<double, List<GlycoRecord>> tempdict = new Dictionary<double, List<GlycoRecord>>();

            for (int i = 0; i < map.Count; i++)
            {
                double key = Math.Round((Convert.ToInt32((map[i].net / GlobalVar.BINNET) + 1.0) * GlobalVar.BINNET), 2);

                if (tempdict.ContainsKey(key))
                {
                    tempdict[key].Add(map[i]);
                }
                else
                {
                    tempdict[key] = new List<GlycoRecord>() { map[i] };
                }
            }
            return tempdict;
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

        public static Color binColor(double maxscore, double minscore, double score)
        {
            int index = Convert.ToInt32(((score - minscore) / (maxscore - minscore)) * 100);
            return GlobalVar.GRADIENT[index];
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

        public static double correlation(Dictionary<float, float> fz, Dictionary<float, float> gz)
        {
            double dot = 0.0;
            double val1 = 0.0;
            double val2 = 0.0;
            double cosine;

            foreach (float key in GlobalVar.MZKEYS)
            {
                dot += fz[key] * gz[key];
                val1 += Math.Pow(fz[key], 2);
                val2 += Math.Pow(gz[key], 2);
            }
            cosine = (dot / (Math.Sqrt(val1) * Math.Sqrt(val2)));
            if (double.IsNaN(cosine))
            {
                return 1;
            }
            if (cosine == 0)
            {
                return 1.0E-300;
            }
            return cosine;
        }

        public static double deviation(List<double> values)
        {
            double std;
            double avg = values.Average();
            double sum = values.Sum(d => Math.Pow(d - avg, 2));
            std = Math.Sqrt((sum) / (values.Count() - 1));
            return std;
        }

        public static Path findMax(double d, double l, double u)
        {
            Path temp;

            if (d >= l && d >= u)
            {
                temp.upd = d;
                temp.pos = 0;
            }
            else if (l > d && l >= u)
            {
                temp.upd = l;
                temp.pos = 1;
            }
            else
            {
                temp.upd = u;
                temp.pos = 2;
            }

            return temp;
        }

        public static int maxFreq(Dictionary<double, int> err)
        {
            int max = 0;
            foreach (double key in err.Keys)
            {
                if (err[key] > max)
                {
                    max = err[key];
                }
            }
            return max;
        }

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

        public static Spectra parseBase64(int speclen, string specstr)
        {
            byte[] cid = System.Convert.FromBase64String(specstr);
            Spectra spec = new Spectra();
            spec.mz = new float[speclen];
            spec.intensity = new float[speclen];
            int offset;
            for (int i = 0; i < speclen; i++)
            {
                xypair val;
                val.x = 0;
                val.y = 0;
                offset = i * 8;
                val.b0 = cid[offset + 7];
                val.b1 = cid[offset + 6];
                val.b2 = cid[offset + 5];
                val.b3 = cid[offset + 4];
                val.b4 = cid[offset + 3];
                val.b5 = cid[offset + 2];
                val.b6 = cid[offset + 1];
                val.b7 = cid[offset];
                spec.mz[i] = val.x;
                spec.intensity[i] = val.y;
            }
            return spec;
        }
    }
}
