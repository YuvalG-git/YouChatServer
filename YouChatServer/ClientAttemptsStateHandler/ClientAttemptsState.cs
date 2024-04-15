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

        public ClientAttemptsState(Client client) : base(client)
        {
            FailedAttempts = 0;
        }

        protected override void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.Timer_Tick(sender, e);
        }
        protected override void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object messageData)
        {
            if (messageId == EnumHandler.CommunicationMessageID_Enum.LoginBanFinish)
            {
                base.SendMessage(messageId, null);
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
