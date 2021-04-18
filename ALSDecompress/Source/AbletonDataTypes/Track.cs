using System.Collections.Generic;
using System.Xml;

namespace ALSDecompress.AbletonDataTypes
{
    public enum TrackType
    {
        MidiTrack, AudioTrack, ReturnTrack, MasterTrack
    }

    public class Track
    {
        public struct TrackDelay
        {
            public int Value { get; }
            public bool IsValueSampleBased { get; }

            public TrackDelay(int v, bool s)
            {
                Value = v;
                IsValueSampleBased = s;
            }

            public void CreateXmlElement(XmlNode parent, XmlDocument doc)
            {
                var valueEl = doc.CreateElement("Value");
                valueEl.SetAttribute("Value", Value.ToString());
                var sampleBasedEl = doc.CreateElement("IsValueSampleBased");
                sampleBasedEl.SetAttribute("Value", IsValueSampleBased.ToString().ToLower());
                parent.AppendChild(valueEl);
                parent.AppendChild(sampleBasedEl);
            }
        }

        public struct Name
        {
            public string EffectiveName { get; }
            public string UserName { get; }
            public string Annotaion { get; }
            public string MemorizedFirstClip { get; }

            public Name(string en, string un, string a, string m)
            {
                EffectiveName = en;
                UserName = un;
                Annotaion = a;
                MemorizedFirstClip = m;
            }

            public void CreateXmlElement(XmlNode parent, XmlDocument doc)
            {
                var enEl = doc.CreateElement("EffectiveName");
                var unEl = doc.CreateElement("UserName");
                var anEl = doc.CreateElement("Annotation");
                var mfcEl = doc.CreateElement("MemorizedFirstClipName");
                enEl.SetAttribute("Value", EffectiveName);
                unEl.SetAttribute("Value", UserName);
                anEl.SetAttribute("Value", Annotaion);
                mfcEl.SetAttribute("Value", MemorizedFirstClip);
                parent.AppendChild(enEl);
                parent.AppendChild(unEl);
                parent.AppendChild(anEl);
                parent.AppendChild(mfcEl);
            }
        }

        public int id;
        public Dictionary<string, int> intValues;
        public Dictionary<string, bool> boolValues;
        public string viewData;
        public TrackDelay trackDelay;
        public Name name;
        public TrackType type;

        public virtual void CreateXmlNode(XmlDocument xmlDoc, XmlNode trackNode)
        {
            var tNode = xmlDoc.CreateElement(type.ToString());
            if(type != TrackType.MasterTrack)
            {
                tNode.SetAttribute("Id", id.ToString());
            }            
            foreach (var (name, value) in intValues)
            {
                var el = xmlDoc.CreateElement(name);
                el.SetAttribute("Value", value.ToString());
                tNode.AppendChild(el);
            }
            foreach (var (name, value) in boolValues)
            {
                var el = xmlDoc.CreateElement(name);
                el.SetAttribute("Value", value.ToString().ToLower());
                tNode.AppendChild(el);
            }
            var trackDelayNode = xmlDoc.CreateElement("TrackDelay");
            trackDelay.CreateXmlElement(trackDelayNode, xmlDoc);
            tNode.AppendChild(trackDelayNode);
            var nameNode = xmlDoc.CreateElement("Name");
            name.CreateXmlElement(nameNode, xmlDoc);
            tNode.AppendChild(nameNode);
            
            trackNode.AppendChild(tNode);
        }
    }
}
