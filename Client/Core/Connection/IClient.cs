/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: IClient.cs
* Created date:2022/5/27 12:54 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using static IotSystem.ClientEvent;

namespace IotSystem.Core
{
    public interface IClient
    {
        void ShowMessage(DelegateShowMessage showMessage);      
        void Start();
        void Stop(bool isUserStop);
        void Exit();
    }
}
