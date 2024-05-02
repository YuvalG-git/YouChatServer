using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses.MessageClasses
{
    /// <summary>
    /// The "MessageHistory" class represents a history of messages in a chat.
    /// </summary>
    /// <remarks>
    /// This class contains a list of messages that represent the history of messages in a chat.
    /// </remarks>
    public class MessageHistory
    {
        #region Private Fields

        /// <summary>
        /// The List<Message> "_messages" represents the list of messages.
        /// </summary>
        private List<Message> _messages;

        #endregion

        #region Constructors

        /// <summary>
        /// The "MessageHistory" constructor initializes a new instance of the <see cref="MessageHistory"/> class with the specified list of messages.
        /// </summary>
        /// <param name="messages">The list of messages in the message history.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the MessageHistory class, representing a history of messages in a chat,
        /// using the list of messages provided in the parameters. It initializes the message history with the given messages.
        /// </remarks>
        public MessageHistory(List<Message> messages)
        {
            _messages = messages;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Messages" property represents a list of messages.
        /// It gets or sets the list of messages.
        /// </summary>
        /// <value>
        /// The list of messages.
        /// </value>
        public List<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }

        #endregion
    }
}
