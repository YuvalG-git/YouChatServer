using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "EnumConverter" class provides a generic JSON converter for converting enum values to and from JSON.
    /// </summary>
    /// <typeparam name="T">The type of the enum to convert.</typeparam>
    public class EnumConverter<T> : JsonConverter where T : Enum
    {
        #region Public Methods

        /// <summary>
        /// The "CanConvert" override method determines whether this converter can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type of the object to convert.</param>
        /// <returns>true if this converter can convert the specified object type; otherwise, false.</returns>
        /// <remarks>
        /// This method checks if the specified object type is assignable from the <see cref="Enum"/> type.
        /// </remarks>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Enum).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// The "ReadJson" override method reads the JSON representation of the object and converts it to an instance of the specified type.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="existingValue">The existing value of the object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <remarks>
        /// This method reads the JSON representation from the <paramref name="reader"/> and converts it to an instance of the specified <paramref name="objectType"/>.
        /// If the token type is <see cref="JsonToken.Integer"/>, it converts the integer value to an enum of type <typeparamref name="T"/>.
        /// If the token type is <see cref="JsonToken.String"/>, it parses the string value to an enum of type <typeparamref name="T"/>.
        /// </remarks>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                int value = Convert.ToInt32(reader.Value);
                return Enum.ToObject(typeof(T), value);
            }
            else if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value.ToString();
                return Enum.Parse(typeof(T), value);
            }
            throw new JsonSerializationException($"Unexpected token type {reader.TokenType}.");
        }

        /// <summary>
        /// The "WriteJson" override method writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <remarks>
        /// This method writes the JSON representation of the object value as a string.
        /// </remarks>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        #endregion
    }
}
