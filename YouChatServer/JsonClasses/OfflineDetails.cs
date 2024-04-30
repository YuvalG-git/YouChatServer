using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "OfflineDetails" class represents the details of an offline user.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and last seen time of an offline user.
    /// </remarks>
    public class OfflineDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The DateTime "_lastSeenTime" stores the last time the user was seen.
        /// </summary>
        private DateTime _lastSeenTime;

        #endregion

        #region Constructors

        /// <summary>
        /// The "OfflineDetails" constructor initializes a new instance of the <see cref="OfflineDetails"/> class with the specified username and last seen time.
        /// </summary>
        /// <param name="username">The username of the offline user.</param>
        /// <param name="lastSeenTime">The last seen time of the offline user.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the OfflineDetails class, which represents the details of an offline user.
        /// It initializes the username and last seen time of the offline user.
        /// </remarks>
        public OfflineDetails(string username, DateTime lastSeenTime)
        {
            _username = username;
            _lastSeenTime = lastSeenTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Username" property represents the username of a user.
        /// It gets or sets the username of the user.
        /// </summary>
        /// <value>
        /// The username of the user.
        /// </value>
        public string Username
        {
            get 
            { 
                return _username;
            }
            set 
            { 
                _username = value;
            }
        }

        /// <summary>
        /// The "LastSeenTime" property represents the last time a user was seen.
        /// It gets or sets the last seen time of the user.
        /// </summary>
        /// <value>
        /// The last seen time of the user.
        /// </value>
        public DateTime LastSeenTime
        {
            get
            {
                return _lastSeenTime;
            }
            set
            {
                _lastSeenTime = value;
            }
        }

        #endregion
    }
}
