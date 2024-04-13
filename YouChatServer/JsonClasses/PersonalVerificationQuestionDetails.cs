using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class PersonalVerificationQuestionDetails
    {
        private PersonalVerificationQuestions _personalVerificationQuestions;
        private CaptchaRotationSuccessRate _captchaRotationSuccessRate;

        public PersonalVerificationQuestionDetails(PersonalVerificationQuestions personalVerificationQuestions, CaptchaRotationSuccessRate captchaRotationSuccessRate)
        {
            _personalVerificationQuestions = personalVerificationQuestions;
            _captchaRotationSuccessRate = captchaRotationSuccessRate;
        }
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
