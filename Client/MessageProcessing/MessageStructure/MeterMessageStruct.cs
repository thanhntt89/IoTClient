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
                byte[] data = new byte[DataLength  + 1];
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

    public class RuntimeStruct
    {
        public FieldStruct FieldDeviceNo { get; set; }
        public FieldStruct FieldTemp1 { get; set; }
        public FieldStruct FieldTemp2 { get; set; }
        public FieldStruct FieldRssi { get; set; }
        public FieldStruct FieldLowBattery { get; set; }
        public FieldStruct FieldHummidity { get; set; }                

        //Total fields in Runtime message
        public virtual int FiledCount
        {
            get
            {
                return 6;
            }
        }

        public virtual byte[] Data
        {
            get
            {
                int offSet = 0;
                int buffLength = FieldDeviceNo.TotalBytes + FieldTemp1.TotalBytes + FieldTemp2.TotalBytes + FieldRssi.TotalBytes + FieldLowBattery.TotalBytes + FieldHummidity.TotalBytes;
                byte[] data = new byte[buffLength];
                //AddDevice
                Buffer.BlockCopy(FieldDeviceNo.FieldBytes, 0, data, offSet, FieldDeviceNo.TotalBytes);
                offSet += FieldDeviceNo.TotalBytes;
                //Add Temp1
                Buffer.BlockCopy(FieldTemp1.FieldBytes, 0, data, offSet, FieldTemp1.TotalBytes);
                offSet += FieldTemp1.TotalBytes;
                //Add Temp2
                Buffer.BlockCopy(FieldTemp2.FieldBytes, 0, data, offSet, FieldTemp2.TotalBytes);
                offSet += FieldTemp2.TotalBytes;
                //Add Rssi
                Buffer.BlockCopy(FieldRssi.FieldBytes, 0, data, offSet, FieldRssi.TotalBytes);
                offSet += FieldRssi.TotalBytes;
                //Add LowBattery
                Buffer.BlockCopy(FieldLowBattery.FieldBytes, 0, data, offSet, FieldLowBattery.TotalBytes);
                offSet += FieldLowBattery.TotalBytes;
                //Add Hummidity
                Buffer.BlockCopy(FieldHummidity.FieldBytes, 0, data, offSet, FieldHummidity.TotalBytes);
                offSet += FieldHummidity.TotalBytes;

                return data;
            }
        }
    }

    public class AlarmStruct: RuntimeStruct
    {
        public FieldStruct FieldAlarmTemp1 { get; set; }
        public FieldStruct FiledAlarmTemp2 { get; set; }
        public FieldStruct FieldAlarmBattery { get; set; }
        public FieldStruct FieldAlarmHummidity { get; set; }
        public FieldStruct FieldAlarmLight { get; set; }

        //Total fields in Alarm message
        public override int FiledCount
        {
            get
            {
                return 11;
            }
        }

        public override byte[] Data
        {
            get
            {
                int offSet = 0;
                int buffLength = FieldDeviceNo.TotalBytes + FieldTemp1.TotalBytes + FieldTemp2.TotalBytes + FieldRssi.TotalBytes + FieldLowBattery.TotalBytes + FieldHummidity.TotalBytes + FieldAlarmTemp1.TotalBytes + FiledAlarmTemp2.TotalBytes + FieldAlarmBattery.TotalBytes + FieldAlarmHummidity.TotalBytes + FieldAlarmLight.TotalBytes;
                byte[] data = new byte[buffLength];
                //AddDevice
                Buffer.BlockCopy(FieldDeviceNo.FieldBytes, 0, data, offSet, FieldDeviceNo.TotalBytes);
                offSet += FieldDeviceNo.TotalBytes;
                //Add Temp1
                Buffer.BlockCopy(FieldTemp1.FieldBytes, 0, data, offSet, FieldTemp1.TotalBytes);
                offSet += FieldTemp1.TotalBytes;
                //Add Temp2
                Buffer.BlockCopy(FieldTemp2.FieldBytes, 0, data, offSet, FieldTemp2.TotalBytes);
                offSet += FieldTemp2.TotalBytes;
                //Add Rssi
                Buffer.BlockCopy(FieldRssi.FieldBytes, 0, data, offSet, FieldRssi.TotalBytes);
                offSet += FieldRssi.TotalBytes;
                //Add LowBattery
                Buffer.BlockCopy(FieldLowBattery.FieldBytes, 0, data, offSet, FieldLowBattery.TotalBytes);
                offSet += FieldLowBattery.TotalBytes;
                //Add Hummidity
                Buffer.BlockCopy(FieldHummidity.FieldBytes, 0, data, offSet, FieldHummidity.TotalBytes);
                offSet += FieldHummidity.TotalBytes;
                //Add Alarm_Temp1
                Buffer.BlockCopy(FieldAlarmTemp1.FieldBytes, 0, data, offSet, FieldAlarmTemp1.TotalBytes);
                offSet += FieldAlarmTemp1.TotalBytes;
                //Add Alarm_Temp2
                Buffer.BlockCopy(FiledAlarmTemp2.FieldBytes, 0, data, offSet, FiledAlarmTemp2.TotalBytes);
                offSet += FiledAlarmTemp2.TotalBytes;
                //Add Alarm_Battery
                Buffer.BlockCopy(FieldAlarmBattery.FieldBytes, 0, data, offSet, FieldAlarmBattery.TotalBytes);
                offSet += FieldAlarmBattery.TotalBytes;
                //Add Alarm_Hummidity
                Buffer.BlockCopy(FieldAlarmHummidity.FieldBytes, 0, data, offSet, FieldAlarmHummidity.TotalBytes);
                offSet += FieldAlarmHummidity.TotalBytes;
                //Add Alarm_Light
                Buffer.BlockCopy(FieldAlarmLight.FieldBytes, 0, data, offSet, FieldAlarmLight.TotalBytes);
                offSet += FieldAlarmLight.TotalBytes;
                return data;
            }
        }
    }
}
