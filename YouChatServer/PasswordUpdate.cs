using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "PasswordUpdate" class provides functionality for determining if a password update is needed.
    /// </summary>
    internal class PasswordUpdate
    {
        #region Public Static Methods

        /// <summary>
        /// The "IsNeededToUpdatePassword" method checks if a user's password needs to be updated based on the last password update date.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>True if the password needs to be updated; otherwise, false.</returns>
        /// <remarks>
        /// This method retrieves the last password update date for the specified username.
        /// It calculates the difference in months between the last password update date and the current date.
        /// If the difference is greater than or equal to 1 month, the method returns true indicating that the password needs to be updated.
        /// </remarks>
        public static bool IsNeededToUpdatePassword(string username)
        {
            bool NeededToUpdatePassword = false;
            DateTime LastPasswordUpdateDate = UserDetails.DataHandler.GetLastPasswordUpdateDate(username);
            DateTime CurrentDate = DateTime.Now;
            int MonthsDifference = CalculateMonthsDifference(LastPasswordUpdateDate, CurrentDate);
            if (MonthsDifference >= 1)
            {
                NeededToUpdatePassword = true;
            }
            return NeededToUpdatePassword;
        }

        /// <summary>
        /// The "CalculateMonthsDifference" method calculates the difference in months between two dates, accounting for partial months.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>The difference in months between the start and end dates.</returns>
        /// <remarks>
        /// This method calculates the difference in months between the start and end dates.
        /// It considers any partial months as a full month if the end date is after the start date.
        /// If the end date is before the start date, it subtracts a month from the total.
        /// </remarks>
        private static int CalculateMonthsDifference(DateTime StartDate, DateTime EndDate)
        {
            if (StartDate >= EndDate)
            {
                return 0;
            }
            int YearDifference = (EndDate.Year - StartDate.Year);
            int MonthDifference = (EndDate.Month - StartDate.Month);
            int monthsApart = 12 * YearDifference + MonthDifference;
            if (EndDate.Day < StartDate.Day)
            {
                monthsApart--;
            }
            return monthsApart;
        }

        #endregion
    }
}
