using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GlycoMap_Align
{
    struct path
    {
        public double upd;
        public int pos;
    }

    struct spectra
    {
        public float[] mz, intensity;
    }

    struct GlycoRecord
    {
        public int id, cidlen, hcdlen, etdlen;
        public double mass, net, pepmass, glymass, cidscore, hcdscore, etdscore;
        public string protein, peptide, site, glycan, cidstr, hcdstr, etdstr;
        public spectra cidspec, hcdspec, etdspec;
    }

    class GlobalVar
    {
        public static double TOLMAS = 0.00001;
        public static double TOLNET = 0.025;
        public static double BIN = 0.05;
        public static List<Color> GRADIENT;

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
