using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Utils;
using UnityEngine;

namespace Unibas.DBIS.VREP.Network
{
    /**
     * The CovisClientImpl assumes that it's only used by one external instance responsible for handling syncables.
     */
    public class CovisClientImpl : CovisClient
    {
        
        private Channel channel;
        private CovisService.CovisServiceClient client;
        private AsyncDuplexStreamingCall<UpdateMessage, UpdateMessage> duplexStreamingCall;
   
        public CovisClientImpl(String host, int port)
        {
            channel = new Channel(host, port, ChannelCredentials.Insecure);
            client = new CovisService.CovisServiceClient(channel);
        }
        
        public Task Update(UpdateMessage update)
        {
            if (duplexStreamingCall == null)
            {
                Debug.Log("Called update() before subscribe(). Don't do that.");
                throw new Exception();
            }

            return duplexStreamingCall.RequestStream.WriteAsync(update);
        }

        public void Subscribe(StreamObserver<UpdateMessage> observer)
        {
            if (duplexStreamingCall != null)
            {
                Debug.Log("Called subscribe() twice. This is a grave error");
                throw new Exception();
            }
            duplexStreamingCall = client.Sync(new CallOptions());
            duplexStreamingCall.ResponseStream.ForEachAsync(message => Task.Run(() => observer.onNext(message)));
        }

        public void PingBlocking()
        {
            client.Ping(new Empty());
        }

        public void CloseBlocking()
        {
            channel.ShutdownAsync().Wait();
        }
    }
}