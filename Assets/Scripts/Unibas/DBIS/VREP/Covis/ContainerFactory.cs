using System;
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
        public GameObject virtualAssistant;

        private void Awake()
        {
            SynchronizationManager.Initialize();
            
            var syncableContainers = Resources.FindObjectsOfTypeAll<SyncableContainer>();
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
                    Debug.Log("Instantiating tracker with model: \"" + container.Model + "\"...");
                    var instance = (GameObject)Instantiate(Resources.Load("Prefabs/Syncables/" + container.Model));
                    var syncables = new Dictionary<string, Syncable>();
                    var syncable = container.Syncables["Tracker"];
                    var syncableComponent = instance.AddComponent<Syncable>();
                    var containerComp = instance.AddComponent<SyncableContainer>();
                    syncables.Add("Tracker", syncableComponent);
                    
                    syncableComponent.InitializeFromMessage(syncable);
                    containerComp.InitializeFromMessage(container, syncables);
                    break;
                }
                case SyncableContainerType.VirtualPerson:
                {
                    Debug.Log("Instantiating virtual person...");
                    var instance = Instantiate(body);
                    var syncables = new Dictionary<string, Syncable>();
                    var head = instance.transform.Find("Head").gameObject;
                    var rightHand = instance.transform.Find("RightHand").gameObject;
                    var leftHand = instance.transform.Find("LeftHand").gameObject;
                    
                    var headSyncable = head.AddComponent<Syncable>();
                    var rightHandSyncable = rightHand.AddComponent<Syncable>();
                    var leftHandSyncable = leftHand.AddComponent<Syncable>();
                    var containerComp = instance.AddComponent<SyncableContainer>();
                    
                    headSyncable.InitializeFromMessage(container.Syncables["Head"]);
                    rightHandSyncable.InitializeFromMessage(container.Syncables["RightHand"]);
                    leftHandSyncable.InitializeFromMessage(container.Syncables["LeftHand"]);
                    
                    syncables.Add("Head", headSyncable);
                    syncables.Add("RightHand", rightHandSyncable);
                    syncables.Add("LeftHand", leftHandSyncable);

                    containerComp.InitializeFromMessage(container, syncables);
                    break;
                }
                case SyncableContainerType.VirualAssistant:
                {
                    Debug.Log("Instantiating virtual assistant...");
                    var instance = Instantiate(virtualAssistant);
                    var syncables = new Dictionary<string, Syncable>();
                    var syncable = container.Syncables["Assistant"];
                    var syncableComponent = instance.AddComponent<Syncable>();
                    var containerComp = instance.AddComponent<SyncableContainer>();
                    syncables.Add("Assistant", syncableComponent);
                    
                    syncableComponent.InitializeFromMessage(syncable);
                    containerComp.InitializeFromMessage(container, syncables);
                    break;
                }
                default:
                {
                    Debug.LogError("Received unhandled syncable container type: " + container.Type + "\nContainer: " + container);
                    break;
                }
            }
        }
    }
}