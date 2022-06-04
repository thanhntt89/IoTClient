/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: IPublishMessageThread.cs
* Created date:2022/6/4 11:28 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.ThreadManagement
{
    public interface IPublishMessageThread
    {
        void ShowMessage(DelegateShowMessage showMessage);
        void PublishMessage(DelegatePublishMessage publishMessage);
        void ThreadDecode(CancellationToken cancellation);
    }
}
