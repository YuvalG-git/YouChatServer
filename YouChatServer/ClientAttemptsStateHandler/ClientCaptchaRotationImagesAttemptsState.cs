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
    /// <summary>
    /// The "ClientCaptchaRotationImagesAttemptsState" class represents the state for managing client attempts with captcha rotation images.
    /// </summary>
    internal class ClientCaptchaRotationImagesAttemptsState : ClientAttemptsStateBase
    {
        #region Private Fields

        /// <summary>
        /// The integer 'score' represents the user's score.
        /// </summary>
        private int score;

        #endregion

        #region Private Const Fields

        /// <summary>
        /// The integer constant 'MaxAttempts' represents the maximum number of attempts allowed.
        /// </summary>
        private const int MaxAttempts = 5;


        #endregion

        #region Constructors

        /// <summary>
        /// The "ClientCaptchaRotationImagesAttemptsState" constructor initializes a new instance of the <see cref="ClientCaptchaRotationImagesAttemptsState"/> class with the specified client.
        /// </summary>
        /// <param name="client">The client associated with the state.</param>
        /// <remarks>
        /// This constructor sets up the initial state for client captcha rotation images attempts, including setting the client and initializing the score.
        /// </remarks>
        public ClientCaptchaRotationImagesAttemptsState(Client client) : base(client)
        {
            score = 0;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// The "Timer_Tick" methods overrides the base class's Timer_Tick method to handle the timer tick event.
        /// This method is called when the timer interval has elapsed. It stops the timer and resets the ban status.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method is used to handle the timer tick event in the context of managing user bans. 
        /// When the timer interval has elapsed, it stops the timer and resets the ban status, allowing the user to attempt login again.
        /// </remarks>
        protected override void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.Timer_Tick(sender, e);
        }

        /// <summary>
        /// The "SendMessage" method sends a message to the client with the specified message ID and message data.
        /// If the message ID is for finishing a ban, the method retrieves captcha rotation image details from the client and sends them along with the message.
        /// </summary>
        /// <param name="messageId">The ID of the message to send.</param>
        /// <param name="messageData">The data to send with the message.</param>
        /// <remarks>
        /// This method is used to send messages to the client. If the message ID is for finishing a ban, the method requests captcha rotation image details from the client to include in the message.
        /// </remarks>
        protected override void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object messageData)
        {
            if (messageId == EnumHandler.CommunicationMessageID_Enum.LoginBanFinish)
            {
                CaptchaRotationImageDetails captchaRotationImageDetails = Client.GetCaptchaRotationImageDetails(score, MaxAttempts);
                base.SendMessage(messageId, captchaRotationImageDetails);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "HandleBan" method handles the ban state for the user.
        /// </summary>
        /// <param name="score">The user's score.</param>
        /// <remarks>
        /// This method sets the user's score to the provided value and calls the base class's "HandleBan" method to initiate the ban process.
        /// </remarks>
        public void HandleBan(int score)
        {
            this.score = score;
            base.HandleBan();
        }

        #endregion
    }
}
