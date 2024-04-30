using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "PersonalVerificationQuestionDetails" class represents personal verification question details.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing personal verification questions and captcha rotation success rate.
    /// </remarks>
    internal class PersonalVerificationQuestionDetails
    {
        #region Private Fields

        /// <summary>
        /// The _personalVerificationQuestions object of type PersonalVerificationQuestions stores the personal verification questions for the user.
        /// </summary>
        private PersonalVerificationQuestions _personalVerificationQuestions;

        /// <summary>
        /// The _captchaRotationSuccessRate object of type CaptchaRotationSuccessRate stores the success rate of rotating CAPTCHA challenges.
        /// </summary>
        private CaptchaRotationSuccessRate _captchaRotationSuccessRate;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PersonalVerificationQuestionDetails" constructor initializes a new instance of the <see cref="PersonalVerificationQuestionDetails"/> class with the specified personal verification questions and captcha rotation success rate.
        /// </summary>
        /// <param name="personalVerificationQuestions">The personal verification questions.</param>
        /// <param name="captchaRotationSuccessRate">The captcha rotation success rate.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PersonalVerificationQuestionDetails class, which represents personal verification question details.
        /// It initializes the personal verification questions and captcha rotation success rate.
        /// </remarks>
        public PersonalVerificationQuestionDetails(PersonalVerificationQuestions personalVerificationQuestions, CaptchaRotationSuccessRate captchaRotationSuccessRate)
        {
            _personalVerificationQuestions = personalVerificationQuestions;
            _captchaRotationSuccessRate = captchaRotationSuccessRate;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "PersonalVerificationQuestions" property represents the personal verification questions of a user.
        /// It gets or sets the personal verification questions of the user.
        /// </summary>
        /// <value>
        /// The personal verification questions of the user.
        /// </value>
        public PersonalVerificationQuestions PersonalVerificationQuestions
        {
            get
            {
                return _personalVerificationQuestions;
            }
            set
            {
                _personalVerificationQuestions = value;
            }
        }

        /// <summary>
        /// The "CaptchaRotationSuccessRate" property represents the success rate of a captcha rotation.
        /// It gets or sets the success rate of the captcha rotation.
        /// </summary>
        /// <value>
        /// The success rate of the captcha rotation.
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
