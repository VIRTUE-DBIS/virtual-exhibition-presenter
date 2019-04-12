using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;

namespace Unibas.DBIS.VREP.Network
{
    
    public class PingTest : MonoBehaviour
    {
        private void Start()
        {
            Send();
        }

        public void Send()
        {
            try
            {
                var channel = new Channel("127.0.0.1:9734", ChannelCredentials.Insecure);

                var client = new CovisService.CovisServiceClient(channel);

                var reply = client.Ping(new Empty());

                Debug.Log("Successful ping");
                
                channel.ShutdownAsync().Wait();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.Log("Unsuccessful ping");
            }
        }
    }
}