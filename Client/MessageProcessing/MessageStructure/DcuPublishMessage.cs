

using IotSystem.Core;
using IotSystem.Core.Utils;
using System;
using System.Runtime.InteropServices;
using static IotSystem.MessageProcessing.MessageStructure.FieldBase;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: MessageRealTime.cs
* Created date:2022/6/7 3:51 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.MessageProcessing.MessageStructure
{
    public class DcuPublishMessage
    {
        public static MessageBase CreatePublishMessageSetup(string topic, string dcuId)
        {
            DcuSetupStruct dcuSetup = new DcuSetupStruct();
            //Settings Hum High
            dcuSetup.HumHigh = new FieldStruct()
            {
                Obis = new byte[1] { (byte)EnumObis.Hum_High },
                Data = new byte[6] {1,2,3,4,5,6},
                DataLength = new byte[1] { 6 }
            };
            return new MessageBase() { Message = dcuSetup.Data, Topic = string.Format(topic, dcuId) };
        }

        public static MessageBase CreatePublishMessageTime(string topic, string dcuId)
        {
            byte[] dataDateTime = ConvertUtil.HexToByteArray(ConvertUtil.StringToHex(DateTime.Now.ToString("yyMMddHHmmss")));
            DcuTimeStruct dcuMessage = new DcuTimeStruct();
            
            dcuMessage.Time = new FieldStruct()
            {
                Obis = new byte[1] { (byte)EnumObis.Time },
                Data = dataDateTime,
                DataLength = new byte[1] { (byte)dataDateTime.Length }
            };
            return new MessageBase() { Message = dcuMessage.Data, Topic = string.Format(topic, dcuId) };
        }

        private DcuPublishMessage()
        {

        }
    }

    public struct DcuSetupStruct
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
                int buffLength = TempHigh.TotalBytes + TempLow.TotalBytes + HumHigh.TotalBytes + HumLow.TotalBytes + TimeUpdate.TotalBytes + TimeSample.TotalBytes + 1;
                byte[] data = new byte[buffLength];
                if (TempHigh.MessageBytes != null)
                {
                    Buffer.BlockCopy(TempHigh.MessageBytes, 0, data, offSet, TempHigh.TotalBytes);
                    offSet += TempHigh.TotalBytes;
                }
                if (TempLow.MessageBytes != null)
                {
                    Buffer.BlockCopy(TempLow.MessageBytes, 0, data, offSet, TempLow.TotalBytes);
                    offSet += TempLow.TotalBytes;
                }
                if (HumHigh.MessageBytes != null)
                {
                    Buffer.BlockCopy(HumHigh.MessageBytes, 0, data, offSet, HumHigh.TotalBytes);
                    offSet += HumHigh.TotalBytes;
                }
                if (HumLow.MessageBytes != null)
                {
                    Buffer.BlockCopy(HumLow.MessageBytes, 0, data, offSet, HumLow.TotalBytes);
                    offSet += HumLow.TotalBytes;
                }
                if (TimeUpdate.MessageBytes != null)
                {
                    Buffer.BlockCopy(TimeUpdate.MessageBytes, 0, data, offSet, TimeUpdate.TotalBytes);
                    offSet += TimeUpdate.TotalBytes;
                }
                if (TimeSample.MessageBytes != null)
                {
                    Buffer.BlockCopy(TimeSample.MessageBytes, 0, data, offSet, TimeSample.TotalBytes);
                    offSet += TimeSample.TotalBytes;
                }
                Crc = ByteUtil.CalCheckSum(data);
                //Crc
                Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);
                return data;
            }
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct DcuTimeStruct
    {
        public FieldStruct Time { get; set; }
        private byte Crc { get; set; }

        public byte[] Data
        {
            get
            {
                int offSet = 0;
                int buffLength = Time.TotalBytes + 1;
                byte[] data = new byte[buffLength];
                Buffer.BlockCopy(Time.MessageBytes, 0, data, offSet, Time.TotalBytes);
                offSet += Time.TotalBytes;
                Crc = ByteUtil.CalCheckSum(data);
                //Crc
                Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);
                return data;
            }
        }
    }
}
