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
        TIME = 0x01,
        DEVICE_NO = 0x02,
        TEMP1 = 0x03,
        TEMP2 = 0x04,
        RSSI = 0x05,
        LOW_BATTERY = 0x06,
        HUMMIDITY = 0x07,
        ALARM_TEMP1 = 0xA3,
        ALARM_TEMP2 = 0xA4,
        ALARM_BATTERY = 0xA6,
        ALARM_HIMIDITY = 0xA7,
        ALARM_LIGTH = 0xAA,
        TEMP_HIGH = 0xE1,
        TEMP_LOW = 0xE2,
        HUM_HIGH = 0xE3,
        HUM_LOW = 0xE4,
        TIME_UPDATE = 0xEB,
        TIME_SAMPLE = 0xEC
    }
}
