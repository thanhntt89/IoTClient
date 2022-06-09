

using static IotSystem.MessageProcessing.MessageStructure.FieldBase;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: AlarmMessageStruct.cs
* Created date:2022/6/8 8:55 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.MessageProcessing.MessageStructure
{
    public struct RuntimeStruct
    {
        public FieldStruct Time { get; set; }
        public FieldStruct DeviceNo { get; set; }
        public FieldStruct Temp1 { get; set; }
        public FieldStruct Temp2 { get; set; }
        public FieldStruct Rssi { get; set; }
        public FieldStruct LowBattery { get; set; }
        public FieldStruct Hummidity { get; set; }
    }

    public struct AlarmStruct
    {
        public FieldStruct Time { get; set; }
        public FieldStruct DeviceNo { get; set; }
        public FieldStruct Temp1 { get; set; }
        public FieldStruct Temp2 { get; set; }
        public FieldStruct Rssi { get; set; }
        public FieldStruct LowBattery { get; set; }
        public FieldStruct Hummidity { get; set; }
        public FieldStruct AlarmTemp1 { get; set; }
        public FieldStruct Alarm_Temp2 { get; set; }
        public FieldStruct Alarm_Battery { get; set; }
        public FieldStruct Alarm_Hummidity { get; set; }
        public FieldStruct Alarm_Light { get; set; }
    }   
}
