using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "CaptchaRotationBanDetails" class represents the details of a ban due to failed captcha rotation attempts.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the ban duration and captcha rotation success rate.
    /// </remarks>
    internal class CaptchaRotationBanDetails
    {
        #region Private Fields

        /// <summary>
        /// The double "_banDuration" represents the duration of a ban in minutes.
        /// </summary>
        private double _banDuration;

        /// <summary>
        /// The CaptchaRotationSuccessRate object "_captchaRotationSuccessRate" represents the success rate of captcha rotation.
        /// </summary>
        private CaptchaRotationSuccessRate _captchaRotationSuccessRate;

        #endregion

        #region Constructors

        /// <summary>
        /// The "CaptchaRotationBanDetails" constructor initializes a new instance of the <see cref="CaptchaRotationBanDetails"/> class with the specified ban duration and captcha rotation success rate.
        /// </summary>
        /// <param name="banDuration">The duration of the ban in minutes.</param>
        /// <param name="captchaRotationSuccessRate">The success rate of the captcha rotation.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the CaptchaRotationBanDetails class, which represents the details of a ban due to failed captcha rotation attempts.
        /// It initializes the ban duration and captcha rotation success rate.
        /// </remarks>
        public CaptchaRotationBanDetails(double banDuration, CaptchaRotationSuccessRate captchaRotationSuccessRate)
        {
            _banDuration = banDuration;
            _captchaRotationSuccessRate = captchaRotationSuccessRate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "BanDuration" property represents the duration of a ban in seconds.
        /// It gets or sets the duration of the ban in seconds.
        /// </summary>
        /// <value>
        /// The duration of the ban in seconds.
        /// </value>
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

        /// <summary>
        /// The "CaptchaRotationSuccessRate" property represents the success rate of captcha rotation.
        /// It gets or sets the success rate of captcha rotation.
        /// </summary>
        /// <value>
        /// The success rate of captcha rotation.
        /// </value>
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

        #endregion
    }
}
