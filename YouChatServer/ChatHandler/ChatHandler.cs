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
            List<ChatDetails> chats = DataHandler.GetAllChats();
            string chatId;
            string chatFilePath;
            XmlFileManager xmlFileManager;
            foreach (ChatDetails chat in chats)
            {
                chatId = chat.ChatTagLineId;
                AllChats.Add(chatId, chat);
                chatFilePath = chat.MessageHistory;
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
    }
}
