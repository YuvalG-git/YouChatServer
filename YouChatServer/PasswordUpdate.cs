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
        /// <summary>
        /// The "IsNeededToUpdatePassword" method checks if a password update is needed for the given username.
        /// </summary>
        /// <param name="username">The username for which to check if a password update is needed.</param>
        /// <returns>True if a password update is needed, otherwise false.</returns>
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
        /// The "CalculateMonthsDifference" method calculates the difference in months between two dates.
        /// </summary>
        /// <param name="StartDate">The start date.</param>
        /// <param name="EndDate">The end date.</param>
        /// <returns>The difference in months between the two dates.</returns>
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
    }
}
