using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "ChatInformation" class represents information about a chat, including its details and message history path.
    /// </summary>
    /// <remarks>
    /// This class is used to manage and store information about a chat, such as its participants, last message, and message history.
    /// </remarks>
    internal class ChatInformation
    {
        #region Private Fields

        /// <summary>
        /// The ChatDetails object "_chatDetails" represents the details of the chat.
        /// </summary>
        private ChatDetails _chatDetails;

        /// <summary>
        /// The string object "_messageHistoryPath" represents the path to the message history file for the chat.
        /// </summary>
        private string _messageHistoryPath;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ChatInformation" constructor initializes a new instance of the <see cref="ChatInformation"/> class with the specified chat details and message history path.
        /// </summary>
        /// <param name="chatDetails">The details of the chat.</param>
        /// <param name="messageHistoryPath">The path to the message history file for the chat.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ChatInformation class, which represents the information about a chat.
        /// It initializes the chat's details and message history path.
        /// </remarks>
        public ChatInformation(ChatDetails chatDetails, string messageHistoryPath)
        {
            _chatDetails = chatDetails;
            _messageHistoryPath = messageHistoryPath;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ChatDetails" property represents the details of the chat.
        /// It gets or sets the details of the chat.
        /// </summary>
        /// <value>
        /// The details of the chat.
        /// </value>
        public ChatDetails ChatDetails
        {
            get 
            { 
                return _chatDetails;
            }
            set 
            { 
                _chatDetails = value;
            }
        }

        /// <summary>
        /// The "MessageHistoryPath" property represents the path to the message history file for the chat.
        /// It gets or sets the path to the message history file for the chat.
        /// </summary>
        /// <value>
        /// The path to the message history file for the chat.
        /// </value>
        public string MessageHistoryPath
        {
            get
            {
                return _messageHistoryPath;
            }  
            set
            {
                _messageHistoryPath = value;
            }
        }

        #endregion
    }
}
