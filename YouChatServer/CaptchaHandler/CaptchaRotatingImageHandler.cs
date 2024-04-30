using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YouChatServer.JsonClasses;

namespace YouChatServer.CaptchaHandler
{
    /// <summary>
    /// The "CaptchaRotatingImageHandler" class manages the rotation and presentation of captcha images for verification.
    /// </summary>
    internal class CaptchaRotatingImageHandler
    {
        #region Private Fields

        /// <summary>
        /// The double variable 'captchaFlippedImageAngle' stores the angle for flipping CAPTCHA images.
        /// </summary>
        private double captchaFlippedImageAngle;

        /// <summary>
        /// The double variable 'captchaRotatedImageAngle' stores the angle for rotating CAPTCHA images.
        /// </summary>
        private double captchaRotatedImageAngle;

        /// <summary>
        /// The Queue<int> object 'NumbersForCaptchaPictures' stores numbers used for CAPTCHA pictures.
        /// </summary>
        private Queue<int> NumbersForCaptchaPictures;

        /// <summary>
        /// The int variable 'CaptchaPicturesScore' stores the score for CAPTCHA pictures.
        /// </summary>
        private int CaptchaPicturesScore = 0;

        /// <summary>
        /// The int variable 'CaptchaPictureAttempts' stores the number of attempts for CAPTCHA pictures.
        /// </summary>
        private int CaptchaPictureAttempts = 0;

        /// <summary>
        /// The int variable 'CurrentPictureIndex' stores the index of the current CAPTCHA picture.
        /// </summary>
        private int CurrentPictureIndex;

        /// <summary>
        /// The List<int> object 'NumbersList' stores a list of numbers.
        /// </summary>
        private List<int> NumbersList;

        #endregion

        #region Constructors

        /// <summary>
        /// The "CaptchaRotatingImageHandler" constructor Initializes a new instance of the <see cref="CaptchaRotatingImageHandler"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the order of pictures for the rotating captcha images.
        /// </remarks>
        public CaptchaRotatingImageHandler()
        {
            SetPictureOrderForCaptcha();
        }

        #endregion

        #region Private Drawing Methods

        /// <summary>
        /// The "RotateBothPictureBoxsRandomlly" method rotates both captcha images randomly.
        /// </summary>
        /// <param name="captchaCircularImage">A reference to the circular captcha image to rotate.</param>
        /// <param name="captchaImage">A reference to the captcha image to rotate.</param>
        /// <remarks>
        /// This method rotates both the circular captcha image and the regular captcha image randomly. It selects a random angle
        /// for flipping the regular captcha image and a random angle for rotating the circular captcha image. The angles are
        /// chosen to be sufficiently different to ensure readability of the captcha. The method updates the references to the
        /// captcha images with the rotated images.
        /// </remarks>
        private void RotateBothPictureBoxsRandomlly(ref Image captchaCircularImage, ref Image captchaImage)
        {
            CaptchaPictureAttempts++;
            Random random = new Random();
            captchaFlippedImageAngle = random.Next(1, 4) * 90;
            do
            {
                captchaRotatedImageAngle = random.Next(360);
            }
            while (Math.Abs(captchaFlippedImageAngle - captchaRotatedImageAngle) <= 25);
            Bitmap CaptchaPictureBoxRotatedImage = new Bitmap(captchaImage.Width, captchaImage.Height);
            Bitmap CaptchaCircularPictureBoxAngleRotatedImage = new Bitmap(captchaCircularImage.Width, captchaCircularImage.Height);
            Image originalImage = CaptchaRotatingImageList.CaptchaViewImageList.Images[CurrentPictureIndex];
            RotateImage(ref CaptchaPictureBoxRotatedImage, originalImage, (float)captchaFlippedImageAngle);
            RotateImage(ref CaptchaCircularPictureBoxAngleRotatedImage, originalImage, (float)captchaRotatedImageAngle);

            captchaImage = CaptchaPictureBoxRotatedImage;
            captchaCircularImage = CaptchaCircularPictureBoxAngleRotatedImage;
        }

        /// <summary>
        /// The "RotateImage" method rotates an image by a specified angle.
        /// </summary>
        /// <param name="rotatedImage">A reference to the bitmap that will store the rotated image.</param>
        /// <param name="originalImage">The original image to rotate.</param>
        /// <param name="angle">The angle by which to rotate the image, in degrees.</param>
        /// <remarks>
        /// This method rotates the specified original image by the given angle and stores the result in the provided bitmap.
        /// The rotation is performed around the center of the image. The method uses graphics transformations to rotate the image.
        /// </remarks>
        private void RotateImage(ref Bitmap rotatedImage, Image originalImage, float angle)
        {
            using (Graphics graphics = Graphics.FromImage(rotatedImage))
            {
                graphics.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-rotatedImage.Width / 2, -rotatedImage.Height / 2);
                graphics.DrawImage(originalImage, new PointF(0, 0));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The "SetPictureOrderForCaptcha" method sets the order of pictures for the CAPTCHA challenge.
        /// </summary>
        /// <remarks>
        /// This method initializes a queue and a list to manage the order of pictures used for the CAPTCHA challenge.
        /// It populates the list with sequential numbers representing the indexes of the pictures.
        /// Then, it shuffles the list to randomize the order of the pictures.
        /// Finally, it adds the shuffled numbers to the queue, ensuring that each picture is used once in the challenge.
        /// </remarks>
        private void SetPictureOrderForCaptcha()
        {
            NumbersForCaptchaPictures = new Queue<int>();
            NumbersList = new List<int>();
            SetNumbersList();
            SetNumberForCaptchaPicturesQueue();
        }

        /// <summary>
        /// The "SetNumbersList" method initializes a list with sequential numbers representing the indexes of pictures for the CAPTCHA challenge.
        /// </summary>
        /// <remarks>
        /// This method populates the list with numbers from 0 to the total number of images available for the CAPTCHA challenge.
        /// These numbers correspond to the indexes of the pictures in the image list.
        /// </remarks>
        private void SetNumbersList()
        {
            for (int i = 0; i < CaptchaRotatingImageList.CaptchaViewImageList.Images.Count; i++)
                NumbersList.Add(i);
        }

        /// <summary>
        /// The "SetNumberForCaptchaPicturesQueue" method randomly assigns numbers from the NumbersList to the NumbersForCaptchaPictures queue.
        /// </summary>
        /// <remarks>
        /// This method uses a random number generator to select indexes from the NumbersList and adds them to the NumbersForCaptchaPictures queue.
        /// Once an index is added to the queue, it is removed from the NumbersList to ensure that each index is used only once.
        /// </remarks>
        private void SetNumberForCaptchaPicturesQueue()
        {
            Random random = new Random();
            int Index;
            while (NumbersList.Count > 0)
            {
                Index = random.Next(0, NumbersList.Count);
                NumbersForCaptchaPictures.Enqueue(NumbersList[Index]);
                NumbersList.RemoveAt(Index);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "GetCaptchaRotationImages" method retrieves a pair of rotated images for use in a captcha challenge.
        /// </summary>
        /// <returns>
        /// A <see cref="CaptchaRotationImages"/> object containing the rotated images.
        /// </returns>
        /// <remarks>
        /// This method dequeues an index from the NumbersForCaptchaPictures queue and uses it to retrieve the corresponding images
        /// from the CaptchaRotatingImageList. It then rotates both images randomly and converts them to byte arrays for storage.
        /// The rotated images are stored in an <see cref="ImageContent"/> object and returned as a CaptchaRotationImages object.
        /// If the NumbersForCaptchaPictures queue is empty, the method resets the queue and the index list before proceeding.
        /// </remarks>
        public CaptchaRotationImages GetCaptchaRotationImages()
        {
            if (NumbersForCaptchaPictures.Count == 0)
            {
                SetNumbersList();
                SetNumberForCaptchaPicturesQueue();
            }
            CurrentPictureIndex = NumbersForCaptchaPictures.Dequeue();
            Image captchaCircularImage = CaptchaRotatingImageList.CaptchaViewImageList.Images[CurrentPictureIndex];
            Image captchaImage = CaptchaRotatingImageList.CaptchaViewImageList.Images[CurrentPictureIndex];
            RotateBothPictureBoxsRandomlly( ref captchaCircularImage, ref captchaImage);

            byte[] captchaCircularImageBytes = ConvertHandler.ConvertImageToBytes(captchaCircularImage);
            byte[] captchaImageBytes = ConvertHandler.ConvertImageToBytes(captchaImage);
            ImageContent captchaCircularImageContent = new ImageContent(captchaCircularImageBytes);
            ImageContent captchaImageContent = new ImageContent(captchaImageBytes);
            CaptchaRotationImages captchaRotationImages = new CaptchaRotationImages(captchaImageContent, captchaCircularImageContent);
            return captchaRotationImages;
        }


        /// <summary>
        /// The "CheckAngle" method compares the angles of the flipped and rotated captcha images to determine if they match.
        /// </summary>
        /// <param name="captchaRotatedImageAngle">The angle of rotation applied to the captcha image.</param>
        /// <remarks>
        /// This method first normalizes the angles to ensure they fall within the range of 0 to 360 degrees.
        /// It then calculates the sum of the rotation angles and compares it to the flipped image angle.
        /// If the absolute difference between the angles is less than or equal to 5 degrees, the CaptchaPicturesScore is incremented.
        /// </remarks>
        public void CheckAngle(double captchaRotatedImageAngle)
        {

            if (captchaFlippedImageAngle < 0)
            {
                captchaFlippedImageAngle += 360;
            }
            if (captchaRotatedImageAngle  < 0)
            {
                captchaRotatedImageAngle  += 360;
            }
            double realAngle = (this.captchaRotatedImageAngle + captchaRotatedImageAngle) % 360;
            if (Math.Abs(captchaFlippedImageAngle - realAngle) <= 5)
            {
                CaptchaPicturesScore++;
            }
        }

        /// <summary>
        /// The "CheckAttempts" method checks if the number of captcha picture rotation attempts has reached the maximum allowed.
        /// </summary>
        /// <returns>True if the number of attempts equals 5, otherwise false.</returns>
        /// <remarks>
        /// This method is used to determine if the user has exceeded the maximum number of attempts to rotate the captcha pictures.
        /// If the number of attempts equals 5, it returns true; otherwise, it returns false.
        /// </remarks>
        public bool CheckAttempts()
        {
            return CaptchaPictureAttempts == 5;
        }

        /// <summary>
        /// The "CheckSuccess" method checks if the user successfully solved the captcha.
        /// </summary>
        /// <returns>True if the user's score is equal to or greater than 3, otherwise false.</returns>
        /// <remarks>
        /// This method is used to determine if the user successfully solved the captcha by scoring 3 or more points.
        /// If the user's score is less than 3, it resets the captcha picture attempts and score to 0 and returns false; otherwise, it returns true.
        /// </remarks>
        public bool CheckSuccess()
        {
            if (CaptchaPicturesScore < 3)
            {
                CaptchaPictureAttempts = 0;
                CaptchaPicturesScore = 0;
                return false;
            }
            return true;
        }

        #endregion

        #region Public Get Methods

        /// <summary>
        /// The "GetScore" method retrieves the user's current score for solving captchas.
        /// </summary>
        /// <returns>The number of captchas successfully solved by the user.</returns>
        /// <remarks>
        /// This method returns the current score of the user, indicating the number of captchas successfully solved.
        /// </remarks>
        public int GetScore()
        {
            return CaptchaPicturesScore;
        }

        /// <summary>
        /// The "GetAttempts" method retrieves the number of attempts made to solve captchas.
        /// </summary>
        /// <returns>The number of attempts made to solve captchas.</returns>
        /// <remarks>
        /// This method returns the number of attempts made by the user to solve captchas.
        /// </remarks>
        public int GetAttempts()
        {
            return CaptchaPictureAttempts;
        }

        #endregion
    }
}
