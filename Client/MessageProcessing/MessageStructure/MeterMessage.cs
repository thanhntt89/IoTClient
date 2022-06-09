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
        public DateTime Time { get; set; }
        public string DeviceNo { get; set; }
        public int Temp1 { get; set; }
        public int Temp2 { get; set; }
        public float Rssi { get; set; }
        public float LowBettery { get; set; }
        public float Hummidity { get; set; }
    }
}
