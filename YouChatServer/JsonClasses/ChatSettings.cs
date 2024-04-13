using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses
{
    internal class ChatSettings
    {
        private int _textSizeProperty;
        private int _messageGapProperty;
        private bool _enterKeyPressedProperty;

        public ChatSettings(int textSizeProperty, int messageGapProperty, bool enterKeyPressedProperty)
        {
            _textSizeProperty = textSizeProperty;
            _messageGapProperty = messageGapProperty;
            _enterKeyPressedProperty = enterKeyPressedProperty;
        }
        public int TextSizeProperty
        {
            get
            {
                return _textSizeProperty;
            }
            set
            {
                _textSizeProperty = value;
            }
        }

        public int MessageGapProperty
        {
            get
            {
                return _messageGapProperty;
            }
            set
            {
                _messageGapProperty = value;
            }
        }

        public bool EnterKeyPressedProperty
        {
            get
            {
                return _enterKeyPressedProperty;
            }
            set
            {
                _enterKeyPressedProperty = value;
            }
        }

    }
}
