using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "VideoCallOverDetails" class represents details about a video call, including the chat ID, audio socket port, and video socket port.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing video call details such as the chat ID, audio socket port, and video socket port.
    /// </remarks>
    internal class VideoCallOverDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_chatId" stores the ID of the chat.
        /// </summary>
        private string _chatId;

        /// <summary>
        /// The int "_audioSocketPort" stores the port number for audio socket communication.
        /// </summary>
        private int _audioSocketPort;

        /// <summary>
        /// The int "_videoSocketPort" stores the port number for video socket communication.
        /// </summary>
        private int _videoSocketPort;

        #endregion

        #region Constructors

        /// <summary>
        /// The "VideoCallOverDetails" constructor initializes a new instance of the <see cref="VideoCallOverDetails"/> class with the specified chat ID, audio socket port, and video socket port.
        /// </summary>
        /// <param name="chatId">The ID of the chat associated with the video call.</param>
        /// <param name="audioSocketPort">The port number for audio communication in the video call.</param>
        /// <param name="videoSocketPort">The port number for video communication in the video call.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the VideoCallOverDetails class, which represents details about a video call.
        /// It initializes the chat ID, audio socket port, and video socket port for the video call.
        /// </remarks>
        public VideoCallOverDetails(string chatId, int audioSocketPort, int videoSocketPort)
        {
            _chatId = chatId;
            _audioSocketPort = audioSocketPort;
            _videoSocketPort = videoSocketPort;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ChatId" property represents the ID of a chat.
        /// It gets or sets the ID of the chat.
        /// </summary>
        /// <value>
        /// The ID of the chat.
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
        /// The "AudioSocketPort" property represents the port for audio socket communication.
        /// It gets or sets the port for audio socket communication.
        /// </summary>
        /// <value>
        /// The port for audio socket communication.
        /// </value>
        public int AudioSocketPort
        {
            get
            {
                return _audioSocketPort;
            }
            set
            {
                _audioSocketPort = value;
            }
        }

        /// <summary>
        /// The "VideoSocketPort" property represents the port for video socket communication.
        /// It gets or sets the port for video socket communication.
        /// </summary>
        /// <value>
        /// The port for video socket communication.
        /// </value>
        public int VideoSocketPort
        {
            get
            {
                return _videoSocketPort;
            }
            set
            {
                _videoSocketPort = value;
            }
        }

        #endregion
    }
}
