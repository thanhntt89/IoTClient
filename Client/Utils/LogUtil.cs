using System;
using System.IO;
using System.Text;

namespace IotSystem.Utils
{
    public enum LogType
    {
        Info,
        Error
    }

    public class LogUtil
    {
        private static LogUtil intance;
        private static readonly object locker = new object();

        public void WriteLog(LogType logType, string message)
        {
            try
            {
                string logPath = logType == LogType.Info ? Constant.INFO_LOG_SYSTEM_PATH : Constant.ERROR_LOG_SYSTEM_PATH;

                if (string.IsNullOrEmpty(logPath))
                {
                    Console.WriteLine($"WriteLog-Path is empty!!!");
                    return;
                }
                if (!File.Exists(logPath))
                {
                    File.Create(logPath).Close();
                }

                WriteFile(logPath, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WriteLog-Exception: {ex.Message}");
            }
        }

        private void WriteFile(string filePath, string message)
        {
            //Using for mutil thread
            lock (locker)
            {
                using (FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(file, Encoding.Unicode))
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
            }
        }

        static LogUtil()
        {

        }

        private LogUtil()
        {

        }

        public static LogUtil Intance
        {
            get
            {
                if (intance == null)
                {
                    lock (locker)
                    {
                        if (intance == null)
                            intance = new LogUtil();
                    }
                }
                return intance;
            }
        }
    }
}
