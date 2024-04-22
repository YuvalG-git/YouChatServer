using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer.UdpHandler
{
    public class IpEndPointHandler
    {
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
