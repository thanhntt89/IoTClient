/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: SingletonInserDataThread.cs
* Created date:2022/6/3 9:14 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.Core.ThreadManagement;
using IotSystem.MessageProcessing.MeterMessage;
using System;
using System.Data;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing
{
    public class DatabaseProcessingThread: IDatabaseProcessingThread
    {
        public event DelegateShowMessage EventShowMessage;    

        public void ExecuteData(CancellationToken cancellation)
        {
            EventShowMessage?.Invoke($"SingletonDecodeData-InsertDataThread:Started!!!");
            DataTable dataTableRuntime = new DataTable();
            DataTable dataTableAlarm = new DataTable();

            while (true)
            {
                if (cancellation.IsCancellationRequested && SingletonRuntimeTable.Instance.Rows.Count == 0 && SingletonAlarmTable.Instance.Rows.Count == 0
                    )
                {
                    EventShowMessage?.Invoke($"SingletonDecodeData-InsertDataThread:Stopped!!!");
                    break;
                }

                //Check data to insert
                if (SingletonRuntimeTable.Instance.Rows.Count > 0)
                {
                    lock (SingletonRuntimeTable.Instance)
                    {
                        dataTableRuntime = SingletonRuntimeTable.Instance.Copy();
                        SingletonRuntimeTable.Instance.Clear();
                    }

                    ProcessingRuntime(dataTableRuntime);
                }
                if (SingletonAlarmTable.Instance.Rows.Count > 0)
                {
                    lock (SingletonAlarmTable.Instance)
                    {
                        dataTableAlarm = SingletonAlarmTable.Instance.Copy();
                        SingletonAlarmTable.Instance.Clear();
                    }

                    ProcessingAlarm(dataTableAlarm);
                }
                //Wait 10s for check data 
                Thread.Sleep(10000);
            }
        }

        private bool ProcessingRuntime(DataTable dataTable)
        {
            try
            {
                //Insert to database
                //Code here


                EventShowMessage?.Invoke($"InsertData-Test-RowsCount: {dataTable.Rows.Count} {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                //Clear data in table
                lock (dataTable)
                {
                    dataTable.Rows.Clear();
                }

                return true;
            }
            catch (Exception ex)
            {
                EventShowMessage?.Invoke($"ProcessingInsertData-Fails:{ex.Message}");
                return false;
            }
        }

        private bool ProcessingAlarm(DataTable dataTable)
        {
            try
            {
                //Insert to database
                //Code here


                EventShowMessage?.Invoke($"InsertData-Test-RowsCount: {dataTable.Rows.Count} {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                //Clear data in table
                lock (dataTable)
                {
                    dataTable.Rows.Clear();
                }

                return true;
            }
            catch (Exception ex)
            {
                EventShowMessage?.Invoke($"ProcessingInsertData-Fails:{ex.Message}");
                return false;
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }
    }
}
