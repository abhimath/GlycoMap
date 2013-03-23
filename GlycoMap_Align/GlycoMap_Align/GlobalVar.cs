using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GlycoMap_Align
{
    struct Path
    {
        public double upd;
        public int pos;
    }

    struct Spectra
    {
        public float[] mz, intensity;
    }

    struct GlycoRecord
    {
        public int id, cidlen, hcdlen, etdlen;
        public double mass, net, pepmass, glymass, cidscore, hcdscore, etdscore;
        public string protein, peptide, site, glycan, cidstr, hcdstr, etdstr;
        public Spectra cidspec, hcdspec, etdspec;
    }

    class GlobalVar
    {
        public static double BIN = 0.05;
        public static List<Color> GRADIENT;
        public static double ERRBINMAS = 0.25;
        public static double ERRBINNET = 0.1;
        public static List<double> KEYS;
        public static double RMAXMAS = 0.0;
        public static double RMINMAS = 1000000.0;
        public static double RMAXNET = 0.0;
        public static double RMINNET = 1000000.0;
        public static double TOLMAS = 0.00001;
        public static double TOLNET = 0.025;

        public static void assignKeys()
        {
            KEYS = new List<double>();
            for (double i = GlobalVar.RMINNET; i < (GlobalVar.RMAXNET + GlobalVar.BIN); i += GlobalVar.BIN)
            {
                KEYS.Add(Math.Round(i, 2));
            }
        }

        public static void setGradient()
        {
            int rmax = Color.Red.R;
            int rmin = Color.Blue.R;
            int gmax = Color.Red.G;
            int gmin = Color.Blue.G;
            int bmax = Color.Red.B;
            int bmin = Color.Blue.B;
            GRADIENT = new List<Color>();
            for (int i = 0; i < 101; i++)
            {
                int raverage = rmin + (int)((rmax - rmin) * i / 101);
                int gaverage = gmin + (int)((gmax - gmin) * i / 101);
                int baverage = bmin + (int)((bmax - bmin) * i / 101);
                GRADIENT.Add(Color.FromArgb(raverage, gaverage, baverage));
            }
        }
    }
}
