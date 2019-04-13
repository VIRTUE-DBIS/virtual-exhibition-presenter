using System.Threading.Tasks;

namespace Unibas.DBIS.VREP.Network
{
    public interface CovisClient
    {
        /**
         * Sends an UpdateMessage to covis-server.
         * It is expected that the first time you are sending a Syncable or SyncableContainer, you send the full information associated with it.
         * Call Subscribe() before you send Updates to the server since there is a bidirectional stream in the background.
         * Returns a task for which you can wait. or not.
         */
        Task Update(UpdateMessage update);

        /**
         * Subscribe to updates from the server.
         * All information the client receives will be forwarded to the appropriate method of the StreamObserver.
         */
        void Subscribe(StreamObserver<UpdateMessage> observer);

        /**
         * Send a simple ping and wait for a reply from the server.
         */
        void PingBlocking();
    }
}