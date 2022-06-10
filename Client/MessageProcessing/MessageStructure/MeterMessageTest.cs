/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: MeterMessageTest.cs
* Created date:2022/6/10 3:32 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.Core;
using System;

namespace IotSystem.MessageProcessing.MessageStructure
{
    public class MeterMessageTest
    {
        public static MessageBase CreateRuntimeMessage()
        {
            MessageBase message = new MessageBase();
            byte[] dataDateTime = new byte[6];
            dataDateTime[0] = (byte)int.Parse(DateTime.Now.ToString("yy"));
            dataDateTime[1] = (byte)DateTime.Now.Month;
            dataDateTime[2] = (byte)DateTime.Now.Day;
            dataDateTime[3] = (byte)int.Parse(DateTime.Now.ToString("HH"));
            dataDateTime[4] = (byte)DateTime.Now.Minute;
            dataDateTime[5] = (byte)DateTime.Now.Second;

            RuntimeCollection runtimes = new RuntimeCollection();
            runtimes.Time = new FieldBase.FieldStruct()
            {
                Obis = new byte[1] { (byte)EnumObis.Time },
                Data = dataDateTime,
                DataLength = new byte[1] { (byte)dataDateTime.Length }
            };

            //Device 11
            runtimes.Add(new RuntimeStruct()
            {
                FieldDeviceNo = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.DeviceNo },
                    Data = new byte[2] { 1, 1 },
                    DataLength = new byte[1] { 2 }
                },
                FieldTemp1 = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Temp1 },
                    Data = new byte[2] { 3, 4 },
                    DataLength = new byte[1] { 2 }
                },
                FieldTemp2 = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Temp2 },
                    Data = new byte[2] { 5, 6 },
                    DataLength = new byte[1] { 2 }
                },
                FieldRssi = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Rssi },
                    Data = new byte[2] { 7, 8 },
                    DataLength = new byte[1] { 2 }
                },
                FieldHummidity = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Hummidity },
                    Data = new byte[2] { 9, 10 },
                    DataLength = new byte[1] { 2 }
                },
                FieldLowBattery = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.LowBattery },
                    Data = new byte[2] { 11, 12 },
                    DataLength = new byte[1] { 2 }
                },
            });
            //Device 21
            runtimes.Add(new RuntimeStruct()
            {
                FieldDeviceNo = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.DeviceNo },
                    Data = new byte[2] { 2, 1 },
                    DataLength = new byte[1] { 2 }
                },
                FieldTemp1 = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Temp1 },
                    Data = new byte[2] { 3, 4 },
                    DataLength = new byte[1] { 2 }
                },
                FieldTemp2 = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Temp2 },
                    Data = new byte[2] { 5, 6 },
                    DataLength = new byte[1] { 2 }
                },
                FieldRssi = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Rssi },
                    Data = new byte[2] { 7, 8 },
                    DataLength = new byte[1] { 2 }
                },
                FieldHummidity = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.Hummidity },
                    Data = new byte[2] { 9, 10 },
                    DataLength = new byte[1] { 2 }
                },
                FieldLowBattery = new FieldBase.FieldStruct()
                {
                    Obis = new byte[1] { (byte)EnumObis.LowBattery },
                    Data = new byte[2] { 11, 12 },
                    DataLength = new byte[1] { 2 }
                },
            });

            message.Message = runtimes.Data;
            message.Topic = "PHH/Customer/Sub_Local/RunTime/DCU001";
            return message;
        }

        public MessageBase CreateAlarmMessage()
        {
            MessageBase message = new MessageBase();


            return message;
        }
    }
}
