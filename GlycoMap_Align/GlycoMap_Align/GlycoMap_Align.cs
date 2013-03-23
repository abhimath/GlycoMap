using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace GlycoMap_Align
{
    public partial class GlycoMap_Align : Form
    {
        private List<GlycoRecord> refc, targ, merg;
        private String refloc, targloc;
        private Dictionary<double, List<GlycoRecord>> refc_buck, targ_buck;
        private Dictionary<double, int> maserr, neterr;
        private List<List<double>> score;
        private List<List<int>> traceback;
        private double maxscore, minscore;
        private int masfreqmax, netfreqmax;
        private ZedGraphControl graph1, graph2, graph3, graph4;

        public GlycoMap_Align()
        {
            GlobalVar.setGradient();
            InitializeComponent();
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
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
            tabControl1.Cursor = Cursors.WaitCursor;
            if (radioButton2.Checked)
            {
                Utilities.parseDB();
                refloc = "database.xml";
            }

            if (refloc.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
            {
                ParseXML reference = new ParseXML(refloc, true);
                refc = reference.getMap();
            }
            else
            {
                ParseCSV reference = new ParseCSV(refloc, true);
                refc = reference.getMap();
            }
            if (targloc.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
            {
                ParseXML target = new ParseXML(targloc, false);
                targ = target.getMap();
            }
            else
            {
                ParseCSV target = new ParseCSV(targloc, false);
                targ = target.getMap();
            }

            Align aln = new Align(refc, targ);
            maserr = aln.getMaserr();
            neterr = aln.getNeterr();
            refc_buck = aln.getRefcBuck();
            targ_buck = aln.getTargBuck();
            score = aln.getScore();
            traceback = aln.getTraceback();
            maxscore = Math.Log(aln.getMaxscore());
            minscore = Math.Log(aln.getMinscore());
            masfreqmax = Utilities.maxFreq(maserr);
            netfreqmax = Utilities.maxFreq(neterr);

            Merge mrg = new Merge(refc_buck, targ_buck, traceback);
            merg = mrg.getMergMap();

            graph1 = new ZedGraphControl();
            graph1.Dock = DockStyle.Fill;
            graph1.GraphPane.CurveList.Clear();
            graph1.GraphPane.Title.Text = "Mass-Error Distribution";
            graph1.GraphPane.XAxis.Title.Text = "Mass-Error (PPM)";
            graph1.GraphPane.YAxis.Title.Text = "Frequency";
            graph1.GraphPane.XAxis.Scale.Min = -GlobalVar.TOLMAS * 1000000;
            graph1.GraphPane.XAxis.Scale.Max = GlobalVar.TOLMAS * 1000000;
            graph1.GraphPane.YAxis.Scale.Min = 0;
            graph1.GraphPane.YAxis.Scale.Max = masfreqmax;
            graph1.GraphPane.XAxis.Scale.MajorStep = 1;
            graph1.GraphPane.XAxis.Scale.MinorStep = 0.25;
            graph1.GraphPane.YAxis.Scale.MinorStep = masfreqmax / 20;
            graph1.GraphPane.YAxis.Scale.MajorStep = masfreqmax / 10;
            for (double i = (-GlobalVar.TOLMAS * 1000000); i < ((GlobalVar.TOLMAS * 1000000) - 0.25); i += 0.25)
            {
                BoxObj box = new BoxObj(i, maserr[Math.Round(i, 3)], 0.25, maserr[Math.Round(i, 3)], Color.Black, Color.Red);
                box.IsClippedToChartRect = true;
                graph1.GraphPane.GraphObjList.Add(box);
            }
            graph1.Invalidate();
            graph1.Refresh();
            panel1.Controls.Add(graph1);
            
            graph2 = new ZedGraphControl();
            graph2.Dock = DockStyle.Fill;
            graph2.GraphPane.CurveList.Clear();
            graph2.GraphPane.Title.Text = "NET-Error Distribution";
            graph2.GraphPane.XAxis.Title.Text = "NET-Error (%)";
            graph2.GraphPane.YAxis.Title.Text = "Frequency";
            graph2.GraphPane.XAxis.Scale.Min = -GlobalVar.TOLNET * 100;
            graph2.GraphPane.XAxis.Scale.Max = GlobalVar.TOLNET * 100;
            graph2.GraphPane.XAxis.Scale.MinorStep = 0.1;
            graph2.GraphPane.XAxis.Scale.MajorStep = 0.2;
            graph2.GraphPane.YAxis.Scale.Min = 0;
            graph2.GraphPane.YAxis.Scale.Max = netfreqmax;
            graph2.GraphPane.YAxis.Scale.MinorStep = netfreqmax / 20;
            graph2.GraphPane.YAxis.Scale.MajorStep = netfreqmax / 10;
            for (double i = (-GlobalVar.TOLNET * 100); i < ((GlobalVar.TOLNET * 100) - 0.1); i += 0.1)
            {
                BoxObj box = new BoxObj(i, neterr[Math.Round(i, 1)], 0.1, neterr[Math.Round(i, 1)], Color.Black, Color.Red);
                box.IsClippedToChartRect = true;
                graph2.GraphPane.GraphObjList.Add(box);
            }
            graph2.Invalidate();
            graph2.Refresh();
            panel2.Controls.Add(graph2);

            graph3 = new ZedGraph.ZedGraphControl();
            graph3.Dock = DockStyle.Fill;
            graph3.GraphPane.CurveList.Clear();
            graph3.GraphPane.Title.Text = "Scoring Heat Map";
            graph3.GraphPane.XAxis.Title.Text = "Target GlycoMap";
            graph3.GraphPane.YAxis.Title.Text = "Reference GlycoMap";
            graph3.GraphPane.XAxis.Scale.Min = (GlobalVar.RMINNET - GlobalVar.BIN);
            graph3.GraphPane.XAxis.Scale.Max = GlobalVar.RMAXNET;
            graph3.GraphPane.XAxis.Scale.MinorStep = GlobalVar.BIN;
            graph3.GraphPane.XAxis.Scale.MajorStep = GlobalVar.BIN * 2;
            graph3.GraphPane.YAxis.Scale.Min = (GlobalVar.RMINNET - GlobalVar.BIN);
            graph3.GraphPane.YAxis.Scale.Max = GlobalVar.RMAXNET;
            graph3.GraphPane.YAxis.Scale.MinorStep = GlobalVar.BIN;
            graph3.GraphPane.YAxis.Scale.MajorStep = GlobalVar.BIN * 2;
            for (double i = (GlobalVar.RMINNET - GlobalVar.BIN); i <= GlobalVar.RMAXNET; i += GlobalVar.BIN)
            {
                for (double j = GlobalVar.RMINNET; j <= (GlobalVar.RMAXNET + GlobalVar.BIN); j += GlobalVar.BIN)
                {
                    double val = Math.Log(score[Convert.ToInt32((i / GlobalVar.BIN))][Convert.ToInt32(((j / GlobalVar.BIN) - 1))]);
                    Color color = Utilities.binColor(maxscore, minscore, val);
                    BoxObj box = new BoxObj(i, j, GlobalVar.BIN, GlobalVar.BIN, Color.Black, color);
                    box.IsClippedToChartRect = true;
                    graph3.GraphPane.GraphObjList.Add(box);
                }
            }
            graph3.Invalidate();
            graph3.Refresh();
            panel3.Controls.Add(graph3);

            graph4 = new ZedGraph.ZedGraphControl();
            graph4.Dock = DockStyle.Fill;
            graph4.GraphPane.CurveList.Clear();
            graph4.GraphPane.Title.Text = "Mass vs NET";
            graph4.GraphPane.XAxis.Title.Text = "NET";
            graph4.GraphPane.YAxis.Title.Text = "Mass";
            PointPairList data4 = new PointPairList();
            foreach (GlycoRecord record in merg)
            {
                data4.Add(new PointPair(record.net, record.mass));
            }
            LineItem curve4 = graph4.GraphPane.AddCurve(null, data4, Color.Black, SymbolType.Circle);
            curve4.Line.IsVisible = false;
            curve4.Symbol.Fill = new Fill(Color.Red);
            curve4.Symbol.Size = 4.0F;
            graph4.GraphPane.AxisChange();
            graph4.Invalidate();
            graph4.Refresh();
            panel4.Controls.Add(graph4);
            
            tabControl1.TabPages.Add(tabPage2);
            tabControl1.TabPages.Add(tabPage3);
            tabControl1.SelectedIndex = 1;
            tabControl1.Cursor = Cursors.Default;

            new WriteXML(merg);
        }
    }
}