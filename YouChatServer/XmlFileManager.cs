using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using YouChatServer.JsonClasses.MessageClasses;

namespace YouChatServer
{
    public class XmlFileManager
    {
        static string projectFolderPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

        private string _filename;
        private string _filePath;

        public XmlFileManager(string filename, List<string> chatParticipants, string ChatId)
        {
            _filename = filename;
            InitializeXmlFile(chatParticipants, ChatId);
        }
        public XmlFileManager(string filename)
        {
            _filename = filename;
        }
        public string GetFilePath()
        {
            return _filePath;
        }

        private void InitializeXmlFile(List<string> chatParticipants, string ChatId)
        {
            if (!File.Exists(_filename))
            {
                // Create a new XML document
                XmlDocument doc = new XmlDocument();

                // Create the root element
                XmlElement rootElement = doc.CreateElement("Chat");
                doc.AppendChild(rootElement);

                // Create the chat name, chat id and participants elements
                XmlElement chatNameElement = doc.CreateElement("ChatName");
                chatNameElement.InnerText = _filename;
                rootElement.AppendChild(chatNameElement);

                XmlElement chatIdElement = doc.CreateElement("ChatId");
                chatNameElement.InnerText = ChatId;
                rootElement.AppendChild(chatNameElement);

                string participants = "";
                foreach (string chatParticipant in chatParticipants)
                {
                    participants += chatParticipant + ", ";
                }
                if (participants != "")
                {
                    participants = participants.Substring(0, participants.Length - 2);
                }
                XmlElement participantsElement = doc.CreateElement("Participants");
                participantsElement.InnerText = participants;
                rootElement.AppendChild(participantsElement);



                // Save the XML document to the file
                string filePath = GetFilePath(_filename);
                doc.Save(filePath);
            }
        }
        private string GetFilePath(string fileName)
        {
            //@"C:\Users\Yuval\source\YouChat\YouChatServer\YouChatServer\dat2a.xml";
            //_filePath = projectFolderPath.Substring(0, projectFolderPath.Length - @"bin\Debug".Length) + "MessageHistory\\" + fileName + ".xml";
            _filePath = Path.Combine(projectFolderPath.Substring(0, projectFolderPath.Length - @"bin\Debug".Length), "MessageHistory", fileName + ".xml");

            return _filePath;
        }
        public bool DeleteFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException)
            {
                // Handle the exception, log it, etc.
                return false;
            }
        }


        public void EditMessage(string sender, string type, string content, DateTime date)
        {
            // Load the existing XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            // Find the message element to edit
            XmlNodeList messageNodes = doc.SelectNodes("//Message[Sender='" + sender + "' and Type='" + type + "' and Content='" + content + "' and Date='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "']");

            if (messageNodes.Count == 0)
            {
                // Handle the case where the message is not found
                throw new Exception("Message not found.");
            }
            else if (messageNodes.Count > 1)
            {
                // Handle the case where multiple messages match the criteria
                throw new Exception("Multiple messages match the criteria.");
            }
            else
            {
                XmlNode messageNode = messageNodes[0];

                // Check if all values are the same
                if (messageNode.SelectSingleNode("Sender").InnerText == sender &&
                    messageNode.SelectSingleNode("Type").InnerText == type &&
                    messageNode.SelectSingleNode("Content").InnerText == content &&
                    messageNode.SelectSingleNode("Date").InnerText == date.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    // Update the content of the message
                    XmlNode contentNode = messageNode.SelectSingleNode("Content");
                    if (contentNode != null)
                    {
                        contentNode.InnerText = "";
                    }

                    // Update the type of the message if the content matches
                    XmlNode typeNode = messageNode.SelectSingleNode("Type");
                    if (typeNode != null)
                    {
                        typeNode.InnerText = "DeletedMessage";
                    }

                    // Save the modified XML document back to the file
                    doc.Save(_filename);
                }
            }
        }
       

        /// <summary>
        /// The method adds a new message to a XML File
        /// </summary>
        /// <param name="sender">The message sender username</param>
        /// <param name="type">The message type</param>
        /// <param name="content">The message content</param>
        /// <param name="date">The data and time which the message was sent in</param>
        public void AppendMessage(string sender, string type, string content, DateTime date)
        {
            // Load the existing XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            // Create a new message element
            XmlElement messageElement = doc.CreateElement("Message");

            // Create sender, type, and content elements
            XmlElement senderElement = doc.CreateElement("Sender");
            senderElement.InnerText = sender;

            XmlElement typeElement = doc.CreateElement("Type");
            typeElement.InnerText = type;

            XmlElement contentElement = doc.CreateElement("Content");
            contentElement.InnerText = content;

            XmlElement dateElement = doc.CreateElement("Date");
            dateElement.InnerText = date.ToString("yyyy-MM-dd HH:mm:ss");

            // Append sender, type, and content elements to message element
            messageElement.AppendChild(senderElement);
            messageElement.AppendChild(typeElement);
            messageElement.AppendChild(contentElement);
            messageElement.AppendChild(dateElement);

            // Get the root element of the XML document
            XmlElement root = doc.DocumentElement;

            // Append the new message element to the root element
            root?.AppendChild(messageElement);

            // Save the modified XML document back to the file
            doc.Save(_filename);
        }

        public void EditChatName(string newChatName)
        {
            // Load the existing XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            // Find the chat name element and update its value
            XmlNode chatNameNode = doc.SelectSingleNode("/Chat/ChatName");
            if (chatNameNode != null)
            {
                chatNameNode.InnerText = newChatName;
            }

            // Save the modified XML document back to the file
            doc.Save(_filename);
        }

        public void EditParticipants(string newParticipants)
        {
            // Load the existing XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            // Find the participants element and update its value
            XmlNode participantsNode = doc.SelectSingleNode("/Chat/Participants");
            if (participantsNode != null)
            {
                participantsNode.InnerText = newParticipants;
            }

            // Save the modified XML document back to the file
            doc.Save(_filename);
        }

        public void ClearData()
        {
            // Create an empty XML document
            XmlDocument doc = new XmlDocument();

            // Create the root element
            XmlElement rootElement = doc.CreateElement("Messages");
            doc.AppendChild(rootElement);

            // Save the empty XML document to the file, overwriting existing content
            doc.Save(_filename);
        }

        public void SaveData(XmlDocument doc)
        {
            // Save the XML document back to the file
            doc.Save(_filename);
        }

        public XmlDocument LoadData()
        {
            // Load or create the XML file
            return LoadOrCreateXmlDocument();
        }

        private XmlDocument LoadOrCreateXmlDocument()
        {
            XmlDocument doc = new XmlDocument();

            if (File.Exists(_filename))
            {
                // Load the existing XML file
                doc.Load(_filename);
            }
            else
            {
                // Create a new XML document
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                // Create the root element
                XmlElement rootElement = doc.CreateElement("Messages");
                doc.AppendChild(rootElement);
            }

            return doc;
        }
        public List<MessageData> ReadChatXml()
        {
            List<MessageData> messages = new List<MessageData>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_filename);

                XmlElement root = xmlDoc.DocumentElement;

                string chatName = root.SelectSingleNode("ChatName").InnerText;
                string participants = root.SelectSingleNode("Participants").InnerText;

                XmlNodeList messageNodes = root.SelectNodes("Message");
                foreach (XmlNode messageNode in messageNodes)
                {
                    string sender = messageNode.SelectSingleNode("Sender").InnerText;
                    string type = messageNode.SelectSingleNode("Type").InnerText;
                    string content = messageNode.SelectSingleNode("Content").InnerText;
                    string date = messageNode.SelectSingleNode("Date").InnerText;

                    MessageData message = new MessageData(sender, type, content, date);
                    messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading XML file: {ex.Message}");
            }

            return messages;
        }

    }
}
