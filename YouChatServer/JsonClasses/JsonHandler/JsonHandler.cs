using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.JsonClasses.JsonHandler
{
    /// <summary>
    /// The "JsonHandler" class provides methods for handling JSON serialization and deserialization.
    /// </summary>
    internal class JsonHandler
    {
        #region Public Static Methods

        /// <summary>
        /// The "GetJsonStringFromJsonData" method creates a JSON string representation of an object with a specified enum type.
        /// </summary>
        /// <param name="enumType">The enum type indicating the type of JSON data.</param>
        /// <param name="obj">The object to be serialized into JSON format.</param>
        /// <returns>A JSON string representation of the object.</returns>
        /// <remarks>
        /// This method creates a JsonObject using the specified enum type and object.
        /// It then serializes the JsonObject into a JSON string using the JsonConvert.SerializeObject method,
        /// with TypeNameHandling set to Auto to include type information in the serialized JSON string.
        /// </remarks>
        public static string GetJsonStringFromJsonData(EnumHandler.CommunicationMessageID_Enum enumType, object obj)
        {
            // Create a JsonObject using the specified enum type and object
            JsonObject jsonObject = new JsonObject(enumType, obj);

            // Serialize the JsonObject into a JSON string with TypeNameHandling set to Auto
            string JsonString = JsonConvert.SerializeObject(jsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return JsonString;
        }

        /// <summary>
        /// The "GetJsonDataFromJsonString" method deserializes a JSON string into a JsonObject.
        /// </summary>
        /// <param name="jsonString">The JSON string to be deserialized.</param>
        /// <returns>A JsonObject representing the deserialized JSON data.</returns>
        /// <remarks>
        /// This method deserializes the JSON string into a JsonObject using the JsonConvert.DeserializeObject method,
        /// with TypeNameHandling set to Auto to include type information in the deserialized object,
        /// a custom binder to adjust namespaces, and an EnumConverter to handle enum types.
        /// </remarks>
        public static JsonObject GetJsonDataFromJsonString(string jsonString)
        {
            // Deserialize the JSON string into a JsonObject
            JsonObject jsonObject = JsonConvert.DeserializeObject<JsonObject>(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = new NamespaceAdjustmentBinder(),
                Converters = { new EnumConverter<EnumHandler.CommunicationMessageID_Enum>() }
            });

            return jsonObject;
        }

        /// <summary>
        /// The "GetMessageTypeOfCommunicationMessageID_Enum" method extracts the message type from a JsonObject.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the message type.</param>
        /// <returns>The message type as an EnumHandler.CommunicationMessageID_Enum value.</returns>
        /// <remarks>
        /// This method extracts the message type from the specified JsonObject by casting the MessageType property
        /// of the JsonObject to an EnumHandler.CommunicationMessageID_Enum value.
        /// </remarks>
        public static EnumHandler.CommunicationMessageID_Enum GetMessageTypeOfCommunicationMessageID_Enum(JsonObject jsonObject)
        {
            // Extract the message type from the JsonObject
            EnumHandler.CommunicationMessageID_Enum messageType = (EnumHandler.CommunicationMessageID_Enum)jsonObject.MessageType;
            return messageType;
        }

        #endregion
    }
}
