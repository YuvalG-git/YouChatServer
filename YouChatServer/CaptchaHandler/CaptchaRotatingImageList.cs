using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouChatServer.CaptchaHandler
{
    internal class CaptchaRotatingImageList
    {
        public static ImageList CaptchaViewImageList { get; private set; }
        private const int ImageNumber = 15;

        static CaptchaRotatingImageList()
        {
            CaptchaViewImageList = new ImageList();
            CaptchaViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            CaptchaViewImageList.ImageSize = new Size(256, 256);
            CaptchaViewImageList.ColorDepth = ColorDepth.Depth32Bit;
            LoadCaptchaViewBackgroundImagesFromResources();
        }
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
    }
}
