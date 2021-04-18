using System;
using System.Collections.Generic;
using System.Xml;
using ALSDecompress.AbletonDataTypes;

namespace ALSDecompress
{
    class AbletonLiveSet
    {        
        private readonly string _name;
        private readonly string _xmlVersion, _xmlEncoding;
        public AbletonHeader abletonHeader;
        public List<MidiTrack> midiTracks;
        public List<AudioTrack> audioTracks;
        public List<ReturnTrack> returnTracks;
        public MasterTrack masterTrack;
        public ViewStates viewStates;

        public ContentSplitterProperties contentSplitterProperties;
        public SequencerNavigator sequencerNavigator;
        public TimeSelection timeSelection;
        public ScaleInformation scaleInformation;
        public Grid grid;

        public Dictionary<string, int> intValues;
        public Dictionary<string, bool> boolValues;
        public Dictionary<string, int> videoRect;

        public string viewData, annotation;
        public int autoColourPickerPlayerTracks;
        public int autoColourPickerMasterTracks;


        public AbletonLiveSet(string n, string v, string e, AbletonHeader a)
        {
            _name = n;
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
            
            foreach (var el in intValues)
            {
                var element = xmlDoc.CreateElement(el.Key); 
                element.SetAttribute("Value", el.Value.ToString());
                firstChild.AppendChild(element);
            }

            foreach (var el in boolValues)
            {
                var element = xmlDoc.CreateElement(el.Key);
                element.SetAttribute("Value", el.Value.ToString().ToLower());
                firstChild.AppendChild(element);
            }

            var videoRectElement = xmlDoc.CreateElement("VideoWindowRect");
            foreach (var el in videoRect)
            {
                videoRectElement.SetAttribute(el.Key, el.Value.ToString());
            }

            var viewDataElement = xmlDoc.CreateElement("ViewData");
            viewDataElement.SetAttribute("Value", viewData);
            var annotationElement = xmlDoc.CreateElement("Annotation");
            annotationElement.SetAttribute("Value", annotation);

            var trackNode = xmlDoc.CreateElement("Tracks");

            foreach(var mt in midiTracks)
            {
                mt.CreateXmlNode(xmlDoc, trackNode);           
            }

            foreach (var at in audioTracks)
            {
                at.CreateXmlNode(xmlDoc, trackNode);
            }

            foreach (var rt in returnTracks)
            {
                rt.CreateXmlNode(xmlDoc, trackNode);
            }

            masterTrack.CreateXmlNode(xmlDoc, firstChild);
            firstChild.AppendChild(trackNode);

            contentSplitterProperties.CreateXmlNode(xmlDoc, firstChild);
            sequencerNavigator.CreateXmlNode(xmlDoc, firstChild);
            timeSelection.CreateXmlNode(xmlDoc, firstChild);
            scaleInformation.CreateXmlNode(xmlDoc, firstChild);
            grid.CreateXmlNode(xmlDoc, firstChild);

            var playerColourPickerNode = xmlDoc.CreateElement("AutoColorPickerForPlayerAndGroupTracks");
            var playerNextColourIndex = xmlDoc.CreateElement("NextColorIndex");
            playerNextColourIndex.SetAttribute("Value", autoColourPickerPlayerTracks.ToString());
            playerColourPickerNode.AppendChild(playerNextColourIndex);
            firstChild.AppendChild(playerColourPickerNode);

            var masterColourPickerNode = xmlDoc.CreateElement("AutoColorPickerForReturnAndMasterTracks");
            var masterNextColourIndex = xmlDoc.CreateElement("NextColorIndex");
            masterNextColourIndex.SetAttribute("Value", autoColourPickerMasterTracks.ToString());
            masterColourPickerNode.AppendChild(masterNextColourIndex);
            firstChild.AppendChild(masterColourPickerNode);

            viewStates.CreateXmlNode(xmlDoc, firstChild);
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