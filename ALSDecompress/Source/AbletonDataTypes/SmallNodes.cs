using System;
using System.Xml;

namespace ALSDecompress.AbletonDataTypes
{    
    public struct ContentSplitterProperties : ISmallNode
    {
        public bool Open { get; }
        public double Size { get; }

        public ContentSplitterProperties(bool o, int s)
        {
            Open = o;
            Size = s;
        }

        public void CreateXmlNode(XmlDocument doc, XmlNode parent)
        {
            var node = doc.CreateElement("ContentSplitterProperties");
            var openEl = doc.CreateElement("Open");
            openEl.SetAttribute("Value", Open.ToString().ToLower());
            var sizeEl = doc.CreateElement("Size");
            sizeEl.SetAttribute("Value", Size.ToString());
            node.AppendChild(openEl);
            node.AppendChild(sizeEl);
            parent.AppendChild(node);
        }
    }

    public struct SequencerNavigator : ISmallNode
    {
        public double CurrentZoom { get; }
        public Tuple<int, int> ScrollerPos { get; }
        public Tuple<int, int> ClientSize { get; }

        public SequencerNavigator(double z, int spx, int spy, int csx, int csy)
        {
            CurrentZoom = z;
            ScrollerPos = new Tuple<int, int>(spx, spy);
            ClientSize = new Tuple<int, int>(csx, csy);
        }

        public void CreateXmlNode(XmlDocument doc, XmlNode parent)
        {
            var sequencerNode = doc.CreateElement("SequencerNavigator");
            var beatTimeHelperNode = doc.CreateElement("BeatTimeHelper");
            var zoomEl = doc.CreateElement("CurrentZoom");
            zoomEl.SetAttribute("Value", CurrentZoom.ToString());
            beatTimeHelperNode.AppendChild(zoomEl);
            var scrollerEl = doc.CreateElement("ScrollerPos");
            scrollerEl.SetAttribute("X", ScrollerPos.Item1.ToString());
            scrollerEl.SetAttribute("Y", ScrollerPos.Item2.ToString());
            var sizeEl = doc.CreateElement("ClientSize");
            sizeEl.SetAttribute("X", ClientSize.Item1.ToString());
            sizeEl.SetAttribute("Y", ClientSize.Item2.ToString());
            sequencerNode.AppendChild(beatTimeHelperNode);
            sequencerNode.AppendChild(scrollerEl);
            sequencerNode.AppendChild(sizeEl);
            parent.AppendChild(sequencerNode);
        }
    }

    public struct TimeSelection : ISmallNode
    {
        public int AnchorTime { get; }
        public int OtherTime { get; }

        public TimeSelection(int a, int o)
        {
            AnchorTime = a;
            OtherTime = o;
        }

        public void CreateXmlNode(XmlDocument doc, XmlNode parent)
        {
            var timeSelNode = doc.CreateElement("TimeSelection");
            var anchorEl = doc.CreateElement("AnchorTime");
            anchorEl.SetAttribute("Value", AnchorTime.ToString());
            var otherEl = doc.CreateElement("OtherTime");
            otherEl.SetAttribute("Value", OtherTime.ToString());
            timeSelNode.AppendChild(anchorEl);
            timeSelNode.AppendChild(otherEl);
            parent.AppendChild(timeSelNode);
        }
    }

    public struct ScaleInformation : ISmallNode
    {
        public int RootNote { get; }
        public string Name { get; }

        public ScaleInformation(int r, string n)
        {
            RootNote = r;
            Name = n;
        }

        public void CreateXmlNode(XmlDocument doc, XmlNode parent)
        {
            var scaleNode = doc.CreateElement("ScaleInformation");
            var rootEl = doc.CreateElement("RootNote");
            rootEl.SetAttribute("Value", RootNote.ToString());
            var nameEl = doc.CreateElement("Name");
            nameEl.SetAttribute("Value", Name);
            scaleNode.AppendChild(rootEl);
            scaleNode.AppendChild(nameEl);
            parent.AppendChild(scaleNode);
        }
    }

    public struct Grid : ISmallNode
    {
        public int FixedNumerator { get; }
        public int FixedDenominator { get; }
        public int GridIntervalPixel { get; }
        public int Ntoles { get; }
        public bool SnapToGrid { get; }
        public bool Fixed { get; }

        public Grid(int fn, int fd, int gip, int nt, bool s, bool f)
        {
            FixedNumerator = fn;
            FixedDenominator = fd;
            GridIntervalPixel = gip;
            Ntoles = nt;
            SnapToGrid = s;
            Fixed = f;
        }

        public void CreateXmlNode(XmlDocument doc, XmlNode node)
        {
            var gridNode = doc.CreateElement("Grid");
            FixedNumerator.AddValueElement(nameof(FixedNumerator), doc, gridNode);
            FixedDenominator.AddValueElement(nameof(FixedDenominator), doc, gridNode);
            GridIntervalPixel.AddValueElement(nameof(GridIntervalPixel), doc, gridNode);
            Ntoles.AddValueElement(nameof(Ntoles), doc, gridNode);
            SnapToGrid.AddValueElement(nameof(SnapToGrid),  doc, gridNode);
            Fixed.AddValueElement(nameof(Fixed), doc, gridNode);
            node.AppendChild(gridNode);
        }
    }
}
