using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.Properties;

namespace YouChatServer.CaptchaHandler
{
    internal class CaptchaCodeHandler 
    {
        private string code;
        private static List<Image> captchaBackground;
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
    

        public string Code
        {
            get { return code; }
        }
        public bool CompareCode(string recievedCode)
        {
            return (recievedCode == code);
        }
        public Image CreateCatpchaBitmap()
        {
            code = RandomStringCreator.RandomString(8,true);
            // Create an empty bitmap with a specified width and height
            int width = 200; // Replace with your desired width
            int height = 50; // Replace with your desired height
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Draw an image on the bitmap
                // Create a Random object
                Random random = new Random();

                // Get a random index within the range of the list
                int randomIndex = random.Next(0, captchaBackground.Count);

                // Get the random image
                Image randomImage = captchaBackground[randomIndex];
                try
                {
                    graphics.DrawImage(randomImage, 0, 0, width, height);
                }
                catch (ArgumentException ex)
                {
                    // Handle the exception or print a message
                    Console.WriteLine($"ArgumentException: {ex.Message}");
                    Console.WriteLine(randomIndex);

                }
                // Draw text on the bitmap
                string text = code;
                Font font = new Font("Lucida Handwriting", 18, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.Black);
                SizeF textSize = graphics.MeasureString(text, font);
                float x = (bitmap.Width - textSize.Width) / 2;
                float y = (bitmap.Height - textSize.Height) / 2;
                PointF textLocation = new PointF(x, y);
                graphics.DrawString(text, font, brush, textLocation);

                // Dispose of resources
                font.Dispose();
                brush.Dispose();
            }
            return bitmap;

            //create code
            //save code
            //create bitmap
            //send bitmap

            //than compare code
        }
    }
}
