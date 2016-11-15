using System;
using System.IO;
using System.Xml.Serialization;

namespace MessageBroker
{
    [Serializable]
    public class SendMessage
    {
        public XmlSerializer XmlSerializer;

        [XmlElement("Subject", DataType = "string")]
        public string Subject { get; set; }

        [XmlElement("SenderName", DataType = "string")]
        public string SenderName { get; set; }

        [XmlElement("Message", DataType = "string")]
        public string Message { get; set; }

        [XmlElement("Date", DataType = "string")]
        public string Date { get; set; }

        // Serializarea
        public static string SerializeUserData(SendMessage user)
        {
            string message;
            var serializer = new XmlSerializer(typeof(SendMessage)); //format to xml

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, user);
                stream.Position = 0;
                var sr = new StreamReader(stream);
                message = sr.ReadToEnd();
            }
            return message;
        }
    }
}