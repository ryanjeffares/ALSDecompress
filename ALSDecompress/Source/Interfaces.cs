using System.Xml;

namespace ALSDecompress
{
    interface ISmallNode
    {
        public void CreateXmlNode(XmlDocument doc, XmlNode parent);        
    }
}
