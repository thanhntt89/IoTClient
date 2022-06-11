/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: FieldEnum.cs
* Created date:2022/6/9 12:15 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/

namespace IotSystem.MessageProcessing.MessageStructure
{
    public enum EnumObis
    {
        UnKnow=0xFF,
        Time = 0x01,
        DeviceNo = 0x02,
        Temp1 = 0x03,
        Temp2 = 0x04,
        Rssi = 0x05,
        LowBattery = 0x06,
        Hummidity = 0x07,
        AlarmTemp1 = 0xA3,
        AlarmTemp2 = 0xA4,
        AlarmBattery = 0xA6,
        AlarmHummidity = 0xA7,
        AlarmLight = 0xAA,
        HighTemp = 0xE1,
        LowTemp = 0xE2,
        HighHummidity = 0xE3,
        LowHummidity = 0xE4,
        UpdateTime = 0xEB,
        SampleTime = 0xEC
    }
}
