using System.Runtime.InteropServices;

namespace Logger
{
    public static class Logger
    {
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();

        private static readonly string baseDir = @"C:\Logs";
        private static readonly bool canWrite = true;
        private static readonly string logLevels = "INFO,DEBUG,WARNING,ERROR";
        public static void Log(LogLevel logLevel, string message)
        {
            if (CanWrite(logLevel))
            {
                WriteLog(logLevel, message);
            }
        }

        public static void Log(Exception exception, string message = null)
        {
            if (CanWrite(LogLevel.ERROR))
            {
                WriteErrorLog(exception, message);
            }
        }

        private static bool CanWrite(LogLevel logLevel)
        {
            return CanWriteLog() && CanWriteLogLevel(logLevel);
        }

        private static string LogLevelText(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.INFO: return "INFO";
                case LogLevel.DEBUG: return "DEBUG";
                case LogLevel.WARNING: return "WARNING";
                case LogLevel.ERROR: return "ERROR";
                default:
                    break;
            }
            return String.Empty;
        }

        private static bool CanWriteLog()
        {
            return canWrite;
        }

        private static bool CanWriteLogLevel(LogLevel logLevel)
        {
            string[] strLevels = logLevels.Split(',');
            string logLevelText = LogLevelText(logLevel);

            foreach (string level in strLevels)
            {
                if (level.ToUpper() == logLevelText)
                {
                    return true;
                }
            }

            return false;
        }

        private static void WriteLog(LogLevel logLevel, string message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(GetLogFilePath(), true);
                sw.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") +
                    " " +
                    "[" + LogLevelText(logLevel) + "]" +
                    "[" + GetCurrentWin32ThreadId() + "]");
                sw.WriteLine("Message: " + message);
                sw.WriteLine("------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
            }
        }

        private static void WriteErrorLog(Exception exception, string message = null)
        {
            try
            {
                StreamWriter sw = new StreamWriter(GetLogFilePath(), true);
                sw.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") +
                    " " +
                    "[" + LogLevelText(LogLevel.ERROR) + "]" +
                    "[" + GetCurrentWin32ThreadId() + "]" +
                    " " + (message != null ? "[Message: " + message + "]" : ""));

                sw.WriteLine("Error: " + exception.Message.ToString().Trim());
                sw.WriteLine("Stack Trace: " + exception.StackTrace.ToString().Trim());
                sw.WriteLine("------------------------------------------------------------------");

                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
            }
        }

        private static string GetLogFilePath()
        {
            try
            {
                string retFilePath = baseDir + "//" + DateTime.Now.ToString("yyyyMMdd") + "_log.txt";

                if (File.Exists(retFilePath) == true)
                {
                    return retFilePath;
                }
                else
                {
                    if (false == CheckDirectory(baseDir))
                    {
                        return string.Empty;
                    }

                    FileStream fs = new FileStream(retFilePath,
                          FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }

                return retFilePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                if (false == Directory.Exists(strLogPath))
                {
                    Directory.CreateDirectory(strLogPath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}