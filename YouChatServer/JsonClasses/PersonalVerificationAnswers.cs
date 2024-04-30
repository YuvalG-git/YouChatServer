using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "PersonalVerificationAnswers" class represents answers to personal verification questions.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing answers to three personal verification questions.
    /// </remarks>
    internal class PersonalVerificationAnswers
    {
        #region Private Fields

        /// <summary>
        /// The string "_questionNumber1" stores the security question for the first question.
        /// </summary>
        private string _questionNumber1;

        /// <summary>
        /// The string "_questionNumber2" stores the security question for the second question.
        /// </summary>
        private string _questionNumber2;

        /// <summary>
        /// The string "_questionNumber3" stores the security question for the third question.
        /// </summary>
        private string _questionNumber3;

        /// <summary>
        /// The string "_answerNumber1" stores the answer to the first security question.
        /// </summary>
        private string _answerNumber1;

        /// <summary>
        /// The string "_answerNumber2" stores the answer to the second security question.
        /// </summary>
        private string _answerNumber2;

        /// <summary>
        /// The string "_answerNumber3" stores the answer to the third security question.
        /// </summary>
        private string _answerNumber3;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PersonalVerificationAnswers" constructor initializes a new instance of the <see cref="PersonalVerificationAnswers"/> class with the specified answers to personal verification questions.
        /// </summary>
        /// <param name="questionNumber1">The answer to the first personal verification question.</param>
        /// <param name="questionNumber2">The answer to the second personal verification question.</param>
        /// <param name="questionNumber3">The answer to the third personal verification question.</param>
        /// <param name="answerNumber1">The answer to the first personal verification question.</param>
        /// <param name="answerNumber2">The answer to the second personal verification question.</param>
        /// <param name="answerNumber3">The answer to the third personal verification question.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PersonalVerificationAnswers class, which represents answers to personal verification questions.
        /// It initializes the answers to three personal verification questions.
        /// </remarks>
        public PersonalVerificationAnswers(string questionNumber1, string questionNumber2, string questionNumber3, string answerNumber1, string answerNumber2, string answerNumber3)
        {
            _questionNumber1 = questionNumber1;
            _questionNumber2 = questionNumber2;
            _questionNumber3 = questionNumber3;
            _answerNumber1 = answerNumber1;
            _answerNumber2 = answerNumber2;
            _answerNumber3 = answerNumber3;
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
        /// The "AnswerNumber1" property represents the answer to the first security question.
        /// It gets or sets the answer to the first security question.
        /// </summary>
        /// <value>
        /// The answer to the first security question.
        /// </value>
        public string AnswerNumber1
        {
            get
            {
                return _answerNumber1;
            }
            set
            {
                _answerNumber1 = value;
            }
        }

        /// <summary>
        /// The "AnswerNumber2" property represents the answer to the second security question.
        /// It gets or sets the answer to the second security question.
        /// </summary>
        /// <value>
        /// The answer to the second security question.
        /// </value>
        public string AnswerNumber2
        {
            get
            {
                return _answerNumber2;
            }
            set
            {
                _answerNumber2 = value;
            }
        }

        /// <summary>
        /// The "AnswerNumber3" property represents the answer to the third security question.
        /// It gets or sets the answer to the third security question.
        /// </summary>
        /// <value>
        /// The answer to the third security question.
        /// </value>
        public string AnswerNumber3
        {
            get
            {
                return _answerNumber3;
            }
            set
            {
                _answerNumber3 = value;
            }
        }

        #endregion
    }
}
