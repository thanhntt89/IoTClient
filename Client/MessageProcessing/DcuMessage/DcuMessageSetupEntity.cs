/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: DcuMessageSetupEntity.cs
* Created date:2022/6/9 10:39 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/

namespace IotSystem.MessageProcessing.DcuMessage
{
    public class DcuMessageSetupEntity
    {
        public float TempHigh { get; set; }
        public float TempLow { get; set; }
        public float HumHigh { get; set; }
        public float HumLow { get; set; }
        public float TimeUpdate { get; set; }
        public float TimeSample { get; set; }
    }
}
