using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unibas.DBIS.VREP.Network;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Covis
{
    using SyncableUUID = String;
    using SyncableContainerUUID = String;

    public class SynchronizationManager
    {
        public static SynchronizationManager Instance => instance;

        private static SynchronizationManager instance;
        private static bool initialized;

        public readonly Dictionary<SyncableUUID, ConcurrentQueue<UpdateMessage>> SyncableUpdateQueue;
        public readonly Dictionary<SyncableContainerUUID, ConcurrentQueue<UpdateMessage>> ContainerUpdateQueue;
        public readonly ConcurrentQueue<UpdateMessage> NewContainerQueue;

        private SynchronizationManager()
        {
            SyncableUpdateQueue = new Dictionary<SyncableUUID, ConcurrentQueue<UpdateMessage>>();
            ContainerUpdateQueue = new Dictionary<SyncableContainerUUID, ConcurrentQueue<UpdateMessage>>();
            NewContainerQueue = new ConcurrentQueue<UpdateMessage>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize()
        {
            if (initialized) return;

            instance = new SynchronizationManager();
            // TODO: Configure
            CovisClientImpl.Initialize("10.192.5.35", 9734);

            CovisClientImpl.Instance.Subscribe(new StreamObserver());

            initialized = true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddSyncableMessageToQueue(UpdateMessage message)
        {
            string uuid = message.Syncable.Uuid;
            if (instance.SyncableUpdateQueue.ContainsKey(uuid))
            {
                // Add message to queue
                instance.SyncableUpdateQueue[uuid].Enqueue(message);
            }
            else
            {
                // THIS SHOULD NEVER BE THE CASE!
                Debug.LogError("Received syncable update of unknown syncable!");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddContainerMessageToQueue(UpdateMessage message)
        {
            string uuid = message.Container.Uuid;
            if (instance.ContainerUpdateQueue.ContainsKey(uuid))
            {
                // Add message to queue
                instance.ContainerUpdateQueue[uuid].Enqueue(message);
            }
            else
            {
                instance.ContainerUpdateQueue[uuid] = new ConcurrentQueue<UpdateMessage>();
                var container = message.Container;
                container.Syncables.Values.ForEach(syncable =>
                    instance.SyncableUpdateQueue.Add(syncable.Uuid, new ConcurrentQueue<UpdateMessage>()));
                instance.NewContainerQueue.Enqueue(message);
            }
        }

        public static void Register(SyncableContainer container)
        {
            var containerQueue = instance.ContainerUpdateQueue;
            var syncablesQueue = instance.SyncableUpdateQueue;
            if (!containerQueue.ContainsKey(container.uuid))
            {
                containerQueue.Add(container.uuid, new ConcurrentQueue<UpdateMessage>());
                container.syncables.Values.ForEach(syncable =>
                {
                    if (!syncablesQueue.ContainsKey(syncable.uuid))
                    {
                        syncablesQueue.Add(syncable.uuid, new ConcurrentQueue<UpdateMessage>());
                    }
                });
            }
        }
    }
}