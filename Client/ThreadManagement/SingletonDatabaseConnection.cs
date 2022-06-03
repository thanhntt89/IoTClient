/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonDatabaseConnection.cs
* Created date:2022/6/1 2:51 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.ThreadManagement;
using SqlHelper;
using System;
using System.Reflection;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.DataProcessing
{
    public class SingletonDatabaseConnection: IDatabaseConnectionThread
    {
        public event DelegateShowMessage eventShowMessage;
        public event DelegateSqlConnection eventSqlConnectionStatus;
          
        private static IDatabaseConnectionThread _instance;
        
        private static readonly object objLock = new object();
        
        private const int TIME_CHECK_CONNECTION = 60000;//1 min

        public static IDatabaseConnectionThread Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (objLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SingletonDatabaseConnection();
                        }
                    }
                }

                return _instance;
            }
        }

        static SingletonDatabaseConnection()
        {

        }
        private SingletonDatabaseConnection()
        {

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
            eventShowMessage?.Invoke($"ThreadCheckConnection: Started!!!");
            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    eventShowMessage?.Invoke($"ThreadCheckConnection: Stopped!!!");
                    break;
                }
                IsConnected = SqlHelpers.CheckConnectionString();
                
                //Send event connection status
                eventSqlConnectionStatus?.Invoke(IsConnected);

                Thread.Sleep(TIME_CHECK_CONNECTION);
            }
        }

        void IDatabaseConnectionThread.ShowMessage(DelegateShowMessage showMessage)
        {
            eventShowMessage += showMessage;
        }

        public void SqlConnectionStatus(DelegateSqlConnection sqlConnection)
        {
            eventSqlConnectionStatus += sqlConnection;
        }
    }
}
