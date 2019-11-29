using System;
using System.Threading;

namespace TestThreads.Logger
{
    class File : AbstractLogger, Interface.Logger
    {
        string logFile = string.Empty;
        
        public File(string fileName) : base()
        {
            logFile = fileName;
        }

        public void Log(string message)
        {
            try
            {
                locker.AcquireWriterLock(int.MaxValue);
                System.IO.File.AppendAllText(logFile, message + "\r\n");
            }
            catch(Exception e)
            {
                System.Console.WriteLine("Error {0} writing the data {1}", e.Message, message);
            }
            locker.ReleaseWriterLock();
        }
    }
}
