

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
        public static byte[] ToBytes(int intValue)
        {
            return BitConverter.GetBytes(intValue);
        }

        public static double ConvertByteArrayToInt32(byte[] b)
        {
            return BitConverter.ToInt32(b, 0);
        }

        public static float ToFloat(byte[] data)
        {
            return BitConverter.ToSingle(data, 0);
        }

        public static int ToInt(byte[] data)
        {            
            if (data.Length == 4)
                return BitConverter.ToInt32(data, 0);
            return BitConverter.ToInt16(data, 0);
        }


        public static byte CalCheckSum(byte[] _PacketData)
        {
            byte _CheckSumByte = 0x00;
            for (int i = 0; i < _PacketData.Length; i++)
                _CheckSumByte ^= _PacketData[i];
            return _CheckSumByte;
        }

        public static byte[] GetBytes(byte[] data, uint offSet, uint size)
        {
            byte[] buff = new byte[size];
            Array.Copy(data, offSet, buff, 0, size);
            return buff;
        }

        private ByteUtil()
        {

        }
    }
}
