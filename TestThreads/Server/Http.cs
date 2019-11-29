using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TestThreads.Server
{
    class Http : Interface.Named
    {
        private bool isRunning;
        private bool hasError;
        private Socket listener;

        public Http()
        {
            isRunning = false;
            listener = null;
            hasError = false;
        }

        public void Run()
        {
            Factory.Logger.Get().Log(Name() + " -> Starting the server...");

            string hostName = string.Empty;
            try
            {
                hostName = Dns.GetHostName();
            }
            catch(SocketException e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                hasError = true;
                return;
            }
            IPHostEntry ipHost = null;
            try
            {
                ipHost = Dns.GetHostEntry(hostName);
            }
            catch(Exception e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                hasError = true;
                return;
            }

            IPAddress ipAddr = null;
            for (int i = 0; i < ipHost.AddressList.Length; i++)
            {
                if (ipHost.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddr = ipHost.AddressList[i];
                    break;
                }
            }
            if(ipAddr == null)
            {
                Factory.Logger.Get().Log(Name() + " -> No IPV4 addresses found!");
                hasError = true;
                return;
            }

            IPEndPoint localEndPoint = null;
            try
            {
                localEndPoint = new IPEndPoint(ipAddr, Util.Settings.CurrentListeningPort);
            }
            catch(Exception e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                hasError = true;
                return;
            }

            try
            {
                listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch(SocketException e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                hasError = true;
                return;
            }

            try
            {
                listener.Bind(localEndPoint);
            }
            catch(Exception e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                hasError = true;
                return;
            }

            try
            {
                listener.Listen(Util.Settings.MaxConnectionBacklogSize);
            }
            catch(Exception e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                hasError = true;
                return;
            }

            Factory.Logger.Get().Log(Name() + " -> Server started on " + ipAddr.ToString() + ":" + Util.Settings.CurrentListeningPort);

            isRunning = true;
            ThreadPool.SetMinThreads(Util.Settings.ThreadPoolMinConnections, 10);
            while(isRunning)
            {
                this.Listen();
            }
        }

        private void Listen()
        {
            try
            {
                Socket clientSocket = listener.Accept();
                Factory.Logger.Get().Log(Name() + " -> New client connected...");
                if(clientSocket.Available == 0) 
                {
                    int timeSpent = 0;
                    while(true) //until a realiable method to detect the client type will be implemented
                    {
                        Thread.Sleep(100);
                        timeSpent += 100;
                        if(clientSocket.Available > 0 || timeSpent > 500) {
                            break;
                        }
                    }
                }
                Factory.Logger.Get().Log(Name() + " -> Available data length: " + clientSocket.Available);

                Interface.Handler handler = Factory.Handler.Get(clientSocket.Available > 0 ? Util.Settings.HandlerType.Http : Util.Settings.HandlerType.Telnet, clientSocket);
                //Thread thread = new Thread(ThreadStartMethod);
                //thread.Start(handler);
                ThreadPool.QueueUserWorkItem(o => ThreadStartMethod(handler));
                Factory.Logger.Get().Log(Name() + " -> Thread started...");

            }
            catch(Exception e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
            }
        }

        private void ThreadStartMethod(Object obj)
        {
            Interface.Handler handler = obj as Interface.Handler;
            if(handler != null) {
                handler.Handle();
            }
        }

        public void Close()
        {
            isRunning = false;
            if(listener != null)
            {
                if(listener.Connected) {
                    listener.Shutdown(SocketShutdown.Both);
                }
                listener.Close();
                listener = null;
            }
        }

        ~Http()
        {
            Close();
        }

        public string Name()
        {
            return "[" + Thread.CurrentThread.GetHashCode() + "] Server.Html";
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        public bool HasError
        {
            get
            {
                return hasError;
            }
        }
    }
}
