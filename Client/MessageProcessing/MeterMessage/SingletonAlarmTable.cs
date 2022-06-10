/**
*Project name: IotSystem 
* Created by: Nguyen Tat Thanh
* File Name: SingletonAlarmTable.cs
* Created date:2022/6/10 11:40 PM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class SingletonAlarmTable: DataTable
    {
        private static SingletonAlarmTable instance;
        private static readonly object objLock = new object();
        static SingletonAlarmTable()
        {

        }
        private SingletonAlarmTable()
        {

        }

        public static SingletonAlarmTable Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonAlarmTable();
                            instance.Columns.Add("Time", typeof(object));
                            instance.Columns.Add("DeviceCode", typeof(object));
                            instance.Columns.Add("Temp1", typeof(object));
                            instance.Columns.Add("Temp2", typeof(object));
                            instance.Columns.Add("Rssi", typeof(object));
                            instance.Columns.Add("LowBattery", typeof(object));
                            instance.Columns.Add("Hummidity", typeof(object));
                            instance.Columns.Add("AlarmTemp1", typeof(object));
                            instance.Columns.Add("AlarmTemp2", typeof(object));
                            instance.Columns.Add("AlarmBattery", typeof(object));
                            instance.Columns.Add("AlarmHummidity", typeof(object));
                            instance.Columns.Add("AlarmLigth", typeof(object));
                        }
                    }
                }
                return instance;
            }
        }
    }
}
