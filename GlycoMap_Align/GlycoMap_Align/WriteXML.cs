using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GlycoMap_Align
{
    class WriteXML
    {
        public WriteXML(List<GlycoRecord> map)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using (XmlWriter writer = XmlWriter.Create("mergedmap.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("GlycoMap");
                foreach (GlycoRecord rec in map)
                {
                    writer.WriteStartElement("GlycoRecord");
                    writer.WriteElementString("ID", rec.id.ToString());
                    writer.WriteElementString("Mass", rec.mass.ToString());
                    writer.WriteElementString("NET", rec.net.ToString());
                    writer.WriteElementString("Protein", rec.protein);
                    writer.WriteElementString("Peptide", rec.peptide);
                    writer.WriteElementString("PeptideMass", rec.pepmass.ToString());
                    writer.WriteElementString("Site", rec.site);
                    writer.WriteElementString("Glycan", rec.glycan);
                    writer.WriteElementString("GlycanMass", rec.glymass.ToString());
                    writer.WriteElementString("RepresentativeCIDLength", rec.cidlen.ToString());
                    writer.WriteElementString("RepresentativeCIDSpectra", rec.cidstr);
                    writer.WriteElementString("RepresentativeHCDLength", rec.hcdlen.ToString());
                    writer.WriteElementString("RepresentativeHCDSpectra", rec.hcdstr);
                    writer.WriteElementString("RepresentativeETDLength", rec.etdlen.ToString());
                    writer.WriteElementString("RepresentativeETDSpectra", rec.etdstr);
                    writer.WriteElementString("RepCIDScore", rec.cidscore.ToString());
                    writer.WriteElementString("RepHCDScore", rec.hcdscore.ToString());
                    writer.WriteElementString("RepETDScore", rec.etdscore.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
