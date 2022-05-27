using IotClient.DataProcessing;
using IotClient.Utils;
using System;
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
        }

        public void Start()
        {
            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application           
            try
            {
                client.Connect(ClientOptions.ClientId);
                if (client.IsConnected)
                {
                    //Subsriber message
                    SubsriberTopic();

                    //Start thread decode
                    SingletonDecodeData.Instance.StartDecodeThread();

                    //Auto reconnect
                    AutoReConnect();
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

            //Test enqueue
            SingletonMessageQueue<MessageData>.Instance.Enqueue(message);

            //DataType: Customer set follow to thread processing data
            if (message.Topic.Contains(ClientOptions.TypeData))
            {
                // SingletonMessageQueue<MessageData>.Instance.Enqueue(message);
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
                ShowMessageEvent?.Invoke($"SupscriberTopic: {ClientOptions.SubscriberTopic} \nQoSLevel:{ClientOptions.QoSLevel}");

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

        public void AutoReConnect()
        {
            int countTime = 0;

            Task reconnect = new Task(() =>
            {
                while (true)
                {
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
            reconnect.Start();
            LogUtil.WriteLog(LogType.Info, "Client-AutoReConnect");
            ShowMessageEvent?.Invoke("Start thread: AutoReconnect!!!");
        }

        public void Stop()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
                //Set stop by user
                isStopClient = true;

                ShowMessageEvent?.Invoke("Client STOPPED!!!");
                LogUtil.WriteLog(LogType.Info, "Client-Stop");
            }
        }
    }
}
