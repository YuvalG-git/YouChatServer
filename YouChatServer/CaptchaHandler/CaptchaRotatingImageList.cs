using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouChatServer.CaptchaHandler
{
    /// <summary>
    /// The "CaptchaRotatingImageList" class manages a list of captcha images for rotation and display.
    /// </summary>
    internal class CaptchaRotatingImageList
    {
        #region Private Const Fields

        /// <summary>
        /// The integer constant 'ImageNumber' represents the number of images in the CAPTCHA view image list.
        /// </summary>
        private const int ImageNumber = 15;

        #endregion

        #region Public Static Fields

        /// <summary>
        /// The ImageList object 'CaptchaViewImageList' represents a collection of images used for CAPTCHA viewing.
        /// </summary>
        public static ImageList CaptchaViewImageList { get; private set; }

        #endregion

        #region Static Constructor

        /// <summary>
        /// Static constructor for the <see cref="CaptchaRotatingImageList"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor initializes the <see cref="CaptchaViewImageList"/> with settings for displaying captcha images.
        /// It sets the image size, color depth, and loads captcha view background images from resources.
        /// </remarks>
        static CaptchaRotatingImageList()
        {
            CaptchaViewImageList = new ImageList();
            CaptchaViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            CaptchaViewImageList.ImageSize = new Size(256, 256);
            CaptchaViewImageList.ColorDepth = ColorDepth.Depth32Bit;
            LoadCaptchaViewBackgroundImagesFromResources();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// The "LoadCaptchaViewBackgroundImagesFromResources" method loads captcha view background images from the application's resources.
        /// </summary>
        /// <remarks>
        /// This method iterates through a list of image names and attempts to load each image from the application's resources. 
        /// If an image is successfully loaded, it is added to the CaptchaViewImageList for later use.
        /// </remarks>
        private static void LoadCaptchaViewBackgroundImagesFromResources()
        {
            List<string> ImageNames = new List<string>();
            for (int i = 1; i <= ImageNumber; i++)
            {
                ImageNames.Add("View" + i);
            }
            foreach (string resourceName in ImageNames)
            {
                Image image = Properties.CaptchaViewImages.ResourceManager.GetObject(resourceName) as Image;
                if (image != null)
                {
                    CaptchaViewImageList.Images.Add(image);
                }
            }
        }

        #endregion
    }
}
