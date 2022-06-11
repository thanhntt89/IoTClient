

using IotSystem.Core;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MessageStructure;
using System;
using static IotSystem.MessageProcessing.MessageStructure.FieldBase;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: MessageRealTime.cs
* Created date:2022/6/7 3:51 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.MessageProcessing.DcuMessage
{
    public class DcuPublishMessage
    {
        public static MessageBase CreatePublishMessageSetup(string topic, string dcuId)
        {
            DcuSetupStruct dcuSetup = new DcuSetupStruct();
            //Settings Hum High
            dcuSetup.HumHigh = new FieldStruct()
            {
                Obis =  (byte)EnumObis.HighHummidity,
                Data = new byte[6] { 1, 2, 3, 4, 5, 6 }               
            };
            return new MessageBase() { Message = dcuSetup.Data, Topic = string.Format(topic, dcuId) };
        }

        public static MessageBase CreatePublishMessageTime(string topicTemplate, string dcuId)
        {
            byte[] dataDateTime = new byte[6];
            dataDateTime[0] = (byte)int.Parse(DateTime.Now.ToString("yy"));
            dataDateTime[1] = (byte)DateTime.Now.Month;
            dataDateTime[2] = (byte)DateTime.Now.Day;
            dataDateTime[3] = (byte)int.Parse(DateTime.Now.ToString("HH"));
            dataDateTime[4] = (byte)DateTime.Now.Minute;
            dataDateTime[5] = (byte)DateTime.Now.Second;

            DcuTimeStruct dcuMessage = new DcuTimeStruct();
            dcuMessage.RawTime = new FieldStruct()
            {
                Obis = (byte)EnumObis.Time ,
                Data = dataDateTime
            };
            return new MessageBase() { Message = dcuMessage.Data, Topic = string.Format(topicTemplate, dcuId) };
        }

        private DcuPublishMessage()
        {

        }
    }

    public class DcuSetupStruct
    {
        public FieldStruct TempHigh { get; set; }
        public FieldStruct TempLow { get; set; }
        public FieldStruct HumHigh { get; set; }
        public FieldStruct HumLow { get; set; }
        public FieldStruct TimeUpdate { get; set; }
        public FieldStruct TimeSample { get; set; }

        private byte Crc { get; set; }
        public byte[] Data
        {
            get
            {
                int offSet = 0;
                //buffLength = Sum(Field) + Crc
                int buffLength = TempHigh.TotalBytes + TempLow.TotalBytes + HumHigh.TotalBytes + HumLow.TotalBytes + TimeUpdate.TotalBytes + TimeSample.TotalBytes + 1;
                byte[] data = new byte[buffLength];
                if (TempHigh.FieldBytes != null)
                {
                    Buffer.BlockCopy(TempHigh.FieldBytes, 0, data, offSet, TempHigh.TotalBytes);
                    offSet += TempHigh.TotalBytes;
                }
                if (TempLow.FieldBytes != null)
                {
                    Buffer.BlockCopy(TempLow.FieldBytes, 0, data, offSet, TempLow.TotalBytes);
                    offSet += TempLow.TotalBytes;
                }
                if (HumHigh.FieldBytes != null)
                {
                    Buffer.BlockCopy(HumHigh.FieldBytes, 0, data, offSet, HumHigh.TotalBytes);
                    offSet += HumHigh.TotalBytes;
                }
                if (HumLow.FieldBytes != null)
                {
                    Buffer.BlockCopy(HumLow.FieldBytes, 0, data, offSet, HumLow.TotalBytes);
                    offSet += HumLow.TotalBytes;
                }
                if (TimeUpdate.FieldBytes != null)
                {
                    Buffer.BlockCopy(TimeUpdate.FieldBytes, 0, data, offSet, TimeUpdate.TotalBytes);
                    offSet += TimeUpdate.TotalBytes;
                }
                if (TimeSample.FieldBytes != null)
                {
                    Buffer.BlockCopy(TimeSample.FieldBytes, 0, data, offSet, TimeSample.TotalBytes);
                    offSet += TimeSample.TotalBytes;
                }
                Crc = ByteUtil.CalCheckSum(data);
                //Crc
                Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);
                return data;
            }
        }
    }

    public class DcuTimeStruct
    {
        public FieldStruct RawTime { get; set; }
        private byte Crc { get; set; }

        public byte[] Data
        {
            get
            {
                int offSet = 0;
                int buffLength = RawTime.TotalBytes + 1;
                byte[] data = new byte[buffLength];
                Buffer.BlockCopy(RawTime.FieldBytes, 0, data, offSet, RawTime.TotalBytes);
                offSet += RawTime.TotalBytes;
                Crc = ByteUtil.CalCheckSum(data);
                //Crc
                Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);
                return data;
            }
        }
    }
}
