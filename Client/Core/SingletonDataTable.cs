/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonDataTable.cs
* Created date:2022/6/2 1:05 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using System.Data;

namespace IotSystem.Core
{
    public class SingletonDataTable : DataTable
    {
        private static SingletonDataTable instance;
        private static readonly object objLock = new object();
        static SingletonDataTable()
        {

        }
        private SingletonDataTable()
        {

        }

        public static SingletonDataTable Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                            instance = new SingletonDataTable();
                    }
                }
                return instance;
            }
        }
    }
}
