using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.JsonClasses;

namespace YouChatServer.ClientAttemptsStateHandler
{
    internal class ClientAttemptsStateBase
    {
        protected Client Client;
        public bool IsBanned { get; set; }
        public double CurrentBanDuration { get; set; }
        protected Queue<double> BanDuration;
        protected readonly System.Timers.Timer Timer = new System.Timers.Timer();
        protected TimeSpan TimerTickTimeSpan;
        protected TimeSpan CountDownTimeSpan;

        public ClientAttemptsStateBase(Client client)
        {
            Client = client;
            IsBanned = false;
            CurrentBanDuration = 0;
            BanDuration = new Queue<double>();
            double[] WaitingTimeArray = { 0.25, 0.5, 1, 2, 3, 5, 10, 15, 20, 30 };
            foreach (double timePeriod in WaitingTimeArray)
            {
                BanDuration.Enqueue(timePeriod);
            }
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Tick;
            TimerTickTimeSpan = TimeSpan.FromMilliseconds(Timer.Interval);
        }

        protected virtual void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CountDownTimeSpan -= TimerTickTimeSpan;
            if (CountDownTimeSpan.TotalMilliseconds <= 0)
            {
                Timer.Stop();
                IsBanned = false;
                SendMessage(EnumHandler.CommunicationMessageID_Enum.LoginBanFinish, null);
            }
        }

        protected virtual void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object data)
        {
            JsonObject messageJsonObject = new JsonObject(messageId, data);
            string messageJson = JsonConvert.SerializeObject(messageJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            Client.SendMessage(messageJson);
        }


        public void HandleBan()
        {
            IsBanned = true;
            if (BanDuration.Count > 0)
            {
                CurrentBanDuration = BanDuration.Dequeue();
            }
            CountDownTimeSpan = TimeSpan.FromMinutes(CurrentBanDuration);
            Timer.Start();
        }

    }
}
