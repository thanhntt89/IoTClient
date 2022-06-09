using IotSystem.Core;
using IotSystem.Core.Queues;
using IotSystem.Core.ThreadManagement;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MessageStructure;
using System;
using System.Threading;
using static IotSystem.ClientEvent;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class DecodeMessageDataThread : IDecodeDataThread
    {
        public event DelegateShowMessage EventShowMessage;
        private MessageType messageType;

        public DecodeMessageDataThread(MessageType type)
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
                //Process data
                //Valid message
                byte crc = message.Message[message.Message.Length];
                byte[] dataMessage = new byte[message.Message.Length - 1];

                if (crc != ByteUtil.CalCheckSum(dataMessage))
                    return false;

                //Message Type

                RuntimeStruct runtime = new RuntimeStruct();
                AlarmStruct alarm = new AlarmStruct();
                uint dataLength = 0;
                uint offSet = 0;

                while (dataMessage.Length != offSet)
                {
                    EnumObis obis = (EnumObis)dataMessage[offSet];
                    offSet++;
                    dataLength = dataMessage[offSet];
                    offSet += dataLength;

                    byte[] data = ByteUtil.GetBytes(dataMessage, offSet);

                    switch (obis)
                    {
                        case EnumObis.Time:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.Time = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.Time = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }

                            break;
                        case EnumObis.DeviceNo:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.DeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.DeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp1:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.Temp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.Temp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp2:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.Temp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.Temp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Rssi:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.Rssi = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.Rssi = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.LowBattery:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.LowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.LowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Hummidity:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.Hummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.Hummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Alarm_Temp1:
                            alarm.Alarm_Temp1 = new FieldBase.FieldStruct()
                            {
                                Obis = new byte[1] { (byte)obis },
                                DataLength = new byte[1] { (byte)dataLength },
                                Data = data
                            };
                            break;
                        case EnumObis.Alarm_Temp2:
                            alarm.Alarm_Temp2 = new FieldBase.FieldStruct()
                            {
                                Obis = new byte[1] { (byte)obis },
                                DataLength = new byte[1] { (byte)dataLength },
                                Data = data
                            };
                            break;
                        case EnumObis.Alarm_Battery:
                            alarm.Alarm_Battery = new FieldBase.FieldStruct()
                            {
                                Obis = new byte[1] { (byte)obis },
                                DataLength = new byte[1] { (byte)dataLength },
                                Data = data
                            };
                            break;
                        case EnumObis.Alarm_Hummidity:
                            alarm.Alarm_Hummidity = new FieldBase.FieldStruct()
                            {
                                Obis = new byte[1] { (byte)obis },
                                DataLength = new byte[1] { (byte)dataLength },
                                Data = data
                            };
                            break;
                        case EnumObis.Alarm_Light:
                            alarm.Alarm_Light = new FieldBase.FieldStruct()
                            {
                                Obis = new byte[1] { (byte)obis },
                                DataLength = new byte[1] { (byte)dataLength },
                                Data = data
                            };
                            break;                        
                        default:
                            break;
                    }
                }

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
