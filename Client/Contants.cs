using System;
using System.IO;
using System.Reflection;

namespace IotClient
{
    public class Contants
    {
        /// <summary>
        /// Get current setting file
        /// </summary>
        public static string SettingPath = string.Format("{0}\\Settings.json", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

        public static string CURRENT_TIME = DateTime.Now.ToString("yyMMddHHmmss");

        private Contants()
        {

        }
    }
}
