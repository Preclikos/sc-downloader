using System.Xml.Serialization;

namespace SCCDownloader.WSModels
{
    [XmlRoot("response")]
    public class LinkResponse
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }
    }
}
