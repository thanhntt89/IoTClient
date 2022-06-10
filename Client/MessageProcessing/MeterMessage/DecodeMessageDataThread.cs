using IotSystem.Core;
using IotSystem.Core.Queues;
using IotSystem.Core.ThreadManagement;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MessageStructure;
using System;
using System.Linq;
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

        public bool ProcessingMessage(MessageBase message)
        {
            try
            {
                //Process data
                //Valid message
                byte crc = message.Message[message.Message.Length - 1];
                byte[] dataMessage = new byte[message.Message.Length - 1];
                Buffer.BlockCopy(message.Message, 0, dataMessage, 0, message.Message.Length - 1);
                //Check sum data
                if (crc != ByteUtil.CalCheckSum(dataMessage))
                    return false;

                //Message Type
                RuntimeCollection runtimes = new RuntimeCollection();
                AlarmCollection alarms = new AlarmCollection();

                RuntimeStruct runtime = new RuntimeStruct();
                AlarmStruct alarm = new AlarmStruct();

                int dataLength = 0;
                int offSet = 0;
                //Count data foreach device
                int fieldCount = 0;

                while (dataMessage.Length != offSet)
                {
                    EnumObis obis = (EnumObis)dataMessage[offSet];
                    offSet++;
                    //Get data length value
                    dataLength = dataMessage[offSet];
                    //Position data
                    offSet++;
                    byte[] data = new byte[dataLength];
                    Buffer.BlockCopy(dataMessage, offSet, data, 0, dataLength);
                    //Next position
                    offSet += dataLength;

                    switch (obis)
                    {
                        case EnumObis.Time:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtimes.Time = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarms.Time = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.DeviceNo:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.FieldDeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.FieldDeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp1:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.FieldTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.FieldTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp2:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.FieldTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.FieldTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Rssi:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.FieldRssi = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.FieldRssi = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.LowBattery:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.FieldLowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.FieldLowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Hummidity:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.FieldHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.FieldHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Alarm_Temp1:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.FieldAlarmTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            break;
                        case EnumObis.Alarm_Temp2:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.FiledAlarmTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            break;
                        case EnumObis.Alarm_Battery:
                            fieldCount++;
                            alarm.FieldAlarmBattery = new FieldBase.FieldStruct()
                            {
                                Obis = new byte[1] { (byte)obis },
                                DataLength = new byte[1] { (byte)dataLength },
                                Data = data
                            };
                            break;
                        case EnumObis.Alarm_Hummidity:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.FieldAlarmHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            break;
                        case EnumObis.Alarm_Light:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.FieldAlarmLight = new FieldBase.FieldStruct()
                                {
                                    Obis = new byte[1] { (byte)obis },
                                    DataLength = new byte[1] { (byte)dataLength },
                                    Data = data
                                };
                            break;
                        default:
                            break;
                    }

                    if (message.Topic.Contains(messageType.TypeRunTime) && fieldCount >= runtime.FiledCount)
                    {
                        fieldCount = 0;
                        runtimes.Add(runtime);
                    }
                    else if (message.Topic.Contains(messageType.TypeAlarm) && fieldCount >= alarm.FiledCount)
                    {
                        fieldCount = 0;
                        alarms.Add(alarm);
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
