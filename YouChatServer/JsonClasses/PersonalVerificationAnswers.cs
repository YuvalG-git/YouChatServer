using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class PersonalVerificationAnswers
    {
        private string _questionNumber1;
        private string _questionNumber2;
        private string _questionNumber3;
        private string _answerNumber1;
        private string _answerNumber2;
        private string _answerNumber3;

        public PersonalVerificationAnswers(string questionNumber1, string questionNumber2, string questionNumber3, string answerNumber1, string answerNumber2, string answerNumber3)
        {
            _questionNumber1 = questionNumber1;
            _questionNumber2 = questionNumber2;
            _questionNumber3 = questionNumber3;
            _answerNumber1 = answerNumber1;
            _answerNumber2 = answerNumber2;
            _answerNumber3 = answerNumber3;
        }
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

    }
}
