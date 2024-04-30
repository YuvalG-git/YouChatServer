using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "UdpPorts" class represents UDP ports for audio and video communication.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the audio and video ports for UDP communication setup.
    /// </remarks>
    internal class UdpPorts
    {
        #region Private Fields

        /// <summary>
        /// The int "_audioPort" stores the audio port number.
        /// </summary>
        private int _audioPort;

        /// <summary>
        /// The int "_videoPort" stores the video port number.
        /// </summary>
        private int _videoPort;

        #endregion

        #region Constructors

        /// <summary>
        /// The "UdpPorts" constructor initializes a new instance of the <see cref="UdpPorts"/> class with the specified audio and video ports.
        /// </summary>
        /// <param name="audioPort">The port number for audio communication.</param>
        /// <param name="videoPort">The port number for video communication.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the UdpPorts class, which represents UDP ports for audio and video communication.
        /// It initializes the audio and video ports for the UDP communication setup.
        /// </remarks>
        public UdpPorts(int audioPort, int videoPort)
        {
            _audioPort = audioPort;
            _videoPort = videoPort;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "AudioPort" property represents the port number used for audio communication.
        /// It gets or sets the port number for audio communication.
        /// </summary>
        /// <value>
        /// The port number used for audio communication.
        /// </value>
        public int AudioPort 
        { 
            get 
            { 
                return _audioPort;
            }
            set
            {
                _audioPort = value;
            }
        }

        /// <summary>
        /// The "VideoPort" property represents the port number used for video communication.
        /// It gets or sets the port number for video communication.
        /// </summary>
        /// <value>
        /// The port number used for video communication.
        /// </value>
        public int VideoPort
        {
            get
            {
                return _videoPort;
            }
            set
            {
                _videoPort = value;
            }
        }

        #endregion
    }
}
