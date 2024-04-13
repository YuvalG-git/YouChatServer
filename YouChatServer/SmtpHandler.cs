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
    internal class SmtpHandler
    {


        //three cases:
        // 1- needs to approve the gmail address exists on registeration
        // 2 - will get the email from the username in case the password is write and will send code
        // 3 - will send code the gmail in order to restrart the password

        //will be better if i make a control for smtp?
        private SmtpClient SmtpClient;
        private readonly string Server = "smtp.gmail.com";
        private readonly int Port = 587;


        private string SmtpMessageContent;
        private string SmtpCode;
        private readonly string YouChatSmtpPassword = "fmgwqaquwfmckchv";
        private readonly string YouChatSmtpSourceEmail = "youchatcyberapplication@gmail.com";

        private readonly string RegistrationMessagePart1 = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome to YouChat!<br>We are glad you chose to join our platform</p>";
        private readonly string RegistrationMessagePart2 = "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code:</p>";
        private readonly string LoginMessagePart1 = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome back to YouChat!</p>";
        private readonly string LoginMessagePart2 = "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code:</p>";
        private readonly string PasswordRenewalMessagePart1 = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome back to YouChat!</p>";
        private readonly string PasswordRenewalMessagePart2 = "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code for renewing your password:</p>";
        private readonly string FriendRequestMessage = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Check YouChat!</p>";

        private string[] EmailBodyContent;
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

        public string GetSmtpCode()
        {
            return SmtpCode;
        }
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

                    // Handle the exception or log the error
                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Exception: " + ex.Message);
                    // Handle the exception or log the error
                }
            }
        }
        private string BuildEmailBody(string UsernameId, string content)
        {
            string body = "<html><body>";
            body += "<p style='color: #006400; font-size: 16px; direction: ltr;'> Hey " + UsernameId + "!</p>";
            body += content;
            body += "</body></html>";
            return body;
        }

        public void SendFriendRequestAlertToUserEmail(string UsernameId, string friendId, string DestinationEmail)
        {
            string subject = "YouChat Friend Request";
            string content = FriendRequestMessage + "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>A friend request has been sent to you from " + friendId + "!</p>";
            string body = BuildEmailBody(UsernameId, content);
            SendEmail(DestinationEmail, subject, body);
        }

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

        private string CreateCodeForSMTP()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s =>
            s[random.Next(s.Length)]).ToArray());
        }
    }
}
