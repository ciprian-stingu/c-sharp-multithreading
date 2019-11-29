using System.Net.Sockets;

namespace TestThreads.Factory
{
    static class Handler
    {
        public static Interface.Handler Get(Util.Settings.HandlerType type, Socket clientSocket)
        {
            Interface.Handler instance = null;

            switch (type)
            {
                case Util.Settings.HandlerType.Telnet:
                    instance = new TestThreads.Handler.Telnet(clientSocket);
                    break;
                default:
                    instance = new TestThreads.Handler.Http(clientSocket);
                    break;

            }

            return instance;
        }
    }
}
