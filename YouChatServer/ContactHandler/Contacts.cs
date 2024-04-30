using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.ContactHandler
{
    /// <summary>
    /// The "Contacts" class represents a collection of contact details.
    /// </summary>
    /// <remarks>
    /// This class contains a list of contact details for a user.
    /// </remarks>
    public class Contacts
    {
        #region Private Fields

        /// <summary>
        /// The List of ContactDetails objects "_contacts" represents the list of contacts.
        /// </summary>
        private List<ContactDetails> _contacts;

        #endregion

        #region Constructors

        /// <summary>
        /// The "Contacts" constructor initializes a new instance of the <see cref="Contacts"/> class with the specified list of contact details.
        /// </summary>
        /// <param name="contacts">The list of contact details.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the Contacts class, which represents a collection of contact details.
        /// It initializes the list of contact details.
        /// </remarks>
        public Contacts(List<ContactDetails> contacts)
        {
            _contacts = contacts;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "ContactList" property represents a list of contact details for a user.
        /// It gets or sets the list of contact details for the user.
        /// </summary>
        /// <value>
        /// The list of contact details for the user.
        /// </value>
        public List<ContactDetails> ContactList
        {
            get
            {
                return _contacts;
            }
            set
            {
                _contacts = value;
            }
        }

        #endregion
    }
}
