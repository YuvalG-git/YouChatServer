using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "CaptchaRotationSuccessRate" class represents the success rate of a captcha rotation.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the score and number of attempts for a captcha rotation.
    /// </remarks>
    internal class CaptchaRotationSuccessRate
    {
        #region Private Fields

        /// <summary>
        /// The integer "_score" represents the user's score.
        /// </summary>
        private int _score;

        /// <summary>
        /// The integer "_attempts" represents the number of attempts made.
        /// </summary>
        private int _attempts;

        #endregion

        #region Constructors

        /// <summary>
        /// The "CaptchaRotationSuccessRate" constructor initializes a new instance of the <see cref="CaptchaRotationSuccessRate"/> class with the specified score and attempts.
        /// </summary>
        /// <param name="score">The score of the captcha rotation.</param>
        /// <param name="attempts">The number of attempts for the captcha rotation.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the CaptchaRotationSuccessRate class, which represents the success rate of a captcha rotation.
        /// It initializes the score and attempts for the captcha rotation.
        /// </remarks>
        public CaptchaRotationSuccessRate(int score, int attempts)
        {
            _score = score;
            _attempts = attempts;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Score" property represents a numeric score value.
        /// It gets or sets the numeric score value.
        /// </summary>
        /// <value>
        /// The numeric score value.
        /// </value>
        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
            }
        }

        /// <summary>
        /// The "Attempts" property represents the number of attempts.
        /// It gets or sets the number of attempts.
        /// </summary>
        /// <value>
        /// The number of attempts.
        /// </value>
        public int Attempts
        {
            get
            {
                return _attempts;
            }
            set
            {
                _attempts = value;
            }
        }

        #endregion
    }
}
