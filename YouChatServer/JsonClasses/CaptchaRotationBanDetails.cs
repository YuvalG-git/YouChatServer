using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class CaptchaRotationBanDetails
    {
        private double _banDuration;
        private CaptchaRotationSuccessRate _captchaRotationSuccessRate;

        public CaptchaRotationBanDetails(double banDuration, CaptchaRotationSuccessRate captchaRotationSuccessRate)
        {
            _banDuration = banDuration;
            _captchaRotationSuccessRate = captchaRotationSuccessRate;
        }
        public double BanDuration
        {
            get
            {
                return _banDuration;
            }
            set
            {
                _banDuration = value;
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
