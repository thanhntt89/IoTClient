using IotClient.MessageProcessing;
using IotClient.Queues;
using System.Threading;
using static IotClient.ClientEvent;

namespace IotClient.MessageProcessing
{
    public class SingletonDecodeMessageTime
    {
        public event DelegatePublishMessage eventPublishMessage;
        public event DelegateShowMessage eventShowMessage;

        private static SingletonDecodeMessageTime intance;
        private static readonly object objLock = new object();

        public string Topic { get; set; }

        public void ThreadDecode(CancellationToken cancellation)
        {
            eventShowMessage?.Invoke("ThreadDecodeMessageTime: Started!!!");
            MessageData message = new MessageData();
            while (!cancellation.IsCancellationRequested)
            {
                if (SingletonMessageTimeQueue<MessageData>.Instance.Count > 0)
                {
                    if (SingletonMessageTimeQueue<MessageData>.Instance.TryDequeue(out message) && message != null)
                    {
                        MessageProcessing(message);
                    }
                    Thread.Sleep(10);
                    continue;
                }
                // Queue is empty wait 1s
                Thread.Sleep(1000);
            }

            eventShowMessage?.Invoke("ThreadDecodeMessageTime: Stopped!!!");
        }

        private void MessageProcessing(MessageData message)
        {
            string dcuId = message.Topic.Split('/')[1];
            eventPublishMessage?.Invoke(dcuId, Constant.CURRENT_TIME);
        }

        static SingletonDecodeMessageTime()
        {

        }

        private SingletonDecodeMessageTime()
        {

        }

        public static SingletonDecodeMessageTime Instance
        {
            get
            {
                if (intance == null)
                {
                    lock (objLock)
                    {
                        if (intance == null)
                        {
                            intance = new SingletonDecodeMessageTime();
                        }
                    }
                }
                return intance;
            }
        }
    }
}
