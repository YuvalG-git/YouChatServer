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
        #region Private Fields

        /// <summary>
        /// The Client object 'Client' represents a client in the system.
        /// </summary>
        private Client Client;

        /// <summary>
        /// The TimeSpan object 'TimerTickTimeSpan' represents the time span for each tick of the timer.
        /// </summary>
        private TimeSpan TimerTickTimeSpan;

        /// <summary>
        /// The TimeSpan object 'CountDownTimeSpan' represents the countdown time span used in the system.
        /// </summary>
        private TimeSpan CountDownTimeSpan;

        #endregion

        #region Private Readonly Fields

        /// <summary>
        /// The System.Timers.Timer object 'Timer' represents a timer used in the system.
        /// </summary>
        private readonly System.Timers.Timer Timer = new System.Timers.Timer();

        #endregion

        #region Private Const Fields

        /// <summary>
        /// The integer constant 'ExpirationDateInMinutes' represents the expiration date in minutes for a certain operation.
        /// </summary>
        private const int ExpirationDateInMinutes = 60;

        #endregion

        #region Constructors

        /// <summary>
        /// The "EncryptionExpirationDate" constructor initializes a new instance of the <see cref="EncryptionExpirationDate"/> class with the specified client.
        /// </summary>
        /// <param name="client">The client associated with the encryption expiration date.</param>
        /// <remarks>
        /// This constructor sets up the encryption expiration date for the specified client.
        /// </remarks>
        public EncryptionExpirationDate(Client client)
        {
            Client = client;
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Tick;
            TimerTickTimeSpan = TimeSpan.FromMilliseconds(Timer.Interval);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The "Timer_Tick" method handles the timer tick event for the countdown timer.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Additional information about the event.</param>
        /// <remarks>
        /// This method decrements the countdown TimeSpan by the TimerTickTimeSpan interval.
        /// If the countdown TimeSpan reaches zero or less, the method stops the timer and sends a message to the client to renew encryption keys.
        /// </remarks>
        private void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CountDownTimeSpan -= TimerTickTimeSpan;
            if (CountDownTimeSpan.TotalMilliseconds <= 0)
            {
                Timer.Stop();
                if (Client != null)
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.EncryptionRenewKeys;
                    object messageContent = null;
                    Client.SendMessage(messageType, messageContent);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "Start" method starts the timer with a countdown TimeSpan based on the expiration date in minutes.
        /// </summary>
        /// <remarks>
        /// This method sets the countdown TimeSpan to the expiration date in minutes and starts the timer.
        /// </remarks>
        public void Start()
        {
            CountDownTimeSpan = TimeSpan.FromMinutes(ExpirationDateInMinutes);
            Timer.Start();
        }

        #endregion
    }
}
