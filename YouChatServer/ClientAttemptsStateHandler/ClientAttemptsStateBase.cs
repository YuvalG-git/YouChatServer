using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.JsonClasses;

namespace YouChatServer.ClientAttemptsStateHandler
{
    /// <summary>
    /// The "ClientAttemptsStateBase" class represents the base state for managing client ban durations.
    /// </summary>
    internal class ClientAttemptsStateBase
    {
        #region Private Fields

        /// <summary>
        /// The double 'currentBanDuration' represents the current ban duration for the client.
        /// </summary>
        private double currentBanDuration;

        #endregion

        #region Protected Fields

        /// <summary>
        /// The Client object 'Client' represents the client.
        /// </summary>
        protected Client Client;

        /// <summary>
        /// The Queue<double> object 'BanDuration' stores the ban durations.
        /// </summary>
        protected Queue<double> BanDuration;

        /// <summary>
        /// The System.Timers.Timer object 'Timer' represents a timer used in the system.
        /// </summary>
        protected readonly System.Timers.Timer Timer = new System.Timers.Timer();

        /// <summary>
        /// The TimeSpan object 'TimerTickTimeSpan' represents the time span for each tick of the timer.
        /// </summary>
        protected TimeSpan TimerTickTimeSpan;

        /// <summary>
        /// The TimeSpan object 'CountDownTimeSpan' represents the countdown time span used in the system.
        /// </summary>
        protected TimeSpan CountDownTimeSpan;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ClientAttemptsStateBase" constructor initializes a new instance of the <see cref="ClientAttemptsStateBase"/> class with the specified client.
        /// </summary>
        /// <param name="client">The client associated with the state.</param>
        /// <remarks>
        /// This constructor sets up the initial state for client attempts, including setting the client, initializing ban-related variables, 
        /// creating a queue of ban durations, and setting up a timer for ban duration tracking.
        /// </remarks>
        public ClientAttemptsStateBase(Client client)
        {
            Client = client;
            currentBanDuration = 0;
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

        #endregion

        #region Properties

        /// <summary>
        /// The "CurrentBanDuration" property represents the current duration of the user's ban in minutes.
        /// </summary>
        /// <value>The current duration of the user's ban in minutes.</value>
        public double CurrentBanDuration
        {
            get
            {
                return currentBanDuration;
            }
            set
            {
                currentBanDuration = value;
            }
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// The "Timer_Tick" method handles the tick event of the timer used for counting down the ban time.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An ElapsedEventArgs object that contains the event data.</param>
        /// <remarks>
        /// This method decrements the CountDownTimeSpan by the TimerTickTimeSpan interval on each tick of the timer.
        /// If the remaining time reaches zero or below, the method stops the timer, sets the isBanned flag to false, 
        /// and sends a message to finish the ban process for the user.
        /// </remarks>
        protected virtual void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CountDownTimeSpan -= TimerTickTimeSpan;
            if (CountDownTimeSpan.TotalMilliseconds <= 0)
            {
                Timer.Stop();
                SendMessage(EnumHandler.CommunicationMessageID_Enum.LoginBanFinish, null);
            }
        }

        /// <summary>
        /// The "SendMessage" method sends a communication message to the client if the client is connected.
        /// </summary>
        /// <param name="messageId">The ID of the communication message to be sent.</param>
        /// <param name="data">The data to be sent along with the message.</param>
        /// <remarks>
        /// This method first checks if the Client object is not null, indicating that a client is connected.
        /// If the client is connected, it constructs a message using the provided messageId and data parameters
        /// and sends it to the client using the SendMessage method of the Client object.
        /// </remarks>
        protected virtual void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object data)
        {
            if (Client != null)
            {
                EnumHandler.CommunicationMessageID_Enum messageType = messageId;
                object messageContent = data;
                Client.SendMessage(messageType, messageContent);
                Logger.LogUserBanOver($"A user ban from the server has ended with the following IP: {Client.GetIpAddress()}.");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "HandleBan" method handles the banning of a user.
        /// </summary>
        /// <remarks>
        /// This method sets the "isBanned" flag to true, indicating that the user is banned.
        /// If there are ban durations remaining in the "BanDuration" queue, it dequeues the next duration
        /// and sets the "CountDownTimeSpan" to that duration, effectively starting the ban countdown.
        /// Finally, it starts the timer to track the ban duration.
        /// </remarks>
        public void HandleBan()
        {
            if (BanDuration.Count > 0)
            {
                currentBanDuration = BanDuration.Dequeue();
            }
            CountDownTimeSpan = TimeSpan.FromMinutes(currentBanDuration);
            Timer.Start();
        }

        #endregion
    }
}
