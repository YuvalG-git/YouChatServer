using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.UserDetails;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The ChatHandler class manages all chat-related operations, including storing chat details and managing XML file managers for message history.
    /// </summary>
    public class ChatHandler
    {
        #region Public Static Fields

        /// <summary>
        /// The Dictionary object "AllChats" stores chat details for each chat ID.
        /// </summary>
        /// <remarks>
        /// The key represents the chat ID, and the value is a ChatDetails object containing the list of clients and other properties.
        /// Every time a message is sent, the system checks if each username is connected using the AllClients hashtable and sends messages to those who are connected.
        /// </remarks>
        public static Dictionary<string, ChatDetails> AllChats = new Dictionary<string, ChatDetails>();

        /// <summary>
        /// The Dictionary object "ChatFileManagers" stores XML file managers for each chat ID.
        /// </summary>
        /// <remarks>
        /// The key represents the chat ID, and the value is an XmlFileManager object.
        /// </remarks>
        public static Dictionary<string, XmlFileManager> ChatFileManagers = new Dictionary<string, XmlFileManager>();
        
        #endregion

        #region Public Static Methods

        /// <summary>
        /// The "SetChats" method initializes the list of all chats and their corresponding XML file managers.
        /// </summary>
        /// <remarks>
        /// This method retrieves all chat information from the data handler, including chat details and message history paths.
        /// It then iterates through each chat, adds the chat details to the AllChats dictionary with the chat ID as the key,
        /// and creates a new XML file manager for the chat's message history file, adding it to the ChatFileManagers dictionary with the chat ID as the key.
        /// </remarks>
        public static void SetChats()
        {
            List<ChatInformation> chatInformationList = DataHandler.GetAllChats();
            ChatDetails chatDetails;
            string chatId;
            string chatFilePath;
            XmlFileManager xmlFileManager;
            foreach(ChatInformation chatInformation in chatInformationList)
            {
                chatDetails = chatInformation.ChatDetails;
                chatFilePath = chatInformation.MessageHistoryPath;
                chatId = chatDetails.ChatTagLineId;
                AllChats.Add(chatId, chatDetails);
                xmlFileManager = new XmlFileManager(chatFilePath);
                ChatFileManagers.Add(chatId, xmlFileManager);
            }
        }

        /// <summary>
        /// The "GetUserChats" method retrieves a list of chat details for a given username.
        /// </summary>
        /// <param name="username">The username for which to retrieve chat details.</param>
        /// <returns>A list of ChatDetails objects containing chat information for the specified user.</returns>
        /// <remarks>
        /// This method iterates through all chats in the AllChats dictionary and checks if the specified username exists in each chat.
        /// If the username is found in a chat, the chat details are added to the returned list of chats.
        /// </remarks>
        public static List<ChatDetails> GetUserChats(string username)
        {
            List<ChatDetails> chats = new List<ChatDetails>();
            foreach (KeyValuePair<string, ChatDetails> chatEntry in AllChats)
            {
                ChatDetails chat = chatEntry.Value;
                if (chat.UserExist(username))
                {
                    chats.Add(chat);
                }
            }
            return chats;
        }

        /// <summary>
        /// The "GetUserAllChatUsernames" method retrieves a list of unique usernames for all chats in which the specified user is a participant.
        /// </summary>
        /// <param name="username">The username for which to retrieve the list of chat participants.</param>
        /// <returns>A list of unique usernames for all chats in which the specified user is a participant, excluding the specified user's own username.</returns>
        /// <remarks>
        /// This method iterates through all chats in the AllChats dictionary and checks if the specified username exists in each chat.
        /// If the username is found, it retrieves the list of chat participants for that chat and adds their usernames to a HashSet to ensure uniqueness.
        /// Finally, the method returns a list containing all unique usernames, excluding the specified username.
        /// </remarks>
        public static List<string> GetUserAllChatUsernames(string username) 
        {
            List<string> usernames;
            HashSet<string> uniqueUsernames = new HashSet<string>();
            ChatDetails chat;
            List<ChatParticipant> chatParticipantList;
            string name;
            foreach (KeyValuePair<string, ChatDetails> chatEntry in AllChats)
            {
                chat = chatEntry.Value;
                if (chat.UserExist(username))
                {
                    chatParticipantList = chat.ChatParticipants;
                    foreach(ChatParticipant chatParticipant in chatParticipantList)
                    {
                        name = chatParticipant.Username;

                        if (name != username)
                            uniqueUsernames.Add(name);
                    }
                }
            }
            usernames =  new List<string>(uniqueUsernames);
            return usernames;
        }

        #endregion
    }
}
