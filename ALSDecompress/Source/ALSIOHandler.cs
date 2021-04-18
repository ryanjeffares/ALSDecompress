using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using ALSDecompress.AbletonDataTypes;

namespace ALSDecompress
{
    class ALSIOHandler
    {
        private AbletonLiveSet _abletonLiveSet;
        private readonly string _inputPath, _outputPath, _setName;
        
        public ALSIOHandler(string alsInputPath, string xmlPath)
        {
            _inputPath = alsInputPath;
            _outputPath = xmlPath;
            var cleanedName = _inputPath.Remove(_inputPath.LastIndexOf("."));
            _setName = cleanedName.Substring(cleanedName.LastIndexOf("/"));
        }

        public void Decompress()
        {
            var fileInfo = new FileInfo(_inputPath);
            using var fs = fileInfo.OpenRead();
            using var decompressed = File.Create(_outputPath);
            using var gz = new GZipStream(fs, CompressionMode.Decompress);
            gz.CopyTo(decompressed);
        }

        public void StoreXmlData()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(_outputPath);
            var rootNode = xmlDoc.FirstChild;   //<?xml version="1.0" encoding="UTF-8"?>
            var rootNodeStr = rootNode.Value;
            var version = rootNodeStr.Substring(9, rootNodeStr.IndexOf("encoding") - 11); // HACKY -- FIX
            var encoding = rootNodeStr.Substring(rootNodeStr.IndexOf("encoding") + 10);
            encoding = encoding.Remove(encoding.Length - 1);
            var secondNode = rootNode.NextSibling;  // ableton node
            var abletonNode = new AbletonHeader(
                int.Parse(secondNode.Attributes[0].Value),
                secondNode.Attributes[1].Value,
                int.Parse(secondNode.Attributes[2].Value),
                secondNode.Attributes[3].Value,
                secondNode.Attributes[4].Value
                );
            _abletonLiveSet = new AbletonLiveSet(_setName, version, encoding, abletonNode);
            GetValues(secondNode.FirstChild);
            GetTracks(secondNode.FirstChild["Tracks"]);
            GetMasterTrack(secondNode.FirstChild["MasterTrack"]);
            GetViewStates(secondNode.FirstChild["ViewStates"]);
        }

        private void GetValues(XmlNode node)
        {
            _abletonLiveSet.intValues ??= new Dictionary<string, int>();
            _abletonLiveSet.boolValues ??= new Dictionary<string, bool>();
            _abletonLiveSet.videoRect ??= new Dictionary<string, int>();
            var intValues = _abletonLiveSet.intValues;
            var boolValues = _abletonLiveSet.boolValues;
            var videoRect = _abletonLiveSet.videoRect;
            var nodeList = node.ChildNodes;
            foreach (XmlElement n in nodeList)
            {
                // single elements that have an attribute
                if (n.Attributes.Count == 1)
                {
                    //Console.WriteLine($"{n.Name} {n.Attributes[0].Value}");
                    if (n.Name == "ViewData")
                    {
                        _abletonLiveSet.viewData = n.Attributes[0].Value;
                    }
                    else if (n.Name == "Annotation")
                    {
                        _abletonLiveSet.annotation = n.Attributes[0].Value;
                    }
                    else
                    {
                        if (int.TryParse(n.Attributes[0].Value, out int iVal))
                        {
                            intValues.Add(n.Name, iVal);
                        }
                        else if (bool.TryParse(n.Attributes[0].Value, out bool bVal))
                        {
                            boolValues.Add(n.Name, bVal);
                        }
                        else
                        {
                            Console.WriteLine($"Couldn't parse node {n.Name} with 1 attribute, found attribute {n.Attributes[0].Value}");
                        }   
                    }
                }
                else if (n.Attributes.Count == 4)
                {
                    try
                    {
                        videoRect.Add("Top", int.Parse(n.Attributes.GetNamedItem("Top").Value));   
                        videoRect.Add("Left", int.Parse(n.Attributes.GetNamedItem("Left").Value));   
                        videoRect.Add("Bottom", int.Parse(n.Attributes.GetNamedItem("Bottom").Value));   
                        videoRect.Add("Right", int.Parse(n.Attributes.GetNamedItem("Right").Value));   
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                // nodes that have many elements
                else
                {
                    switch (n.Name)
                    {
                        case "AutoColorPickerForPlayerAndGroupTracks":
                            _abletonLiveSet.autoColourPickerPlayerTracks = int.Parse(n["NextColorIndex"].Attributes[0].Value);
                            break;
                        case "AutoColorPickerForReturnAndMasterTracks":
                            _abletonLiveSet.autoColourPickerMasterTracks = int.Parse(n["NextColorIndex"].Attributes[0].Value);
                            break;
                        case "ContentSplitterProperties":
                            _abletonLiveSet.contentSplitterProperties = new ContentSplitterProperties(
                                    bool.Parse(n["Open"].Attributes[0].Value),
                                    int.Parse(n["Size"].Attributes[0].Value)
                                );
                            break;
                        case "SequencerNavigator":
                            _abletonLiveSet.sequencerNavigator = new SequencerNavigator(
                                    double.Parse(n["BeatTimeHelper"]["CurrentZoom"].Attributes[0].Value),
                                    int.Parse(n["ScrollerPos"].Attributes[0].Value),
                                    int.Parse(n["ScrollerPos"].Attributes[1].Value),
                                    int.Parse(n["ClientSize"].Attributes[0].Value),
                                    int.Parse(n["ClientSize"].Attributes[1].Value)
                                );
                            break;
                        case "TimeSelection":
                            _abletonLiveSet.timeSelection = new TimeSelection(
                                    int.Parse(n["AnchorTime"].Attributes[0].Value),
                                    int.Parse(n["OtherTime"].Attributes[0].Value)
                                );
                            break;
                        case "ScaleInformation":
                            _abletonLiveSet.scaleInformation = new ScaleInformation(
                                    int.Parse(n["RootNote"].Attributes[0].Value),
                                    n["Name"].Attributes[0].Value
                                );
                            break;
                        case "Grid":
                            _abletonLiveSet.grid = new Grid(
                                    int.Parse(n["FixedNumerator"].Attributes[0].Value),
                                    int.Parse(n["FixedDenominator"].Attributes[0].Value),
                                    int.Parse(n["GridIntervalPixel"].Attributes[0].Value),
                                    int.Parse(n["Ntoles"].Attributes[0].Value),
                                    bool.Parse(n["SnapToGrid"].Attributes[0].Value),
                                    bool.Parse(n["Fixed"].Attributes[0].Value)
                                );
                            break;
                    }
                }
            }
        }

        private void GetTracks(XmlNode node)
        {
            _abletonLiveSet.returnTracks ??= new List<ReturnTrack>();
            _abletonLiveSet.midiTracks ??= new List<MidiTrack>();
            _abletonLiveSet.audioTracks ??= new List<AudioTrack>();
            foreach(XmlNode track in node.ChildNodes)
            {
                Track newTrack;
                switch (track.Name)
                {
                    case "ReturnTrack":
                        newTrack = new ReturnTrack(int.Parse(track.Attributes[0].Value));
                        _abletonLiveSet.returnTracks.Add((ReturnTrack)newTrack);
                        break;
                    case "MidiTrack":
                        newTrack = new MidiTrack(int.Parse(track.Attributes[0].Value));
                        _abletonLiveSet.midiTracks.Add((MidiTrack)newTrack);
                        break;
                    case "AudioTrack":
                        newTrack = new AudioTrack(int.Parse(track.Attributes[0].Value));
                        _abletonLiveSet.audioTracks.Add((AudioTrack)newTrack);
                        break;
                    default:
                        Console.WriteLine($"Could not parse type of track for {track.Name}, returning.");
                        return;
                }
                newTrack.boolValues ??= new Dictionary<string, bool>();
                newTrack.intValues ??= new Dictionary<string, int>();
                foreach(XmlElement el in track.ChildNodes)
                {
                    if(el.Attributes.Count == 1)
                    {
                        if (el.Name == "ViewData")
                        {
                            newTrack.viewData = el.Attributes[0].Value;
                        }
                        else
                        {
                            if (int.TryParse(el.Attributes[0].Value, out int iVal))
                            {
                                newTrack.intValues.Add(el.Name, iVal);
                            }
                            else if (bool.TryParse(el.Attributes[0].Value, out bool bVal))
                            {
                                newTrack.boolValues.Add(el.Name, bVal);
                            }
                            else
                            {
                                Console.WriteLine($"Could not parse value from {el.Name} in track {track.Name}");
                            }
                        }
                    }
                    else
                    {
                        if(el.Name == "TrackDelay")
                        {
                            newTrack.trackDelay = new Track.TrackDelay(int.Parse(el["Value"].Attributes[0].Value), 
                                bool.Parse(el["IsValueSampleBased"].Attributes[0].Value));
                        }
                        else if(el.Name == "Name")
                        {
                            newTrack.name = new Track.Name(el["EffectiveName"].Attributes[0].Value, 
                                el["UserName"].Attributes[0].Value, 
                                el["Annotation"].Attributes[0].Value, 
                                el["MemorizedFirstClipName"].Attributes[0].Value);
                        }
                    }
                }
            }
        }
        
        public void GetMasterTrack(XmlNode masterNode)
        {
            _abletonLiveSet.masterTrack = new MasterTrack();
            var masterTrack = _abletonLiveSet.masterTrack;
            masterTrack.boolValues??= new Dictionary<string, bool>();
            masterTrack.intValues ??= new Dictionary<string, int>();
            foreach (XmlElement el in masterNode.ChildNodes)
            {
                // single elements that have an attribute
                if (el.Attributes.Count == 1)
                {
                    if (el.Name == "ViewData")
                    {
                        masterTrack.viewData = el.Attributes[0].Value;
                    }
                    else
                    {
                        if (int.TryParse(el.Attributes[0].Value, out int iVal))
                        {
                            masterTrack.intValues.Add(el.Name, iVal);
                        }
                        else if (bool.TryParse(el.Attributes[0].Value, out bool bVal))
                        {
                            masterTrack.boolValues.Add(el.Name, bVal);
                        }
                        else
                        {
                            Console.WriteLine($"Could not parse value from {el.Name} in Master Track");
                        }
                    }
                }                
                else
                {
                    if (el.Name == "TrackDelay")
                    {
                        masterTrack.trackDelay = new Track.TrackDelay(int.Parse(el["Value"].Attributes[0].Value),
                            bool.Parse(el["IsValueSampleBased"].Attributes[0].Value));
                    }
                    else if (el.Name == "Name")
                    {
                        masterTrack.name = new Track.Name(el["EffectiveName"].Attributes[0].Value,
                            el["UserName"].Attributes[0].Value,
                            el["Annotation"].Attributes[0].Value,
                            el["MemorizedFirstClipName"].Attributes[0].Value);
                    }
                }
            }
        }

        private void GetViewStates(XmlNode node)
        {
            _abletonLiveSet.viewStates = new ViewStates();
            foreach(XmlElement el in node.ChildNodes)
            {
                _abletonLiveSet.viewStates.elements.Add(el.Name, int.Parse(el.Attributes[0].Value));
            }
        }

        public void WriteToXml()
        {
            var xmlDoc = _abletonLiveSet.CreateDocumentFromData();
            using var fs = new FileStream(_outputPath, FileMode.Create);
            xmlDoc.Save(fs);
        }

        private Tuple<string, string> GetXmlInfo(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("The provided string value is null or empty.");

            using var stringReader = new StringReader(content);
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            using var xmlReader = XmlReader.Create(stringReader, settings);
            if (!xmlReader.Read()) throw new ArgumentException(
                "The provided XML string does not contain enough data to be valid XML (see https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.read)");

            var encoding = xmlReader.GetAttribute("encoding");
            var version = xmlReader.GetAttribute("version");
            return new Tuple<string, string>(version, encoding);
        }
    }
}