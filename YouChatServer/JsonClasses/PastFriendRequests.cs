using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.UserDetails;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "PastFriendRequests" class represents a collection of past friend requests sent by the user.
    /// </summary>
    /// <remarks>
    /// This class provides a property for managing a list of past friend requests.
    /// </remarks>
    internal class PastFriendRequests
    {
        #region Private Fields

        /// <summary>
        /// The List "_friendRequest" stores the past friend requests sent by the user.
        /// </summary>
        private List<PastFriendRequest> _friendRequest;

        #endregion

        #region Constructors

        /// <summary>
        /// The "PastFriendRequests" constructor initializes a new instance of the <see cref="PastFriendRequests"/> class with the specified list of past friend requests.
        /// </summary>
        /// <param name="friendRequest">The list of past friend requests.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the PastFriendRequests class, which represents a collection of past friend requests.
        /// It initializes the list of past friend requests.
        /// </remarks>
        public PastFriendRequests(List<PastFriendRequest> friendRequest)
        {
            _friendRequest = friendRequest;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "FriendRequests" property represents a list of past friend requests.
        /// It gets or sets the list of past friend requests.
        /// </summary>
        /// <value>
        /// The list of past friend requests.
        /// </value>
        public List<PastFriendRequest> FriendRequests
        {
            get
            {
                return _friendRequest;
            }
            set
            {
                _friendRequest = value;
            }
        }

        #endregion
    }
}
