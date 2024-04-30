using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "AudioCallOverDetails" class represents the details of an audio call over a socket connection.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the chat ID and socket port used for the audio call.
    /// </remarks>
    internal class AudioCallOverDetails
    {
        #region Private Fields

        /// <summary>
        /// The string object "_chatId" represents the unique identifier for the chat.
        /// </summary>
        private string _chatId;

        /// <summary>
        /// The integer "_socketPort" represents the port number used for socket communication.
        /// </summary>
        private int _socketPort;

        #endregion

        #region Constructors

        /// <summary>
        /// The "AudioCallOverDetails" constructor initializes a new instance of the <see cref="AudioCallOverDetails"/> class with the specified chat ID and socket port.
        /// </summary>
        /// <param name="chatId">The ID of the chat associated with the audio call.</param>
        /// <param name="socketPort">The socket port used for the audio call.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the AudioCallOverDetails class, which represents the details of an audio call over a socket connection.
        /// It initializes the chat ID and socket port for the audio call.
        /// </remarks>
        public AudioCallOverDetails(string chatId, int socketPort)
        {
            _chatId = chatId;
            _socketPort = socketPort;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ChatId" property represents the unique identifier of a chat.
        /// It gets or sets the unique identifier of the chat.
        /// </summary>
        /// <value>
        /// The unique identifier of the chat.
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
        /// The "SocketPort" property represents the port number for a socket connection.
        /// It gets or sets the port number for the socket connection.
        /// </summary>
        /// <value>
        /// The port number for the socket connection.
        /// </value>
        public int SocketPort
        {
            get
            {
                return _socketPort;
            }
            set
            {
                _socketPort = value;
            }
        }

        #endregion
    }
}
