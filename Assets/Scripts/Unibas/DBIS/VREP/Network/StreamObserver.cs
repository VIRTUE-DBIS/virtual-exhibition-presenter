using System;
using Unibas.DBIS.VREP.Covis;
using UnityEngine;

namespace Unibas.DBIS.VREP.Network
{
    public class StreamObserver
    {
        public void onNext(UpdateMessage message)
        {
            switch (message.UpdateCase)
            {
                case UpdateMessage.UpdateOneofCase.Syncable:
                {
                    SynchronizationManager.AddSyncableMessageToQueue(message);
                    break;
                }
                case UpdateMessage.UpdateOneofCase.Container:
                {
                    SynchronizationManager.AddContainerMessageToQueue(message);
                    break;
                }
            }
        }
    }
}