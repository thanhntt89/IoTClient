using IotSystem.Core;
using IotSystem.Core.Queues;
using IotSystem.Core.ThreadManagement;
using System;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class DecodeMessageDataThread: IDecodeDataThread
    {
        public event DelegateShowMessage EventShowMessage;
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
            MessageBase message = new MessageBase();
            int countData = 0;
            Thread currentThread = Thread.CurrentThread;
            EventShowMessage?.Invoke($"ThreadDecode-{currentThread.Name}:Started!!!");

            while (true)
            {
                countData = SingletonMessageDataQueue<MessageBase>.Instance.Count;
                if (cancellation.IsCancellationRequested && countData == 0)
                {
                    EventShowMessage?.Invoke($"ThreadDecode-{currentThread.Name}:Stopped!!!");
                    break;
                }                                

                //Get data from messagequeue
                if (SingletonMessageDataQueue<MessageBase>.Instance.TryDequeue(out message) && message != null)
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

        public void ThreadDecodeByTraffic(CancellationToken cancellation)
        {
            MessageBase message = new MessageBase();
            Thread currentThread = Thread.CurrentThread;

            EventShowMessage?.Invoke($"ThreadDecodeByTraffic-{currentThread.Name}:Started!!!");
            int countData = 0;
            
            while (true)
            {                
                countData = SingletonMessageDataQueue<MessageBase>.Instance.Count;
                if (cancellation.IsCancellationRequested || countData == 0)
                {
                    EventShowMessage?.Invoke($"ThreadDecodeByTraffic-{currentThread.Name}:Stopped!!!");
                    break;
                }
                
                //Get data from messagequeue
                if (SingletonMessageDataQueue<MessageBase>.Instance.TryDequeue(out message) && message != null)
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

        private bool ProcessingMessage(MessageBase message)
        {
            try
            {
                //Process data
                //Code here

                //Lock table to insert
                lock (SingletonDcuTable.Instance)
                {
                    SingletonDcuTable.Instance.Rows.Add(message.Topic);                    
                }

                return true;
            }
            catch (Exception ex)
            {
                EventShowMessage?.Invoke($"DecodeRunning-Fails:{ex.Message}");
                return false;
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }
    }
}
