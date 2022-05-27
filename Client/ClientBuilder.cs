namespace IotClient
{
    public class ClientBuilder
    {
        private ClientOptions ClientOptions;

        public ClientBuilder()
        {
            ClientOptions = new ClientOptions();
        }

        public ClientBuilder AddClientId(string clientId)
        {
            ClientOptions.ClientId = clientId;
            return this;
        }
        public ClientBuilder AddUserName(string userName)
        {
            ClientOptions.UserName = userName;
            return this;
        }
        public ClientBuilder AddPassword(string password)
        {
            ClientOptions.Password = password;
            return this;
        }
        public ClientBuilder AddWillTopic(string subcriberTopic)
        {
            ClientOptions.SubscriberTopic = subcriberTopic;
            return this;
        }
        public ClientBuilder AddWillPublisherTopic(string willPubishTopic)
        {
            ClientOptions.PublisherTopic = willPubishTopic;
            return this;
        }
        public ClientBuilder AddWillQosLevel(int willQosLevel)
        {
            ClientOptions.QoSLevel = (byte)willQosLevel;
            return this;
        }
        public ClientBuilder AddBroker(string broker)
        {
            ClientOptions.Broker = broker;
            return this;
        }
        public ClientBuilder AddTypeData(string typeData)
        {
            ClientOptions.TypeData = typeData;
            return this;
        }
        public ClientBuilder AddTypeTime(string typeTime)
        {
            ClientOptions.TypeTime = typeTime;
            return this;
        }
        public ClientBuilder AddTimeCheckConnect(int timeCheckConnect)
        {
            ClientOptions.TimeCheckConnect = timeCheckConnect;
            return this;
        }
        public ClientBuilder AddPort(int port)
        {
            ClientOptions.Port = port;
            return this;
        }
        public Client Build()
        {
            return new Client(ClientOptions);
        }
    }
}
