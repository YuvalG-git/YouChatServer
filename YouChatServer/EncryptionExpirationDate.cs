using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.JsonClasses;

namespace YouChatServer
{
    /// <summary>
    /// The "EncryptionExpirationDate" class manages the expiration date for encryption keys.
    /// </summary>
    internal class EncryptionExpirationDate
    {
        /// <summary>
        /// The "Client" object represents the client for communication.
        /// </summary>
        private Client Client;

        /// <summary>
        /// The "Timer" object represents the timer for managing expiration.
        /// </summary>
        private readonly System.Timers.Timer Timer = new System.Timers.Timer();

        /// <summary>
        /// The "TimerTickTimeSpan" object represents the time span for timer ticks.
        /// </summary>
        private TimeSpan TimerTickTimeSpan;

        /// <summary>
        /// The "CountDownTimeSpan" object represents the countdown time span.
        /// </summary>
        private TimeSpan CountDownTimeSpan;

        /// <summary>
        /// The "ExpirationDateInMinutes" constant represents the expiration date in minutes.
        /// </summary>
        private const int ExpirationDateInMinutes = 60;

        /// <summary>
        /// The "EncryptionExpirationDate" constructor initializes a new instance of the <see cref="EncryptionExpirationDate"/> class.
        /// </summary>
        /// <param name="client">The client for communication.</param>
        public EncryptionExpirationDate(Client client)
        {
            Client = client;
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Tick;
            TimerTickTimeSpan = TimeSpan.FromMilliseconds(Timer.Interval);
        }

        /// <summary>
        /// The "Timer_Tick" method handles the timer tick event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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
                if (Client != null)
                    Client.SendMessage(KeyRenewalJson);
            }
        }

        /// <summary>
        /// The "Start" method starts the expiration countdown.
        /// </summary>
        public void Start()
        {
            CountDownTimeSpan = TimeSpan.FromMinutes(ExpirationDateInMinutes);
            Timer.Start();
        }
    }
}
