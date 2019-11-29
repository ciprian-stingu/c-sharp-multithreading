using System.Text;
using System.Threading;

namespace TestThreads.Interpreter
{
    class Telnet : Interface.Interpreter, Interface.Named
    {
        public byte[] Process(string data)
        {
            return Encoding.ASCII.GetBytes("");
        }

        public string Name()
        {
            return "[" + Thread.CurrentThread.GetHashCode() + "] Interpreter.Telnet";
        }
    }
}
