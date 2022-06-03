

using System.Threading;
using static IotSystem.ClientEvent;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: IDecodeDataThread.cs
* Created date:2022/6/3 11:47 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.ThreadManagement
{
    public interface IDecodeDataThread
    {
        void ShowMessage(DelegateShowMessage showMessageEvent);
        void ThreadDecodeByTraffic(CancellationToken cancellation);
        void ThreadDecode(CancellationToken cancellation);
    }
}
