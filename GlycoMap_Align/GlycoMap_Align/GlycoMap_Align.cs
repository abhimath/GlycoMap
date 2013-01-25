using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GlycoMap_Align
{
    public partial class GlycoMap_Align : Form
    {
        private List<GlycoRecord> refc, targ, merg;
        private Dictionary<double, List<GlycoRecord>> refc_buck, targ_buck;
        private List<double> keys;
        private List<List<double>> score, matrix;
        private List<List<int>> traceback;

        public GlycoMap_Align()
        {
            InitializeComponent();

            Utilities.parseDB();
            String refloc = "database.xml";
            String targloc = "F:\\Dropbox\\IUB\\Courses\\Capstone\\Stuff\\Scripts\\pooledtargetediii_map_glycopeptides.xml";

            ParseXML reference = new ParseXML(refloc);
            ParseXML target = new ParseXML(targloc);
            refc = reference.getMap();
            targ = target.getMap();

            Align aln = new Align(refc, targ);
            keys = aln.getKeys();
            refc_buck = aln.getRefcBuck();
            targ_buck = aln.getTargBuck();
            score = aln.getScore();
            matrix = aln.getMatrix();
            traceback = aln.getTraceback();

            Merge mrg = new Merge(keys, refc_buck, targ_buck, traceback);
            merg = mrg.getMergMap();

            new WriteXML(merg);
        }
    }
}
