using IotClient.DataProcessing;
using IotClient.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static IotClient.ClientEvent;

namespace IotClient
{
    public class ClientOptions
    {
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

        private List<Task> clientThreads = new List<Task>();
        private CancellationTokenSource tokenSource;

        private MqttClient client;
        private ClientOptions ClientOptions { get; set; }

        /// <summary>
        /// Stop client by user
        /// </summary>
        private bool isStopClient = false;

        public Client(ClientOptions options)
        {
            client = new MqttClient(options.Broker, options.Port, false, null, MqttSslProtocols.SSLv3);
            ClientOptions = options;

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        public void Start()
        {
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
                    LogUtil.WriteLog(LogType.Info, $"Client-Start Success!!!");
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog(LogType.Error, $"Client-Start-Error: {ex.Message}");
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
                SingletonMessageQueue<MessageData>.Instance.Enqueue(message);
            }
            else if (message.Topic.Contains(ClientOptions.TypeTime) && message.Topic.Contains("/"))
            {
                PublishTimeMessage(message.Topic.Split('/')[1]);
            }

            LogUtil.WriteLog(LogType.Info, $"Client-Client_MqttMsgPublishReceived-Topic:{message.Topic}");
        }

        private void PublishTimeMessage(string dcuId)
        {
            if (client.IsConnected)
            {
                client.Publish(ClientOptions.PublisherTopic, Encoding.ASCII.GetBytes(Contants.CURRENT_TIME));
                ShowMessageEvent?.Invoke($"PuplishTopic: {ClientOptions.PublisherTopic} Data: {Contants.CURRENT_TIME}");
                LogUtil.WriteLog(LogType.Info, $"Client-PublishTimeMessage: {ClientOptions.PublisherTopic} to DCU: {dcuId}");
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

                LogUtil.WriteLog(LogType.Info, $"Client-SubsriberTopic: {ClientOptions.SubscriberTopic}");
            }
            catch (Exception ex)
            {
                LogUtil.WriteLog(LogType.Error, $"Client-SubsriberTopic-Error: {ex.Message}");
                ShowMessageEvent?.Invoke(ex.Message);
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            ShowMessageEvent += showMessage;
            SingletonDecodeData.Instance.ShowMessageEvent += showMessage;
        }

        private Task AutoReConnect(CancellationToken cancellation)
        {
            Task reconnect = new Task(() =>
            {
                int countTime = 0;
                LogUtil.WriteLog(LogType.Info, "Client-AutoReConnect:Started!!!");
                ShowMessageEvent?.Invoke("Client-AutoReconnect: Started!!!");

                while (true)
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        ShowMessageEvent?.Invoke($"Client-AutoReconnect:Stopped!!!");
                        break;
                    }

                    //Check stop byuser
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
                            LogUtil.WriteLog(LogType.Info, "Client-AutoReConnect-Status:Connected");
                            ShowMessageEvent?.Invoke("Client-AutoReConnect-Status:Connected");
                        }
                        //Sleep 10s try reconnect
                        Thread.Sleep(10000);
                        //Loop to untill connted
                        continue;
                    }

                    //Sleep 5min check connect status
                    Thread.Sleep(ClientOptions.TimeCheckConnect);
                    countTime = 0;
                }
            });
            return reconnect;
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
                LogUtil.WriteLog(LogType.Info, "Client-Stop");
            }
        }

        private void StopAllThread()
        {
            ShowMessageEvent?.Invoke("Client-StopAllThread:Waitting for thread stop!!!");
            
            //Set thread start
            tokenSource.Cancel();

            while (true)
            {
                //Check messageQueue has data
                if (SingletonMessageQueue<MessageData>.Instance.Count == 0)
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            //Stop Thread check connected
            foreach (var thread in clientThreads)
            {
                try
                {
                    //Wait until thread finished
                    thread.Wait();
                }
                catch (Exception ex)
                {
                    ShowMessageEvent?.Invoke($"Client-StopAllThread-Error: {ex.Message}");
                }
            }

            //Reset token resouce
            tokenSource = null;
            //Reset thread
            clientThreads.Clear();

            ShowMessageEvent?.Invoke($"Client-StopAllThread: Done!!!");
        }

        private void StartAllThread()
        {
            tokenSource = new CancellationTokenSource();
            clientThreads.Add(AutoReConnect(tokenSource.Token));
            clientThreads.Add(SingletonDecodeData.Instance.DecodeDataThread(tokenSource.Token));
            clientThreads.Add(SingletonDecodeData.Instance.InsertDataThread(tokenSource.Token));
            //clientThreads.Add(ThreadTestMessage(tokenSource.Token));

            foreach (Task thread in clientThreads)
            {
                if (thread.Status == TaskStatus.Created)
                    thread.Start();
            }
        }

        private Task ThreadTestMessage(CancellationToken cancellation)
        {
            Task task = new Task(() =>
            {
                int count = 0;

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

                    SingletonMessageQueue<MessageData>.Instance.Enqueue(message);

                    Thread.Sleep(500);
                }

            });
            return task;
        }
    }
}
