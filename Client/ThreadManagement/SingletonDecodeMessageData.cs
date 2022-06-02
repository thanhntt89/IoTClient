using IotClient.MessageProcessing;
using IotClient.Queues;
using System;
using System.Data;
using System.Threading;
using static IotClient.ClientEvent;

namespace IotClient.DataProcessing
{
    public class SingletonDecodeMessageData
    {
        public event DelegateShowMessage ShowMessageEvent;
        private static readonly SingletonDecodeMessageData instance = new SingletonDecodeMessageData();
        private ProcessingDataFactory processingDataFactory;       

        /// <summary>
        /// Set time to process message
        /// </summary>
        private int TIME_PROCESSING_MESSAGE = 100;

        private int TimeProcessMessage(int countdata)
        {
            //if (countdata / 1000 >= 1000)
            //{
            //    //TIME_PROCESSING_MESSAGE = 1;
            //}else if(countdata/ 1000 >= 100)
            //{
            //   // TIME_PROCESSING_MESSAGE = 10;
            //}
            //else if (countdata / 1000 >= 10)
            //{
            //   // TIME_PROCESSING_MESSAGE = 100;
            //}

            return TIME_PROCESSING_MESSAGE;
        }

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
            int countData = 0;

            while (true)
            {
                countData = SingletonMessageDataQueue<MessageData>.Instance.Count;
                if (cancellation.IsCancellationRequested && countData == 0)
                {
                    ShowMessageEvent?.Invoke($"SingletonDecodeData-StartDecodeThread:Stopped!!!");
                    break;
                }
                //Get data from messagequeue
                if (SingletonMessageDataQueue<MessageData>.Instance.TryDequeue(out message) && message != null)
                {
                    if (ProcessingMessage(message))
                    {
                        Thread.Sleep(TimeProcessMessage(countData));
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
                    lock(SingletonDataTable.Instance)
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

        private bool ProcessingMessage(MessageData message)
        {
            try
            {
                //Process data
                //Code here

                //Lock table to insert
                lock (SingletonDataTable.Instance)
                {
                    SingletonDataTable.Instance.Rows.Add();
                    ShowMessageEvent?.Invoke($"Decode topic: {message.Topic} time:{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
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
