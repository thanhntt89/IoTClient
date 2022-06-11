using IotSystem.Core;
using IotSystem.Core.Queues;
using IotSystem.Core.ThreadManagement;
using IotSystem.Core.Utils;
using System;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class MeterDecodeMessageThread : IDecodeDataThread
    {
        public event DelegateShowMessage EventShowMessage;
        private MessageType messageType;

        public MeterDecodeMessageThread(MessageType type)
        {
            messageType = type;
        }

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
                MeterMessageRaw messageRaw = new MeterMessageRaw(message, messageType);
                messageRaw.GetMessageRaw();

                if (message.Topic.Contains(messageType.TypeRunTime))
                {
                    //Lock table to insert
                    lock (SingletonRuntimeTable.Instance)
                    {          
                        foreach (var item in messageRaw.Runtimes)
                        {
                            //Error time
                            if (messageRaw.Runtimes.RawTime.FieldBytes == null) continue;
                          var  row = new object[7]
                            {
                                messageRaw.Runtimes.Time,
                                item.DeviceCode,
                                item.Temp1,
                                item.Temp2,
                                item.Rssi,
                                item.LowBattery,
                                item.Hummidity
                            };
                            SingletonRuntimeTable.Instance.Rows.Add(row);

                            Thread.Sleep(10);
                        }                        
                    }
                }else if (message.Topic.Contains(messageType.TypeAlarm))
                {
                    //Lock table to insert
                    lock (SingletonAlarmTable.Instance)
                    {
                        foreach (var item in messageRaw.Alarms)
                        {
                            //Error time
                            if (messageRaw.Alarms.RawTime.FieldBytes == null) continue;
                            var row = new object[12]
                              {
                                messageRaw.Alarms.Time,
                                item.DeviceCode,
                                item.Temp1,
                                item.Temp2,
                                item.Rssi,
                                item.LowBattery,
                                item.Hummidity,
                                item.AlarmTemp1,
                                item.AlarmTemp2,
                                item.AlarmBattery,
                                item.AlarmHummidity,
                                item.AlarmLigth
                              };
                            SingletonAlarmTable.Instance.Rows.Add(row);

                            Thread.Sleep(10);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                EventShowMessage?.Invoke($"DecodeRunning-Fails:{ex.Message}");
                LogUtil.Intance.WriteLog(LogType.Error, string.Format("DecodeMessageDataThread-ProcessingMessage-Error: {0}", ex.Message));
                return false;
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            EventShowMessage += showMessage;
        }
    }
}
