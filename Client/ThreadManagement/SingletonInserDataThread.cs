﻿/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonInserDataThread.cs
* Created date:2022/6/3 9:14 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.MessageProcessing;
using System;
using System.Data;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.ThreadManagement
{
    public class SingletonInserDataThread
    {
        public event DelegateShowMessage ShowMessageEvent;

        private static SingletonInserDataThread instance;
        private static object objLock = new object();
        private SingletonInserDataThread()
        {

        }

        static SingletonInserDataThread()
        {

        }

        public static SingletonInserDataThread Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                            instance = new SingletonInserDataThread();
                    }
                }
                return instance;
            }
        }


        public void InsertData(CancellationToken cancellation)
        {
            ShowMessageEvent?.Invoke($"SingletonDecodeData-InsertDataThread:Started!!!");
            DataTable dataTable = new DataTable();

            while (true)
            {
                if (cancellation.IsCancellationRequested && SingletonDataTable.Instance.Rows.Count == 0)
                {
                    ShowMessageEvent?.Invoke($"SingletonDecodeData-InsertDataThread:Stopped!!!");
                    break;
                }

                //Check data to insert
                if (SingletonDataTable.Instance.Rows.Count > 0)
                {
                    lock (SingletonDataTable.Instance)
                    {
                        dataTable = SingletonDataTable.Instance.Copy();
                        SingletonDataTable.Instance.Clear();
                    }

                    ProcessingInsertData(dataTable);
                }
                //Wait 10s for check data 
                Thread.Sleep(10000);
            }
        }

        private bool ProcessingInsertData(DataTable dataTable)
        {
            try
            {
                //Insert to database
                //Code here


                ShowMessageEvent?.Invoke($"InsertData-Test-RowsCount: {dataTable.Rows.Count}");
                //Clear data in table
                lock (dataTable)
                {
                    dataTable.Rows.Clear();
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowMessageEvent?.Invoke($"ProcessingInsertData-Fails:{ex.Message}");
                return false;
            }
        }
    }
}