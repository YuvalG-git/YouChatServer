using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "NamespaceAdjustmentBinder" class is a custom serialization binder that adjusts type names by replacing "YouChatApp" with "YouChatServer".
    /// </summary>
    public class NamespaceAdjustmentBinder : SerializationBinder
    {
        /// <summary>
        /// The method overrides the BindToType method to adjust the type name by replacing "YouChatApp" with "YouChatServer".
        /// </summary>
        /// <param name="assemblyName">The assembly name of the type.</param>
        /// <param name="typeName">The original type name.</param>
        /// <returns>The adjusted Type object.</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Replace "YouChatApp" with "YouChatServer" in the type name
            string adjustedTypeName = typeName.Replace("YouChatApp", "YouChatServer");
            return Type.GetType(adjustedTypeName);
        }
    }
}
