using Newtonsoft.Json;

namespace IotClient
{
    public class DatabaseConfig
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public int ConnectionTimeOut { get; set; }
        public int CommandTimeOut { get; set; }
    }

    public class DcuConfig
    {
        public string ClientId { get; set; }
        public int Port { get; set; }
        public string Broker { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int QoS { get; set; }
        public bool IsClearSection { get; set; }
        public bool IsRetain { get; set; }
        public bool IsAutoReconnect { get; set; }
        public string SubscriberTopic { get; set; }
        public string PublisherTopic { get; set; }
        public string TypeData { get; set; }
        public string TypeTime { get; set; }

        private int timeCheckConnect;
        public int TimeCheckConnect
        {
            get { return timeCheckConnect*60000; }
            set
            {
                timeCheckConnect = value;
            }
            
        }
    }

    public class ClientSetting
    {
        [JsonProperty("DATABASE_CONFIG")]
        public DatabaseConfig DATABASE_CONFIG { get; set; }
        [JsonProperty("DCU_CONFIG")]
        public DcuConfig DCU_CONFIG { get; set; }
    }
}
