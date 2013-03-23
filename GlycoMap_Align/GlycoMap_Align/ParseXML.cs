using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace GlycoMap_Align
{
    class ParseXML
    {
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
        private struct xypair
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
        private List<GlycoRecord> map;

        public ParseXML(String file, Boolean flag)
        {
            XmlReader xml = XmlReader.Create(file);
            map = new List<GlycoRecord>();
            GlycoRecord record = new GlycoRecord();

            while (xml.Read())
            {
                if (xml.IsStartElement())
                {
                    switch (xml.Name)
                    {
                        case "ID":
                            xml.Read();
                            record.id = int.Parse(xml.ReadString());
                            break;
                        case "Mass":
                            xml.Read();
                            record.mass = double.Parse(xml.ReadString());
                            if (flag)
                            {
                                if (GlobalVar.RMAXMAS < record.mass)
                                {
                                    GlobalVar.RMAXMAS = record.mass;
                                }
                                if (GlobalVar.RMINMAS > record.mass)
                                {
                                    GlobalVar.RMINMAS = record.mass;
                                }
                            }
                            break;
                        case "NET":
                            xml.Read();
                            record.net = double.Parse(xml.ReadString());
                            if (flag)
                            {
                                if (GlobalVar.RMAXNET < record.net)
                                {
                                    GlobalVar.RMAXNET = record.net;
                                }
                                if (GlobalVar.RMINNET > record.net)
                                {
                                    GlobalVar.RMINNET = record.net;
                                }
                            }
                            break;
                        case "Protein":
                            xml.Read();
                            record.protein = xml.ReadString();
                            break;
                        case "Peptide":
                            xml.Read();
                            record.peptide = xml.ReadString();
                            break;
                        case "PeptideMass":
                            xml.Read();
                            record.pepmass = double.Parse(xml.ReadString());
                            break;
                        case "Site":
                            xml.Read();
                            record.site = xml.ReadString();
                            break;
                        case "Glycan":
                            xml.Read();
                            record.glycan = xml.ReadString();
                            break;
                        case "GlycanMass":
                            xml.Read();
                            record.glymass = double.Parse(xml.ReadString());
                            break;
                        case "RepresentativeCIDLength":
                            xml.Read();
                            record.cidlen = int.Parse(xml.ReadString());
                            break;
                        case "RepresentativeCIDSpectra":
                            xml.Read();
                            record.cidstr = xml.ReadString();
                            record.cidspec = parseBase64(record.cidlen, record.cidstr);
                            break;
                        case "RepresentativeHCDLength":
                            xml.Read();
                            record.hcdlen = int.Parse(xml.ReadString());
                            break;
                        case "RepresentativeHCDSpectra":
                            xml.Read();
                            record.hcdstr = xml.ReadString();
                            record.hcdspec = parseBase64(record.hcdlen, record.hcdstr);
                            break;
                        case "RepresentativeETDLength":
                            xml.Read();
                            record.etdlen = int.Parse(xml.ReadString());
                            break;
                        case "RepresentativeETDSpectra":
                            xml.Read();
                            record.etdstr = xml.ReadString();
                            record.etdspec = parseBase64(record.etdlen, record.etdstr);
                            break;
                        case "RepCIDScore":
                            xml.Read();
                            record.cidscore = double.Parse(xml.ReadString());
                            break;
                        case "RepHCDScore":
                            xml.Read();
                            record.hcdscore = double.Parse(xml.ReadString());
                            break;
                        case "RepETDScore":
                            xml.Read();
                            record.etdscore = double.Parse(xml.ReadString());
                            map.Add(record);
                            break;
                    }
                }
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
                    GlobalVar.RMINNET = GlobalVar.BIN;
                }
                GlobalVar.RMAXMAS = ((GlobalVar.RMAXMAS * (1 - GlobalVar.TOLMAS)) / (1 + GlobalVar.TOLMAS));
                GlobalVar.RMAXMAS = (Convert.ToInt32(GlobalVar.RMAXMAS / GlobalVar.BIN) + 1) * GlobalVar.BIN;
                GlobalVar.RMINMAS = ((GlobalVar.RMINMAS * (1 + GlobalVar.TOLMAS)) / (1 - GlobalVar.TOLMAS));
                GlobalVar.RMINMAS = (Convert.ToInt32(GlobalVar.RMINMAS / GlobalVar.BIN) + 1) * GlobalVar.BIN;

                GlobalVar.assignKeys();
            }
        }

        private Spectra parseBase64(int speclen, string specstr)
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

        public List<GlycoRecord> getMap()
        {
            return map;
        }
    }
}