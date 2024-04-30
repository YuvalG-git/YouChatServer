using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChatServer.ChatHandler;
using YouChatServer.ContactHandler;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "ContactAndChat" class represents the combination of chat details and contact details.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing the chat details and contact details.
    /// </remarks>
    public class ContactAndChat
    {
        #region Private Fields

        /// <summary>
        /// The ChatHandler.ChatDetails object "_chat" represents the chat details.
        /// </summary>
        private ChatHandler.ChatDetails _chat;

        /// <summary>
        /// The ContactDetails object "_contactDetails" represents the contact details.
        /// </summary>
        private ContactDetails _contactDetails;

        #endregion

        #region Constructors

        /// <summary>
        /// The "ContactAndChat" constructor initializes a new instance of the <see cref="ContactAndChat"/> class with the specified chat details and contact details.
        /// </summary>
        /// <param name="chat">The chat details.</param>
        /// <param name="contactDetails">The contact details.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the ContactAndChat class, which represents the combination of chat details and contact details.
        /// It initializes the chat details and contact details.
        /// </remarks>
        public ContactAndChat(ChatHandler.ChatDetails chat, ContactDetails contactDetails)
        {
            _chat = chat;
            _contactDetails = contactDetails;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "Chat" property represents the chat details.
        /// It gets or sets the chat details.
        /// </summary>
        /// <value>
        /// The chat details.
        /// </value>
        public ChatHandler.ChatDetails Chat
        {
            get
            {
                return _chat;
            }
            set
            {
                _chat = value;
            }
        }

        /// <summary>
        /// The "ContactDetails" property represents the details of a contact.
        /// It gets or sets the details of the contact.
        /// </summary>
        /// <value>
        /// The details of the contact.
        /// </value>
        public ContactDetails ContactDetails
        {
            get
            {
                return _contactDetails;
            }
            set
            {
                _contactDetails = value;
            }
        }

        #endregion
    }
}
