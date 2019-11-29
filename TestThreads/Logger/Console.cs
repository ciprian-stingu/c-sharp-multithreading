
namespace TestThreads.Logger
{
    class Console : AbstractLogger, Interface.Logger
    {
        public Console() : base()
        {

        }

        public void Log(string message)
        {
            locker.AcquireWriterLock(int.MaxValue);
            System.Console.WriteLine(message);
            locker.ReleaseWriterLock();
        }
    }
}
