/**
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
    public class SingletonInsertDataThread: IInsertDataThread
    {
        public event DelegateShowMessage ShowMessageEvent;

        private static IInsertDataThread instance;

        private static object objLock = new object();
        private SingletonInsertDataThread()
        {

        }

        static SingletonInsertDataThread()
        {

        }

        public static IInsertDataThread Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                            instance = new SingletonInsertDataThread();
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

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            ShowMessageEvent += showMessage;
        }
    }
}
