using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "JsonObject" class represents a JSON object with a specific message type and body.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the message type and body of the JSON object.
    /// </remarks>
    internal class JsonObject
    {
        #region Private Fields

        /// <summary>
        /// The Enum "messageType" represents the type of message.
        /// </summary>
        private Enum messageType;

        /// <summary>
        /// The object "messageBody" stores the body of the message.
        /// </summary>
        private object messageBody;

        #endregion

        #region Constructors

        /// <summary>
        /// The "JsonObject" constructor initializes a new instance of the <see cref="JsonObject"/> class with the specified message type and message body.
        /// </summary>
        /// <param name="messageType">The type of the JSON message.</param>
        /// <param name="messageBody">The body of the JSON message.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the JsonObject class, which represents a JSON object with a specific message type and body.
        /// It initializes the message type and message body of the JsonObject.
        /// </remarks>
        public JsonObject(Enum messageType, object messageBody)
        {
            this.messageType = messageType;
            this.messageBody = messageBody;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "MessageType" property represents the type of message.
        /// It gets or sets the type of message.
        /// </summary>
        /// <value>
        /// The type of message.
        /// </value>
        public Enum MessageType
        {
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }

        /// <summary>
        /// The "MessageBody" property represents the body content of a message.
        /// It gets or sets the body content of the message.
        /// </summary>
        /// <value>
        /// The body content of the message.
        /// </value>
        public object MessageBody
        {
            get
            {
                return messageBody;
            }
            set
            {
                messageBody = value;
            }
        }

        #endregion
    }
}
