using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.Properties;

namespace YouChatServer.CaptchaHandler
{
    /// <summary>
    /// The "CaptchaCodeHandler" class manages captcha bitmap code generation and comparison.
    /// </summary>
    internal class CaptchaCodeHandler 
    {
        #region Private Fields

        /// <summary>
        /// The string object "code" stores the generated CAPTCHA code.
        /// </summary>
        private string code;

        #endregion

        #region Private Static Fields

        /// <summary>
        /// The List of Image objects "captchaBackground" stores the background images used for generating CAPTCHA.
        /// </summary>
        private static List<Image> captchaBackground;

        #endregion

        #region Constructors

        /// <summary>
        /// The "CaptchaCodeHandler" constructor initializes a new instance of the <see cref="CaptchaCodeHandler"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the captcha background images for generating captcha codes.
        /// </remarks>
        public CaptchaCodeHandler()
        {
            captchaBackground = new List<Image>
            {
            Properties.CaptchaBackground.CaptchaBackground1,
            Properties.CaptchaBackground.CaptchaBackground2,
            Properties.CaptchaBackground.CaptchaBackground3,
            Properties.CaptchaBackground.CaptchaBackground4
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "CompareCode" method compares the received code with the internally stored code.
        /// </summary>
        /// <param name="receivedCode">The code received from a client.</param>
        /// <returns>
        /// True if the received code matches the internally stored code, otherwise false.
        /// </returns>
        /// <remarks>
        /// This method compares the received code with the code stored internally. It returns true if the received code
        /// matches the stored code, indicating a successful comparison. Otherwise, it returns false, indicating a mismatch.
        /// </remarks>
        public bool CompareCode(string recievedCode)
        {
            return recievedCode == code;
        }

        /// <summary>
        /// The "CreateCatpchaBitmap" method generates a captcha image with a random string.
        /// </summary>
        /// <returns>
        /// An Image object containing the captcha image.
        /// </returns>
        /// <remarks>
        /// This method creates a captcha image with a random string of characters. It selects a random background image
        /// from a collection of background images and draws the captcha text onto it. The generated captcha image is returned
        /// as an Image object.
        /// </remarks>
        public Image CreateCatpchaBitmap()
        {
            code = RandomStringCreator.RandomString(8,true);

            int width = 200;
            int height = 50;
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Random random = new Random();
                int randomIndex = random.Next(0, captchaBackground.Count);
                Image randomImage = captchaBackground[randomIndex];
                try
                {
                    graphics.DrawImage(randomImage, 0, 0, width, height);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"ArgumentException: {ex.Message}");
                    Console.WriteLine(randomIndex);

                }
                string text = code;
                Font font = new Font("Lucida Handwriting", 18, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.Black);
                SizeF textSize = graphics.MeasureString(text, font);
                float x = (bitmap.Width - textSize.Width) / 2;
                float y = (bitmap.Height - textSize.Height) / 2;
                PointF textLocation = new PointF(x, y);
                graphics.DrawString(text, font, brush, textLocation);

                font.Dispose();
                brush.Dispose();
            }
            return bitmap;
        }

        #endregion
    }
}
