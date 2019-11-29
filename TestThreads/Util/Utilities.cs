using System;
using System.Reflection;

namespace TestThreads.Util
{
    static public class Utilities
    {
        static Utilities()
        {

        }

        public static string GetLogFileName()
        {
            return GetCurrentFolder() + @"\TestThreads-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";
        }

        public static string GetCurrentFolder()
        {
            return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}
