/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: IInsertDataThread.cs
* Created date:2022/6/4 11:13 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.Core.ThreadManagement
{
    public interface IDatabaseProcessingThread
    {
        void ShowMessage(DelegateShowMessage showMessage);       
        void InsertData(CancellationToken cancellation);
    }
}
