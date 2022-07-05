
using IotSystem.Core;
using IotSystem.Core.Utils;
using IotSystem.MessageProcessing.MeterMessage;
using System;
using System.Threading;
/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: ProcessingDataFactory.cs
* Created date:2022/5/27 1:05 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
namespace IotSystem.MessageProcessing
{
    public class FactoryMeterMessageProcessing
    {
        private MessageType messageType;

        public FactoryMeterMessageProcessing(MessageType Type)
        {
            messageType = Type;
        }

        public bool ProcessingMessage(MessageBase message)
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

                            var row = new object[7]
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
                            Thread.Sleep(2);
                        }
                    }
                }
                else if (message.Topic.Contains(messageType.TypeAlarm))
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

                            Thread.Sleep(2);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
               // EventShowMessage?.Invoke($"DecodeRunning-Fails:{ex.Message}");
                LogUtil.Intance.WriteLog(LogType.Error, string.Format("DecodeMessageDataThread-ProcessingMessage-Error: {0}", ex.Message));
                return false;
            }
        }
    }
}
