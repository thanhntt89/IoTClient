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
        UNKNOW=0xFF,
        Time = 0x01,
        DeviceNo = 0x02,
        Temp1 = 0x03,
        Temp2 = 0x04,
        Rssi = 0x05,
        LowBattery = 0x06,
        Hummidity = 0x07,
        Alarm_Temp1 = 0xA3,
        Alarm_Temp2 = 0xA4,
        Alarm_Battery = 0xA6,
        Alarm_Hummidity = 0xA7,
        Alarm_Light = 0xAA,
        Temp_High = 0xE1,
        Temp_Low = 0xE2,
        Hum_High = 0xE3,
        Hum_Low = 0xE4,
        Time_Update = 0xEB,
        Time_Sample = 0xEC
    }
}
