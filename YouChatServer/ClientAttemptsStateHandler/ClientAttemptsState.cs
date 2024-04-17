using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using YouChatServer.JsonClasses;

namespace YouChatServer.ClientAttemptsStateHandler
{
    internal class ClientAttemptsState : ClientAttemptsStateBase
    {
        public int FailedAttempts { get; set; }
        private EnumHandler.UserAuthentication_Enum userAuthenticationState;
        public ClientAttemptsState(Client client, EnumHandler.UserAuthentication_Enum userAuthenticationState) : base(client)
        {
            FailedAttempts = 0;
            this.userAuthenticationState = userAuthenticationState;
        }
        public EnumHandler.UserAuthentication_Enum UserAuthenticationState
        {
            get { return userAuthenticationState; }
        }

        protected override void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.Timer_Tick(sender, e);
        }
        protected override void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object messageData)
        {
            base.SendMessage(GetMessageIdForAuthenticationState(userAuthenticationState), messageData);
        }

        private EnumHandler.CommunicationMessageID_Enum GetMessageIdForAuthenticationState(EnumHandler.UserAuthentication_Enum userAuthenticationState)
        {
            switch (userAuthenticationState)
            {
                case EnumHandler.UserAuthentication_Enum.Login:
                    return EnumHandler.CommunicationMessageID_Enum.LoginBanFinish;
                case EnumHandler.UserAuthentication_Enum.Registration:
                    return EnumHandler.CommunicationMessageID_Enum.RegistrationBanFinish;
                case EnumHandler.UserAuthentication_Enum.PasswordUpdate:
                    return EnumHandler.CommunicationMessageID_Enum.PasswordUpdateBanFinish;
                default:
                    return EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanFinish;

            }
        }
        public void ResetFailedAttempts()
        {
            FailedAttempts = 0;
        }
        public virtual void HandleFailedAttempt()
        {
            FailedAttempts++;
        }
        public bool IsUserBanned()
        {
            if (FailedAttempts >= 5)
            {
                return true;
            }
            return false;
        }
    }
}
