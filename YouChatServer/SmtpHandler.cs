using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace YouChatApp
{
    internal class SmtpHandler
    {

        //to send a email on friend request and more...
        const int RegistrationMessage = 1;
        const int LoginMessage = 2;
        const int PasswordRenewalMessage = 3;


        //three cases:
        // 1- needs to approve the gmail address exists on registeration
        // 2 - will get the email from the username in case the password is write and will send code
        // 3 - will send code the gmail in order to restrart the password

        //will be better if i make a control for smtp?
        private SmtpClient SmtpClient;
        private readonly string Server = "smtp.gmail.com";
        private readonly int Port = 587;


        public string SmtpMessageContent;
        public string SmtpCode;
        private readonly string YouChatSmtpPassword = "fmgwqaquwfmckchv";
        private readonly string YouChatSmtpSourceEmail = "youchatcyberapplication@gmail.com";

        private static string[] EmailBodyContent;
        public SmtpHandler()
        {
            SmtpClient = new SmtpClient(Server, Port);
            SmtpClient.UseDefaultCredentials = false;
            SmtpClient.EnableSsl = true;
            SmtpClient.Credentials = new NetworkCredential(YouChatSmtpSourceEmail, YouChatSmtpPassword);
            EmailBodyContent = new string[3];
            EmailBodyContent[0] = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome to YouChat!<br>We are glad you chose to join our platform</p>";
            EmailBodyContent[0] += "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code:</p>";
            EmailBodyContent[1] = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome back to YouChat!</p>";
            EmailBodyContent[1] += "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code:</p>";
            EmailBodyContent[2] = "<p style='color: #008080; font-size: 16px; direction: ltr;'>Welcome back to YouChat!</p>";
            EmailBodyContent[2] += "<p style='color: black; font-size: 16px; font-weight: bold; direction: ltr;'>Here is your code for renewing your password:</p>";

        }

        public string GetSmtpCode()
        {
            return SmtpCode;
        }

        public void SendCodeToUserEmail(string UsernameId, string DestinationEmail, int CaseNumber)
        {

            SmtpCode = CreateCodeForSMTP();
            string subject = "YouChat Verification Code"; //todo change it as well
            //string body = EmailBodyContent[CaseNumber-1] + SmtpCode; //todo need to change it due to evert one of the three cases mentioned above...
            string body = "<html><body>";
            body += "<p style='color: #006400; font-size: 16px; direction: ltr;'> Hey " + UsernameId + "!</p>";
            body += EmailBodyContent[CaseNumber - 1];
            body += "<p style='color: black; font-size: 20px; font-weight: bold; direction: ltr;'>" + SmtpCode + "</p>";
            body += "</body></html>";
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

        private static string CreateCodeForSMTP()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s =>
            s[random.Next(s.Length)]).ToArray());
        }
    }
}
