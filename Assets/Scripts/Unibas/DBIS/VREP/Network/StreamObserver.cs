using System;

namespace Unibas.DBIS.VREP.Network
{
    public interface StreamObserver<T>
    {
        void onNext(T message);

        /**
         * Can be called multiple times. After onComplete(), you can ignore all onNext() calls.
         */
        void onComplete();

        void onError(Exception e);
    }
}