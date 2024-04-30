using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "ImageContent" class represents image content in byte format.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the bytes representing an image.
    /// </remarks>
    internal class ImageContent
    {
        #region Private Fields

        /// <summary>
        /// The byte array "_imageBytes" stores the bytes of an image.
        /// </summary>
        private byte[] _imageBytes;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ImageContent" constructor initializes a new instance of the <see cref="ImageContent"/> class with the specified image bytes.
        /// </summary>
        /// <param name="imageBytes">The bytes representing the image.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ImageContent class, which represents image content in byte format.
        /// It initializes the image bytes of the ImageContent object.
        /// </remarks>
        public ImageContent(byte[] imageBytes)
        {
            _imageBytes = imageBytes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ImageBytes" property represents the image data as a byte array.
        /// It gets or sets the image data as a byte array.
        /// </summary>
        /// <value>
        /// The image data as a byte array.
        /// </value>
        public byte[] ImageBytes
        { 
            get 
            { 
                return _imageBytes;
            }
            set
            {
                _imageBytes = value;
            }
        }

        #endregion
    }
}
