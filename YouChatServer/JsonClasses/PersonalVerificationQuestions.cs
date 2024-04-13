using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class PersonalVerificationQuestions
    {
        private string _questionNumber1;
        private string _questionNumber2;
        private string _questionNumber3;
        private string _questionNumber4;
        private string _questionNumber5;

        public PersonalVerificationQuestions(string questionNumber1, string questionNumber2, string questionNumber3, string questionNumber4, string questionNumber5)
        {
            _questionNumber1 = questionNumber1;
            _questionNumber2 = questionNumber2;
            _questionNumber3 = questionNumber3;
            _questionNumber4 = questionNumber4;
            _questionNumber5 = questionNumber5;

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
    }
}
