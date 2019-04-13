using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Unibas.DBIS.VREP.Network;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Covis
{
    public class ContainerFactory : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject body;

        private void Awake()
        {
            SynchronizationManager.Initialize();
            
            var syncableContainers = FindObjectsOfType<SyncableContainer>();
            syncableContainers.ForEach(syncableContainer =>
            {
                syncableContainer.Initialize();
                SynchronizationManager.Register(syncableContainer);
                syncableContainer.SendUpdate();
            });
        }

        void Update()
        {
            var containerQueue = SynchronizationManager.Instance.NewContainerQueue;
            UpdateMessage message;
            while (!containerQueue.IsEmpty && containerQueue.TryDequeue(out message))
            {
                instantiate(message.Container);
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Quitting.");
            CovisClientImpl.Instance.CloseBlocking();
        }

        private void instantiate(global::SyncableContainer container)
        {
            switch (container.Type)
            {
                case SyncableContainerType.Tracker:
                {
                    var instance = Instantiate(prefab);
                    var syncables = new Dictionary<string, Syncable>();
                    var syncable = container.Syncables["Tracker"];
                    var syncableComponent = instance.AddComponent<Syncable>();
                    var containerComp = instance.AddComponent<SyncableContainer>();
                    containerComp.Initialize();
                    syncableComponent.uuid = syncable.Uuid;
                    syncables.Add("Tracker", syncableComponent);

                    containerComp.name = container.Name;
                    containerComp.type = container.Type;
                    containerComp.uuid = container.Uuid;
                    containerComp.syncables = syncables;
                    break;
                }
                case SyncableContainerType.VirtualPerson:
                {
                    var instance = Instantiate(body);
                    var syncables = new Dictionary<string, Syncable>();
                    var head = instance.transform.Find("Head").gameObject;
                    var rightHand = instance.transform.Find("RightHand").gameObject;
                    var leftHand = instance.transform.Find("LeftHand").gameObject;
                    
                    var headSyncable = head.AddComponent<Syncable>();
                    var rightHandSyncable = rightHand.AddComponent<Syncable>();
                    var leftHandSyncable = leftHand.AddComponent<Syncable>();
                    var containerComp = instance.AddComponent<SyncableContainer>();
                    
                    containerComp.Initialize();

                    headSyncable.uuid = container.Syncables["Head"].Uuid;
                    rightHandSyncable.uuid = container.Syncables["RightHand"].Uuid;
                    leftHandSyncable.uuid = container.Syncables["LeftHand"].Uuid;
                    
                    syncables.Add("Head", headSyncable);
                    syncables.Add("RightHand", rightHandSyncable);
                    syncables.Add("LeftHand", leftHandSyncable);

                    containerComp.name = container.Name;
                    containerComp.type = container.Type;
                    containerComp.uuid = container.Uuid;
                    containerComp.syncables = syncables;
                    break;
                }
                case SyncableContainerType.RealPerson:
                {
                    break;
                }
            }
        }
    }
}