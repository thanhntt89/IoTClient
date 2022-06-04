using IotSystem.Core.ThreadManagement;
/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: ClientBuilder.cs
* Created date:2022/5/27 1:12 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.Core.Connection
{
    public class ClientBuilder
    {
        private ClientOptions clientOptions;

        public ClientBuilder()
        {
            clientOptions = new ClientOptions();
        }

        //Database
        public ClientBuilder AddDatabaseServer(string serverName)
        {
            clientOptions.DbServerName = serverName;
            return this;
        }
        public ClientBuilder AddDatabaseName(string databaseName)
        {
            clientOptions.DatabaseName = databaseName;
            return this;
        }
        public ClientBuilder AddDbUserName(string userName)
        {
            clientOptions.DbUserName = userName;
            return this;
        }
        public ClientBuilder AddDbPassword(string password)
        {
            clientOptions.DbPassword = password;
            return this;
        }
        public ClientBuilder AddDbPort(int port)
        {
            clientOptions.DbPort = port;
            return this;
        }
        public ClientBuilder AddDbConnectionTimeOut(int timeOut)
        {
            clientOptions.DbConnectionTimeOut = timeOut;
            return this;
        }
        public ClientBuilder AddDbCommandTimeOut(int timeOut)
        {
            clientOptions.DbCommandTimeOut = timeOut;
            return this;
        }
        public ClientBuilder AddClientId(string clientId)
        {
            clientOptions.ClientId = clientId;
            return this;
        }
        public ClientBuilder AddUserName(string userName)
        {
            clientOptions.UserName = userName;
            return this;
        }
        public ClientBuilder AddPassword(string password)
        {
            clientOptions.Password = password;
            return this;
        }
        public ClientBuilder AddWillTopic(string subcriberTopic)
        {
            clientOptions.SubscriberTopic = subcriberTopic;
            return this;
        }
        public ClientBuilder AddWillPublisherTopic(string willPubishTopic)
        {
            clientOptions.PublisherTopic = willPubishTopic;
            return this;
        }
        public ClientBuilder AddWillQosLevel(int willQosLevel)
        {
            clientOptions.QoSLevel = (byte)willQosLevel;
            return this;
        }
        public ClientBuilder AddBroker(string broker)
        {
            clientOptions.Broker = broker;
            return this;
        }
        public ClientBuilder AddTypeData(string typeData)
        {
            clientOptions.TypeData = typeData;
            return this;
        }
        public ClientBuilder AddTypeTime(string typeTime)
        {
            clientOptions.TypeTime = typeTime;
            return this;
        }
        public ClientBuilder AddTimeCheckConnect(int timeCheckConnect)
        {
            clientOptions.TimeCheckConnect = timeCheckConnect;
            return this;
        }
        public ClientBuilder AddPort(int port)
        {
            clientOptions.Port = port;
            return this;
        }
        public ClientBuilder AddIDecodeDataThread(IDecodeDataThread iDecodeDataThread)
        {
            clientOptions.iDecodeDataThread = iDecodeDataThread;
            return this;
        }
        public ClientBuilder AddIDatabaseConnectionThread(IDatabaseConnectionThread iDatabaseConnectionThread)
        {
            clientOptions.iDatabaseConnectionThread = iDatabaseConnectionThread;
            return this;
        }
        public ClientBuilder AddIInsertDataThread(IInsertDataThread iInsertDataThread)
        {
            clientOptions.iInsertDataThread = iInsertDataThread;
            return this;
        }
        public ClientBuilder AddIPublishMessageThread(IPublishMessageThread iPublishMessageThread)
        {
            clientOptions.iPublishMessageThread = iPublishMessageThread;
            return this;
        }
        public Client Build()
        {
            return new Client(clientOptions);
        }
    }
}
