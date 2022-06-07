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
    public class DatabaseConnectionThread: IDatabaseConnectionThread
    {
        public event DelegateShowMessage EventShowMessage;
        public event DelegateSqlConnection EventSqlConnectionStatus;
        private const int TIME_CHECK_CONNECTION = 60000;//1 min
        private DatabaseConfig DatabaseConfig { get; set; }      
       
        public DatabaseConnectionThread(DatabaseConfig databaseConfig)
        {
            DatabaseConfig = databaseConfig;
        }

        public bool IsConnected { get; set; }

        public bool CheckDatabaseConnect()
        {
            SqlHelpers.CreateConnectionString(new ConnectionInfo()
            {
                DatabaseName = DatabaseConfig.DatabaseName,
                ServerName = DatabaseConfig.Server,
                Password = DatabaseConfig.Password,
                UserName = DatabaseConfig.UserName,
                TimeOutCommand = DatabaseConfig.CommandTimeOut,
                TimeOutConnection = DatabaseConfig.ConnectionTimeOut,
                Port = DatabaseConfig.Port
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
