using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class CaptchaRotationImages
    {
        private ImageContent _backgroundImage;
        private ImageContent _rotatedImage;

        public CaptchaRotationImages(ImageContent backgroundImage, ImageContent rotatedImage)
        {
            _backgroundImage = backgroundImage;
            _rotatedImage = rotatedImage;
        }
        public ImageContent BackgroundImage
        { 
            get 
            { 
                return _backgroundImage; 
            } 
            set 
            { 
                _backgroundImage = value;
            }
        }
        public ImageContent RotatedImage
        {
            get
            {
                return _rotatedImage;
            }
            set
            {
                _rotatedImage = value;
            }
        }
    }
}
