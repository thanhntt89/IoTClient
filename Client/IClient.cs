/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: IClient.cs
* Created date:2022/5/27 12:54 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using static IotClient.ClientEvent;

namespace IotClient
{
    public interface IClient
    {
        void ShowMessage(DelegateShowMessage showMessage);      
        void Start();
        void Stop(bool isUserStop);
        void StopAllThread();
    }
}
