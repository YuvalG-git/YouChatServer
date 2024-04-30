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
    /// <summary>
    /// The "ClientAttemptsState" class represents the state of failed authentication attempts for a client.
    /// </summary>
    internal class ClientAttemptsState : ClientAttemptsStateBase
    {
        #region Private Fields

        /// <summary>
        /// The integer 'failedAttempts' represents the number of failed authentication attempts.
        /// </summary>
        private int failedAttempts;

        /// <summary>
        /// The UserAuthentication_Enum object 'userAuthenticationState' represents the user's authentication state.
        /// </summary>
        private EnumHandler.UserAuthentication_Enum userAuthenticationState;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ClientAttemptsState" constructor initializes a new instance of the <see cref="ClientAttemptsState"/> class with the specified client and user authentication state.
        /// </summary>
        /// <param name="client">The client associated with the state.</param>
        /// <param name="userAuthenticationState">The user authentication state.</param>
        /// <remarks>
        /// This constructor sets the failed attempts to 0 and initializes the user authentication state for the client.
        /// </remarks>
        public ClientAttemptsState(Client client, EnumHandler.UserAuthentication_Enum userAuthenticationState) : base(client)
        {
            failedAttempts = 0;
            this.userAuthenticationState = userAuthenticationState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "UserAuthenticationState" property gets the current user authentication state.
        /// </summary>
        public EnumHandler.UserAuthentication_Enum UserAuthenticationState
        {
            get 
            { 
                return userAuthenticationState;
            }
        }

        /// <summary>
        /// The "FailedAttempts" property gets or sets the number of failed login attempts.
        /// </summary>
        public int FailedAttempts
        {
            get
            {
                return failedAttempts;
            }
            set
            { 
                failedAttempts = value;
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// The "Timer_Tick" method overrides the base class's method to handle the timer tick event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void Timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.Timer_Tick(sender, e);
        }

        /// <summary>
        /// The "SendMessage" method overrides the base class's method to send a message with the appropriate message ID based on the user's authentication state.
        /// </summary>
        /// <param name="messageId">The message ID to send.</param>
        /// <param name="messageData">The message data to send.</param>
        protected override void SendMessage(EnumHandler.CommunicationMessageID_Enum messageId, object messageData)
        {
            base.SendMessage(GetMessageIdForAuthenticationState(userAuthenticationState), messageData);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The "GetMessageIdForAuthenticationState" method returns the appropriate message ID based on the user's authentication state.
        /// </summary>
        /// <param name="userAuthenticationState">The user's authentication state.</param>
        /// <returns>The message ID corresponding to the user's authentication state.</returns>
        /// <remarks>
        /// This method is used to determine the message ID to send when a user's authentication state requires a ban finish message.
        /// </remarks>
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

        #endregion

        #region Public Methods

        /// <summary>
        /// The "HandleFailedAttempt" method increments the count of failed authentication attempts.
        /// </summary>
        /// <remarks>
        /// This method is typically called when an authentication attempt fails. It increments the <see cref="failedAttempts"/> field, which is used to track the number of failed attempts.
        /// </remarks>
        public virtual void HandleFailedAttempt()
        {
            failedAttempts++;
        }

        /// <summary>
        /// The "IsUserBanned" method determines if the user is banned based on the number of failed attempts.
        /// </summary>
        /// <returns>true if the user is banned; otherwise, false.</returns>
        /// <remarks>
        /// This method checks if the number of failed attempts is greater than or equal to 5, which is the threshold for banning a user.
        /// </remarks>
        public bool IsUserBanned()
        {
            return failedAttempts >= 5;
        }

        #endregion
    }
}
