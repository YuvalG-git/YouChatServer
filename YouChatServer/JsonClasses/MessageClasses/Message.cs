using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "Message" class represents a message sent in a chat.
    /// </summary>
    /// <remarks>
    /// This class contains information about the message sender, the chat ID associated with the message,
    /// the content of the message, and the date and time when the message was sent.
    /// </remarks>
    public class Message
    {
        #region Private Fields

        /// <summary>
        /// The string "_messageSenderName" represents the name of the message sender.
        /// </summary>
        private string _messageSenderName;

        /// <summary>
        /// The string "_chatId" represents the ID of the chat associated with the message.
        /// </summary>
        private string _chatId;

        /// <summary>
        /// The object "_messageContent" represents the content of the message.
        /// It might be from type string or type ImageContent.
        /// </summary>
        private object _messageContent;

        /// <summary>
        /// The DateTime "_messageDateAndTime" represents the date and time when the message was sent.
        /// </summary>
        private DateTime _messageDateAndTime;

        #endregion

        #region Constructors

        /// <summary>
        /// The "Message" constructor initializes a new instance of the <see cref="Message"/> class with the specified parameters.
        /// </summary>
        /// <param name="messageSenderName">The name of the sender of the message.</param>
        /// <param name="chatId">The ID of the chat associated with the message.</param>
        /// <param name="messageContent">The content of the message.</param>
        /// <param name="messageDateAndTime">The date and time when the message was sent.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the Message class, representing a message sent in a chat,
        /// using the details provided in the parameters. It initializes the message's sender name, chat ID, content, and date and time.
        /// </remarks>
        public Message(string messageSenderName, string chatId, object messageContent, DateTime messageDateAndTime)
        {
            _messageSenderName = messageSenderName;
            _chatId = chatId;
            _messageContent = messageContent;
            _messageDateAndTime = messageDateAndTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "MessageContent" property represents the content of a message.
        /// It gets or sets the content of the message.
        /// </summary>
        /// <value>
        /// The content of the message.
        /// </value>
        public object MessageContent
        {
            get
            {
                return _messageContent;
            }
            set
            {
                _messageContent = value;
            }
        }

        /// <summary>
        /// The "MessageDateAndTime" property represents the date and time when a message was sent.
        /// It gets or sets the date and time of the message.
        /// </summary>
        /// <value>
        /// The date and time when the message was sent.
        /// </value>
        public DateTime MessageDateAndTime
        {
            get
            {
                return _messageDateAndTime;
            }
            set
            {
                _messageDateAndTime = value;
            }
        }

        /// <summary>
        /// The "ChatId" property represents the identifier of the chat to which a message belongs.
        /// It gets or sets the chat identifier of the message.
        /// </summary>
        /// <value>
        /// The identifier of the chat to which the message belongs.
        /// </value>
        public string ChatId
        {
            get
            {
                return _chatId;
            }
            set
            {
                _chatId = value;
            }
        }

        /// <summary>
        /// The "MessageSenderName" property represents the name of the sender of a message.
        /// It gets or sets the name of the message sender.
        /// </summary>
        /// <value>
        /// The name of the sender of the message.
        /// </value>
        public string MessageSenderName
        {
            get
            {
                return _messageSenderName;
            }
            set
            {
                _messageSenderName = value;
            }
        }

        #endregion
    }
}
