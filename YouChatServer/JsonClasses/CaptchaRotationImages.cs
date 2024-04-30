using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "CaptchaRotationImages" class represents the images used in captcha rotation.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the background image and rotated image used in captcha rotation.
    /// </remarks>
    internal class CaptchaRotationImages
    {
        #region Private Fields

        /// <summary>
        /// The ImageContent object "_backgroundImage" represents the background image used for captcha generation.
        /// </summary>
        private ImageContent _backgroundImage;

        /// <summary>
        /// The ImageContent object "_rotatedImage" represents the rotated image used for captcha generation.
        /// </summary>
        private ImageContent _rotatedImage;

        #endregion

        #region Constructors

        /// <summary>
        /// The "CaptchaRotationImages" constructor initializes a new instance of the <see cref="CaptchaRotationImages"/> class with the specified background image and rotated image.
        /// </summary>
        /// <param name="backgroundImage">The background image for the captcha rotation.</param>
        /// <param name="rotatedImage">The rotated image for the captcha rotation.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the CaptchaRotationImages class, which represents the images used in captcha rotation.
        /// It initializes the background image and rotated image for the captcha rotation.
        /// </remarks>
        public CaptchaRotationImages(ImageContent backgroundImage, ImageContent rotatedImage)
        {
            _backgroundImage = backgroundImage;
            _rotatedImage = rotatedImage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "BackgroundImage" property represents the background image of a chat window.
        /// It gets or sets the background image of the chat window.
        /// </summary>
        /// <value>
        /// The background image of the chat window.
        /// </value>
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

        /// <summary>
        /// The "RotatedImage" property represents an image that has been rotated.
        /// It gets or sets the rotated image.
        /// </summary>
        /// <value>
        /// The rotated image.
        /// </value>
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

        #endregion
    }
}
