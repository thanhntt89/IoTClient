using IotClient.DataProcessing;
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

        private ThreadCollection threadCollection;

        private CancellationTokenSource tokenSource;

        private MqttClient client;
        private ClientOptions ClientOptions { get; set; }

        private const int TIME_RECONNECT = 60000;//60s

        /// <summary>
        /// Client started
        /// </summary>
        private bool isStopClient { get; set; }

        public Client(ClientOptions options)
        {
            client = new MqttClient(options.Broker, options.Port, false, null, MqttSslProtocols.SSLv3);
            ClientOptions = options;
            threadCollection = new ThreadCollection();
            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            //Register publish message
            SingletonDecodeMessageTime.Instance.eventPublishMessage += PublishTimeMessage;
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
            if (!SingletonDatabaseConnection.Instance.CheckDatabaseConnect(ClientOptions.DbServerName, ClientOptions.DatabaseName, ClientOptions.DbUserName, ClientOptions.DbPassword, ClientOptions.DbPort, ClientOptions.DbCommandTimeOut, ClientOptions.DbConnectionTimeOut))
            {
                ShowMessageEvent?.Invoke($"Database:Disconnected!!!");
                return;
            }

            // use a unique id as client id, each time we start the application           
            try
            {
                client.Connect(ClientOptions.ClientId);

                if (client.IsConnected)
                {
                    ShowMessageEvent?.Invoke($"Client-Status: Connected!!!");

                    //Subsriber message
                    SubsriberTopic();

                    // Start all client thread
                    StartAllThread();

                    ShowMessageEvent?.Invoke($"Client-Start Success!!!");
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
                if (!client.IsConnected && !isStopClient)
                {
                    try
                    {
                        client.Connect(ClientOptions.ClientId);
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
                    //Loop to untill connted
                    continue;
                }

                //Sleep 5min check connect status
                Thread.Sleep(ClientOptions.TimeCheckConnect);
                countTime = 0;
            }
        }

        public void Stop()
        {
            while (client.IsConnected)
            {
                client.Disconnect();

                ShowMessageEvent?.Invoke("Client-Status:Disconnected!!!");
                //Set stop by user
                isStopClient = true;

                //Stop all thread
                StopAllThread();

                ShowMessageEvent?.Invoke("Client-Stop:Done!!!");
                LogUtil.Intance.WriteLog(LogType.Info, "Client-Stop");
            }
        }

        private void StopAllThread()
        {
            ShowMessageEvent?.Invoke("Client-StopAllThread:Waitting for thread stop!!!");

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

            //Reset token resouce
            tokenSource = null;
            //Reset thread
            threadCollection.Clear();

            ShowMessageEvent?.Invoke($"Client-StopAllThread: Done!!!");
        }

        private void StartAllThread()
        {
            tokenSource = new CancellationTokenSource();
            threadCollection.AddThread(AutoReConnect, tokenSource.Token);
            threadCollection.AddThread(SingletonDatabaseConnection.Instance.ThreadCheckConnection, tokenSource.Token);
            threadCollection.AddThread(SingletonDecodeMessageTime.Instance.ThreadDecode, tokenSource.Token);
            threadCollection.AddThread(SingletonDecodeMessageData.Instance.DecodeDataThread, tokenSource.Token);
            threadCollection.AddThread(SingletonDecodeMessageData.Instance.InsertDataThread, tokenSource.Token);
            threadCollection.AddThread(ThreadMessageTest, tokenSource.Token);

            threadCollection.StartThread();
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
