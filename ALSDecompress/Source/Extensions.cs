using System.Xml;

namespace ALSDecompress
{
    public static class Extensions
    {
        public static void AddValueElement<T>(this T val, string name, XmlDocument doc, XmlNode node)
        {
            var el = doc.CreateElement(name);
            el.SetAttribute("Value", val.ToString());
            node.AppendChild(el);            
        }

        public static void AddValueElement(this string val, XmlDocument doc, XmlNode node)
        {
            var el = doc.CreateElement(nameof(val));
            el.SetAttribute("Value", val);
            node.AppendChild(el);
        }
    }
}
