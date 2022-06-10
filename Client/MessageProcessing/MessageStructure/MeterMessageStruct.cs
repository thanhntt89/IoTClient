using IotSystem.Core.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public class RuntimeCollection : List<RuntimeStruct>
    {
        public FieldStruct Time { get; set; }

        private byte Crc { get; set; }
        private int DataLength
        {
            get
            {
                int dataLength = Time.TotalBytes;
                foreach (var field in this)
                {
                    dataLength += field.Data.Length;
                }

                return dataLength;
            }
        }
        public byte[] Data
        {
            get
            {
                byte[] data = new byte[DataLength + 1];
                int offSet = 0;
                //Add Time              
                Buffer.BlockCopy(Time.FieldBytes, 0, data, offSet, Time.TotalBytes);
                offSet += Time.TotalBytes;

                //Add DeviceData
                foreach (var field in this)
                {
                    Buffer.BlockCopy(field.Data, 0, data, offSet, field.Data.Length);
                    offSet += field.Data.Length;
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
        public FieldStruct Time { get; set; }
        private byte Crc { get; set; }
        private int DataLength
        {
            get
            {
                int dataLength = Time.TotalBytes;
                foreach (var field in this)
                {
                    dataLength += field.Data.Length;
                }

                return dataLength;
            }
        }
        public byte[] Data
        {
            get
            {
                byte[] data = new byte[DataLength + 1];
                int offSet = 0;
                //Add Time              
                Buffer.BlockCopy(Time.Data, 0, data, offSet, Time.TotalBytes);
                offSet += Time.TotalBytes;

                //Add DeviceData
                foreach (var field in this)
                {
                    Buffer.BlockCopy(field.Data, 0, data, offSet, field.Data.Length);
                    offSet += field.Data.Length;
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

        //Total fields in Runtime message
        public int FiledCount
        {
            get
            {
                return 6;
            }
        }

        public byte[] Data
        {
            get
            {
                int offSet = 0;
                int buffLength = RawDeviceNo.TotalBytes + RawTemp1.TotalBytes + RawTemp2.TotalBytes + RawRssi.TotalBytes + RawLowBattery.TotalBytes + RawHummidity.TotalBytes;
                byte[] data = new byte[buffLength];
                //AddDevice
                if (RawDeviceNo.FieldBytes != null)
                {
                    Buffer.BlockCopy(RawDeviceNo.FieldBytes, 0, data, offSet, RawDeviceNo.TotalBytes);
                    offSet += RawDeviceNo.TotalBytes;
                }
                //Add Temp1
                if (RawTemp1.FieldBytes != null)
                {
                    Buffer.BlockCopy(RawTemp1.FieldBytes, 0, data, offSet, RawTemp1.TotalBytes);
                    offSet += RawTemp1.TotalBytes;
                }
                //Add Temp2
                if (RawTemp2.FieldBytes != null)
                {
                    Buffer.BlockCopy(RawTemp2.FieldBytes, 0, data, offSet, RawTemp2.TotalBytes);
                    offSet += RawTemp2.TotalBytes;
                }
                if (RawRssi.FieldBytes != null)
                {
                    //Add Rssi
                    Buffer.BlockCopy(RawRssi.FieldBytes, 0, data, offSet, RawRssi.TotalBytes);
                    offSet += RawRssi.TotalBytes;
                }
                if (RawLowBattery.FieldBytes !=null)
                {
                    //Add LowBattery
                    Buffer.BlockCopy(RawLowBattery.FieldBytes, 0, data, offSet, RawLowBattery.TotalBytes);
                    offSet += RawLowBattery.TotalBytes;
                }
                if (RawHummidity.FieldBytes != null)
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

        public float Temp1
        {
            get
            {
                return ByteUtil.ToInt(RawTemp1.Data);
            }
        }
        public float Temp2
        {
            get
            {
                return ByteUtil.ToInt(RawTemp2.Data);
            }
        }
        public float Rssi
        {
            get
            {
                return ByteUtil.ToInt(RawRssi.Data);
            }
        }
        public float LowBattery
        {
            get
            {
                return ByteUtil.ToInt(RawLowBattery.Data);
            }
        }
        public float Hummidity
        {
            get
            {                
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

        //Total fields in Alarm message
        public int FiledCount
        {
            get
            {
                return 11;
            }
        }

        public byte[] Data
        {
            get
            {
                int offSet = 0;
                int buffLength = RawDeviceNo.TotalBytes + RawTemp1.TotalBytes + RawTemp2.TotalBytes + RawRssi.TotalBytes + RawLowBattery.TotalBytes + RawHummidity.TotalBytes + RawAlarmTemp1.TotalBytes + RawAlarmTemp2.TotalBytes + RawAlarmBattery.TotalBytes + RawAlarmHummidity.TotalBytes + RawAlarmLigth.TotalBytes;
                byte[] data = new byte[buffLength];
                //AddDevice
                Buffer.BlockCopy(RawDeviceNo.FieldBytes, 0, data, offSet, RawDeviceNo.TotalBytes);
                offSet += RawDeviceNo.TotalBytes;
                //Add Temp1
                Buffer.BlockCopy(RawTemp1.FieldBytes, 0, data, offSet, RawTemp1.TotalBytes);
                offSet += RawTemp1.TotalBytes;
                //Add Temp2
                Buffer.BlockCopy(RawTemp2.FieldBytes, 0, data, offSet, RawTemp2.TotalBytes);
                offSet += RawTemp2.TotalBytes;
                //Add Rssi
                Buffer.BlockCopy(RawRssi.FieldBytes, 0, data, offSet, RawRssi.TotalBytes);
                offSet += RawRssi.TotalBytes;
                //Add LowBattery
                Buffer.BlockCopy(RawLowBattery.FieldBytes, 0, data, offSet, RawLowBattery.TotalBytes);
                offSet += RawLowBattery.TotalBytes;
                //Add Hummidity
                Buffer.BlockCopy(RawHummidity.FieldBytes, 0, data, offSet, RawHummidity.TotalBytes);
                offSet += RawHummidity.TotalBytes;
                //Add Alarm_Temp1
                Buffer.BlockCopy(RawAlarmTemp1.FieldBytes, 0, data, offSet, RawAlarmTemp1.TotalBytes);
                offSet += RawAlarmTemp1.TotalBytes;
                //Add Alarm_Temp2
                Buffer.BlockCopy(RawAlarmTemp2.FieldBytes, 0, data, offSet, RawAlarmTemp2.TotalBytes);
                offSet += RawAlarmTemp2.TotalBytes;
                //Add Alarm_Battery
                Buffer.BlockCopy(RawAlarmBattery.FieldBytes, 0, data, offSet, RawAlarmBattery.TotalBytes);
                offSet += RawAlarmBattery.TotalBytes;
                //Add Alarm_Hummidity
                Buffer.BlockCopy(RawAlarmHummidity.FieldBytes, 0, data, offSet, RawAlarmHummidity.TotalBytes);
                offSet += RawAlarmHummidity.TotalBytes;
                //Add Alarm_Light
                Buffer.BlockCopy(RawAlarmLigth.FieldBytes, 0, data, offSet, RawAlarmLigth.TotalBytes);
                offSet += RawAlarmLigth.TotalBytes;
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
        public float Temp1
        {
            get
            {
                return ByteUtil.ToInt(RawTemp1.Data);
            }
        }
        public float Temp2
        {
            get
            {
                return ByteUtil.ToInt(RawTemp2.Data);
            }
        }
        public float Rssi
        {
            get
            {
                return ByteUtil.ToInt(RawRssi.Data);
            }
        }
        public float LowBattery
        {
            get
            {
                return ByteUtil.ToInt(RawLowBattery.Data);
            }
        }
        public float Hummidity
        {
            get
            {
                return ByteUtil.ToInt(RawHummidity.Data);
            }
        }
        public int AlarmTemp1
        {
            get
            {
                return ByteUtil.ToInt(RawAlarmTemp1.Data);
            }
        }
        public int AlarmTemp2
        {
            get
            {
                return ByteUtil.ToInt(RawAlarmTemp2.Data);
            }
        }
        public int AlarmBattery
        {
            get
            {
                return ByteUtil.ToInt(RawAlarmBattery.Data);
            }
        }
        public int AlarmHummidity
        {
            get
            {
                return ByteUtil.ToInt(RawAlarmHummidity.Data);
            }
        }
        public int AlarmLigth
        {
            get
            {
                return ByteUtil.ToInt(RawAlarmLigth.Data);
            }
        }
    }
}
