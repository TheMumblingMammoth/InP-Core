using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
[XmlRoot("BodyClips")]
public class BodyClipXML{
    
    public class XMLData{
        [XmlAttribute("Name")] public string name;
        [XmlElement("Size")] public int size;
        [XmlArray("Frames")]
        [XmlArrayItem("Frame")]
        public string [] frames;
    }
    [XmlArray("Clips")]
    [XmlArrayItem("Clip")]
    public XMLData [] clips;

    public static BodyClipXML LoadClips(){ 
        string path = "_Data/BodyClips";
        TextAsset _xml = Resources.Load<TextAsset>(path); 
        XmlSerializer serializer = new XmlSerializer(typeof(BodyClipXML));
        StringReader reader = new StringReader(_xml.text);
        BodyClipXML dialogs = serializer.Deserialize(reader) as BodyClipXML;
        reader.Close();
        return dialogs;
    }
}