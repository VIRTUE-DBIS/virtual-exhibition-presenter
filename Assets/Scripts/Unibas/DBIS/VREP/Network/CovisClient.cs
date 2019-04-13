namespace Unibas.DBIS.VREP.Network
{
    public interface CovisClient
    {
        /**
         * Sends an UpdateMessage to covis-server.
         * It is expected that the first time you are sending a Syncable or SyncableContainer, you send the full information associated with it.
         * Is ideally implemented in a non-blocking way?
         */
        void update(UpdateMessage update);

        /**
         * Subscribe to updates from the server.
         * All information the client receives will be forwarded to the appropriate method of the StreamObserver.
         */
        void subscribe(StreamObserver<UpdateMessage> observer);

        /**
         * Send a simple ping and wait for a reply from the server.
         */
        void ping();
    }
}