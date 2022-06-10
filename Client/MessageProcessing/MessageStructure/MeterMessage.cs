/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: MeterMessage.cs
* Created date:2022/6/9 12:07 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System;

namespace IotSystem.MessageProcessing.MessageStructure
{
    public class MeterMessage
    {   


        public class RuntimeMessage
        {
            public DateTime Time { get; set; }
            public int DeviceNo { get; set; }
            public float Temp1 { get; set; }
            public float Temp2 { get; set; }
            public float Rssi { get; set; }
            public float LowBettery { get; set; }
            public float Hummidity { get; set; }
        }

        public class AlarmClass: RuntimeMessage
        {
            public float AlarmTemp1 { get; set; }
            public float AlarmTemp2 { get; set; }
            public float AlarmBattery { get; set; }
            public float AlarmHummidity { get; set; }
            public float AlarmLight { get; set; }            
        }       
    }
}
