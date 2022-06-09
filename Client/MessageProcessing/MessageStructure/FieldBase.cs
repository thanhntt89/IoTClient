using IotSystem.Core.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: FieldStructure.cs
* Created date:2022/6/9 12:13 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.MessageProcessing.MessageStructure
{
    public class FieldBase
    {
        public static byte[] GetBytes(FieldStruct field)
        {
            return StructureUtil.Serialize<FieldStruct>(field);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FieldStruct
        {
            public byte Obis { get; set; }
            public byte Length { get; set; }
            public byte[] Data { get; set; }
        }
    }

}
