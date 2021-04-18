using System.Collections.Generic;
using System.Xml;

namespace ALSDecompress.AbletonDataTypes
{
    class ViewStates
    {
        public Dictionary<string, int> elements;

        public ViewStates()
        {
            elements = new Dictionary<string, int>();
        }

        public void CreateXmlNode(XmlDocument doc, XmlNode parent)
        {
            var node = doc.CreateElement("ViewStates");
            foreach(var (name, value) in elements)
            {
                var el = doc.CreateElement(name);
                el.SetAttribute("Value", value.ToString());
                node.AppendChild(el);
            }
            parent.AppendChild(node);
        }
    }
}
