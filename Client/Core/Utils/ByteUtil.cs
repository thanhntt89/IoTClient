

using System;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: ByteUtil.cs
* Created date:2022/6/9 12:49 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.Core.Utils
{
    public class ByteUtil
    {
        public static byte[] GetBytes(byte[] data, uint offSet)
        {
            uint size = (uint)data.Length - offSet;
            byte[] buff = new byte[size];
            Array.Copy(data, offSet, buff, 0, size);
            return buff;
        }

        public static byte CalCheckSum(byte[] _PacketData)
        {
            byte _CheckSumByte = 0x00;
            for (int i = 0; i < _PacketData.Length; i++)
                _CheckSumByte ^= _PacketData[i];
            return _CheckSumByte;
        }

        private ByteUtil()
        {

        }
    }
}
