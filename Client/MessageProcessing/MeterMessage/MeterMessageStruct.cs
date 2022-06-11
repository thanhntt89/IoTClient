using IotSystem.Core.Utils;
using System;
using System.Collections.Generic;
using static IotSystem.MessageProcessing.MessageStructure.FieldBase;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: AlarmMessageStruct.cs
* Created date:2022/6/8 8:55 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.MessageProcessing.MeterMessage
{
    public class RuntimeCollection : List<RuntimeStruct>
    {
        public FieldStruct RawTime { get; set; }

        public string Time
        {
            get
            {
                if (RawTime.FieldBytes == null)
                    return string.Empty;

                int year = 2000 + RawTime.Data[0];
                int month = RawTime.Data[1];
                int day = RawTime.Data[2];
                int hour = RawTime.Data[3];
                int min = RawTime.Data[4];
                int sec = RawTime.Data[5];
                DateTime dt = new DateTime(year, month, day, hour, min, sec);
                return dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        private byte Crc { get; set; }
       
        private int DataLength
        {
            get
            {
                int dataLength = RawTime.TotalBytes;
                //This = List<RuntimeStruct>
                foreach (var field in this)
                {
                    dataLength += field.TotalBytes;
                }

                return dataLength;
            }
        }

        public byte[] Data
        {
            get
            {
                //Total Bytes = Sum(Field) + Crc(1byte)
                byte[] data = new byte[DataLength + 1];
                int offSet = 0;
                //Add Time              
                Buffer.BlockCopy(RawTime.FieldBytes, 0, data, offSet, RawTime.TotalBytes);
                offSet += RawTime.TotalBytes;

                //Add DeviceData
                foreach (var field in this)
                {
                    Buffer.BlockCopy(field.Data, 0, data, offSet, field.TotalBytes);
                    offSet += field.TotalBytes;
                }

                //Add Crc
                Crc = ByteUtil.CalCheckSum(data);
                //Crc
                Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);

                return data;
            }
        }
    }

    public class AlarmCollection : List<AlarmStruct>
    {
        public FieldStruct RawTime { get; set; }

        public string Time
        {
            get
            {
                if (RawTime.FieldBytes == null)
                    return string.Empty;

                int year = 2000 + RawTime.Data[0];
                int month = RawTime.Data[1];
                int day = RawTime.Data[2];
                int hour = RawTime.Data[3];
                int min = RawTime.Data[4];
                int sec = RawTime.Data[5];
                DateTime dt = new DateTime(year, month, day, hour, min, sec);
                return dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }
        private byte Crc { get; set; }
       
        private int DataLength
        {
            get
            {
                int dataLength = RawTime.TotalBytes;
                foreach (var field in this)
                {
                    dataLength += field.TotalBytes;
                }

                return dataLength;
            }
        }

        public byte[] Data
        {
            get
            {
                //DataLength = Sum(Fields) + Crc(1byte)
                byte[] data = new byte[DataLength + 1];
                int offSet = 0;
                //Add Time              
                Buffer.BlockCopy(RawTime.Data, 0, data, offSet, RawTime.TotalBytes);
                offSet += RawTime.TotalBytes;

                //Add DeviceData
                foreach (var field in this)
                {
                    Buffer.BlockCopy(field.Data, 0, data, offSet, field.TotalBytes);
                    offSet += field.TotalBytes;
                }

                //Add Crc
                Crc = ByteUtil.CalCheckSum(data);
                //Crc
                Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);

                return data;
            }
        }
    }

    public struct RuntimeStruct
    {
        public FieldStruct RawDeviceNo { get; set; }
        public FieldStruct RawTemp1 { get; set; }
        public FieldStruct RawTemp2 { get; set; }
        public FieldStruct RawRssi { get; set; }
        public FieldStruct RawLowBattery { get; set; }
        public FieldStruct RawHummidity { get; set; }

        /// <summary>
        /// Total bytes = Sum(Fields Bytes)
        /// </summary>
        public int TotalBytes => (RawDeviceNo.TotalBytes + RawTemp1.TotalBytes + RawTemp2.TotalBytes + RawRssi.TotalBytes + RawLowBattery.TotalBytes + RawHummidity.TotalBytes);

        public byte[] Data
        {
            get
            {                   
                if (TotalBytes == 0) 
                    return null;

                int offSet = 0;
                byte[] data = new byte[TotalBytes];
                //AddDevice
                if (RawDeviceNo.TotalBytes != 0)
                {
                    Buffer.BlockCopy(RawDeviceNo.FieldBytes, 0, data, offSet, RawDeviceNo.TotalBytes);
                    offSet += RawDeviceNo.TotalBytes;
                }
                //Add Temp1
                if (RawTemp1.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawTemp1.FieldBytes, 0, data, offSet, RawTemp1.TotalBytes);
                    offSet += RawTemp1.TotalBytes;
                }
                //Add Temp2
                if (RawTemp2.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawTemp2.FieldBytes, 0, data, offSet, RawTemp2.TotalBytes);
                    offSet += RawTemp2.TotalBytes;
                }
                if (RawRssi.TotalBytes > 0)
                {
                    //Add Rssi
                    Buffer.BlockCopy(RawRssi.FieldBytes, 0, data, offSet, RawRssi.TotalBytes);
                    offSet += RawRssi.TotalBytes;
                }
                if (RawLowBattery.TotalBytes > 0)
                {
                    //Add LowBattery
                    Buffer.BlockCopy(RawLowBattery.FieldBytes, 0, data, offSet, RawLowBattery.TotalBytes);
                    offSet += RawLowBattery.TotalBytes;
                }
                if (RawHummidity.TotalBytes > 0)
                {
                    //Add Hummidity
                    Buffer.BlockCopy(RawHummidity.FieldBytes, 0, data, offSet, RawHummidity.TotalBytes);
                    offSet += RawHummidity.TotalBytes;
                }
                return data;
            }
        }

        public int DeviceCode
        {
            get
            {
                return ByteUtil.ToInt(RawDeviceNo.Data);
            }
        }

        public float? Temp1
        {
            get
            {
                if (RawTemp1.Data == null)
                    return null;
                return ByteUtil.ToInt(RawTemp1.Data);
            }
        }
        public float? Temp2
        {
            get
            {
                if (RawTemp2.Data == null)
                    return null;
                return ByteUtil.ToInt(RawTemp2.Data);
            }
        }
        public double? Rssi
        {
            get
            {
                if (RawRssi.Data == null)
                    return null;
                return ByteUtil.ConvertByteArrayToInt32(RawRssi.Data);
            }
        }
        public float? LowBattery
        {
            get
            {
                if (RawLowBattery.Data == null)
                    return null;
                return ByteUtil.ToInt(RawLowBattery.Data);
            }
        }
        public float? Hummidity
        {
            get
            {
                if (RawHummidity.Data == null)
                    return null;
                return ByteUtil.ToInt(RawHummidity.Data);
            }
        }
    }

    public struct AlarmStruct
    {
        public FieldStruct RawDeviceNo { get; set; }
        public FieldStruct RawTemp1 { get; set; }
        public FieldStruct RawTemp2 { get; set; }
        public FieldStruct RawRssi { get; set; }
        public FieldStruct RawLowBattery { get; set; }
        public FieldStruct RawHummidity { get; set; }
        public FieldStruct RawAlarmTemp1 { get; set; }
        public FieldStruct RawAlarmTemp2 { get; set; }
        public FieldStruct RawAlarmBattery { get; set; }
        public FieldStruct RawAlarmHummidity { get; set; }
        public FieldStruct RawAlarmLigth { get; set; }

        public int TotalBytes => (RawDeviceNo.TotalBytes + RawTemp1.TotalBytes + RawTemp2.TotalBytes + RawRssi.TotalBytes + RawLowBattery.TotalBytes + RawHummidity.TotalBytes + RawAlarmTemp1.TotalBytes + RawAlarmTemp2.TotalBytes + RawAlarmBattery.TotalBytes + RawAlarmHummidity.TotalBytes + RawAlarmLigth.TotalBytes);

        public byte[] Data
        {
            get
            {
                if (TotalBytes == 0) 
                    return null;
                int offSet = 0;                
                byte[] data = new byte[TotalBytes];
                //AddDevice
                if (RawDeviceNo.TotalBytes > 0)
                {                    
                    Buffer.BlockCopy(RawDeviceNo.FieldBytes, 0, data, offSet, RawDeviceNo.TotalBytes);
                    offSet += RawDeviceNo.TotalBytes;
                }
                //Add Temp1
                if (RawTemp1.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawTemp1.FieldBytes, 0, data, offSet, RawTemp1.TotalBytes);
                    offSet += RawTemp1.TotalBytes;
                }
                //Add Temp2
                if (RawTemp2.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawTemp2.FieldBytes, 0, data, offSet, RawTemp2.TotalBytes);
                    offSet += RawTemp2.TotalBytes;
                }
                //Add Rssi
                if (RawRssi.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawRssi.FieldBytes, 0, data, offSet, RawRssi.TotalBytes);
                    offSet += RawRssi.TotalBytes;
                }
                //Add LowBattery
                if (RawLowBattery.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawLowBattery.FieldBytes, 0, data, offSet, RawLowBattery.TotalBytes);
                    offSet += RawLowBattery.TotalBytes;
                }
                //Add Hummidity
                if (RawHummidity.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawHummidity.FieldBytes, 0, data, offSet, RawHummidity.TotalBytes);
                    offSet += RawHummidity.TotalBytes;
                }
                //Add Alarm_Temp1
                if (RawAlarmTemp1.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawAlarmTemp1.FieldBytes, 0, data, offSet, RawAlarmTemp1.TotalBytes);
                    offSet += RawAlarmTemp1.TotalBytes;
                }
                //Add Alarm_Temp2
                if (RawAlarmTemp2.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawAlarmTemp2.FieldBytes, 0, data, offSet, RawAlarmTemp2.TotalBytes);
                    offSet += RawAlarmTemp2.TotalBytes;
                }
                //Add Alarm_Battery
                if (RawAlarmBattery.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawAlarmBattery.FieldBytes, 0, data, offSet, RawAlarmBattery.TotalBytes);
                    offSet += RawAlarmBattery.TotalBytes;
                }
                //Add Alarm_Hummidity
                if (RawAlarmHummidity.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawAlarmHummidity.FieldBytes, 0, data, offSet, RawAlarmHummidity.TotalBytes);
                    offSet += RawAlarmHummidity.TotalBytes;
                }
                //Add Alarm_Light
                if (RawAlarmLigth.TotalBytes > 0)
                {
                    Buffer.BlockCopy(RawAlarmLigth.FieldBytes, 0, data, offSet, RawAlarmLigth.TotalBytes);
                    offSet += RawAlarmLigth.TotalBytes;
                }
                return data;
            }
        }

        public int DeviceCode
        {
            get
            {
                return ByteUtil.ToInt(RawDeviceNo.Data);
            }
        }
        public float? Temp1
        {
            get
            {
                if (RawTemp1.Data == null)
                    return null;
                return ByteUtil.ToInt(RawTemp1.Data);
            }
        }
        public float? Temp2
        {
            get
            {
                if (RawTemp2.Data == null)
                    return null;
                return ByteUtil.ToInt(RawTemp2.Data);
            }
        }
        public double? Rssi
        {
            get
            {
                if (RawRssi.Data == null)
                    return null;
                return ByteUtil.ConvertByteArrayToInt32(RawRssi.Data);
            }
        }
        public float? LowBattery
        {
            get
            {
                if (RawLowBattery.Data == null)
                    return null;
                return ByteUtil.ToInt(RawLowBattery.Data);
            }
        }
        public float? Hummidity
        {
            get
            {
                if (RawHummidity.Data == null)
                    return null;
                return ByteUtil.ToInt(RawHummidity.Data);
            }
        }
        public int? AlarmTemp1
        {
            get
            {
                if (RawAlarmTemp1.Data == null)
                    return null;
                return ByteUtil.ToInt(RawAlarmTemp1.Data);
            }
        }
        public int? AlarmTemp2
        {
            get
            {
                if (RawAlarmTemp2.Data == null)
                    return null;
                return ByteUtil.ToInt(RawAlarmTemp2.Data);
            }
        }
        public int? AlarmBattery
        {
            get
            {
                if (RawAlarmBattery.Data == null)
                    return null;
                return ByteUtil.ToInt(RawAlarmBattery.Data);
            }
        }
        public int? AlarmHummidity
        {
            get
            {
                if (RawAlarmHummidity.Data == null)
                    return null;
                return ByteUtil.ToInt(RawAlarmHummidity.Data);
            }
        }
        public int? AlarmLigth
        {
            get
            {
                if (RawAlarmLigth.Data == null)
                    return null;
                return ByteUtil.ToInt(RawAlarmLigth.Data);
            }
        }
    }

    public struct GlexOperationStruct
    {
        public FieldStruct RawTime { get; set; }
        public FieldStruct Value1 { get; set; }
        public FieldStruct Value2 { get; set; }
    }
}
