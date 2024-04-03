﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace YouChatServer
{
    internal class XmlFileManager
    {
        private string _filename;

        public XmlFileManager(string filename)
        {
            _filename = filename;
            InitializeXmlFile();
        }

        private void InitializeXmlFile()
        {
            if (!File.Exists(_filename))
            {
                // Create a new XML document
                XmlDocument doc = new XmlDocument();

                // Create the root element
                XmlElement rootElement = doc.CreateElement("Chat");
                doc.AppendChild(rootElement);

                // Create the chat name and participants elements
                XmlElement chatNameElement = doc.CreateElement("ChatName");
                chatNameElement.InnerText = "New Chat";
                rootElement.AppendChild(chatNameElement);

                XmlElement participantsElement = doc.CreateElement("Participants");
                participantsElement.InnerText = "Participant1, Participant2";
                rootElement.AppendChild(participantsElement);



                // Save the XML document to the file
                doc.Save(_filename);
            }
        }

        public void EditMessage(string sender, string originalContent, string newContent, DateTime date)
        {
            // Load the existing XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            // Find the message element to edit
            XmlNodeList messageNodes = doc.SelectNodes("//Message");
            foreach (XmlNode messageNode in messageNodes)
            {
                XmlNode senderNode = messageNode.SelectSingleNode("Sender");
                XmlNode contentNode = messageNode.SelectSingleNode("Content");
                XmlNode dateNode = messageNode.SelectSingleNode("Date");


                if (senderNode != null && contentNode != null &&
                    senderNode.InnerText == sender && contentNode.InnerText == originalContent && dateNode.InnerText == date.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    // Update the content of the message
                    contentNode.InnerText = newContent;
                    break; // Exit loop since we found and edited the message
                }
            }

            // Save the modified XML document back to the file
            doc.Save(_filename);
        }
        public void EditMessage2(string sender, string originalContent, string newContent, DateTime date)
        {
            // Load the existing XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            // Find the message element to edit
            XmlNodeList messageNodes = doc.SelectNodes("//Message[Sender='" + sender + "' and Content='" + originalContent + "' and Date='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "']");

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
                // Update the content of the message
                XmlNode contentNode = messageNodes[0].SelectSingleNode("Content");
                if (contentNode != null)
                {
                    contentNode.InnerText = newContent;

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

    }
}