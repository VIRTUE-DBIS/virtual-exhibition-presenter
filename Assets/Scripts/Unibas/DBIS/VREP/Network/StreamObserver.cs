using System;

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
                    // TODO: Add to queue
                    break;
                }
                case UpdateMessage.UpdateOneofCase.Container:
                {
                    // TODO: Add to queue
                    break;
                }
            }
        }
    }
}