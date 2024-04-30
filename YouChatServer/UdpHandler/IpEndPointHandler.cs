using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    /// <summary>
    /// The "IpEndPointHandler" class provides static methods for managing IP endpoints and client keys dictionaries used in UDP communication.
    /// </summary>
    /// <remarks>
    /// This class includes methods for removing endpoints from the dictionary based on a provided local endpoint, as well as updating an endpoint with a new endpoint.
    /// These methods are used to manage the mapping between client IP endpoints and server IP endpoints for communication.
    /// </remarks>
    public class IpEndPointHandler
    {
        #region Public Static Methods

        /// <summary>
        /// The "RemoveEndpoints" method removes endpoints from the endpoint dictionary and client keys dictionary based on the provided local endpoint.
        /// </summary>
        /// <param name="localEndpoint">The local endpoint to match against.</param>
        /// <param name="endpointDictionary">The dictionary containing the endpoints.</param>
        /// <param name="clientKeys">The dictionary containing client keys.</param>
        /// <remarks>
        /// This method iterates through the endpoint dictionary and removes entries where either the key or the value matches the local endpoint.
        /// It also removes the corresponding entry from the client keys dictionary.
        /// </remarks>
        public static void RemoveEndpoints(IPEndPoint localEndpoint, Dictionary<IPEndPoint, IPEndPoint> endpointDictionary, Dictionary<IPEndPoint, string> clientKeys)
        {
            IPEndPoint friendEndpoint = null;
            List<IPEndPoint> keysToRemove = new List<IPEndPoint>();

            foreach (KeyValuePair<IPEndPoint, IPEndPoint> keyValuePair in endpointDictionary)
            {
                if (keyValuePair.Key.Address.Equals(localEndpoint.Address) && keyValuePair.Key.Port == localEndpoint.Port)
                {
                    friendEndpoint = keyValuePair.Value;
                    keysToRemove.Add(keyValuePair.Key);
                }
                else if (keyValuePair.Value.Address.Equals(localEndpoint.Address) && keyValuePair.Value.Port == localEndpoint.Port)
                {
                    friendEndpoint = keyValuePair.Key;
                    keysToRemove.Add(keyValuePair.Key);
                }
            }

            foreach (IPEndPoint key in keysToRemove)
            {
                endpointDictionary.Remove(key);
                clientKeys.Remove(key);
            }

            if (friendEndpoint != null) //to make sure it gets deleted...
            {
                clientKeys.Remove(friendEndpoint);
            }
        }

        /// <summary>
        /// The "UpdateEndPoint" method updates an endpoint in the dictionary with a new endpoint.
        /// </summary>
        /// <param name="endpoints">The dictionary containing the endpoints.</param>
        /// <param name="clientEndPoint">The endpoint to update.</param>
        /// <param name="newEndPoint">The new endpoint to replace the existing endpoint.</param>
        /// <remarks>
        /// This method iterates through the keys of the dictionary and updates the value associated with the clientEndPoint to newEndPoint.
        /// If clientEndPoint is found as a key, its corresponding value is updated to newEndPoint.
        /// </remarks>
        public static void UpdateEndPoint(Dictionary<IPEndPoint, IPEndPoint> endpoints, IPEndPoint clientEndPoint, IPEndPoint newEndPoint)
        {
            var keys = endpoints.Keys.ToList();
            foreach (var key in keys)
            {
                if (endpoints[key] == clientEndPoint)
                {
                    endpoints.Remove(key);
                    endpoints[key] = newEndPoint;
                }
                else if (key == clientEndPoint)
                {
                    IPEndPoint value = endpoints[key];
                    endpoints.Remove(key);
                    endpoints[newEndPoint] = value;
                }
            }
        }

        #endregion
    }
}
