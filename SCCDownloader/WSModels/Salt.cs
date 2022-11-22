using System.Xml.Serialization;

namespace SCCDownloader.WSModels
{
    [XmlRoot("response")]
    public class SaltResponse
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("salt")]
        public string Salt { get; set; }
    }
}
