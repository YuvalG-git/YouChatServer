using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class ImageContent
    {
        private byte[] _imageBytes;

        public ImageContent(byte[] imageBytes)
        {
            _imageBytes = imageBytes;
        }
        public byte[] ImageBytes
        { get 
            { return _imageBytes; }
            set
            {
                _imageBytes = value;
            }
        }
    }
}
