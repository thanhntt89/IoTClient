using IotSystem.Core;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MessageStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            try
            {
                //Process data
                //Valid message
                byte crc = message.Message[message.Message.Length - 1];
                byte[] dataMessage = new byte[message.Message.Length - 1];
                Buffer.BlockCopy(message.Message, 0, dataMessage, 0, message.Message.Length - 1);
                //Check sum data
                if (crc != ByteUtil.CalCheckSum(dataMessage))
                    return;

                RuntimeStruct runtime = new RuntimeStruct();
                AlarmStruct alarm = new AlarmStruct();

                int dataLength = 0;
                int offSet = 0;
                //Count data foreach device
                int fieldCount = 0;

                EnumObis nextObis = EnumObis.UnKnow;
                //Message struct: [Time][Obis|Length|Data][Obis|Length|Data]....
                while (dataMessage.Length != offSet)
                {
                    //Fist obis
                    EnumObis obis = (EnumObis)dataMessage[offSet];
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
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                Alarms.RawTime = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.DeviceNo:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawDeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawDeviceNo = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp1:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Temp2:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Rssi:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawRssi = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawRssi = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.LowBattery:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawLowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawLowBattery = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.Hummidity:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeRunTime))
                            {
                                runtime.RawHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            else if (message.Topic.Contains(messageType.TypeAlarm))
                            {
                                alarm.RawHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            }
                            break;
                        case EnumObis.AlarmTemp1:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmTemp1 = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            break;
                        case EnumObis.AlarmTemp2:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmTemp2 = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            break;
                        case EnumObis.AlarmBattery:
                            fieldCount++;
                            alarm.RawAlarmBattery = new FieldBase.FieldStruct()
                            {
                                Obis = (byte)obis,
                                Data = data
                            };
                            break;
                        case EnumObis.AlarmHummidity:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmHummidity = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            break;
                        case EnumObis.AlarmLight:
                            fieldCount++;
                            if (message.Topic.Contains(messageType.TypeAlarm))
                                alarm.RawAlarmLigth = new FieldBase.FieldStruct()
                                {
                                    Obis = (byte)obis,
                                    Data = data
                                };
                            break;
                        default:
                            break;
                    }

                    //Check Next obis
                    if (offSet < dataMessage.Length)
                        nextObis = (EnumObis)dataMessage[offSet];

                    //Add to list when offSet = dataLength || Next Obis = DeviceNo and preObis == Time
                    if (offSet == dataMessage.Length || nextObis == EnumObis.DeviceNo && obis != EnumObis.Time)
                    {
                        if (message.Topic.Contains(messageType.TypeRunTime))
                        {
                            fieldCount = 0;
                            Runtimes.Add(runtime);
                            runtime = new RuntimeStruct();
                        }
                        else if (message.Topic.Contains(messageType.TypeAlarm))
                        {
                            fieldCount = 0;
                            Alarms.Add(alarm);
                            alarm = new AlarmStruct();
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
