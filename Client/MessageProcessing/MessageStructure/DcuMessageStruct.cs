

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
                Obis = (byte)EnumObis.Time,
                Data = dataDateTime,
                Length = (byte)dataDateTime.Length
            };
            byte[] dataTime = FieldBase.GetBytes(dcuMessage.Time);
            dcuMessage.Crc = ByteUtil.CalCheckSum(dataTime);
                        

            return StructureUtil.Serialize<DcuTimeStruct>(dcuMessage);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DcuTimeStruct
        {
            public FieldStruct Time { get; set; }
            public byte Crc { get; set; }
        }
    }
}
