using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class VideoCallOverDetails
    {
        private string _chatId;
        private int _audioSocketPort;
        private int _videoSocketPort;

        public VideoCallOverDetails(string chatId, int audioSocketPort, int videoSocketPort)
        {
            _chatId = chatId;
            _audioSocketPort = audioSocketPort;
            _videoSocketPort = videoSocketPort;
        }

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
    }
}
