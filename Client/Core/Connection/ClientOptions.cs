

using IotSystem.Core.ThreadManagement;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: ClientOptions.cs
* Created date:2022/6/4 12:35 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem
{
    public class ClientOptions
    {
        public string DbServerName { get; set; }
        public string DatabaseName { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public int DbPort { get; set; }
        public int DbConnectionTimeOut { get; set; }
        public int DbCommandTimeOut { get; set; }
        public string ClientId { get; set; }
        public string Broker { get; set; }
        public int Port { get; set; }
        public string SubscriberTopic { get; set; }
        public string PublisherTopic { get; set; }
        public byte QoSLevel { get; set; }
        public string TypeData { get; set; }
        public string TypeTime { get; set; }
        public bool IsClearSection { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int TimeCheckConnect { get; set; }

        public IDecodeDataThread iDecodeDataThread { get; set; }
        public IDatabaseConnectionThread iDatabaseConnectionThread { get; set; }
        public IInsertDataThread iInsertDataThread { get; set; }
        public IPublishMessageThread iPublishMessageThread { get; set; }
    }
}
