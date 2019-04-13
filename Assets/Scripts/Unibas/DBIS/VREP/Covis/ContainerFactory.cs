using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unibas.DBIS.VREP.Covis
{
    public class ContainerFactory : MonoBehaviour
    {
        public GameObject prefab;

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


        private void instantiate(global::SyncableContainer container)
        {
            switch (container.Type)
            {
                case SyncableContainerType.Tracker:
                {
                    var instance = Instantiate(prefab);
                    var syncableList = new List<Syncable>();
                    var syncable = instance.AddComponent<Syncable>();
                    syncable.original = false;
                    syncable.uuid = container.Syncables.Values.First().Uuid;
                    syncableList.Add(syncable);

                    var containerComp = instance.AddComponent<SyncableContainer>();
                    containerComp.name = container.Name;
                    containerComp.type = container.Type;
                    containerComp.uuid = container.Uuid;
                    containerComp.syncables = syncableList;
                    break;
                }
                case SyncableContainerType.VirtualPerson:
                {
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