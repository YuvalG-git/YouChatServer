using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "RegistrationInformation" class represents user registration information.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username, password, first name, last name, email address, city name, gender, date of birth, registration date, and verification questions and answers for the user.
    /// </remarks>
    internal class RegistrationInformation
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_password" stores the password of the user.
        /// </summary>
        private string _password;

        /// <summary>
        /// The string "_firstName" stores the first name of the user.
        /// </summary>
        private string _firstName;

        /// <summary>
        /// The string "_lastName" stores the last name of the user.
        /// </summary>
        private string _lastName;

        /// <summary>
        /// The string "_emailAddress" stores the email address of the user.
        /// </summary>
        private string _emailAddress;

        /// <summary>
        /// The string "_cityName" stores the city name of the user.
        /// </summary>
        private string _cityName;

        /// <summary>
        /// The string "_gender" stores the gender of the user.
        /// </summary>
        private string _gender;

        /// <summary>
        /// The DateTime "_dateOfBirth" stores the date of birth of the user.
        /// </summary>
        private DateTime _dateOfBirth;

        /// <summary>
        /// The DateTime "_registrationDate" stores the registration date of the user.
        /// </summary>
        private DateTime _registrationDate;

        /// <summary>
        /// The List "_verificationQuestionsAndAnswers" stores the verification questions and answers for the user.
        /// </summary>
        private List<string[]> _verificationQuestionsAndAnswers;

        #endregion

        #region Constructors

        /// <summary>
        /// The "RegistrationInformation" constructor initializes a new instance of the <see cref="RegistrationInformation"/> class with the specified registration information.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="firstName">The first name of the user.</param>
        /// <param name="lastName">The last name of the user.</param>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <param name="cityName">The city name of the user.</param>
        /// <param name="gender">The gender of the user.</param>
        /// <param name="dateOfBirth">The date of birth of the user.</param>
        /// <param name="registrationDate">The registration date of the user.</param>
        /// <param name="verificationQuestionsAndAnswers">The list of verification questions and answers for the user.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the RegistrationInformation class, which represents user registration information.
        /// It initializes the username, password, first name, last name, email address, city name, gender, date of birth, registration date, and verification questions and answers for the user.
        /// </remarks>
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

        #endregion

        #region Properties

        /// <summary>
        /// The "Username" property represents the username of a user.
        /// It gets or sets the username of the user.
        /// </summary>
        /// <value>
        /// The username of the user.
        /// </value>
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

        /// <summary>
        /// The "Password" property represents the password of a user.
        /// It gets or sets the password of the user.
        /// </summary>
        /// <value>
        /// The password of the user.
        /// </value>
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

        /// <summary>
        /// The "FirstName" property represents the first name of a user.
        /// It gets or sets the first name of the user.
        /// </summary>
        /// <value>
        /// The first name of the user.
        /// </value>
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

        /// <summary>
        /// The "LastName" property represents the last name of a user.
        /// It gets or sets the last name of the user.
        /// </summary>
        /// <value>
        /// The last name of the user.
        /// </value>
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

        /// <summary>
        /// The "EmailAddress" property represents the email address of a user.
        /// It gets or sets the email address of the user.
        /// </summary>
        /// <value>
        /// The email address of the user.
        /// </value>
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

        /// <summary>
        /// The "CityName" property represents the name of a city.
        /// It gets or sets the name of the city.
        /// </summary>
        /// <value>
        /// The name of the city.
        /// </value>
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

        /// <summary>
        /// The "Gender" property represents the gender of a user.
        /// It gets or sets the gender of the user.
        /// </summary>
        /// <value>
        /// The gender of the user.
        /// </value>
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

        /// <summary>
        /// The "DateOfBirth" property represents the date of birth of a user.
        /// It gets or sets the date of birth of the user.
        /// </summary>
        /// <value>
        /// The date of birth of the user.
        /// </value>
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

        /// <summary>
        /// The "RegistrationDate" property represents the registration date of a user.
        /// It gets or sets the registration date of the user.
        /// </summary>
        /// <value>
        /// The registration date of the user.
        /// </value>
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

        /// <summary>
        /// The "VerificationQuestionsAndAnswers" property represents a list of verification questions and their corresponding answers.
        /// It gets or sets the list of verification questions and answers.
        /// </summary>
        /// <value>
        /// A list of string arrays where each array contains a verification question at index 0 and its corresponding answer at index 1.
        /// </value>
        public List<string[]> VerificationQuestionsAndAnswers
        {
            get
            {
                return _verificationQuestionsAndAnswers;
            }
            set
            {
                _verificationQuestionsAndAnswers = value;
            }
        }

        #endregion
    }
}
