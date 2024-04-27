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
        /// <summary>
        /// The string object "code" stores the generated CAPTCHA code.
        /// </summary>
        private string code;

        /// <summary>
        /// The List of Image objects "captchaBackground" stores the background images used for generating CAPTCHA.
        /// </summary>
        private static List<Image> captchaBackground;

        /// <summary>
        /// The "CaptchaCodeHandler" constructor initializes a new instance of the <see cref="CaptchaCodeHandler"/> class by initializes the list of captcha backgrounds.
        /// </summary>
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

        /// <summary>
        /// The CompareCode method compares the provided code with the generated code.
        /// </summary>
        /// <param name="receivedCode">The code to compare.</param>
        /// <returns>True if the codes match, false otherwise.</returns>
        public bool CompareCode(string recievedCode)
        {
            return (recievedCode == code);
        }

        /// <summary>
        /// The "CreateCatpchaBitmap" method generates a new CAPTCHA code and bitmap image.
        /// </summary>
        /// <returns>The generated bitmap image containing the CAPTCHA code.</returns>
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
    }
}
