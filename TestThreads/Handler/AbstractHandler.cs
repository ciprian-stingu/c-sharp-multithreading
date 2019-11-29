using System.Net.Sockets;

namespace TestThreads.Handler
{
    abstract public class AbstractHandler
    {
        protected Socket clientSocket;
        protected Interface.Interpreter interpreter;

        public AbstractHandler(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            interpreter = null;
        }

        ~AbstractHandler()
        {
            CloseSocket();
        }

        protected void CloseSocket()
        {
            if(clientSocket != null)
            {
                if(clientSocket.Connected) {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                clientSocket.Close();
                clientSocket = null;
            }
        }

    }
}
