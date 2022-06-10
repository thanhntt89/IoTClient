
using System.Linq;
using System.Runtime.InteropServices;
/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: StructureUtil.cs
* Created date:2022/6/8 10:00 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.Core.Utils
{
    public class StructureUtil
    {        

        /// <summary>
        /// Convert struct to byte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T str)
     where T : struct
        {
            int size = Marshal.SizeOf(str);

            byte[] arr = new byte[size];

            GCHandle h = default(GCHandle);

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);

                Marshal.StructureToPtr<T>(str, h.AddrOfPinnedObject(), false);
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

        /// <summary>
        /// Convert bytes to struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] arr)
            where T : struct
        {
            T str = default(T);

            GCHandle h = default(GCHandle);

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);

                str = Marshal.PtrToStructure<T>(h.AddrOfPinnedObject());
            }
            finally
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }

            return str;
        }

        private StructureUtil()
        {

        }
    }
}
