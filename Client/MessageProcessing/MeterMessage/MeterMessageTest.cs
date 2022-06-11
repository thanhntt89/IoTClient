/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: MeterMessageTest.cs
* Created date:2022/6/10 3:32 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.Core;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MessageStructure;
using System;
using static IotSystem.MessageProcessing.MessageStructure.FieldBase;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class MeterMessageTest
    {
        public static MessageBase CreateRuntimeMessage(string topic)
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
            runtimes.RawTime = new FieldStruct()
            {
                Obis = (byte)EnumObis.Time,
                Data = dataDateTime
            };

            //Device 11
            runtimes.Add(new RuntimeStruct()
            {
                RawDeviceNo = new FieldStruct()
                {
                    Obis = (byte)EnumObis.DeviceNo,
                    Data = ByteUtil.ToBytes(12345)
                },
                RawTemp1 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp1,
                    Data = new byte[2] { 3, 4 }
                },
                RawTemp2 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp2,
                    Data = new byte[2] { 5, 6 },
                },
                RawRssi = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Rssi,
                    Data = ByteUtil.ToBytes(2345)
                },
                RawHummidity = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Hummidity,
                    Data = new byte[2] { 9, 10 }
                },
                RawLowBattery = new FieldStruct()
                {
                    Obis = (byte)EnumObis.LowBattery,
                    Data = new byte[2] { 11, 12 }
                },
            });
            //Device 21
            runtimes.Add(new RuntimeStruct()
            {
                RawDeviceNo = new FieldBase.FieldStruct()
                {
                    Obis = (byte)EnumObis.DeviceNo,
                    Data = ByteUtil.ToBytes(667788)
                },
                RawTemp1 = new FieldBase.FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp1,
                    Data = new byte[2] { 3, 4 }
                },
                RawTemp2 = new FieldBase.FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp2,
                    Data = new byte[2] { 5, 6 }
                },
                RawRssi = new FieldBase.FieldStruct()
                {
                    Obis = (byte)EnumObis.Rssi,
                    Data = ByteUtil.ToBytes(6789)
                },
                RawHummidity = new FieldBase.FieldStruct()
                {
                    Obis = (byte)EnumObis.Hummidity,
                    Data = new byte[2] { 9, 10 }
                }
                ,
                RawLowBattery = new FieldBase.FieldStruct()
                {
                    Obis = (byte)EnumObis.LowBattery,
                    Data = new byte[2] { 11, 12 }
                }
            });
            
            message.Message = runtimes.Data;
            message.Topic = topic;
            return message;
        }

        public MessageBase CreateAlarmMessage()
        {
            MessageBase message = new MessageBase();
            AlarmCollection alarms = new AlarmCollection();

            byte[] dataDateTime = new byte[6];
            dataDateTime[0] = (byte)int.Parse(DateTime.Now.ToString("yy"));
            dataDateTime[1] = (byte)DateTime.Now.Month;
            dataDateTime[2] = (byte)DateTime.Now.Day;
            dataDateTime[3] = (byte)int.Parse(DateTime.Now.ToString("HH"));
            dataDateTime[4] = (byte)DateTime.Now.Minute;
            dataDateTime[5] = (byte)DateTime.Now.Second;

            alarms.RawTime = new FieldStruct()
            {
                Obis = (byte)EnumObis.Time,
                Data = dataDateTime
            };

            alarms.Add(new AlarmStruct
            {
                RawDeviceNo = new FieldStruct()
                {
                    Obis = (byte)EnumObis.DeviceNo,
                    Data = ByteUtil.ToBytes(12345)
                },
                RawTemp1 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp1,
                    Data = new byte[2] { 3, 4 }
                },
                RawTemp2 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp2,
                    Data = new byte[2] { 5, 6 },
                },
                RawRssi = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Rssi,
                    Data = ByteUtil.ToBytes(2345)
                },
                RawHummidity = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Hummidity,
                    Data = new byte[2] { 9, 10 }
                },
                RawLowBattery = new FieldStruct()
                {
                    Obis = (byte)EnumObis.LowBattery,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmTemp1 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmTemp1,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmTemp2 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmTemp2,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmBattery = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmBattery,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmHummidity = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmHummidity,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmLigth = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmLight,
                    Data = new byte[2] { 11, 12 }
                }
            });

            alarms.Add(new AlarmStruct
            {
                RawDeviceNo = new FieldStruct()
                {
                    Obis = (byte)EnumObis.DeviceNo,
                    Data = ByteUtil.ToBytes(986522)
                },
                RawTemp1 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp1,
                    Data = new byte[2] { 3, 4 }
                },
                RawTemp2 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Temp2,
                    Data = new byte[2] { 5, 6 },
                },
                RawRssi = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Rssi,
                    Data = ByteUtil.ToBytes(2345)
                },
                RawHummidity = new FieldStruct()
                {
                    Obis = (byte)EnumObis.Hummidity,
                    Data = new byte[2] { 9, 10 }
                },
                RawLowBattery = new FieldStruct()
                {
                    Obis = (byte)EnumObis.LowBattery,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmTemp1 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmTemp1,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmTemp2 = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmTemp2,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmBattery = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmBattery,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmHummidity = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmHummidity,
                    Data = new byte[2] { 11, 12 }
                },
                RawAlarmLigth = new FieldStruct()
                {
                    Obis = (byte)EnumObis.AlarmLight,
                    Data = new byte[2] { 11, 12 }
                }
            });

            return message;
        }
    }
}
