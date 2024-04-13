using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class CaptchaRotationSuccessRate
    {
        private int _score;
        private int _attempts;

        public CaptchaRotationSuccessRate(int score, int attempts)
        {
            _score = score;
            _attempts = attempts;
        }

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
    }
}
