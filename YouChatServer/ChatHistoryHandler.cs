using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace YouChatServer
{
    internal class ChatHistoryHandler
    {
        private string ChatName { get; set; }
        private List<string> ChatParticipants { get; set; }
        private XmlSerializer ChatXmlSerializer;
        public ChatHistoryHandler(string ChatName, string[] ChatParticipants)
        {
            this.ChatName = ChatName;
            foreach (string ChatParticipant in ChatParticipants)
            {
                this.ChatParticipants.Add(ChatParticipant);
            }
            this.ChatXmlSerializer = new XmlSerializer(typeof(Message));

        }
        public void WriteToXml(Message Message)
        {
            string FileName = $"{ChatName}.xml";
            String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Serialize the object to the specified XML file
            using (FileStream fileStream = new FileStream(FileName, FileMode.Create))
            {
                this.ChatXmlSerializer.Serialize(fileStream, Message);
            }
        }
        public void LoadToXml()
        {

        }
        public void OverWriteXml()
        {

        }
    }
}
