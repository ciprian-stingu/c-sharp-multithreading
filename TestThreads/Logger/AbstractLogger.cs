using System.Threading;

namespace TestThreads.Logger
{
    class AbstractLogger
    {
        protected ReaderWriterLock locker = null;

        public AbstractLogger()
        {
            locker = new ReaderWriterLock();
        }

    }
}
