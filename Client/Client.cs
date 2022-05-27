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
                    AutoReConnect();
                }
            }
            catch (Exception ex)
            {
                ShowMessageEvent?.Invoke(ex.Message);
            }
        }

        private void Client_MqttMsgPublishReceived(object obj, MqttMsgPublishEventArgs e)
        {
            //Check topic data
            MessageData message = new MessageData() { Topic = e.Topic, Message = e.Message };

            //DataType: Customer set follow to thread processing data
            if (message.Topic.Contains(ClientOptions.TypeData))
            {

            }
            else if (message.Topic.Contains(ClientOptions.TypeTime) && message.Topic.Contains("/"))
            {
                PublishTimeMessage(message.Topic.Split('/')[1]);
            }
        }

        private void PublishTimeMessage(string dcuId)
        {
            if (client.IsConnected)
            {
                client.Publish(ClientOptions.PublisherTopic, Encoding.ASCII.GetBytes(Contants.CURRENT_TIME));
                ShowMessageEvent?.Invoke($"PuplishTopic: {ClientOptions.PublisherTopic} Data: {Contants.CURRENT_TIME}");
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
            }
            catch (Exception ex)
            {
                ShowMessageEvent?.Invoke(ex.Message);
            }
        }

        public void ShowMessage(DelegateShowMessage showMessage)
        {
            ShowMessageEvent = showMessage;
        }

        public void AutoReConnect()
        {
            int countTime = 0;

            Task reconnect = new Task(() =>
            {
                while (true)
                {
                    if (!client.IsConnected && isStopClient)
                    {
                        countTime++;
                        client.Connect(ClientOptions.ClientId);
                        //Sleep 10s try reconnect
                        Thread.Sleep(10000);
                    }

                    //Sleep 5min check connect status
                    Thread.Sleep(ClientOptions.TimeCheckConnect);
                }
            });
            reconnect.Start();
            ShowMessageEvent?.Invoke("Start thread: AutoReconnect!!!");
        }

        public void Stop()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
                isStopClient = true;

                ShowMessageEvent?.Invoke("Client STOPPED!!!");
            }
        }
    }
}
