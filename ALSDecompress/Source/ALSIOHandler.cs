using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using ALSDecompress.AbletonDataTypes;

namespace ALSDecompress
{
    public class ALSIOHandler
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
        }

        private void GetValues(XmlNode node)
        {
            _abletonLiveSet.abletonHeader.intValues ??= new Dictionary<string, int>();
            _abletonLiveSet.abletonHeader.boolValues ??= new Dictionary<string, bool>();
            _abletonLiveSet.abletonHeader.videoRect ??= new Dictionary<string, int>();
            var intValues = _abletonLiveSet.abletonHeader.intValues;
            var boolValues = _abletonLiveSet.abletonHeader.boolValues;
            var videoRect = _abletonLiveSet.abletonHeader.videoRect;
            var nodeList = node.ChildNodes;
            foreach (XmlElement n in nodeList)
            {
                if (n.Attributes.Count == 1)
                {
                    Console.WriteLine($"{n.Name} {n.Attributes[0].Value}");
                    if (n.Name == "ViewData")
                    {
                        _abletonLiveSet.abletonHeader.viewData = n.Attributes[0].Value;
                    }
                    else if (n.Name == "Annotation")
                    {
                        _abletonLiveSet.abletonHeader.annotation = n.Attributes[0].Value;
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