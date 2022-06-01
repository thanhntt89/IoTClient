/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: Constant.cs
* Created date:2022/5/27 2:28 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System;
using System.IO;
using System.Reflection;

namespace IotClient
{
    public class Constant
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

        private Constant()
        {

        }
    }
}
