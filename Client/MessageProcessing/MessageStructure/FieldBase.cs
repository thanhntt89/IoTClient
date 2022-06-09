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
            int size = Marshal.SizeOf(field);

            byte[] arr = new byte[size];

            GCHandle h = default(GCHandle);

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);

                Marshal.StructureToPtr<FieldStruct>(field, h.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }

            return arr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FieldStruct
        {
            public byte[] Obis { get; set; }
            public byte[] DataLength { get; set; }
            public byte[] Data { get; set; }

            public int TotalBytes => Obis.Length + DataLength.Length + Data.Length;

            public byte[] MessageBytes
            {
                get
                {
                    int offSet = 0;
                    byte[] data = new byte[TotalBytes];
                    Buffer.BlockCopy(Obis, 0, data, offSet, Obis.Length);
                    offSet += Obis.Length;
                    //Length
                    Buffer.BlockCopy(DataLength, 0, data, offSet, DataLength.Length);
                    offSet += DataLength.Length;
                    //Data
                    Buffer.BlockCopy(Data, 0, data, offSet, Data.Length);
                    offSet += Data.Length;
                    return data;
                }
            }
        }
    }

}
