using System;
using System.Threading;

namespace TestThreads
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessArgs(args);

            Server.Http server = new Server.Http();
            Thread serverThread = new Thread(ThreadStartMethod);
            serverThread.Start(server);

            while(!server.IsRunning && !server.HasError) {
                Thread.Sleep(100);
            }
            if(!server.HasError)
            {
                Console.WriteLine("Press any key to end the server...");
                Console.ReadKey();
            }

            server.Close();
            Thread.Sleep(1000);
            serverThread.Join();
        }

        static private void ProcessArgs(string[] args)
        {
            Util.Settings.CurrentLoggerType = Util.Settings.LoggerType.Console;
            if(Array.IndexOf(args, "/fileLogger") >= 0) {
                Util.Settings.CurrentLoggerType = Util.Settings.LoggerType.File;
            }
        }

        private static void ThreadStartMethod(Object obj)
        {
            Server.Http server = obj as Server.Http;
            if(server != null) {
                server.Run();
            }
        }
    }
}
