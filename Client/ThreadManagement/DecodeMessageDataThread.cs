﻿using IotSystem.MessageProcessing;
using IotSystem.Queues;
using IotSystem.ThreadManagement;
using System;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.DataProcessing
{
    public class DecodeMessageDataThread: IDecodeDataThread
    {
        public event DelegateShowMessage eventShowMessage;
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

        public void ThreadDecode(CancellationToken cancellation)
        {
            MessageData message = new MessageData();
            eventShowMessage?.Invoke($"SingletonDecodeData-StartDecodeThread:Started!!!");
            int countData = 0;

            while (true)
            {
                countData = SingletonMessageDataQueue<MessageData>.Instance.Count;
                if (cancellation.IsCancellationRequested && countData == 0)
                {
                    eventShowMessage?.Invoke($"SingletonDecodeData-StartDecodeThread:Stopped!!!");
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
                    eventShowMessage?.Invoke($"Decode topic: {message.Topic} time:{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                }

                return true;
            }
            catch (Exception ex)
            {
                eventShowMessage?.Invoke($"DecodeRunning-Fails:{ex.Message}");
                return false;
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            eventShowMessage += showMessage;
        }
    }
}
