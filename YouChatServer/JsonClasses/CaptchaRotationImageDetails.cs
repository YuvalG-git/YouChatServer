using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class CaptchaRotationImageDetails
    {
        private CaptchaRotationImages _captchaRotationImages;
        private CaptchaRotationSuccessRate _captchaRotationSuccessRate;

        public CaptchaRotationImageDetails(CaptchaRotationImages captchaRotationImages, CaptchaRotationSuccessRate captchaRotationSuccessRate)
        {
            _captchaRotationImages = captchaRotationImages;
            _captchaRotationSuccessRate = captchaRotationSuccessRate;
        }
        public CaptchaRotationImages CaptchaRotationImages
        {
            get
            {
                return _captchaRotationImages;
            }
            set
            {
                _captchaRotationImages = value;
            }
        }
        public CaptchaRotationSuccessRate CaptchaRotationSuccessRate
        {
            get
            {
                return _captchaRotationSuccessRate;
            }
            set
            {
                _captchaRotationSuccessRate = value;
            }
        }
    }
}
