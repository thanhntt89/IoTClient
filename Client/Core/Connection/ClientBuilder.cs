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
        public ClientBuilder AddWillQosLevel(int willQosLevel)
        {
            clientOptions.QoSLevel = (byte)willQosLevel;
            return this;
        }
        public ClientBuilder AddIsClearSection(bool isClearSection)
        {
            clientOptions.IsClearSection = isClearSection;
            return this;
        }
        public ClientBuilder AddBroker(string broker)
        {
            clientOptions.Broker = broker;
            return this;
        }
        public ClientBuilder AddTypeData(string typeData)
        {
            clientOptions.TypeRunTime = typeData;
            return this;
        }
        public ClientBuilder AddTypeTime(string typeTime)
        {
            clientOptions.TypeTime = typeTime;
            return this;
        }
        public ClientBuilder AddTypeAlarm(string typeAlarm)
        {
            clientOptions.TypeAlarm = typeAlarm;
            return this;
        }
        public ClientBuilder AddTypeSetup(string typeSetup)
        {
            clientOptions.TypeSetup = typeSetup;
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
        public ClientBuilder AddIDatabaseProcessingThread(IDatabaseProcessingThread iInsertDataThread)
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
