
namespace TestThreads.Factory
{
    static class Logger
    {
        static private Interface.Logger instance = null;

        public static Interface.Logger Get()
        {
            if(instance == null)
            {
                switch (Util.Settings.CurrentLoggerType)
                {
                    case Util.Settings.LoggerType.File:
                        instance = new TestThreads.Logger.File(Util.Utilities.GetLogFileName());
                        break;
                    default:
                        instance = new TestThreads.Logger.Console();
                        break;
                }
            }
            return instance;
        }
    }
}
