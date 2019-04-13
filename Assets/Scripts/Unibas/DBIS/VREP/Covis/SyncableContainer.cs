using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Unibas.DBIS.VREP.Network;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Covis
{
    public class SyncableContainer : MonoBehaviour
    {
        public string[] syncableKeys = {};
        public Syncable[] syncableValues = {};
        
        public Dictionary<string, Syncable> syncables = new Dictionary<string, Syncable>();
        public string uuid;
        public string name;
        public SyncableContainerType type;

        void Awake()
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                Debug.LogError("Uuid already initialized at syncable container initialize!");
            }
            uuid = Guid.NewGuid().ToString();
            if (syncableKeys.Length != syncableValues.Length)
            {
                Debug.LogError("Number of syncable keys not equal to number of syncable values!");
            }

            for (int i = 0; i < syncableKeys.Length; i++)
            {
                syncables.Add(syncableKeys[i], syncableValues[i]);
            }
            
            FindSyncables(gameObject, syncables);

            syncables.Values.ForEach(syncable => syncable.Initialize());
            
            SynchronizationManager.Register(this);
        }

        public void SendUpdate()
        {
            UpdateMessage message = new UpdateMessage();
            global::SyncableContainer container = new global::SyncableContainer();

            MapField<string, global::Syncable> syncablesMap = new MapField<string, global::Syncable>();
            syncables.ForEach(keyValue => syncablesMap[keyValue.Key] = keyValue.Value.toProtoSyncable());
            container.Syncables.Add(syncablesMap);
            container.Uuid = uuid;
            container.Name = name;
            container.Type = type;
            // TODO: Model

            message.Container = container;
            // TODO: Set timestamp

            CovisClientImpl.Instance.Update(message);
        }

        private void FindSyncables(GameObject gobj, Dictionary<string, Syncable> dictionary)
        {
            var localSyncable = gobj.GetComponent<Syncable>();
            if (localSyncable != null && !dictionary.Values.Contains(localSyncable))
            {
                dictionary.Add(gobj.name, localSyncable);
            }

            foreach (Transform child in gobj.transform)
            {
                FindSyncables(child.gameObject, dictionary);
            }
        }

        private void Update()
        {
            // TODO: Send update if something changes
            // TODO: Check for updates
        }
    }
}