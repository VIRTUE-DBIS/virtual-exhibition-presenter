using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Unibas.DBIS.VREP.Network;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Covis
{
    public class SyncableContainer : MonoBehaviour
    {
        public List<Syncable> syncables = new List<Syncable>();
        public String uuid;
        public String name;
        public SyncableContainerType type;
        public bool original;

        private void FindSyncables(GameObject gobj, List<Syncable> list)
        {
            var localSyncable = gobj.GetComponent<Syncable>();
            if (localSyncable != null && !list.Contains(localSyncable))
            {
                list.Add(localSyncable);
            }

            foreach (Transform child in gobj.transform)
            {
                FindSyncables(child.gameObject, list);
            }
        }

        private void Start()
        {
            FindSyncables(gameObject, syncables);

            SynchronizationManager.Initialize();
            UpdateMessage message = new UpdateMessage();
            global::SyncableContainer container = new global::SyncableContainer();
            
            MapField<string, global::Syncable> syncablesMap = new MapField<string, global::Syncable>();
            syncables.ForEach(syncable => syncablesMap[syncable.uuid] = syncable.toProtoSyncable());
            container.Syncables.Add(syncablesMap);
            container.Uuid = uuid;
            container.Name = name;
            container.Type = type;
            // TODO: Model

            message.Container = container;
            // TODO: Set timestamp

            CovisClientImpl.Instance.Update(message);
        }

        private void Update()
        {
            if (original)
            {
                // TODO: Send update if something changes
            }
            else
            {
                // TODO: Check for updates
            }
        }
    }
}