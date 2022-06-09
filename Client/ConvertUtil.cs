/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: ConvertUtil.cs
* Created date:2022/6/8 11:22 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System;
using System.Linq;

namespace IotSystem
{
    public class ConvertUtil
    {

        public static string StringToHex(string dataString) => string.Join("", dataString.Select(c => ((int) c).ToString("X2")));

        public static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string IntToHex(int value)
        {
            return value.ToString("x");
        }

        public static int HexToInt(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        private ConvertUtil()
        {

        }
    }
}
