using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using YouChatServer.CaptchaHandler;
using YouChatServer.JsonClasses;

namespace YouChatServer.ClientAttemptsStateHandler
{
    internal class ClientCaptchaRotationImagesAttemptsState : ClientAttemptsStateBase
    {
        private int score;
        private const int MaxAttempts = 5;

        public ClientCaptchaRotationImagesAttemptsState(Client client) : base(client)
        {
            score = 0;
        }

        protected override void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.Timer_Tick(sender, e);
        }
        protected override void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object messageData)
        {
            if (messageId == EnumHandler.CommunicationMessageID_Enum.LoginBanFinish)
            {
                CaptchaRotationImageDetails captchaRotationImageDetails = Client.GetCaptchaRotationImageDetails(score, MaxAttempts);
                base.SendMessage(messageId, captchaRotationImageDetails);
            }
        }
        public void HandleBan(int score)
        {
            this.score = score;
            base.HandleBan();

        }
    }
}
