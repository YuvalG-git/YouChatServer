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
    /// <summary>
    /// The "XmlFileManager" class provides functionality for managing XML files.
    /// It handles the creation, editing, and reading of XML files containing chat messages and participant information.
    /// </summary>
    public class XmlFileManager
    {
        #region Private Fields

        /// <summary>
        /// The string object "_filePath" represents the file path.
        /// </summary>
        private string _filePath;

        #endregion

        #region Static Readonly Fields

        /// <summary>
        /// The string object "projectFolderPath" represents the path to the project folder.
        /// </summary>
        static readonly string projectFolderPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

        #endregion

        #region Constructors

        /// <summary>
        /// The "XmlFileManager" constructor initializes a new instance of the <see cref="XmlFileManager"/> class with the specified filename, chat participants, and chat ID.
        /// It is used for creating a new XML file.
        /// </summary>
        /// <param name="filename">The filename of the XML file.</param>
        /// <param name="chatParticipants">The list of chat participants.</param>
        /// <param name="ChatId">The ID of the chat.</param>
        public XmlFileManager(string filename, List<string> chatParticipants, string ChatId)
        {
            InitializeXmlFile(filename, chatParticipants, ChatId);
        }

        /// <summary>
        /// The "XmlFileManager" constructor initializes a new instance of the <see cref="XmlFileManager"/> class with the specified file path.
        /// It is used for uploading an existing XML file.
        /// </summary>
        /// <param name="filePath">The path to the XML file.</param>
        public XmlFileManager(string filePath)
        {
            _filePath = filePath;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "GetFilePath" method returns the file path associated with the current instance.
        /// </summary>
        /// <returns>The file path string.</returns>
        public string GetFilePath()
        {
            return _filePath;
        }

        /// <summary>
        /// The "DeleteFile" method attempts to delete the file specified by the file path.
        /// </summary>
        /// <returns>True if the file was successfully deleted; otherwise, false.</returns>
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
                return false;
            }
        }

        /// <summary>
        /// The "EditMessage" method edits a message in the XML file based on the provided criteria.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="type">The type of the message.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="date">The date and time of the message.</param>
        /// <exception cref="Exception">Thrown when the message is not found or when multiple messages match the criteria.</exception>
        /// <remarks>
        /// This method loads an XML document from the file path specified by <see cref="_filePath"/>.
        /// It then searches for a message node in the XML document that matches the provided sender, type, content, and date.
        /// If the message is found and is unique, it updates the content to an empty string and changes the type to "DeletedMessage".
        /// Finally, it saves the updated XML document back to the file.
        /// </remarks>
        public void EditMessage(string sender, string type, string content, DateTime date)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filePath);

            XmlNodeList messageNodes = doc.SelectNodes("//Message[Sender='" + sender + "' and Type='" + type + "' and Content='" + content + "' and Date='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "']");

            if (messageNodes.Count == 0)
            {
                throw new Exception("Message not found.");
            }
            else if (messageNodes.Count > 1)
            {
                throw new Exception("Multiple messages match the criteria.");
            }
            else
            {
                XmlNode messageNode = messageNodes[0];

                if (messageNode.SelectSingleNode("Sender").InnerText == sender &&
                    messageNode.SelectSingleNode("Type").InnerText == type &&
                    messageNode.SelectSingleNode("Content").InnerText == content &&
                    messageNode.SelectSingleNode("Date").InnerText == date.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    XmlNode contentNode = messageNode.SelectSingleNode("Content");
                    if (contentNode != null)
                    {
                        contentNode.InnerText = "";
                    }

                    XmlNode typeNode = messageNode.SelectSingleNode("Type");
                    if (typeNode != null)
                    {
                        typeNode.InnerText = "DeletedMessage";
                    }

                    doc.Save(_filePath);
                }
            }
        }

        /// <summary>
        /// The "AppendMessage" method appends a new message to the XML file with the provided sender, type, content, and date.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="type">The type of the message.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="date">The date and time of the message.</param>
        /// <remarks>
        /// This method loads an XML document from the file path specified by <see cref="_filePath"/>.
        /// It creates a new XML element for the message with sub-elements for the sender, type, content, and date.
        /// The new message element is appended to the root element of the XML document, and the document is saved back to the file.
        /// </remarks>
        public void AppendMessage(string sender, string type, string content, DateTime date)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filePath);

            XmlElement messageElement = doc.CreateElement("Message");

            XmlElement senderElement = doc.CreateElement("Sender");
            senderElement.InnerText = sender;

            XmlElement typeElement = doc.CreateElement("Type");
            typeElement.InnerText = type;

            XmlElement contentElement = doc.CreateElement("Content");
            contentElement.InnerText = content;

            XmlElement dateElement = doc.CreateElement("Date");
            dateElement.InnerText = date.ToString("yyyy-MM-dd HH:mm:ss");

            messageElement.AppendChild(senderElement);
            messageElement.AppendChild(typeElement);
            messageElement.AppendChild(contentElement);
            messageElement.AppendChild(dateElement);

            XmlElement root = doc.DocumentElement;

            root?.AppendChild(messageElement);

            doc.Save(_filePath);
        }

        /// <summary>
        /// The "ReadChatXml" method reads the chat XML file and returns a list of messages.
        /// </summary>
        /// <returns>A list of MessageData objects representing the messages in the chat.</returns>
        /// <remarks>
        /// This method loads an XML document from the file path specified by <see cref="_filePath"/>.
        /// It reads the chat name and participants from the XML document's root element.
        /// It then iterates over the message nodes in the XML document, extracting the sender, type, content, and date for each message.
        /// Finally, it creates a MessageData object for each message and adds it to the list of messages to be returned.
        /// </remarks>
        public List<MessageData> ReadChatXml()
        {
            List<MessageData> messages = new List<MessageData>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_filePath);

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

        #endregion

        #region Private Methods

        /// <summary>
        /// The "InitializeXmlFile" method initializes an XML file with the given file name, list of chat participants, and chat ID.
        /// </summary>
        /// <param name="fileName">The name of the XML file to initialize.</param>
        /// <param name="chatParticipants">The list of chat participants.</param>
        /// <param name="ChatId">The ID of the chat.</param>
        /// <remarks>
        /// This method creates a new XML document and adds elements for the chat name, chat ID, and participants based on the provided parameters.
        /// If the specified file already exists, the method does nothing.
        /// </remarks>
        private void InitializeXmlFile(string fileName, List<string> chatParticipants, string ChatId)
        {
            _filePath = GetFilePath(fileName);
            if (!File.Exists(_filePath))
            {
                XmlDocument doc = new XmlDocument();

                XmlElement rootElement = doc.CreateElement("Chat");
                doc.AppendChild(rootElement);

                XmlElement chatNameElement = doc.CreateElement("ChatName");
                chatNameElement.InnerText = fileName;
                rootElement.AppendChild(chatNameElement);

                XmlElement chatIdElement = doc.CreateElement("ChatId");
                chatIdElement.InnerText = ChatId;
                rootElement.AppendChild(chatIdElement);

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

                doc.Save(_filePath);
            }
        }

        /// <summary>
        /// The "GetFilePath" method constructs and returns the file path for an XML file based on the project folder path and the provided file name.
        /// </summary>
        /// <param name="fileName">The name of the XML file.</param>
        /// <returns>The file path string.</returns>
        private string GetFilePath(string fileName)
        {
            _filePath = Path.Combine(projectFolderPath.Substring(0, projectFolderPath.Length - @"bin\Debug".Length), "MessageHistory", fileName + ".xml");
            return _filePath;
        }

        #endregion
    }
}
