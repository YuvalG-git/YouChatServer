using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ContactHandler
{
    /// <summary>
    /// The "ContactDetails" class represents the details of a contact.
    /// </summary>
    /// <remarks>
    /// This class contains properties for the contact's name, ID, profile picture, status, last seen time, and online status.
    /// </remarks>
    public class ContactDetails
    {
        #region Private Fields

        /// <summary>
        /// The string object "_name" represents the name of the user.
        /// </summary>
        private string _name;

        /// <summary>
        /// The string object "_id" represents the ID of the user.
        /// </summary>
        private string _id;

        /// <summary>
        /// The string object "_profilePicture" represents the path or URL to the user's profile picture.
        /// </summary>
        private string _profilePicture;

        /// <summary>
        /// The string object "_status" represents the status of the user.
        /// </summary>
        private string _status;

        /// <summary>
        /// The DateTime object "_lastSeenTime" represents the last time the user was seen online.
        /// </summary>
        private DateTime _lastSeenTime;

        /// <summary>
        /// The boolean value "_online" represents whether the user is currently online.
        /// </summary>
        private bool _online;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ContactDetails" constructor initializes a new instance of the <see cref="ContactDetails"/> class with the specified name, ID, profile picture, status, last seen time, and online status.
        /// </summary>
        /// <param name="name">The name of the contact.</param>
        /// <param name="id">The ID of the contact.</param>
        /// <param name="profilePicture">The profile picture of the contact.</param>
        /// <param name="status">The status message of the contact.</param>
        /// <param name="lastSeenTime">The last seen time of the contact.</param>
        /// <param name="online">The online status of the contact.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ContactDetails class, which represents the details of a contact.
        /// It initializes the contact's name, ID, profile picture, status, last seen time, and online status.
        /// </remarks>
        public ContactDetails(string name, string id, string profilePicture, string status, DateTime lastSeenTime, bool online)
        {
            _name = name;
            _id = id;
            _profilePicture = profilePicture;
            _status = status;
            _lastSeenTime = lastSeenTime;
            _online = online;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Name" property represents the name of an object.
        /// It gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// The "Id" property represents the identifier of an object.
        /// It gets or sets the identifier of the object.
        /// </summary>
        /// <value>
        /// The identifier of the object.
        /// </value>
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// The "ProfilePicture" property represents the profile picture of a user.
        /// It gets or sets the profile picture of the user.
        /// </summary>
        /// <value>
        /// The profile picture of the user.
        /// </value>
        public string ProfilePicture
        {
            get
            {
                return _profilePicture;
            }
            set
            {
                _profilePicture = value;
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

        /// <summary>
        /// The "LastSeenTime" property represents the date and time when a user was last seen.
        /// It gets or sets the date and time when the user was last seen.
        /// </summary>
        /// <value>
        /// The date and time when the user was last seen.
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

        /// <summary>
        /// The "Online" property represents the online status of a user.
        /// It gets or sets the online status of the user.
        /// </summary>
        /// <value>
        /// True if the user is online, false otherwise.
        /// </value>
        public bool Online
        {
            get
            {
                return _online;
            }
            set
            {
                _online = value;
            }
        }

        #endregion
    }
}
