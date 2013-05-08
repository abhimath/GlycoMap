using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GlycoMap_Align
{
    class ParseXML
    {
        private List<GlycoRecord> map;

        public ParseXML(String file, Boolean flag)
        {
            XmlReader xml = XmlReader.Create(file);
            map = new List<GlycoRecord>();
            GlycoRecord record = new GlycoRecord();
            GlobalVar.assignMzKeys();

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
                                if (GlobalVar.REFCMAXMAS < record.mass)
                                {
                                    GlobalVar.REFCMAXMAS = record.mass;
                                }
                                if (GlobalVar.REFCMINMAS > record.mass)
                                {
                                    GlobalVar.REFCMINMAS = record.mass;
                                }
                            }
                            else
                            {
                                if (GlobalVar.TARGMAXMAS < record.mass)
                                {
                                    GlobalVar.TARGMAXMAS = record.mass;
                                }
                                if (GlobalVar.TARGMINMAS > record.mass)
                                {
                                    GlobalVar.TARGMINMAS = record.mass;
                                }
                            }
                            break;
                        case "NET":
                            xml.Read();
                            record.net = double.Parse(xml.ReadString());
                            if (flag)
                            {
                                //record.net = double.Parse(xml.ReadString());//
                                if (GlobalVar.REFCMAXNET < record.net)
                                {
                                    GlobalVar.REFCMAXNET = record.net;
                                }
                                if (GlobalVar.REFCMINNET > record.net)
                                {
                                    GlobalVar.REFCMINNET = record.net;
                                }
                            }
                            else
                            {
                                //record.net = double.Parse(xml.ReadString()) + 0.024;//
                                if (GlobalVar.TARGMAXNET < record.net)
                                {
                                    GlobalVar.TARGMAXNET = record.net;
                                }
                                if (GlobalVar.TARGMINNET > record.net)
                                {
                                    GlobalVar.TARGMINNET = record.net;
                                }
                            }
                            break;
                        case "Protein":
                            xml.Read();
                            if (flag)
                            {
                                string prot = xml.ReadString();
                                if (!prot.Contains("decoy"))
                                {
                                    record.protein = prot;
                                }
                                else
                                {
                                    record = new GlycoRecord();
                                    continue;
                                }
                            }
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
                        case "GlycanSequence":
                            xml.Read();
                            break;
                        case "GlycanMass":
                            xml.Read();
                            record.glymass = double.Parse(xml.ReadString());
                            break;
                        case "FalseHit":
                            xml.Read();
                            break;
                        case "IDType":
                            record.type = xml.ReadString();
                            break;
                        case "RepresentativeCIDLength":
                            xml.Read();
                            record.cidlen = int.Parse(xml.ReadString());
                            break;
                        case "RepresentativeCIDSpectra":
                            xml.Read();
                            record.cidstr = xml.ReadString();
                            Spectra cid = Utilities.parseBase64(record.cidlen, record.cidstr);
                            record.cidspec = Utilities.assignMzBucket(cid.mz);
                            break;
                        case "RepresentativeHCDLength":
                            xml.Read();
                            record.hcdlen = int.Parse(xml.ReadString());
                            break;
                        case "RepresentativeHCDSpectra":
                            xml.Read();
                            record.hcdstr = xml.ReadString();
                            //Spectra hcd = Utilities.parseBase64(record.hcdlen, record.hcdstr);
                            //record.hcdspec = Utilities.assignMzBucket(hcd.mz);
                            break;
                        case "RepresentativeETDLength":
                            xml.Read();
                            record.etdlen = int.Parse(xml.ReadString());
                            break;
                        case "RepresentativeETDSpectra":
                            xml.Read();
                            record.etdstr = xml.ReadString();
                            //Spectra etd = Utilities.parseBase64(record.etdlen, record.etdstr);
                            //record.etdspec = Utilities.assignMzBucket(etd.mz);
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
                            break;
                        case "RepCIDSequencing":
                            xml.Read();
                            if (record.mass > 0)
                            {
                                map.Add(record);
                            }
                            break;
                    }
                }
            }
            /*if (flag)
            {
                if ((GlobalVar.RMAXNET + GlobalVar.TOLNET) < 1.0)
                {
                    GlobalVar.RMAXNET += GlobalVar.TOLNET;
                    GlobalVar.RMAXNET = (Convert.ToInt32(GlobalVar.RMAXNET / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;
                }
                else
                {
                    GlobalVar.RMAXNET = 1.0;
                }
                if ((GlobalVar.RMINNET - GlobalVar.TOLNET) > 0.0)
                {
                    GlobalVar.RMINNET -= GlobalVar.TOLNET;
                    GlobalVar.RMINNET = (Convert.ToInt32(GlobalVar.RMINNET / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;
                }
                else
                {
                    GlobalVar.RMINNET = GlobalVar.BINNET;
                }
                GlobalVar.RMAXMAS = ((GlobalVar.RMAXMAS * (1 - GlobalVar.TOLMAS)) / (1 + GlobalVar.TOLMAS));
                GlobalVar.RMAXMAS = (Convert.ToInt32(GlobalVar.RMAXMAS / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;
                GlobalVar.RMINMAS = ((GlobalVar.RMINMAS * (1 + GlobalVar.TOLMAS)) / (1 - GlobalVar.TOLMAS));
                GlobalVar.RMINMAS = (Convert.ToInt32(GlobalVar.RMINMAS / GlobalVar.BINNET) + 1) * GlobalVar.BINNET;

                GlobalVar.assignNetKeys();
            }*/
        }

        public List<GlycoRecord> getMap()
        {
            return map;
        }
    }
}