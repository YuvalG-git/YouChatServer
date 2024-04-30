using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    /// <summary>
    /// The "UdpDetails" class represents UDP details with two encrypted symmetric keys.
    /// </summary>
    /// <remarks>
    /// This class provides properties for managing two encrypted symmetric keys for UDP communication.
    /// </remarks>
    public class UdpDetails
    {
        #region Private Fields

        /// <summary>
        /// The string "_fieldNumber1" stores the value for encrypted audio symmertic key.
        /// </summary>
        private string _fieldNumber1;

        /// <summary>
        /// The string "_fieldNumber2" stores the value for encrypted video symmertic key.
        /// </summary>
        private string _fieldNumber2;

        #endregion

        #region Constructors

        /// <summary>
        /// The "UdpDetails" constructor initializes a new instance of the <see cref="UdpDetails"/> class with the specified encrypted symmertic keys.
        /// </summary>
        /// <param name="fieldNumber1">The value for the encrypted audio symmertic key.</param>
        /// <param name="fieldNumber2">The value for the encrypted video symmertic key.</param>
        /// <remarks>
        /// This constructor is used to create a new instance of the UdpDetails class, which represents UDP details with two encrypted symmertic keys.
        /// It initializes the values for the encrypted symmertic keys in the UDP details.
        /// </remarks>
        public UdpDetails(string fieldNumber1, string fieldNumber2)
        {
            _fieldNumber1 = fieldNumber1;
            _fieldNumber2 = fieldNumber2;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The "FieldNumber1" property represents an encrypted audio symmertic key.
        /// It gets or sets the value of the encrypted audio symmertic key.
        /// </summary>
        /// <value>
        /// The value of the encrypted audio symmertic key.
        /// </value>
        public string FieldNumber1 
        { 
            get 
            { 
                return _fieldNumber1;
            }
            set
            {
                _fieldNumber1 = value;
            }
        }

        /// <summary>
        /// The "FieldNumber2" property represents an encrypted video symmertic key.
        /// It gets or sets the value of the encrypted video symmertic key.
        /// </summary>
        /// <value>
        /// The value of the encrypted video symmertic key.
        /// </value>
        public string FieldNumber2
        {
            get
            {
                return _fieldNumber2;
            }
            set
            {
                _fieldNumber2 = value;
            }
        }

        #endregion
    }
}
