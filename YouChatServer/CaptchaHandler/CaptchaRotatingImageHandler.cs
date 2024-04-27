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
    internal class CaptchaRotatingImageHandler
    {
        private double captchaFlippedImageAngle;
        private double captchaRotatedImageAngle;

        Queue<int> NumbersForCaptchaPictures;
        int CaptchaPicturesScore = 0;
        int CaptchaPictureAttempts = 0;
        int CurrentPictureIndex;
        List<int> NumbersList;
        public CaptchaRotatingImageHandler()
        {
            SetPictureOrderForCaptcha();
        }

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
        public bool CheckAttempts()
        {
            return (CaptchaPictureAttempts == 5);
        }
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
        public int GetScore()
        {
            return CaptchaPicturesScore;
        }
        public int GetAttempts()
        {
            return CaptchaPictureAttempts;
        }

        private void SetPictureOrderForCaptcha()
        {
            NumbersForCaptchaPictures = new Queue<int>();
            NumbersList = new List<int>();
            SetNumbersList();
            SetNumberForCaptchaPicturesQueue();
        }

        private void SetNumbersList()
        {
            for (int i = 0; i < CaptchaRotatingImageList.CaptchaViewImageList.Images.Count; i++)
                NumbersList.Add(i);
        }
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
    }
}
