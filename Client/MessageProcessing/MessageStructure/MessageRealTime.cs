

using System;
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
            byte[] rst = new byte[6];
            rst[0] = 0x01;
            rst[1] = 0x06;
           

            return rst;
        }
    }

    public struct MessageRealTime
    {
        public byte Obis { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }
    }
}
