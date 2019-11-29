using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TestThreads.Handler
{
    class Telnet : AbstractHandler, Interface.Handler, Interface.Named
    {
        private bool isRunning;
        private string currentFolder;

        public Telnet(Socket clientSocket) : base(clientSocket)
        {
            interpreter = Factory.Interpreter.Get(Util.Settings.InterpreterType.Telnet);
            currentFolder = Util.Utilities.GetCurrentFolder();
        }

        public void Handle()
        {
            clientSocket.Send(GetHelloMessage());

            isRunning = true;
            while(isRunning)
            {
                string command = GetCommand().Trim();
                Factory.Logger.Get().Log(Name() + " -> Received command: " + command);
                ProcessCommand(command);
            }

            byte[] message = Encoding.ASCII.GetBytes("Bye\r\n");
            clientSocket.Send(message);

            CloseSocket();
            Thread.CurrentThread.Join();
        }

        ~Telnet()
        {

        }

        public string Name()
        {
            return "[" + Thread.CurrentThread.GetHashCode() + "] Handler.Telnet";
        }

        private byte[] GetHelloMessage()
        {
            return Encoding.ASCII.GetBytes("Welcome\r\nServer time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\nType 'help' for commands list, and 'quit' to close the connection.\r\n");
        }

        private byte[] GetHelpMessage()
        {
            StringBuilder help = new StringBuilder();
            help.Append("Following command are available:\r\n");
            help.Append("dir - list current folder content\r\n");
            help.Append("cd - change directory\r\n");
            help.Append("cat - display file content\r\n");
            help.Append("quit - to close the session\r\n");

            return Encoding.ASCII.GetBytes(help.ToString());
        }

        private string GetCommand()
        {
            byte[] bytes = new Byte[Util.Settings.MaxReceiveBufferSize];
            StringBuilder data = new StringBuilder();
            while (true)
            {
                int numByte = clientSocket.Receive(bytes);
                string tmpData = Encoding.ASCII.GetString(bytes, 0, numByte);
                data.Append(tmpData);
                if (tmpData.IndexOf('\r') >= 0 || tmpData.IndexOf('\n') >= 0)
                {
                    break;
                }
            }

            return data.ToString();
        }

        private void ProcessCommand(string command)
        {
            if(command == "quit")
            {
                isRunning = false;
            }
            else if (command == "help")
            {
                clientSocket.Send(GetHelpMessage());
            }
            else if(command == "dir")
            {
                clientSocket.Send(GetCurrentFolderContent());
            }
            else if (command.IndexOf("cd ") >= 0)
            {
                clientSocket.Send(ChangeDirectory(command.Substring(command.IndexOf("cd ") + 3).Trim()));
            }
            else if (command.IndexOf("cat ") >= 0)
            {
                clientSocket.Send(CatFile(command.Substring(command.IndexOf("cat ") + 4).Trim()));
            }
            else
            {
                byte[] message = Encoding.ASCII.GetBytes("Unknown command '" + command + "'\r\n");
                clientSocket.Send(message);
            }
        }

        private byte[] GetCurrentFolderContent()
        {
            StringBuilder files = new StringBuilder();
            string[] dirArray = Directory.GetDirectories(currentFolder);
            files.Append("<DIR>\t..\r\n");
            foreach (string dir in dirArray)
            {
                files.Append("<DIR>\t" + Path.GetFileName(dir) + "\r\n");
            }
            string[] fileArray = Directory.GetFiles(currentFolder);
            foreach (string file in fileArray)
            {
                FileInfo f = new FileInfo(file);
                files.Append(f.Length + "\t" + Path.GetFileName(file) + "\r\n");
            }
            return Encoding.ASCII.GetBytes(files.ToString());
        }

        private byte[] ChangeDirectory(string folder)
        {
            string newCurrentFolder = currentFolder;
            if(folder == "..") {
                newCurrentFolder = newCurrentFolder.Substring(0, newCurrentFolder.LastIndexOf(@"\"));
            }
            else {
                newCurrentFolder = Path.Combine(newCurrentFolder, folder);
            }
            
            if(!Directory.Exists(newCurrentFolder))
            {
                return Encoding.ASCII.GetBytes("Not a directory.\r\n");
            }
            currentFolder = newCurrentFolder;
            return Encoding.ASCII.GetBytes("OK\r\n");
        }

        private byte[] CatFile(string file)
        {
            string realFileName = Path.Combine(currentFolder, file);
            if (!File.Exists(realFileName))
            {
                return Encoding.ASCII.GetBytes("Not a file.\r\n");
            }
            try
            {
                string fileData = File.ReadAllText(realFileName);
            }
            catch(Exception e)
            {
                return Encoding.ASCII.GetBytes(e.Message);
            }
            return Encoding.ASCII.GetBytes(File.ReadAllText(realFileName));
        }
    }
}
