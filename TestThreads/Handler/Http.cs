using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace TestThreads.Handler
{
    class Http : AbstractHandler, Interface.Handler, Interface.Named
    {
        public Http(Socket clientSocket) : base(clientSocket)
        {
            interpreter = Factory.Interpreter.Get(Util.Settings.InterpreterType.Http);
        }

        public void Handle()
        {
            byte[] bytes = new Byte[Util.Settings.MaxReceiveBufferSize];
            int numByte = clientSocket.Receive(bytes);
            string data = Encoding.ASCII.GetString(bytes, 0, numByte);

            //Factory.Logger.Get().Log(Name() + " -> Received data: " + data);

            clientSocket.Send(interpreter.Process(data));
            
            CloseSocket();
            Factory.Logger.Get().Log(Name() + " -> done...");
        }

        /// <summary>
        /// finalizer...
        /// </summary>
        ~Http()
        {

        }
        public string Name()
        {
            return "[" + Thread.CurrentThread.GetHashCode() + "] Handler.Html";
        }
    }
}
