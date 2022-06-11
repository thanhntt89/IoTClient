using IotSystem.Core;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MessageStructure;
using System;

namespace IotSystem.MessageProcessing.MeterMessage
{
    public class MeterMessageRaw
    {
        public RuntimeCollection Runtimes { get; set; }
        public AlarmCollection Alarms { get; set; }

        private MessageBase message { get; set; }
        private MessageType messageType { get; set; }
        public MeterMessageRaw(MessageBase messageBase, MessageType type)
        {
            message = messageBase;
            Runtimes = new RuntimeCollection();
            Alarms = new AlarmCollection();
            messageType = type;
        }

        public void GetMessageRaw()
        {
            GetRawAll();
            return;
            if (message.Topic.Contains(messageType.TypeRunTime))
            {
                GetRawRuntime();
            }
            else if (message.Topic.Contains(messageType.TypeAlarm))
            {
                GetRawAlarm();
            }
        }

        private void GetRawRuntime()
        {
            try
            {
                //Process data
                //Get Crc 1byte
                byte crc = message.Message[message.Message.Length - 1];
                //Data length = Data - crc(1byte)
                byte[] dataMessage = new byte[message.Message.Length - 1];
                //Get raw data
                Buffer.BlockCopy(message.Message, 0, dataMessage, 0, message.Message.Length - 1);

                //Valid Check sum data
                if (crc != ByteUtil.CalCheckSum(dataMessage))
                    return;

                RuntimeStruct runtime = default;

                int dataLength = 0;
                int offSet = 0;
                byte byteObisCheck = new byte();

                EnumObis nextObis = EnumObis.UnKnow;
                EnumObis obis = EnumObis.UnKnow;

                //Message struct: [Time][Obis|Length|Data][Obis|Length|Data]....
                while (dataMessage.Length != offSet)
                {
                    //Get byte obis
                    byteObisCheck = dataMessage[offSet];

                    //Get obis type
                    obis = (EnumObis)byteObisCheck;

                    offSet++;
                    //Get data length value
                    dataLength = dataMessage[offSet];
                    //Position data
                    offSet++;
                    byte[] data = new byte[dataLength];
                    Buffer.BlockCopy(dataMessage, offSet, data, 0, dataLength);

                    //Next obis
                    offSet += dataLength;

                    switch (obis)
                    {
                        case EnumObis.Time:
                            Runtimes.RawTime = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            //Next loop when obis is Time
                            continue;
                        case EnumObis.DeviceNo:
                            runtime.RawDeviceNo = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.Temp1:
                            runtime.RawTemp1 = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.Temp2:
                            runtime.RawTemp2 = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.Rssi:
                            runtime.RawRssi = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.LowBattery:
                            runtime.RawLowBattery = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.Hummidity:
                            runtime.RawHummidity = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        default:
                            break;
                    }

                    //Check Next obis
                    if (offSet < dataMessage.Length)
                        nextObis = (EnumObis)dataMessage[offSet];

                    //Add to list when offSet = dataLength || Next Obis = DeviceNo
                    if (offSet == dataMessage.Length || nextObis == EnumObis.DeviceNo)
                    {
                        Runtimes.Add(runtime);
                        runtime = default(RuntimeStruct);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Intance.WriteLog(LogType.Error, string.Format("DecodeMessageDataThread-ProcessingMessage-Error: {0}", ex.Message));
            }
        }

        private void GetRawAlarm()
        {
            try
            {
                //Process data
                //Get Crc 1byte
                byte crc = message.Message[message.Message.Length - 1];
                //Data length = Data - crc(1byte)
                byte[] dataMessage = new byte[message.Message.Length - 1];
                //Get raw data
                Buffer.BlockCopy(message.Message, 0, dataMessage, 0, message.Message.Length - 1);

                //Valid Check sum data
                if (crc != ByteUtil.CalCheckSum(dataMessage))
                    return;

                AlarmStruct alarm = default;

                int dataLength = 0;
                int offSet = 0;
                byte byteObisCheck = new byte();

                EnumObis nextObis = EnumObis.UnKnow;
                EnumObis obis = EnumObis.UnKnow;

                //Message struct: [Time][Obis|Length|Data][Obis|Length|Data]....
                while (dataMessage.Length != offSet)
                {
                    //Get byte obis
                    byteObisCheck = dataMessage[offSet];

                    //Get obis type
                    obis = (EnumObis)byteObisCheck;

                    offSet++;
                    //Get data length value
                    dataLength = dataMessage[offSet];
                    //Position data
                    offSet++;
                    byte[] data = new byte[dataLength];
                    Buffer.BlockCopy(dataMessage, offSet, data, 0, dataLength);

                    //Next obis
                    offSet += dataLength;

                    switch (obis)
                    {
                        case EnumObis.Time:
                            Alarms.RawTime = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            //Next loop when obis is Time
                            continue;
                        case EnumObis.DeviceNo:
                            alarm.RawDeviceNo = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };

                            break;
                        case EnumObis.Temp1:
                            alarm.RawTemp1 = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };

                            break;
                        case EnumObis.Temp2:
                            alarm.RawTemp2 = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };

                            break;
                        case EnumObis.Rssi:
                            alarm.RawRssi = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };

                            break;
                        case EnumObis.LowBattery:
                            alarm.RawLowBattery = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };

                            break;
                        case EnumObis.Hummidity:
                            alarm.RawHummidity = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };

                            break;
                        case EnumObis.AlarmTemp1:
                            alarm.RawAlarmTemp1 = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.AlarmTemp2:
                            alarm.RawAlarmTemp2 = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.AlarmBattery:
                            alarm.RawAlarmBattery = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.AlarmHummidity:
                            alarm.RawAlarmHummidity = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.AlarmLight:
                            alarm.RawAlarmLigth = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        default:
                            break;
                    }

                    //Check Next obis
                    if (offSet < dataMessage.Length)
                        nextObis = (EnumObis)dataMessage[offSet];

                    //Add to list when offSet = dataLength || Next Obis = DeviceNo
                    if (offSet == dataMessage.Length || nextObis == EnumObis.DeviceNo)
                    {                       
                        Alarms.Add(alarm);
                        alarm = default(AlarmStruct);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Intance.WriteLog(LogType.Error, string.Format("DecodeMessageDataThread-ProcessingMessage-Error: {0}", ex.Message));
            }
        }

        private void GetRawAll()
        {
            try
            {
                //Process data
                //Get Crc 1byte
                byte crc = message.Message[message.Message.Length - 1];
                //Data length = Data - crc(1byte)
                byte[] dataMessage = new byte[message.Message.Length - 1];
                //Get raw data
                Buffer.BlockCopy(message.Message, 0, dataMessage, 0, message.Message.Length - 1);

                //Valid Check sum data
                if (crc != ByteUtil.CalCheckSum(dataMessage))
                    return;

                RuntimeStruct runtime = default;
                AlarmStruct alarm = default;

                int dataLength = 0;
                int offSet = 0;                
                byte byteObisCheck = new byte();

                EnumObis nextObis = EnumObis.UnKnow;
                EnumObis obis = EnumObis.UnKnow;

                //Message struct: [Time][Obis|Length|Data][Obis|Length|Data]....
                while (dataMessage.Length != offSet)
                {
                    //Get byte obis
                    byteObisCheck = dataMessage[offSet];

                    //Get obis type
                    obis = (EnumObis)byteObisCheck;

                    offSet++;
                    //Get data length value
                    dataLength = dataMessage[offSet];
                    //Position data
                    offSet++;
                    byte[] data = new byte[dataLength];
                    Buffer.BlockCopy(dataMessage, offSet, data, 0, dataLength);

                    //Next obis
                    offSet += dataLength;

                    switch (obis)
                    {
                        case EnumObis.Time:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                Runtimes.RawTime = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                Alarms.RawTime = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            //Next loop when obis is Time
                            continue;
                        case EnumObis.DeviceNo:
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawDeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawDeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp1: 
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp2: 
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Rssi: 
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawRssi = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawRssi = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.LowBattery: 
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawLowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawLowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Hummidity: 
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.AlarmTemp1: 
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            break;
                        case EnumObis.AlarmTemp2: 
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            break;
                        case EnumObis.AlarmBattery: 
                            alarm.RawAlarmBattery = new FieldBase.FieldStruct()
                            {
                                Obis = byteObisCheck,
                                Data = data
                            };
                            break;
                        case EnumObis.AlarmHummidity: 
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            break;
                        case EnumObis.AlarmLight: 
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmLigth = new FieldBase.FieldStruct()
                                {
                                    Obis = byteObisCheck,
                                    Data = data
                                };
                            break;
                        default:
                            break;
                    }

                    //Check Next obis
                    if (offSet < dataMessage.Length)
                        nextObis = (EnumObis)dataMessage[offSet];

                    //Add to list when offSet = dataLength || Next Obis = DeviceNo
                    if (offSet == dataMessage.Length || nextObis == EnumObis.DeviceNo)
                    {
                        if (message.Topic.Contains(messageType.TypeRunTime))
                        { 
                            Runtimes.Add(runtime);
                            runtime = default(RuntimeStruct);
                        }
                        else if (message.Topic.Contains(messageType.TypeAlarm))
                        { 
                            Alarms.Add(alarm);
                            alarm = default(AlarmStruct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Intance.WriteLog(LogType.Error, string.Format("DecodeMessageDataThread-ProcessingMessage-Error: {0}", ex.Message));
            }
        }
    }
}
