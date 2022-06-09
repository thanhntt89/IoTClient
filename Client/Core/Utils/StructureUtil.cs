

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
        public static byte[] Serialize<T>(T s)
     where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var array = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(s, ptr, true);
            Marshal.Copy(ptr, array, 0, size);
            Marshal.FreeHGlobal(ptr);
            return array;
        }

        /// <summary>
        /// Convert bytes to struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] array)
            where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(array, 0, ptr, size);
            var s = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return s;
        }

        private StructureUtil()
        {

        }
    }
}
