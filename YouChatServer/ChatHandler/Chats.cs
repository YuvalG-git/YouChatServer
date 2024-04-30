using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ChatHandler
{
    /// <summary>
    /// The "Chats" class represents a collection of chat details.
    /// </summary>
    /// <remarks>
    /// This class is used to manage and store a list of chat details.
    /// </remarks>
    internal class Chats
    {
        #region Private Fields

        /// <summary>
        /// The List of ChatDetails objects "_chats" represents the list of chats.
        /// </summary>
        private List<ChatDetails> _chats;

        #endregion

        #region Constructors

        /// <summary>
        /// The "Chats" constructor initializes a new instance of the <see cref="Chats"/> class with the specified list of chat details.
        /// </summary>
        /// <param name="chats">The list of chat details.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the Chats class, which represents a collection of chat details.
        /// It initializes the list of chat details.
        /// </remarks>
        public Chats(List<ChatDetails> chats)
        {
            _chats = chats;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ChatList" property represents a list of chat details.
        /// It gets or sets the list of chat details.
        /// </summary>
        /// <value>
        /// The list of chat details.
        /// </value>
        public List<ChatDetails> ChatList
        {
            get
            {
                return _chats;
            }
            set
            {
                _chats = value;
            }
        }

        #endregion
    }
}
