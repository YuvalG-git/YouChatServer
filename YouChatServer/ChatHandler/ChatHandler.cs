using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.UserDetails;

namespace YouChatServer.ChatHandler
{
    public class ChatHandler
    {
        public static Dictionary<string, ChatDetails> AllChats = new Dictionary<string, ChatDetails>(); //key-chatid, value - chat object containg the list of clients and other properties, everytime a message is sent i check for every username if he is connected using the allclients hashtable and send for those how are okay
        public static Dictionary<string, XmlFileManager> ChatFileManagers = new Dictionary<string, XmlFileManager>(); //key-chatid, value - 

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
    }
}
