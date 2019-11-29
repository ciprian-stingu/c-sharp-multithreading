
namespace TestThreads.Factory
{
    static class Interpreter
    {
        static public Interface.Interpreter Get(Util.Settings.InterpreterType type)
        {
            Interface.Interpreter instance = null;
            switch(type)
            {
                case Util.Settings.InterpreterType.Telnet:
                    instance = new TestThreads.Interpreter.Telnet();
                    break;
                default:
                    instance = new TestThreads.Interpreter.Html();
                    break;
            }
            return instance;
        }
    }
}
