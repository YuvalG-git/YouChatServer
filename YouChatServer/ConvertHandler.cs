using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "ConvertHandler" class provides methods for converting images to and from byte arrays.
    /// </summary>
    internal class ConvertHandler
    {
        #region Public Static Methods

        /// <summary>
        /// The "ConvertImageToBytes" method converts an Image object to a byte array.
        /// </summary>
        /// <param name="image">The Image object to convert.</param>
        /// <returns>A byte array representing the Image object.</returns>
        /// <remarks>
        /// This method saves the Image object to a MemoryStream as a JPEG image, then converts the MemoryStream to a byte array.
        /// </remarks>
        public static byte[] ConvertImageToBytes(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        #endregion
    }
}
