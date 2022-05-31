using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using static IotClient.ClientEvent;

namespace IotClient.DataProcessing
{
    public class SingletonDecodeMessageData
    {
        public event DelegateShowMessage ShowMessageEvent;
        private static readonly SingletonDecodeMessageData instance = new SingletonDecodeMessageData();
        private ProcessingDataFactory processingDataFactory;
        private DataTable dataTableMeterData = new DataTable();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SingletonDecodeMessageData()
        {
        }

        private SingletonDecodeMessageData()
        {

        }

        public static SingletonDecodeMessageData Instance
        {
            get
            {
                return instance;
            }
        }

        public void ThreadDecode(CancellationToken cancellation)
        {
            MessageData message = new MessageData();
            ShowMessageEvent?.Invoke($"SingletonDecodeData-StartDecodeThread:Started!!!");

            while (true)
            {
                if (cancellation.IsCancellationRequested && SingletonMessageDataQueue<MessageData>.Instance.Count == 0)
                {
                    ShowMessageEvent?.Invoke($"SingletonDecodeData-StartDecodeThread:Stopped!!!");
                    break;
                }
                //Get data from messagequeue
                if (SingletonMessageDataQueue<MessageData>.Instance.TryDequeue(out message) && message != null)
                {
                    if (ProcessingMessage(message))
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                }

                //Sleep thread 10sec if queue has no data
                Thread.Sleep(10000);
            }
        }

        public void ThreadInsertData(CancellationToken cancellation)
        {
            ShowMessageEvent?.Invoke($"SingletonDecodeData-InsertDataThread:Started!!!");

            while (true)
            {
                if (cancellation.IsCancellationRequested && dataTableMeterData.Rows.Count == 0)
                {
                    ShowMessageEvent?.Invoke($"SingletonDecodeData-InsertDataThread:Stopped!!!");
                    break;
                }

                //Check data to insert
                if (dataTableMeterData.Rows.Count > 0)
                {
                    ProcessingInsertData(dataTableMeterData);
                }

                //Wait 10s for check data 
                Thread.Sleep(10000);
            }

        }

        private bool ProcessingMessage(MessageData message)
        {
            try
            {
                //Process data
                //Code here

                //Lock table to insert
                lock (dataTableMeterData)
                {
                    dataTableMeterData.Rows.Add();
                    ShowMessageEvent?.Invoke($"Decode topic: {message.Topic}");
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowMessageEvent?.Invoke($"DecodeRunning-Fails:{ex.Message}");
                return false;
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
