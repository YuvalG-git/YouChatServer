using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses.MessageClasses
{
    /// <summary>
    /// The "MessageData" class represents a message sent in a chat.
    /// </summary>
    /// <remarks>
    /// This class contains information about the message sender, the type of message,
    /// the content of the message, and the date when the message was sent.
    /// </remarks>
    public class MessageData
    {
        #region Private Fields

        /// <summary>
        /// The string "_sender" represents the sender of the message.
        /// </summary>
        private string _sender;

        /// <summary>
        /// The string "_type" represents the type of the message.
        /// </summary>
        private string _type;

        /// <summary>
        /// The string "_content" represents the content of the message.
        /// </summary>
        private string _content;

        /// <summary>
        /// The string "_date" represents the date of the message.
        /// </summary>
        private string _date;

        #endregion

        #region Constructors

        /// <summary>
        /// The "MessageData" constructor initializes a new instance of the <see cref="MessageData"/> class with the specified parameters.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="type">The type of the message.</param>
        /// <param name="content">The content of the message.</param>
        /// <param name="date">The date when the message was sent.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the MessageData class, representing a message sent in a chat,
        /// using the details provided in the parameters. It initializes the message's sender, type, content, and date.
        /// </remarks>
        public MessageData(string sender, string type, string content, string date)
        {
            _sender = sender;
            _type = type;
            _content = content;
            _date = date;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Sender" property represents the sender of a message.
        /// It gets or sets the sender of the message.
        /// </summary>
        /// <value>
        /// The sender of the message.
        /// </value>
        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
            }
        }

        /// <summary>
        /// The "Type" property represents the type of a message.
        /// It gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// The "Content" property represents the content of a message.
        /// It gets or sets the content of the message.
        /// </summary>
        /// <value>
        /// The content of the message.
        /// </value>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// The "Date" property represents the date of a message.
        /// It gets or sets the date of the message.
        /// </summary>
        /// <value>
        /// The date of the message.
        /// </value>
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        #endregion
    }
}
