/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: SingletonRuntimeTable.cs
* Created date:2022/6/10 11:02 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Data;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class SingletonRuntimeTable: DataTable
    {
        private static SingletonRuntimeTable instance;
        private static readonly object objLock = new object();
        static SingletonRuntimeTable()
        {

        }
        private SingletonRuntimeTable()
        {

        }

        public static SingletonRuntimeTable Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonRuntimeTable();
                            instance.Columns.Add("Time", typeof(string));
                            instance.Columns.Add("DeviceCode", typeof(int));
                            instance.Columns.Add("Temp1", typeof(float));
                            instance.Columns.Add("Temp2", typeof(float));
                            instance.Columns.Add("Rssi", typeof(float));
                            instance.Columns.Add("LowBattery", typeof(float));
                            instance.Columns.Add("Hummidity", typeof(float));
                        }
                    }
                }
                return instance;
            }
        }
    }
}
