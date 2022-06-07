/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonDataTable.cs
* Created date:2022/6/2 1:05 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Data;

namespace IotSystem.MessageProcessing
{
    public class SingletonDcuTable : DataTable
    {
        private static SingletonDcuTable instance;
        private static readonly object objLock = new object();
        static SingletonDcuTable()
        {

        }
        private SingletonDcuTable()
        {

        }
        
        public static SingletonDcuTable Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonDcuTable();
                            instance.Columns.Add("DcuCode");
                        }
                    }
                }
                return instance;
            }
        }
    

    }
}
