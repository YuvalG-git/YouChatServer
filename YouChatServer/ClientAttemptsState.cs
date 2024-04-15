using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using YouChatServer.JsonClasses;

namespace YouChatServer
{
    internal class ClientAttemptsState
    {
        private Client Client;
        public bool IsBanned { get; set; }
        public int FailedAttempts { get; set; }
        public double CurrentBanDuration { get; set; }
        private Queue<double> BanDuration;
        private readonly System.Timers.Timer Timer = new System.Timers.Timer();
        private TimeSpan TimerTickTimeSpan;
        private TimeSpan CountDownTimeSpan;
        public ClientAttemptsState(Client client)
        {
            Client = client;
            IsBanned = false;
            FailedAttempts = 0;
            CurrentBanDuration = 0;
            BanDuration = new Queue<double>();
            double[] WaitingTimeArray = { 0.25, 0.5, 1, 2, 3, 5, 10, 15, 20, 30 };
            foreach (double timePeriod in WaitingTimeArray)
            {
                BanDuration.Enqueue(timePeriod);
            }
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Tick;
            TimerTickTimeSpan = TimeSpan.FromMilliseconds(Timer.Interval); //todo - in order to handle the user setting the timer to zero when the clock gets zero if it will wait a few seconds and if the server didn't send something it will send a message asking for the current time..

        }
        private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {

            CountDownTimeSpan -= TimerTickTimeSpan;
            if (CountDownTimeSpan.TotalMilliseconds <= 0)
            {
                Timer.Stop();
                //send message ban over.. //until the server sends the message the client will show the time and then waiting... or something like that
                IsBanned = false;
                JsonObject banMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.LoginBanFinish, null);
                string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                Client.SendMessage(banMessageJson);
            }
        }
        public void ResetFailedAttempts()
        {
            FailedAttempts = 0;
        }

        public void HandleBan() // i can replace this by doing it as a bool method that will return true if the timer should start and if yes it will start manage that in client class
            // this way i wont need to send an instance of the client object in the constractor
        {
            IsBanned = true;
            if (BanDuration.Count > 0)
            {
                CurrentBanDuration = BanDuration.Dequeue();
            }
            CountDownTimeSpan = TimeSpan.FromMinutes(CurrentBanDuration);

            Timer.Start();
        }
        public void HandleFailedAttempt()
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
