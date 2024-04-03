using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatApp
{
    public class EnumConverter<T> : JsonConverter where T : Enum
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Enum).IsAssignableFrom(objectType);
        }

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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
