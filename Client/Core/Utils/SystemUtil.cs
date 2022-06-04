/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SystemUtil.cs
* Created date:2022/6/3 11:06 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System;

namespace IotSystem.Core.Utils
{
    public class SystemUtil
    {
        private static SystemUtil instance;
        private static readonly object objLock = new object();


        public int ProcessCount => Environment.ProcessorCount;

        public int GetMaxThreadNumber => (2 * Environment.ProcessorCount - 1);


        private SystemUtil()
        {

        }

        static SystemUtil()
        {

        }

        public static SystemUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                            instance = new SystemUtil();
                    }
                }
                return instance;
            }
        }
    }
}
