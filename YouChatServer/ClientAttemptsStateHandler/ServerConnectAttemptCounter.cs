using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ClientAttemptsStateHandler
{
    /// <summary>
    /// The "ServerConnectAttemptCounter" class manages login attempts and blocks login after exceeding the maximum allowed attempts within a specified time span.
    /// </summary>
    public class ServerConnectAttemptCounter
    {
        #region Private Fields

        /// <summary>
        /// The integer 'attemptsCount' represents the number of attempts made.
        /// </summary>
        private int attemptsCount;

        /// <summary>
        /// The integer 'MaxAttemptsNum' represents the maximum number of attempts allowed.
        /// </summary>
        private int MaxAttemptsNum;

        /// <summary>
        /// The DateTime 'LastAttemptDateTime' represents the date and time of the last attempt.
        /// </summary>
        private DateTime LastAttemptDateTime;

        /// <summary>
        /// The TimeSpan 'TimeSpanToCheck' represents the time span to check for the maximum number of attempts.
        /// </summary>
        private TimeSpan TimeSpanToCheck;

        /// <summary>
        /// The DateTime 'StartBlockDateTime' represents the date and time when the block started.
        /// </summary>
        private DateTime StartBlockDateTime;

        /// <summary>
        /// The TimeSpan 'BlockTimeSpan' represents the time span for which the user is blocked.
        /// </summary>
        private TimeSpan BlockTimeSpan;

        #endregion

        #region Private Const Fields

        /// <summary>
        /// The integer constant 'BlockTime' represents the duration of the block in minutes.
        /// </summary>
        private const int BlockTime = 10;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ServerConnectAttemptCounter" constructor initializes a new instance of the <see cref="ServerConnectAttemptCounter"/> class with the specified maximum number of attempts and time span to check.
        /// </summary>
        /// <param name="maxAttemptsNumber">The maximum number of attempts allowed.</param>
        /// <param name="timeSpanToCheck">The time span to check for attempts.</param>
        /// <remarks>
        /// This constructor sets up the initial state for tracking server connection attempts, including initializing the maximum number of attempts,
        /// the time span to check for attempts, and other related variables.
        /// </remarks>
        public ServerConnectAttemptCounter(int maxAttemptsNumber, TimeSpan timeSpanToCheck)
        {
            attemptsCount = 0;
            MaxAttemptsNum = maxAttemptsNumber;
            TimeSpanToCheck = timeSpanToCheck;
            LastAttemptDateTime = DateTime.MaxValue - TimeSpanToCheck;
            StartBlockDateTime = DateTime.MinValue;
            BlockTimeSpan = TimeSpan.FromMinutes(BlockTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "AttemptsCount" property gets the number of attempts made by the user.
        /// </summary>
        /// <remarks>
        /// This property returns the number of attempts made by the user during authentication.
        /// </remarks>
        public int AttemptsCount
        {
            get 
            { 
                return attemptsCount;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "HandleNewAttempt" method handles a new login attempt by the user.
        /// </summary>
        /// <returns>
        /// Returns true if the login attempt is allowed, or false if the login is blocked.
        /// </returns>
        /// <remarks>
        /// This method checks the current time against the start of the block time span. If the current time is within the block time span, the method returns false,
        /// indicating that the login is blocked. If the current time is outside the block time span, the method checks the time since the last login attempt.
        /// If the time since the last attempt is greater than the time span to check, the method resets the login attempts count and allows the login attempt.
        /// If the time since the last attempt is within the time span to check, the method increments the login attempts count. If the login attempts count is less than or equal
        /// to the maximum allowed attempts, the method allows the login attempt. If the login attempts count exceeds the maximum allowed attempts, the method resets the login attempts count
        /// to 1, sets the start block date time to the current time, and doubles the block time span.
        /// </remarks>
        public bool HandleNewAttempt()
        {
            if (DateTime.Now <= StartBlockDateTime.Add(BlockTimeSpan))
            {
                return false; // Login is blocked within the defined time span.
            }

            if (DateTime.Now > LastAttemptDateTime + TimeSpanToCheck)
            {
                // Reset login attempts count if the time span has elapsed since the last attempt.
                attemptsCount = 0;
                LastAttemptDateTime = DateTime.Now;
                return true;
            }

            attemptsCount++;
            LastAttemptDateTime = DateTime.Now;

            if (attemptsCount <= MaxAttemptsNum)
            {
                return true; // Login attempt is allowed.
            }
            else if (attemptsCount > MaxAttemptsNum)
            {
                // Block further login attempts for an increasing duration.
                attemptsCount = 1;
                StartBlockDateTime = DateTime.Now;
                BlockTimeSpan = BlockTimeSpan.Add(BlockTimeSpan);
            }

            return false; // Login is blocked after exceeding the maximum allowed attempts.
        }

        #endregion
    }

}
