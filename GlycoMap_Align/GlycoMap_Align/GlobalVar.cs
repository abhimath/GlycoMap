using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
