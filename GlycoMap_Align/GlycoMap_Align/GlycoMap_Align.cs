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
        private String refloc, targloc;
        private Dictionary<double, List<GlycoRecord>> refc_buck, targ_buck;
        private List<double> keys;
        private List<List<double>> score, matrix;
        private List<List<int>> traceback;

        public GlycoMap_Align()
        {
            InitializeComponent();
        }

        private void GlycoMap_Align_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                targloc = openFileDialog1.FileName;
                Console.WriteLine(targloc);
                textBox1.Text = targloc;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                refloc = openFileDialog2.FileName;
                Console.WriteLine(refloc);
                textBox2.Text = refloc;
            }
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox2.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                textBox2.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                Utilities.parseDB();
                refloc = "database.xml";
            }

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
