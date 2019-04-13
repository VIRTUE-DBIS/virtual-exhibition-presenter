using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Unibas.DBIS.VREP.Network;
using UnityEngine;

namespace Unibas.DBIS.VREP.Covis
{
    public class ContainerFactory : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject body;

        private void Awake()
        {
            SynchronizationManager.Initialize();
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
                    syncableComponent.Initialize();
                    syncableComponent.uuid = syncable.Uuid;
                    syncables.Add("Tracker", syncableComponent);

                    var containerComp = instance.AddComponent<SyncableContainer>();
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
                    
                    headSyncable.Initialize();
                    rightHandSyncable.Initialize();
                    leftHandSyncable.Initialize();

                    headSyncable.uuid = container.Syncables["Head"].Uuid;
                    rightHandSyncable.uuid = container.Syncables["RightHand"].Uuid;
                    leftHandSyncable.uuid = container.Syncables["LeftHand"].Uuid;
                    
                    syncables.Add("Head", headSyncable);
                    syncables.Add("RightHand", rightHandSyncable);
                    syncables.Add("LeftHand", leftHandSyncable);

                    Debug.Log("Before adding SyncableContainer.");
                    var containerComp = instance.AddComponent<SyncableContainer>();
                    Debug.Log("After adding SyncableContainer.");
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