using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "CaptchaRotationImageDetails" class represents the details of captcha rotation images and success rate.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the captcha rotation images and captcha rotation success rate.
    /// </remarks>
    internal class CaptchaRotationImageDetails
    {
        #region Private Fields

        /// <summary>
        /// The CaptchaRotationImages object "_captchaRotationImages" represents the images used for captcha rotation.
        /// </summary>
        private CaptchaRotationImages _captchaRotationImages;

        /// <summary>
        /// The CaptchaRotationSuccessRate object "_captchaRotationSuccessRate" represents the success rate of captcha rotation.
        /// </summary>
        private CaptchaRotationSuccessRate _captchaRotationSuccessRate;

        #endregion

        #region Constructors

        /// <summary>
        /// The "CaptchaRotationImageDetails" constructor initializes a new instance of the <see cref="CaptchaRotationImageDetails"/> class with the specified captcha rotation images and captcha rotation success rate.
        /// </summary>
        /// <param name="captchaRotationImages">The captcha rotation images.</param>
        /// <param name="captchaRotationSuccessRate">The success rate of the captcha rotation.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the CaptchaRotationImageDetails class, which represents the details of captcha rotation images and success rate.
        /// It initializes the captcha rotation images and captcha rotation success rate.
        /// </remarks>
        public CaptchaRotationImageDetails(CaptchaRotationImages captchaRotationImages, CaptchaRotationSuccessRate captchaRotationSuccessRate)
        {
            _captchaRotationImages = captchaRotationImages;
            _captchaRotationSuccessRate = captchaRotationSuccessRate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "CaptchaRotationImages" property represents the images used for captcha rotation.
        /// It gets or sets the images used for captcha rotation.
        /// </summary>
        /// <value>
        /// The images used for captcha rotation.
        /// </value>
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
