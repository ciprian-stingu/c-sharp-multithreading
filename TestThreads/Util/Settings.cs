
namespace TestThreads.Util
{
    public static class Settings
    {
        public enum LoggerType : int { Console, File };
        public enum HandlerType : int { Http, Telnet };
        public enum InterpreterType : int { Http, Telnet };

        static private LoggerType currentLoggerType;
        static private int currentListeningPort;

        static Settings()
        {
            currentLoggerType = LoggerType.Console;
            currentListeningPort = 8080;
        }

        static public LoggerType CurrentLoggerType
        {
            get
            {
                return currentLoggerType;
            }
            set
            {
                currentLoggerType = value;
            }
        }

        static public int MaxReceiveBufferSize
        {
            get
            {
                return 0xFFFF;
            }
        }

        static public int MaxConnectionBacklogSize
        {
            get
            {
                return 0xFF;
            }
        }

        static public int CurrentListeningPort
        {
            get
            {
                return currentListeningPort;
            }
            set
            {
                currentListeningPort = value;
            }
        }

        static public int ThreadPoolMinConnections
        {
            get
            {
                return 50;
            }
        }
    }

   
}
