/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: ClientEvent.cs
* Created date:2022/5/27 3:32 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/

namespace IotSystem
{
    public class ClientEvent
    {
        public delegate void DelegateShowMessage(string message);

        public delegate void DelegatePublishMessage(string topic, string content);
        public delegate void DelegateSqlConnection(bool status);
    }
}
