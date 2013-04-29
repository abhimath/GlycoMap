using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

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
        public string protein, peptide, site, glycan, type, cidstr, hcdstr, etdstr;
        public Dictionary<float, float> cidspec;//, hcdspec, etdspec;
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    struct xypair
    {
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;
        [FieldOffset(4)]
        public byte b4;
        [FieldOffset(5)]
        public byte b5;
        [FieldOffset(6)]
        public byte b6;
        [FieldOffset(7)]
        public byte b7;

        [FieldOffset(4)]
        public float x;
        [FieldOffset(0)]
        public float y;
    }

    class GlobalVar
    {
        public static double BINNET = 0.05;
        public static float BINMZ = 1.0f;
        public static List<Color> GRADIENT;
        public static double ERRBINMAS = 0.25;
        public static double ERRBINNET = 0.1;
        public static List<float> MZKEYS;
        public static List<double> NETKEYS;
        public static float MAXMZ = 2000.0f;
        public static float MINMZ = 200.0f;
        public static double RMAXMAS = 0.0;
        public static double RMINMAS = 1000000.0;
        public static double RMAXNET = 0.0;
        public static double RMINNET = 1.0;
        public static double REFCMAXMAS = 0.0;
        public static double REFCMINMAS = 1000000.0;
        public static double REFCMAXNET = 0.0;
        public static double REFCMINNET = 1.0;
        public static double TARGMAXMAS = 0.0;
        public static double TARGMINMAS = 1000000.0;
        public static double TARGMAXNET = 0.0;
        public static double TARGMINNET = 1.0;
        public static double TOLMAS = 0.00001;
        public static double TOLNET = 0.025;

        public static void assignMzKeys()
        {
            MZKEYS = new List<float>();
            for (double i = (MINMZ + BINMZ); i <= MAXMZ; i += BINMZ)
            {
                MZKEYS.Add((float)Math.Round(i, 2));
            }
        }

        public static void assignNetKeys()
        {
            NETKEYS = new List<double>();
            for (double i = RMINNET; i < (RMAXNET + BINNET); i += BINNET)
            {
                NETKEYS.Add(Math.Round(i, 2));
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
