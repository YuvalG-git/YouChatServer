using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class UdpPorts
    {
        private int _audioPort;
        private int _videoPort;

        public UdpPorts(int audioPort, int videoPort)
        {
            _audioPort = audioPort;
            _videoPort = videoPort;
        }
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
    }
}
