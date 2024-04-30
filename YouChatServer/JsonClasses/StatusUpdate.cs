using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "StatusUpdate" class represents an update to a user's status.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the username and status for the status update operation.
    /// </remarks>
    public class StatusUpdate
    {
        #region Private Fields

        /// <summary>
        /// The string "_username" stores the username of the user.
        /// </summary>
        private string _username;

        /// <summary>
        /// The string "_status" stores the status of the user.
        /// </summary>
        private string _status;

        #endregion

        #region Constructors

        /// <summary>
        /// The "StatusUpdate" constructor initializes a new instance of the <see cref="StatusUpdate"/> class with the specified username and status.
        /// </summary>
        /// <param name="username">The username associated with the status update.</param>
        /// <param name="status">The new status value.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the StatusUpdate class, which represents an update to a user's status.
        /// It initializes the username and status for the status update operation.
        /// </remarks>
        public StatusUpdate(string username, string status)
        {
            _username = username;
            _status = status;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "username" property represents the username of a user.
        /// It gets or sets the username of the user.
        /// </summary>
        /// <value>
        /// The username of the user.
        /// </value>
        public string username
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
        /// The "Status" property represents the status of a user.
        /// It gets or sets the status of the user.
        /// </summary>
        /// <value>
        /// The status of the user.
        /// </value>
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        #endregion
    }
}
