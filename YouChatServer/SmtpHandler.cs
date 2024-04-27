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
        /// <summary>
        /// The SmtpClient object represents an SMTP client used for sending emails.
        /// </summary>
        private SmtpClient SmtpClient;

        /// <summary>
        /// The string object "Server" represents the SMTP server address.
        /// </summary>
        private readonly string Server = "smtp.gmail.com";

        /// <summary>
        /// The int object "Port" represents the SMTP server port number.
        /// </summary>
        private readonly int Port = 587;

        /// <summary>
        /// The string object "SmtpCode" stores the current SMTP verification code.
        /// </summary>
        private string SmtpCode;

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

        /// <summary>
        /// The array of strings "EmailBodyContent" stores different email body content.
        /// </summary>
        private string[] EmailBodyContent;

        /// <summary>
        /// The method initializes a new instance of the <see cref="SmtpHandler"/> class with SMTP server settings and email message parts.
        /// </summary>
        public SmtpHandler()
        {
            SmtpClient = new SmtpClient(Server, Port);
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.EnableSsl = true;
            SmtpClient.Credentials = new NetworkCredential(YouChatSmtpSourceEmail, YouChatSmtpPassword);
            EmailBodyContent = new string[3];
            EmailBodyContent[0] = RegistrationMessagePart1 + RegistrationMessagePart2;
            EmailBodyContent[1] = LoginMessagePart1 + LoginMessagePart2;
            EmailBodyContent[2] = PasswordRenewalMessagePart1 + PasswordRenewalMessagePart2;
        }


        /// <summary>
        /// The "SendEmail" method sends an email using SMTP to the specified destination email address.
        /// </summary>
        /// <param name="DestinationEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        private void SendEmail(string DestinationEmail, string subject, string body)
        {
            using (MailMessage mail = new MailMessage(YouChatSmtpSourceEmail, DestinationEmail))
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                try
                {
                    SmtpClient.Send(mail);
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

        /// <summary>
        /// The "BuildEmailBody" method builds the HTML email body with the specified username and content.
        /// </summary>
        /// <param name="UsernameId">The username to include in the email body.</param>
        /// <param name="content">The content to include in the email body.</param>
        /// <returns>The complete HTML email body.</returns>
        private string BuildEmailBody(string UsernameId, string content)
        {
            string body = "<html><body>";
            body += "<p style='color: #006400; font-size: 16px; direction: ltr;'> Hey " + UsernameId + "!</p>";
            body += content;
            body += "</body></html>";
            return body;
        }

        /// <summary>
        /// The "SendFriendRequestAlertToUserEmail" method sends a friend request alert email to the specified user's email address.
        /// </summary>
        /// <param name="UsernameId">The username of the recipient.</param>
        /// <param name="friendId">The ID of the friend who sent the request.</param>
        /// <param name="DestinationEmail">The email address of the recipient.</param>
        public void SendFriendRequestAlertToUserEmail(string UsernameId, string friendId, string DestinationEmail)
        {
            string subject = "YouChat Friend Request";
            string content = FriendRequestMessage + "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>A friend request has been sent to you from " + friendId + "!</p>";
            string body = BuildEmailBody(UsernameId, content);
            SendEmail(DestinationEmail, subject, body);
        }

        /// <summary>
        /// The "SendCodeToUserEmail" method sends a verification code email to the user's email address based on the message type.
        /// </summary>
        /// <param name="UsernameId">The username of the recipient.</param>
        /// <param name="DestinationEmail">The email address of the recipient.</param>
        /// <param name="smtpMessageType_Enum">The type of SMTP message to send.</param>
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
        /// The "CreateCodeForSMTP" method creates a random 6-character code for SMTP verification.
        /// </summary>
        /// <returns>A randomly generated code.</returns>
        private string CreateCodeForSMTP()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s =>
            s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// The "GetSmtpCode" method retrieves the current SMTP verification code.
        /// </summary>
        /// <returns>The SMTP verification code as a string.</returns>
        public string GetSmtpCode()
        {
            return SmtpCode;
        }
    }
}
