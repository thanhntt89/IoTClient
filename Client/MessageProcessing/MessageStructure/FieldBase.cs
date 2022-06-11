using System;
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
        public struct FieldStruct
        {
            public byte? Obis { get; set; }
            public byte[] Data { get; set; }

            /// <summary>
            /// Total bytes = Obis(1byte)+ DataLength(1byte)+ Data(Nbytes)
            /// </summary>
            public int TotalBytes => (Obis == null || Data == null) ? 0 : Data.Length + 2;

            public byte[] FieldBytes
            {
                get
                {
                    if (Obis == null || Data == null) 
                        return null;
                    int offSet = 0;
                    byte[] data = new byte[Data.Length + 2];
                    //Obis
                    Buffer.BlockCopy(new byte[1] { (byte)Obis }, 0, data, offSet, 1);
                    offSet += 1;
                    //Length
                    Buffer.BlockCopy(new byte[1] { (byte)Data.Length }, 0, data, offSet, 1);
                    offSet += 1;
                    //Data
                    Buffer.BlockCopy(Data, 0, data, offSet, Data.Length);
                    return data;
                }
            }
        }
    }


}
