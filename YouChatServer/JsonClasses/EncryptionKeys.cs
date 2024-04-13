using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class EncryptionKeys
    {
        private string _symmetricKey;
        private string _asymmetricKey;

        public EncryptionKeys(string symmetricKey, string asymmetricKey)
        {
            _symmetricKey = symmetricKey;
            _asymmetricKey = asymmetricKey;
        }

        public string SymmetricKey
        {
            get { return _symmetricKey; }
            set { _symmetricKey = value; }
        }
        public string AsymmetricKey
        {
            get { return _asymmetricKey; }
            set { _asymmetricKey = value; }
        }

    }
}
