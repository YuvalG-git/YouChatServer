using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "PersonalVerificationQuestions" class represents personal verification questions.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing five personal verification questions.
    /// </remarks>
    internal class PersonalVerificationQuestions
    {
        #region Private Fields

        /// <summary>
        /// The string "_questionNumber1" stores the first security question.
        /// </summary>
        private string _questionNumber1;

        /// <summary>
        /// The string "_questionNumber2" stores the second security question.
        /// </summary>
        private string _questionNumber2;

        /// <summary>
        /// The string "_questionNumber3" stores the third security question.
        /// </summary>
        private string _questionNumber3;

        /// <summary>
        /// The string "_questionNumber4" stores the fourth security question.
        /// </summary>
        private string _questionNumber4;

        /// <summary>
        /// The string "_questionNumber5" stores the fifth security question.
        /// </summary>
        private string _questionNumber5;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PersonalVerificationQuestions" constructor initializes a new instance of the <see cref="PersonalVerificationQuestions"/> class with the specified personal verification questions.
        /// </summary>
        /// <param name="questionNumber1">The first personal verification question.</param>
        /// <param name="questionNumber2">The second personal verification question.</param>
        /// <param name="questionNumber3">The third personal verification question.</param>
        /// <param name="questionNumber4">The fourth personal verification question.</param>
        /// <param name="questionNumber5">The fifth personal verification question.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PersonalVerificationQuestions class, which represents personal verification questions.
        /// It initializes the five personal verification questions.
        /// </remarks>
        public PersonalVerificationQuestions(string questionNumber1, string questionNumber2, string questionNumber3, string questionNumber4, string questionNumber5)
        {
            _questionNumber1 = questionNumber1;
            _questionNumber2 = questionNumber2;
            _questionNumber3 = questionNumber3;
            _questionNumber4 = questionNumber4;
            _questionNumber5 = questionNumber5;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "QuestionNumber1" property represents the first security question.
        /// It gets or sets the first security question.
        /// </summary>
        /// <value>
        /// The first security question.
        /// </value>
        public string QuestionNumber1
        {
            get
            {
                return _questionNumber1;
            }
            set
            {
                _questionNumber1 = value;
            }
        }

        /// <summary>
        /// The "QuestionNumber2" property represents the second security question.
        /// It gets or sets the second security question.
        /// </summary>
        /// <value>
        /// The second security question.
        /// </value>
        public string QuestionNumber2
        {
            get
            {
                return _questionNumber2;
            }
            set
            {
                _questionNumber2 = value;
            }
        }

        /// <summary>
        /// The "QuestionNumber3" property represents the third security question.
        /// It gets or sets the third security question.
        /// </summary>
        /// <value>
        /// The third security question.
        /// </value>
        public string QuestionNumber3
        {
            get
            {
                return _questionNumber3;
            }
            set
            {
                _questionNumber3 = value;
            }
        }

        /// <summary>
        /// The "QuestionNumber4" property represents the fourth security question.
        /// It gets or sets the fourth security question.
        /// </summary>
        /// <value>
        /// The fourth security question.
        /// </value>
        public string QuestionNumber4
        {
            get
            {
                return _questionNumber4;
            }
            set
            {
                _questionNumber4 = value;
            }
        }

        /// <summary>
        /// The "QuestionNumber5" property represents the fifth security question.
        /// It gets or sets the fifth security question.
        /// </summary>
        /// <value>
        /// The fifth security question.
        /// </value>
        public string QuestionNumber5
        {
            get
            {
                return _questionNumber5;
            }
            set
            {
                _questionNumber5 = value;
            }
        }

        #endregion
    }
}
