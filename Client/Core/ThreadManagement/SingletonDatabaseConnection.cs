/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonDatabaseConnection.cs
* Created date:2022/6/1 2:51 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.Core.ThreadManagement
{
    public class SingletonDatabaseConnection: IDatabaseConnectionThread
    {
        public event DelegateShowMessage EventShowMessage;
        public event DelegateSqlConnection EventSqlConnectionStatus;
        private const int TIME_CHECK_CONNECTION = 60000;//1 min
        private static IDatabaseConnectionThread _instance;        
        private static readonly object objLock = new object();   

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
            EventShowMessage?.Invoke($"ThreadCheckConnection: Started!!!");
            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    EventShowMessage?.Invoke($"ThreadCheckConnection: Stopped!!!");
                    break;
                }

                IsConnected = SqlHelpers.CheckConnectionString();
                
                //Send event connection status
                EventSqlConnectionStatus?.Invoke(IsConnected);

                Thread.Sleep(TIME_CHECK_CONNECTION);
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }

        public void SqlConnectionStatus(DelegateSqlConnection sqlConnection)
        {
            EventSqlConnectionStatus += sqlConnection;
        }
    }
}
