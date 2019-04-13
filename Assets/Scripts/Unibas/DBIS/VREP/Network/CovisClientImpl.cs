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
        public static CovisClient Instance => instance;

        private static CovisClientImpl instance;
        
        private Channel channel;
        private CovisService.CovisServiceClient client;
        private AsyncDuplexStreamingCall<UpdateMessage, UpdateMessage> duplexStreamingCall;

        public static void Initialize(String host, int port)
        {
            instance = new CovisClientImpl(host, port);
        }
   
        public CovisClientImpl(String host, int port)
        {
            channel = new Channel(host, port, ChannelCredentials.Insecure);
            client = new CovisService.CovisServiceClient(channel);
        }
        
        /**
         * Sends an UpdateMessage to covis-server.
         * It is expected that the first time you are sending a Syncable or SyncableContainer, you send the full information associated with it.
         * Call Subscribe() before you send Updates to the server since there is a bidirectional stream in the background.
         * Returns a task for which you can wait. or not.
         */
        public Task Update(UpdateMessage update)
        {
            if (duplexStreamingCall == null)
            {
                Debug.Log("Called update() before subscribe(). Don't do that.");
                throw new Exception();
            }

            return duplexStreamingCall.RequestStream.WriteAsync(update);
        }

        /**
         * Subscribe to updates from the server.
         * All information the client receives will be forwarded to the appropriate method of the StreamObserver.
         */
        public void Subscribe(StreamObserver observer)
        {
            if (duplexStreamingCall != null)
            {
                Debug.Log("Called subscribe() twice. This is a grave error");
                throw new Exception();
            }
            duplexStreamingCall = client.Sync(new CallOptions());
            duplexStreamingCall.ResponseStream.ForEachAsync(message => Task.Run(() => observer.onNext(message)));
        }

        /**
         * Send a simple ping and wait for a reply from the server.
         */
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