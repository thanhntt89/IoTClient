﻿using System;
using System.IO;
using System.Reflection;

namespace IotClient
{
    public class Contants
    {
        /// <summary>
        /// Root Folder
        /// </summary>
        private static string ROOT_FOLDER = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        /// <summary>
        /// Get current setting file
        /// </summary>
        public static string SettingPath = string.Format("{0}\\Settings.json", ROOT_FOLDER);

        /// <summary>
        /// Error log
        /// </summary>
        public static string ERROR_LOG_SYSTEM_PATH = string.Format("{0}\\Error.Log", ROOT_FOLDER);

        /// <summary>
        /// INFOR log system
        /// </summary>
        public static string INFO_LOG_SYSTEM_PATH = string.Format("{0}\\Info.Log", ROOT_FOLDER);



        public static string CURRENT_TIME = DateTime.Now.ToString("yyMMddHHmmss");

        private Contants()
        {

        }
    }
}
