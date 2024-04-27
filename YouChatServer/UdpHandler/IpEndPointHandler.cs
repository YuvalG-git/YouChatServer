using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    /// <summary>
    /// The "IpEndPointHandler" class provides methods for managing IP endpoints in a dictionary.
    /// </summary>
    public class IpEndPointHandler
    {
        /// <summary>
        /// The "RemoveEndpoints" method removes endpoints from the dictionary based on the provided local endpoint.
        /// </summary>
        /// <param name="localEndpoint">The local endpoint to match against.</param>
        /// <param name="endpointDictionary">The dictionary of endpoints to modify.</param>
        /// <param name="clientKeys">The dictionary of client keys to modify.</param>
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
        /// The "UpdateEndPoint" method updates the endpoint in the dictionary with a new endpoint for a client.
        /// </summary>
        /// <param name="endpoints">The dictionary of endpoints to update.</param>
        /// <param name="clientEndPoint">The old endpoint of the client.</param>
        /// <param name="newEndPoint">The new endpoint for the client.</param>
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
    }
}
