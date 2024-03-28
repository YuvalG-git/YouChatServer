using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class RegistrationInformation
    {
        private string _username;
        private string _password;
        private string _firstName;
        private string _lastName;
        private string _emailAddress;
        private string _cityName;
        private string _gender;
        private DateTime _dateOfBirth;
        private DateTime _registrationDate;
        private List<string[]> _verificationQuestionsAndAnswers;


        public RegistrationInformation(string username, string password, string firstName, string lastName, string emailAddress, string cityName, string gender, DateTime dateOfBirth, DateTime registrationDate, List<string[]> verificationQuestionsAndAnswers)
        {
            _username = username;
            _password = password;
            _firstName = firstName;
            _lastName = lastName;
            _emailAddress = emailAddress;
            _cityName = cityName;
            _gender = gender;
            _dateOfBirth = dateOfBirth;
            _registrationDate = registrationDate;
            _verificationQuestionsAndAnswers = verificationQuestionsAndAnswers;
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set 
            { 
                _username = value; 
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }
        public string EmailAddress
        {
            get
            {
                return _emailAddress;
            }
            set
            {
                _emailAddress = value;
            }
        }
        public string CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                _cityName = value;
            }
        }
        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                _gender = value;
            }
        }
        public DateTime DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                _dateOfBirth = value;
            }
        }
        public DateTime RegistrationDate
        {
            get
            {
                return _registrationDate;
            }
            set
            {
                _registrationDate = value;
            }
        }
        public List<string[]> VerificationQuestionsAndAnswers
        {
            get
            {
                return _verificationQuestionsAndAnswers;
            }
            set //maybe i need a different get set for this because of it is complicated
            {
                _verificationQuestionsAndAnswers = value;
            }
        }

    }
}
