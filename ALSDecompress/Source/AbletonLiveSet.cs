using System.Xml;
using ALSDecompress.AbletonDataTypes;

namespace ALSDecompress
{
    public class AbletonLiveSet
    {
        public string name;
        public AbletonHeader abletonHeader;
        private string _xmlVersion, _xmlEncoding;
        
        public AbletonLiveSet(string n, string v, string e, AbletonHeader a)
        {
            name = n;
            _xmlVersion = v;
            _xmlEncoding = e;
            abletonHeader = a;
        }

        public AbletonLiveSet()
        {
        }
        
        public XmlDocument CreateDocumentFromData()
        {
            var xmlDoc = new XmlDocument();
            var decl = xmlDoc.CreateXmlDeclaration(_xmlVersion, _xmlEncoding, "yes");
            var abletonNode = xmlDoc.CreateElement("Ableton");
            abletonNode.SetAttribute("MajorVersion", abletonHeader.MajorVersion.ToString());
            abletonNode.SetAttribute("MinorVersion", abletonHeader.MinorVersion);
            abletonNode.SetAttribute("SchemaChangeCount", abletonHeader.SchemaChangeCount.ToString());
            abletonNode.SetAttribute("Creator", abletonHeader.Creator);
            abletonNode.SetAttribute("Revision", abletonHeader.Revision);
            var firstChild = xmlDoc.CreateElement("LiveSet");
            
            foreach (var el in abletonHeader.intValues)
            {
                var element = xmlDoc.CreateElement(el.Key); 
                element.SetAttribute("Value", el.Value.ToString());
                firstChild.AppendChild(element);
            }

            foreach (var el in abletonHeader.boolValues)
            {
                var element = xmlDoc.CreateElement(el.Key);
                element.SetAttribute("Value", el.Value.ToString().ToLower());
                firstChild.AppendChild(element);
            }

            var videoRectElement = xmlDoc.CreateElement("VideoWindowRect");
            foreach (var el in abletonHeader.videoRect)
            {
                videoRectElement.SetAttribute(el.Key, el.Value.ToString());
            }

            var viewDataElement = xmlDoc.CreateElement("ViewData");
            viewDataElement.SetAttribute("Value", abletonHeader.viewData);
            var annotationElement = xmlDoc.CreateElement("Annotation");
            annotationElement.SetAttribute("Value", abletonHeader.annotation);

            firstChild.AppendChild(videoRectElement);
            firstChild.AppendChild(viewDataElement);
            firstChild.AppendChild(annotationElement);
            abletonNode.AppendChild(firstChild);
            xmlDoc.AppendChild(abletonNode);
            xmlDoc.InsertBefore(decl, abletonNode);
            return xmlDoc;
        }
    }
}