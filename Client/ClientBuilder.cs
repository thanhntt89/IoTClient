
using IotSystem.DataProcessing;
/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: ClientBuilder.cs
* Created date:2022/5/27 1:12 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem
{
    public class ClientBuilder
    {
        private ClientOptions ClientOptions;

        public ClientBuilder()
        {
            ClientOptions = new ClientOptions();
        }

        //Database
        public ClientBuilder AddDatabaseServer(string serverName)
        {
            ClientOptions.DbServerName = serverName;
            return this;
        }
        public ClientBuilder AddDatabaseName(string databaseName)
        {
            ClientOptions.DatabaseName = databaseName;
            return this;
        }
        public ClientBuilder AddDbUserName(string userName)
        {
            ClientOptions.DbUserName = userName;
            return this;
        }
        public ClientBuilder AddDbPassword(string password)
        {
            ClientOptions.DbPassword = password;
            return this;
        }
        public ClientBuilder AddDbPort(int port)
        {
            ClientOptions.DbPort = port;
            return this;
        }
        public ClientBuilder AddDbConnectionTimeOut(int timeOut)
        {
            ClientOptions.DbConnectionTimeOut = timeOut;
            return this;
        }
        public ClientBuilder AddDbCommandTimeOut(int timeOut)
        {
            ClientOptions.DbCommandTimeOut = timeOut;
            return this;
        }
        public ClientBuilder AddClientId(string clientId)
        {
            ClientOptions.ClientId = clientId;
            return this;
        }
        public ClientBuilder AddUserName(string userName)
        {
            ClientOptions.UserName = userName;
            return this;
        }
        public ClientBuilder AddPassword(string password)
        {
            ClientOptions.Password = password;
            return this;
        }
        public ClientBuilder AddWillTopic(string subcriberTopic)
        {
            ClientOptions.SubscriberTopic = subcriberTopic;
            return this;
        }
        public ClientBuilder AddWillPublisherTopic(string willPubishTopic)
        {
            ClientOptions.PublisherTopic = willPubishTopic;
            return this;
        }
        public ClientBuilder AddWillQosLevel(int willQosLevel)
        {
            ClientOptions.QoSLevel = (byte)willQosLevel;
            return this;
        }
        public ClientBuilder AddBroker(string broker)
        {
            ClientOptions.Broker = broker;
            return this;
        }
        public ClientBuilder AddTypeData(string typeData)
        {
            ClientOptions.TypeData = typeData;
            return this;
        }
        public ClientBuilder AddTypeTime(string typeTime)
        {
            ClientOptions.TypeTime = typeTime;
            return this;
        }
        public ClientBuilder AddTimeCheckConnect(int timeCheckConnect)
        {
            ClientOptions.TimeCheckConnect = timeCheckConnect;
            return this;
        }
        public ClientBuilder AddPort(int port)
        {
            ClientOptions.Port = port;
            return this;
        }
        public Client Build()
        {
            return new Client(ClientOptions, new DecodeMessageDataThread(), SingletonDatabaseConnection.Instance);
        }
    }
}
