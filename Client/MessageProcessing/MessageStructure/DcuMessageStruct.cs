﻿

using IotSystem.Core.Utils;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    public class MessageTime
    {
        public byte[] CreateMessagePublish()
        {
            byte[] dataDateTime = ConvertUtil.HexToByteArray(ConvertUtil.StringToHex(DateTime.Now.ToString("yyMMddHHmmss")));
            DcuTimeStruct dcuMessage = new DcuTimeStruct();
            dcuMessage.Time = new FieldStruct()
            {
                Obis = new byte[1] { (byte)EnumObis.Time },
                Data = dataDateTime,
                DataLength = new byte[1] { (byte)dataDateTime.Length }
            };
            return dcuMessage.Data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DcuTimeStruct
        {
            public FieldStruct Time { get; set; }
            private byte Crc => ByteUtil.CalCheckSum(Time.MessageBytes);

            public byte[] Data
            {
                get
                {
                    int offSet = 0;
                    int buffLength = Time.TotalBytes + 1;
                    byte[] data = new byte[buffLength];                    
                    Buffer.BlockCopy(Time.MessageBytes, 0, data, offSet, Time.TotalBytes);
                    offSet += Time.TotalBytes;
                    //Crc
                    Buffer.BlockCopy(new byte[1] { Crc }, 0, data, offSet, 1);
                    return data;
                }
            }
        }
    }
}
