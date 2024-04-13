using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.JsonClasses;

namespace YouChatServer
{
    internal class EncryptionExpirationDate
    {
        private Client Client;
        private readonly System.Timers.Timer Timer = new System.Timers.Timer();
        private TimeSpan TimerTickTimeSpan;
        private TimeSpan CountDownTimeSpan;
        private const int ExpirationDateInMinutes = 1;
        public EncryptionExpirationDate(Client client)
        {
            Client = client;
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Tick;
            TimerTickTimeSpan = TimeSpan.FromMilliseconds(Timer.Interval);
        }
        private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {

            CountDownTimeSpan -= TimerTickTimeSpan;
            if (CountDownTimeSpan.TotalMilliseconds <= 0)
            {
                Timer.Stop();
                JsonObject KeyRenewalJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EncryptionRenewKeys, null);
                string KeyRenewalJson = JsonConvert.SerializeObject(KeyRenewalJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                Client.SendMessage(KeyRenewalJson);
            }
        }
        public void Start()
        {
            CountDownTimeSpan = TimeSpan.FromMinutes(ExpirationDateInMinutes);
            Timer.Start();
        }
    }
}
