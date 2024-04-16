using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ClientAttemptsStateHandler
{
    /// <summary>
    /// Represents a utility class for counting and managing login attempts with a blocking mechanism.
    /// </summary>
    public class ServerConnectAttemptCounter
    {
        /// <summary>
        /// Gets the current number of login attempts.
        /// </summary>
        public int AttemptsCount { get; private set; }

        private int MaxAttemptsNum;
        private DateTime LastAttemptDateTime;
        private TimeSpan TimeSpanToCheck;
        private DateTime StartBlockDateTime;
        private TimeSpan BlockTimeSpan;

        /// <summary>
        /// Initializes a new instance of the AttemptsCounter class with the specified maximum attempts number
        /// and time span to check for login attempts.
        /// </summary>
        /// <param name="maxAttemptsNumber">The maximum number of allowed login attempts.</param>
        /// <param name="timeSpanToCheck">The time span to consider for counting consecutive login attempts.</param>
        public ServerConnectAttemptCounter(int maxAttemptsNumber, TimeSpan timeSpanToCheck)
        {
            AttemptsCount = 0;
            MaxAttemptsNum = maxAttemptsNumber;
            TimeSpanToCheck = timeSpanToCheck;
            LastAttemptDateTime = DateTime.MaxValue - TimeSpanToCheck;
            StartBlockDateTime = DateTime.MinValue;
            BlockTimeSpan = TimeSpan.FromMinutes(30); // Starting block time is one hour (doubles by default).
        }

        /// <summary>
        /// Registers a new login attempt and returns a boolean value indicating if the attempt is allowed.
        /// </summary>
        /// <returns>True if the login attempt is allowed; otherwise, false.</returns>
        public bool NewAttempt()
        {
            if (DateTime.Now <= StartBlockDateTime.Add(BlockTimeSpan))
            {
                return false; // Login is blocked within the defined time span.
            }

            if (DateTime.Now > LastAttemptDateTime + TimeSpanToCheck)
            {
                // Reset login attempts count if the time span has elapsed since the last attempt.
                AttemptsCount = 0;
                LastAttemptDateTime = DateTime.Now;
                return true;
            }

            AttemptsCount++;
            LastAttemptDateTime = DateTime.Now;

            if (AttemptsCount <= MaxAttemptsNum)
            {
                return true; // Login attempt is allowed.
            }
            else if (AttemptsCount > MaxAttemptsNum)
            {
                // Block further login attempts for an increasing duration.
                AttemptsCount = 1;
                StartBlockDateTime = DateTime.Now;
                BlockTimeSpan = BlockTimeSpan.Add(BlockTimeSpan);
            }

            return false; // Login is blocked after exceeding the maximum allowed attempts.
        }
    }

}
