/**
*Project name: IotClient 
* Created by: Nguyen Tat Thanh
* File Name: Client.cs
* Created date:2022/5/27 12:48 AM 
* Copyright (c) by MVN Viet Nam Inc. All rights reserved
**/
using IotSystem.Core.Queues;
using IotSystem.Core.ThreadManagement;
using IotSystem.Core.Utils;
using IotSystem.Queues;
using System;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static IotSystem.ClientEvent;

namespace IotSystem.Core.Connection
{
    public partial class Client
    {
        private string DbServerName { get; set; }
        private string DatabaseName { get; set; }
        private string DbUserName { get; set; }
        private string DbPassword { get; set; }
        private int DbPort { get; set; }
        private int DbConnectionTimeOut { get; set; }
        private int DbCommandTimeOut { get; set; }
        private string ClientId { get; set; }
        private string Broker { get; set; }
        private int Port { get; set; }
        private string SubscriberTopic { get; set; }
        private string PublisherTopic { get; set; }
        private byte QoSLevel { get; set; }
        private string TypeData { get; set; }
        private string TypeTime { get; set; }
        private bool IsClearSection { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        private int TimeCheckConnect { get; set; }

        private IDecodeDataThread iDecodeDataThread { get; set; }
        private IDatabaseConnectionThread iDatabaseConnectionThread { get; set; }
        private IInsertDataThread iInsertDataThread { get; set; }
        private IPublishMessageThread iPublishMessageThread { get; set; }

        void LoadOptions(ClientOptions clientOptions)
        {
            DbServerName = clientOptions.DbServerName;
            DatabaseName = clientOptions.DatabaseName;
            DbUserName = clientOptions.DbUserName;
            DbPassword = clientOptions.DbPassword;
            DbCommandTimeOut = clientOptions.DbCommandTimeOut;
            DbConnectionTimeOut = clientOptions.DbConnectionTimeOut;
            DbPort = clientOptions.DbPort;

            ClientId = clientOptions.ClientId;
            Broker = clientOptions.Broker;
            Port = clientOptions.Port;
            SubscriberTopic = clientOptions.SubscriberTopic;
            PublisherTopic = clientOptions.PublisherTopic;
            QoSLevel = clientOptions.QoSLevel;
            TypeData = clientOptions.TypeData;
            TypeTime = clientOptions.TypeTime;
            IsClearSection = clientOptions.IsClearSection;
            UserName = clientOptions.UserName;
            Password = clientOptions.Password;
            TimeCheckConnect = clientOptions.TimeCheckConnect;

            iDecodeDataThread = clientOptions.iDecodeDataThread;
            iDatabaseConnectionThread = clientOptions.iDatabaseConnectionThread;
            iInsertDataThread = clientOptions.iInsertDataThread;
            iPublishMessageThread = clientOptions.iPublishMessageThread;
        }

        private event DelegateShowMessage ShowMessageEvent;

        /// <summary>
        /// Threads in client
        /// </summary>
        private ThreadCollection threadCollection;

        private ThreadCollection decodeThreads;

        /// <summary>
        /// Using status for control all thread
        /// </summary>
        private CancellationTokenSource tokenSource;
        private CancellationTokenSource tokenDecode;

        private MqttClient client;

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
        private bool IsUserStopReceived { get; set; }
    }

    public partial class Client : IClient
    {        
        public Client(ClientOptions clientOptions)
        {
            //Get option
            LoadOptions(clientOptions);

            client = new MqttClient(Broker, Port, false, null, MqttSslProtocols.TLSv1_2);
            decodeThreads = new ThreadCollection();

            threadCollection = new ThreadCollection();
            tokenSource = new CancellationTokenSource();
            tokenDecode = new CancellationTokenSource();

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //Init all thread
            InitAllThread();

            //Register publish message
            iPublishMessageThread.PublishMessage(PublishTimeMessage);
            iDatabaseConnectionThread.SqlConnectionStatus(SqlConnectionStatus);
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

            if (DisconnectCount >= MAX_TRY_RECONNECT_SQL)
            {
                //Set stop by user
                Stop(true);
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

            //Check database connection
            if (!iDatabaseConnectionThread.CheckDatabaseConnect(DbServerName, DatabaseName, DbUserName, DbPassword, DbPort, DbCommandTimeOut, DbConnectionTimeOut))
            {
                ShowMessageEvent?.Invoke($"Database:Disconnected!!!\nClient: Not start!!!");
                return;
            }

            // use a unique id as client id, each time we start the application           
            try
            {
                client.Connect(ClientId);

                if (client.IsConnected)
                {
                    ShowMessageEvent?.Invoke($"Client-Status: Connected!!!");

                    //Subsriber message
                    SubsriberTopic();

                    // Start all client thread
                    StartAllThread();

                    ShowMessageEvent?.Invoke($"Client-Start: Success!!!");
                    LogUtil.Intance.WriteLog(LogType.Info, $"Client-Start Success!!!");
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
            if (message.Topic.Contains(TypeData))
            {
                SingletonMessageDataQueue<MessageData>.Instance.Enqueue(message);
            }
            else if (message.Topic.Contains(TypeTime))
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
            string[] topics = SubscriberTopic.Split(';');
            byte[] qos = new byte[topics.Length];
            //Create Qos foreach message
            for (int index = 0; index < topics.Length; index++)
            {
                qos[index] = QoSLevel;
            }
            try
            {
                client.Subscribe(topics, qos);
                ShowMessageEvent?.Invoke($"Client-SupscriberTopic: {SubscriberTopic} \nQoSLevel:{QoSLevel}");

                LogUtil.Intance.WriteLog(LogType.Info, $"Client-SubsriberTopic: {SubscriberTopic}");
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
            iDecodeDataThread.ShowMessage(showMessage);
            iDatabaseConnectionThread.ShowMessage(showMessage);
            iInsertDataThread.ShowMessage(showMessage);
            iPublishMessageThread.ShowMessage(showMessage);
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
                if (!client.IsConnected && !IsUserStopReceived)
                {
                    try
                    {
                        client.Connect(ClientId);
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
                Thread.Sleep(TimeCheckConnect);
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
                //Set stop by user
                IsUserStopReceived = isUserStop;

                while (client.IsConnected)
                {
                    client.Disconnect();

                    ShowMessageEvent?.Invoke("Client-Status:Disconnected!!!");

                    LogUtil.Intance.WriteLog(LogType.Info, "Client-Stop:Done!!!");
                }
            }
        }

        public void Exit()
        {
            ShowMessageEvent?.Invoke("Client-StopAllThread:Waitting for thread stop!!!");

            //Disconnect to broker
            Stop(false);

            //Set thread stop
            tokenSource.Cancel();

            //Waiting for empty queue
            while (true)
            {
                //Check messageQueue has data
                if (SingletonMessageDataQueue<MessageData>.Instance.Count == 0)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            tokenDecode.Cancel();
            //Stop Thread check connected
            //Wait until thread finished
            threadCollection.Join();
            decodeThreads.Join();

            ShowMessageEvent?.Invoke($"Client-StopAllThread: Done!!!");
        }

        private void StartAllThread()
        {
            threadCollection.StartThread();
        }

        private void InitAllThread()
        {
            threadCollection.AddThread(AutoReConnect, tokenSource.Token);
            threadCollection.AddThread(iDatabaseConnectionThread.ThreadCheckConnection, tokenSource.Token);
            threadCollection.AddThread(iDecodeDataThread.ThreadDecode, tokenSource.Token, "DecodeMain");
            threadCollection.AddThread(iPublishMessageThread.ThreadDecode, tokenSource.Token);
            threadCollection.AddThread(iInsertDataThread.InsertData, tokenSource.Token);
            threadCollection.AddThread(ThreadMessageTest, tokenSource.Token);
        }

        private void ActiveDecodeThreadByTraffic()
        {
            if (SingletonMessageDataQueue<MessageData>.Instance.Count < 1000)
            {
                if (decodeThreads.RunningCount > 0)
                {
                    //Stop all decode thread
                    tokenDecode.Cancel();
                    Thread.Sleep(1000);
                    //Clear all thread traffic
                    decodeThreads.Clear();
                }
            }
            if (SingletonMessageDataQueue<MessageData>.Instance.Count > 5000)
            {
                if (decodeThreads.RunningCount == 0)
                {                  
                    if (decodeThreads.Count == 0)
                    {
                        tokenDecode.Dispose();
                        //Reset token
                        tokenDecode = new CancellationTokenSource();

                        for (int thread = 0; thread < SystemUtil.Instance.GetMaxThreadNumber - threadCollection.Count - 1; thread++)
                        {
                            decodeThreads.AddThread(iDecodeDataThread.ThreadDecodeByTraffic, tokenDecode.Token, $"ThreadName_{thread}");
                        }
                    }

                    decodeThreads.StartThread();
                }
            }
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

                MessageData message = new MessageData() { Topic = $"Topic/DCU{count}" };

                SingletonMessageDataQueue<MessageData>.Instance.Enqueue(message);
                Thread.Sleep(10);

                ActiveDecodeThreadByTraffic();
            }
        }
    }
}
