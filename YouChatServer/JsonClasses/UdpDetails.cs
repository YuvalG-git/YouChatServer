using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    public class UdpDetails
    {
        private string _fieldNumber1;
        private string _fieldNumber2;

        public UdpDetails(string fieldNumber1, string fieldNumber2)
        {
            _fieldNumber1 = fieldNumber1;
            _fieldNumber2 = fieldNumber2;
        }
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

    }
}
