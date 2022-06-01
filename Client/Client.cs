/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: Client.cs
* Created date:2022/5/27 12:48 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotClient.DataProcessing;
using IotClient.MessageProcessing;
using IotClient.Queues;
using IotClient.ThreadManagement;
using IotClient.Utils;
using System;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static IotClient.ClientEvent;

namespace IotClient
{
    public class ClientOptions
    {
        public string DbServerName { get; set; }
        public string DatabaseName { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public int DbPort { get; set; }
        public int DbConnectionTimeOut { get; set; }
        public int DbCommandTimeOut { get; set; }
        public string ClientId { get; set; }
        public string Broker { get; set; }
        public int Port { get; set; }
        public string SubscriberTopic { get; set; }
        public string PublisherTopic { get; set; }
        public byte QoSLevel { get; set; }
        public string TypeData { get; set; }
        public string TypeTime { get; set; }
        public bool IsClearSection { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int TimeCheckConnect { get; set; }
    }

    public class Client : IClient
    {
        private event DelegateShowMessage ShowMessageEvent;

        /// <summary>
        /// Threads in client
        /// </summary>
        private ThreadCollection threadCollection;

        /// <summary>
        /// Using status for control all thread
        /// </summary>
        private CancellationTokenSource tokenSource;

        private MqttClient client;
        private ClientOptions ClientOptions { get; set; }

        private const int TIME_RECONNECT = 60000;//60s

        /// <summary>
        /// Count time connection disconnect
        /// </summary>
        private int DisconnectCount = 0;

        /// <summary>
        /// Max sql to try reconnect to server
        /// </summary>
        private const int MAX_TRY_RECONNECT_SQL = 3;

        /// <summary>
        /// Client started
        /// </summary>
        private bool isStoppedByClient { get; set; }

        public Client(ClientOptions options)
        {
            client = new MqttClient(options.Broker, options.Port, false, null, MqttSslProtocols.TLSv1_2);
            ClientOptions = options;

            threadCollection = new ThreadCollection();
            tokenSource = new CancellationTokenSource();

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //Init all thread
            InitAllThread();

            //Register publish message
            SingletonDecodeMessageTime.Instance.eventPublishMessage += PublishTimeMessage;
            SingletonDatabaseConnection.Instance.eventSqlConnectionStaus += SqlConnectionStatus;
        }

        /// <summary>
        /// Get sql connect status
        /// </summary>
        /// <param name="status"></param>
        private void SqlConnectionStatus(bool status)
        {
            if (!status)
            {
                DisconnectCount++;
            }
            else
            {
                DisconnectCount = 0;
            }

            if(DisconnectCount >= MAX_TRY_RECONNECT_SQL)
            {
                Stop(false);
                ShowMessageEvent?.Invoke("Client-SqlConnectionStatus:Disconnect!!!");
            }
        }

        public void Start()
        {
            //Check client connection status
            if (client.IsConnected)
            {
                ShowMessageEvent?.Invoke($"Client:Started!!!");
                return;
            }

            // use a unique id as client id, each time we start the application           
            try
            {
                client.Connect(ClientOptions.ClientId);

                if (client.IsConnected)
                {
                    ShowMessageEvent?.Invoke($"Client-Status: Connected!!!");

                    //Check database connection
                    if (!SingletonDatabaseConnection.Instance.CheckDatabaseConnect(ClientOptions.DbServerName, ClientOptions.DatabaseName, ClientOptions.DbUserName, ClientOptions.DbPassword, ClientOptions.DbPort, ClientOptions.DbCommandTimeOut, ClientOptions.DbConnectionTimeOut))
                    {
                        ShowMessageEvent?.Invoke($"Database:Disconnected!!!");
                        return;
                    }

                    //Subsriber message
                    SubsriberTopic();

                    if (!isStoppedByClient)
                    {
                        // Start all client thread
                        StartAllThread();
                    }

                    ShowMessageEvent?.Invoke($"Client-Start Success!!!");
                    LogUtil.Intance.WriteLog(LogType.Info, $"Client-Start Success!!!");

                    isStoppedByClient = false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Intance.WriteLog(LogType.Error, $"Client-Start-Error: {ex.Message}");
                ShowMessageEvent?.Invoke($"Client-Start-Error: {ex.Message}");
            }
        }

        private void Client_MqttMsgPublishReceived(object obj, MqttMsgPublishEventArgs e)
        {
            //Check topic data
            MessageData message = new MessageData() { Topic = e.Topic, Message = e.Message };

            //DataType: Customer set follow to thread processing data
            if (message.Topic.Contains(ClientOptions.TypeData))
            {
                SingletonMessageDataQueue<MessageData>.Instance.Enqueue(message);
            }
            else if (message.Topic.Contains(ClientOptions.TypeTime))
            {
                SingletonMessageTimeQueue<MessageData>.Instance.Enqueue(message);
            }

            LogUtil.Intance.WriteLog(LogType.Info, $"Client-Client_MqttMsgPublishReceived-Topic:{message.Topic}");
        }

        private void PublishTimeMessage(string topic, string contents)
        {
            if (client.IsConnected)
            {
                client.Publish(topic, Encoding.ASCII.GetBytes(contents));
                ShowMessageEvent?.Invoke($"PuplishTopic: {topic} Data: {contents}");
                LogUtil.Intance.WriteLog(LogType.Info, $"Client-PublishTimeMessage: {topic} to DCU: {contents}");
            }
        }

        private void SubsriberTopic()
        {
            string[] topics = ClientOptions.SubscriberTopic.Split(';');
            byte[] qos = new byte[topics.Length];
            //Create Qos foreach message
            for (int index = 0; index < topics.Length; index++)
            {
                qos[index] = ClientOptions.QoSLevel;
            }
            try
            {
                client.Subscribe(topics, qos);
                ShowMessageEvent?.Invoke($"Client-SupscriberTopic: {ClientOptions.SubscriberTopic} \nQoSLevel:{ClientOptions.QoSLevel}");

                LogUtil.Intance.WriteLog(LogType.Info, $"Client-SubsriberTopic: {ClientOptions.SubscriberTopic}");
            }
            catch (Exception ex)
            {
                LogUtil.Intance.WriteLog(LogType.Error, $"Client-SubsriberTopic-Error: {ex.Message}");
                ShowMessageEvent?.Invoke(ex.Message);
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            ShowMessageEvent += showMessage;
            SingletonDecodeMessageData.Instance.ShowMessageEvent += showMessage;
            SingletonDatabaseConnection.Instance.ShowMessageEvent += showMessage;
            SingletonDecodeMessageTime.Instance.eventShowMessage += showMessage;
        }

        private void AutoReConnect(CancellationToken cancellation)
        {
            int countTime = 0;
            LogUtil.Intance.WriteLog(LogType.Info, "Client-AutoReConnect:Started!!!");
            ShowMessageEvent?.Invoke("Client-AutoReconnect: Started!!!");

            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    ShowMessageEvent?.Invoke($"Client-AutoReconnect:Stopped!!!");
                    break;
                }

                //Check stop by user
                if (!client.IsConnected && !isStoppedByClient)
                {
                    try
                    {
                        Start();
                        countTime++;
                        ShowMessageEvent?.Invoke($"Client-AutoReConnect-Try reconnect count:{countTime}");
                    }
                    catch (Exception ex)
                    {
                        ShowMessageEvent?.Invoke($"Client-AutoReConnect-Exception: {ex.Message}");
                        continue;
                    }
                    if (client.IsConnected)
                    {
                        LogUtil.Intance.WriteLog(LogType.Info, "Client-AutoReConnect-Status:Connected");
                        ShowMessageEvent?.Invoke("Client-AutoReConnect-Status:Connected");
                    }
                    //Sleep 60s try reconnect
                    Thread.Sleep(TIME_RECONNECT);
                    //Loop to untill connected
                    continue;
                }

                //Sleep 5min check connect status
                Thread.Sleep(ClientOptions.TimeCheckConnect);
                countTime = 0;
            }
        }

        public void Stop(bool isUserStop)
        {
            if (!client.IsConnected)
            {
                ShowMessageEvent?.Invoke("Client-Status:Disconnected!!!");
            }
            else
            {
                while (client.IsConnected)
                {
                    client.Disconnect();

                    ShowMessageEvent?.Invoke("Client-Status:Disconnected!!!");
                    //Set stop by user
                    isStoppedByClient = isUserStop;

                    LogUtil.Intance.WriteLog(LogType.Info, "Client-Stop:Done!!!");
                }
            }
        }

        public void StopAllThread()
        {
            ShowMessageEvent?.Invoke("Client-StopAllThread:Waitting for thread stop!!!");

            //Disconnect to broker
            Stop(false);

            //Set thread stop
            tokenSource.Cancel();

            while (true)
            {
                //Check messageQueue has data
                if (SingletonMessageDataQueue<MessageData>.Instance.Count == 0)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            //Stop Thread check connected
            try
            {
                //Wait until thread finished
                threadCollection.StopThread();
            }
            catch (Exception ex)
            {
                ShowMessageEvent?.Invoke($"Client-StopAllThread-Error: {ex.Message}");
            }

            ShowMessageEvent?.Invoke($"Client-StopAllThread: Done!!!");
        }

        private void StartAllThread()
        {
            threadCollection.StartThread();
        }

        private void InitAllThread()
        {
            threadCollection.AddThread(AutoReConnect, tokenSource.Token);
            threadCollection.AddThread(SingletonDatabaseConnection.Instance.ThreadCheckConnection, tokenSource.Token);
            threadCollection.AddThread(SingletonDecodeMessageTime.Instance.ThreadDecode, tokenSource.Token);
            threadCollection.AddThread(SingletonDecodeMessageData.Instance.ThreadDecode, tokenSource.Token);
            threadCollection.AddThread(SingletonDecodeMessageData.Instance.ThreadInsertData, tokenSource.Token);
            threadCollection.AddThread(ThreadMessageTest, tokenSource.Token);
        }


        private int count = 0;

        private void ThreadMessageTest(CancellationToken cancellation)
        {
            ShowMessageEvent?.Invoke($"ThreadTestMessage:Started!!!");
            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    ShowMessageEvent?.Invoke($"ThreadTestMessage:Stopped!!!");
                    break;
                }

                count++;

                MessageData message = new MessageData() { Topic = $"Topic/Test{count}" };

                SingletonMessageDataQueue<MessageData>.Instance.Enqueue(message);
                Thread.Sleep(500);
            }
        }
    }
}
