/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: IDatabaseConnection.cs
* Created date:2022/6/3 1:09 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.Core.ThreadManagement
{
    public interface IDatabaseConnectionThread
    {
        void ShowMessage(DelegateShowMessage showMessage);
        void SqlConnectionStatus(DelegateSqlConnection sqlConnection);
        bool CheckDatabaseConnect(string serverName, string databaseName, string userName, string password, int port, int commandTimeOut, int connectionTimeOut);
        void ThreadCheckConnection(CancellationToken cancellation);
    }
}
