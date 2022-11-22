using System.Xml.Serialization;

namespace SCCDownloader.WSModels
{
    [XmlRoot("response")]
    public class LoginResponse
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("token")]
        public string Token { get; set; }
    }
}
