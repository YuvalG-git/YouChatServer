using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using YouChatServer.JsonClasses;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace YouChatServer
{
    /// <summary>
    /// The "SmtpHandler" class handles SMTP operations for sending emails.
    /// </summary>
    public class SmtpHandler
    {
        #region Private Fields

        /// <summary>
        /// The SmtpClient object "_smtpClient" represents an SMTP client used for sending emails.
        /// </summary>
        private SmtpClient _smtpClient;

        /// <summary>
        /// The string object "SmtpCode" stores the current SMTP verification code.
        /// </summary>
        private string SmtpCode;

        /// <summary>
        /// The array of strings "EmailBodyContent" stores different email body content.
        /// </summary>
        private string[] EmailBodyContent;

        #endregion

        #region Private Readonly Fields

        /// <summary>
        /// The string object "Server" represents the SMTP server address.
        /// </summary>
        private readonly string Server = "smtp.gmail.com";

        /// <summary>
        /// The int object "Port" represents the SMTP server port number.
        /// </summary>
        private readonly int Port = 587;

        /// <summary>
        /// The string object "YouChatSmtpPassword" represents the password for the YouChat SMTP account.
        /// </summary>
        private readonly string YouChatSmtpPassword = "fmgwqaquwfmckchv";

        /// <summary>
        /// The string object "YouChatSmtpSourceEmail" represents the email address for the YouChat SMTP account.
        /// </summary>
        private readonly string YouChatSmtpSourceEmail = "youchatcyberapplication@gmail.com";

        /// <summary>
        /// The string object "RegistrationMessagePart1" represents part 1 of the registration email message.
        /// </summary>
        private readonly string RegistrationMessagePart1 = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome to YouChat!<br>We are glad you chose to join our platform</p>";
        
        /// <summary>
        /// The string object "RegistrationMessagePart2" represents part 2 of the registration email message.
        /// </summary>
        private readonly string RegistrationMessagePart2 = "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code:</p>";

        /// <summary>
        /// The string object "LoginMessagePart1" represents part 1 of the login email message.
        /// </summary>
        private readonly string LoginMessagePart1 = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome back to YouChat!</p>";
        
        /// <summary>
        /// The string object "LoginMessagePart2" represents part 2 of the login email message.
        /// </summary>
        private readonly string LoginMessagePart2 = "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code:</p>";

        /// <summary>
        /// The string object "PasswordRenewalMessagePart1" represents part 1 of the password renewal email message.
        /// </summary>
        private readonly string PasswordRenewalMessagePart1 = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome back to YouChat!</p>";

        /// <summary>
        /// The string object "PasswordRenewalMessagePart2" represents part 2 of the password renewal email message.
        /// </summary>
        private readonly string PasswordRenewalMessagePart2 = "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code for renewing your password:</p>";

        /// <summary>
        /// The string object "FriendRequestMessage" represents the HTML content of the friend request email message.
        /// </summary>
        private readonly string FriendRequestMessage = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Check YouChat!</p>";

        #endregion

        #region Constructors

        /// <summary>
        /// The "SmtpHandler" constructor initializes a new instance of the <see cref="SmtpHandler"/> class with SMTP server settings and email message parts.
        /// </summary>
        /// <remarks>
        /// The constructor sets up the SMTP client with the server, port, and credentials.
        /// It also initializes the <see cref="EmailBodyContent"/> array with three predefined email message parts for registration, login, and password renewal.
        /// </remarks>
        public SmtpHandler()
        {
            _smtpClient = new SmtpClient(Server, Port);
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.EnableSsl = true;
            _smtpClient.Credentials = new NetworkCredential(YouChatSmtpSourceEmail, YouChatSmtpPassword);
            EmailBodyContent = new string[3];
            EmailBodyContent[0] = RegistrationMessagePart1 + RegistrationMessagePart2;
            EmailBodyContent[1] = LoginMessagePart1 + LoginMessagePart2;
            EmailBodyContent[2] = PasswordRenewalMessagePart1 + PasswordRenewalMessagePart2;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The "SendEmail" method sends an email using SMTP to the specified destination email address.
        /// </summary>
        /// <param name="destinationEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <remarks>
        /// This method sends an email message to the specified recipient using the SMTP client configured in the application. 
        /// It sets the email's subject, body, and format (HTML or plain text) based on the provided parameters. 
        /// If the email fails to send due to an SMTP exception or another general exception, 
        /// the method writes an error message to the console.
        /// </remarks>
        private void SendEmail(string DestinationEmail, string subject, string body)
        {
            using (MailMessage mail = new MailMessage(YouChatSmtpSourceEmail, DestinationEmail))
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                try
                {
                    _smtpClient.Send(mail);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (SmtpException smtpEx)
                {
                    Console.WriteLine("SMTP Exception: " + smtpEx.Message);

                    if (smtpEx.InnerException != null)
                    {
                        Console.WriteLine("Inner Exception: " + smtpEx.InnerException.Message);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Exception: " + ex.Message);
                }
            }
        }

        /// The "BuildEmailBody" method builds the body of an HTML email message with the specified username and content.
        /// </summary>
        /// <param name="UsernameId">The username or recipient's name to include in the greeting.</param>
        /// <param name="content">The main content of the email message.</param>
        /// <returns>The HTML body of the email message, including the greeting and content.</returns>
        /// <remarks>
        /// This method constructs the body of an HTML email message with a personalized greeting using the provided username or recipient's name.
        /// The method creates a basic HTML structure with a paragraph element containing the greeting and appends the provided content to it.
        /// The resulting HTML body can be used as the body of an email message.
        /// </remarks>
        private string BuildEmailBody(string UsernameId, string content)
        {
            string body = "<html><body>";
            body += "<p style='color: #006400; font-size: 16px; direction: ltr;'> Hey " + UsernameId + "!</p>";
            body += content;
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// The "CreateCodeForSMTP" method generates a random alphanumeric code of length 6 for use in SMTP messages.
        /// </summary>
        /// <returns>A randomly generated alphanumeric code.</returns>
        /// <remarks>
        /// This method creates a string of uppercase and lowercase letters along with digits (0-9).
        /// It then selects random characters from this string to construct a 6-character alphanumeric code.
        /// The generated code is suitable for use in verification or renewal emails sent via SMTP.
        /// </remarks>
        private string CreateCodeForSMTP()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s =>
            s[random.Next(s.Length)]).ToArray());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "SendFriendRequestAlertToUserEmail" method sends a friend request alert email to the specified user's email address.
        /// </summary>
        /// <param name="UsernameId">The username or recipient's name.</param>
        /// <param name="friendId">The ID of the user sending the friend request.</param>
        /// <param name="DestinationEmail">The email address of the recipient.</param>
        /// <remarks>
        /// This method constructs an email message with a friend request alert and sends it to the specified user's email address.
        /// The email message includes a predefined subject and a personalized message indicating that a friend request has been sent.
        /// </remarks>
        public void SendFriendRequestAlertToUserEmail(string UsernameId, string friendId, string DestinationEmail)
        {
            string subject = "YouChat Friend Request";
            string content = FriendRequestMessage + "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>A friend request has been sent to you from " + friendId + "!</p>";
            string body = BuildEmailBody(UsernameId, content);
            SendEmail(DestinationEmail, subject, body);
        }

        /// <summary>
        /// The "SendCodeToUserEmail" method sends a verification or renewal code email to the specified user's email address.
        /// </summary>
        /// <param name="UsernameId">The username or recipient's name.</param>
        /// <param name="DestinationEmail">The email address of the recipient.</param>
        /// <param name="smtpMessageType_Enum">The type of SMTP message to send (registration, login, or password renewal).</param>
        /// <remarks>
        /// This method generates a verification or renewal code, determines the email subject based on the SMTP message type,
        /// constructs the email content using predefined email body parts and the generated code, and sends the email to the specified user's email address.
        /// </remarks>
        public void SendCodeToUserEmail(string UsernameId, string DestinationEmail, EnumHandler.SmtpMessageType_Enum smtpMessageType_Enum)
        {
            SmtpCode = CreateCodeForSMTP();
            string subject = "";
            switch (smtpMessageType_Enum)
            {
                case EnumHandler.SmtpMessageType_Enum.RegistrationMessage:
                case EnumHandler.SmtpMessageType_Enum.LoginMessage:
                    subject = "YouChat Verification Code";
                    break;
                case EnumHandler.SmtpMessageType_Enum.PasswordRenewalMessage:
                    subject = "YouChat Password Renewal Code";
                    break;
            }
            int enumValue = ((int)smtpMessageType_Enum);
            string content = EmailBodyContent[enumValue];
            content += "<p style='color: black; font-size: 20px; font-weight: bold; direction: ltr;'>" + SmtpCode + "</p>";
            string body = BuildEmailBody(UsernameId, content);
            SendEmail(DestinationEmail, subject, body);
        }

        /// <summary>
        /// The "GetSmtpCode" method retrieves the currently stored SMTP code.
        /// </summary>
        /// <returns>The currently stored SMTP code.</returns>
        /// <remarks>
        /// This method returns the SMTP code that was previously generated by the application.
        /// The SMTP code is used for verification or renewal purposes in SMTP messages.
        /// </remarks>
        public string GetSmtpCode()
        {
            return SmtpCode;
        }

        #endregion
    }
}
