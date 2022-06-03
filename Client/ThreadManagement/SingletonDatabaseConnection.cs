/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonDatabaseConnection.cs
* Created date:2022/6/1 2:51 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using SqlHelper;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.DataProcessing
{
    public class SingletonDatabaseConnection
    {
        public event DelegateShowMessage ShowMessageEvent;
        public event DelegateSqlConnection eventSqlConnectionStaus;

        private static SingletonDatabaseConnection instance;
        private static readonly object objLock = new object();
        
        private const int TIME_CHECK_CONNECTION = 60000;//1 min

        static SingletonDatabaseConnection()
        {

        }
        private SingletonDatabaseConnection()
        {

        }

        public static SingletonDatabaseConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                            instance = new SingletonDatabaseConnection();
                    }
                }

                return instance;
            }
        }

        public bool IsConnected { get; set; }

        public bool CheckDatabaseConnect(string serverName, string databaseName, string userName, string password, int port, int commandTimeOut, int connectionTimeOut)
        {
            SqlHelpers.CreateConnectionString(new ConnectionInfo()
            {
                DatabaseName = databaseName,
                ServerName = serverName,
                Password = password,
                UserName = userName,
                TimeOutCommand = commandTimeOut,
                TimeOutConnection = connectionTimeOut
            });
            return SqlHelpers.CheckConnectionString();            
        }

        public void ThreadCheckConnection(CancellationToken cancellation)
        {
            ShowMessageEvent?.Invoke($"ThreadCheckConnection: Started!!!");
            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    ShowMessageEvent?.Invoke($"ThreadCheckConnection: Stopped!!!");
                    break;
                }
                IsConnected = SqlHelpers.CheckConnectionString();
                
                //Send event connection status
                eventSqlConnectionStaus?.Invoke(IsConnected);

                Thread.Sleep(TIME_CHECK_CONNECTION);
            }
        }
    }
}
